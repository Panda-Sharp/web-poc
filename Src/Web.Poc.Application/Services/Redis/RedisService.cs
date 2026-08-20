using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Services.UrlDowload;

namespace Web.Poc.Application.Services.Redis;

public class RedisService : IRedisService
{
    private const string GroupName = "urlsGroupName";
    private const string ConsumerName = "urlsConsumerName";

    private readonly ILogger<RedisService> _logger;
    private readonly IDatabase _db;

    public RedisService(
        IConnectionMultiplexer multiplexer,
        ILogger<RedisService> logger)
    {
        _logger = logger;

        _db = multiplexer.GetDatabase();
    }

    public async Task StreamAddAsync(string streamKey, IEnumerable<UrlMessage> payload)
    {
        _logger.LogInformation("StreamCreateConsumerGroupAsync Starting to write...");

        foreach (var urlMessage in payload)
        {
            await TryToCreateStreamConsumerGroupAsync(streamKey);

            var pairs = MessageToNameValueEntries(urlMessage);

            await _db.StreamAddAsync(
                streamKey,
                pairs);
        }
    }

    public async IAsyncEnumerable<UrlMessage> StreamReadGroupAsync(string streamKey)
    {
        _logger.LogInformation("Starting StreamReadGroupAsync key: {streamKey} ...", streamKey);

        while (true)
        {
            var result = await _db.StreamReadGroupAsync(
                key: streamKey,
                groupName: GroupName,
                consumerName: ConsumerName,
                position: StreamPosition.NewMessages,
                count: 1);

            if (result == null ||
                result.Length != 1)
            {
                break;
            }

            var streamEntry = result[0];

            var message = new UrlMessage(streamEntry.Id.ToString(), streamEntry.Values[1].Value!, streamEntry.Values[2].Value!);

            _logger.LogInformation("StreamReadGroupAsync read message {messageId}, Url: {Url}, Status: {Status}",
                message.RedisId, message.Url, message.Status);

            yield return message;
        }
    }

    public async IAsyncEnumerable<UrlMessage> StreamReadAndAcknowledgeGroupAsync(string streamKey, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            StreamEntry[]? result = null;

            try
            {
                result = await _db.StreamReadGroupAsync(
                    key: streamKey,
                    groupName: GroupName,
                    consumerName: ConsumerName,
                    position: StreamPosition.NewMessages,
                    count: 1);
            }
            catch { }

            if (result == null ||
                result.Length != 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                continue;
            }

            var streamEntry = result[0];

            var message = new UrlMessage(streamEntry.Id.ToString(), streamEntry.Values[1].Value!, streamEntry.Values[2].Value!);

            _logger.LogInformation("StreamReadAndAcknowledgeGroupAsync read message {messageId}, Url: {Url}, Status: {Status}",
                message.RedisId, message.Url, message.Status);

            await _db.StreamAcknowledgeAsync(
                key: streamKey,
                groupName: GroupName,
                messageId: result[0].Id);

            yield return message;
        }
    }

    private async Task TryToCreateStreamConsumerGroupAsync(string streamKey)
    {
        if (!(await _db.KeyExistsAsync(streamKey)) ||
             (await _db.StreamGroupInfoAsync(streamKey))
                .All(x => x.Name != GroupName))
        {
            await _db.StreamCreateConsumerGroupAsync(
                key: streamKey,
                groupName: GroupName,
                position: "0-0",
                createStream: true);
        }
    }

    private static NameValueEntry[] MessageToNameValueEntries(UrlMessage message) => [
        new NameValueEntry("Id", message.RedisId),
        new NameValueEntry("content", message.Url),
        new NameValueEntry("Sender", message.Status)
    ];
}