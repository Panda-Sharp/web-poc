using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Application.Services.Redis;
using Web.Poc.Application.Services.UrlDowload;
using Web.Poc.Domain;

namespace Web.Poc.WorkerService.Consumer.Services;

public class ConsumerService : IConsumerService
{
    private readonly IRedisService _redisService;
    private readonly IUrlDowloadService _urlDowloadService;
    private readonly ILogger<ConsumerService> _logger;
    private Channel<UrlMessage> _channel;
    private int _channelCapacity;

    public ConsumerService(
        IUrlDowloadService urlDowloadService,
        ILogger<ConsumerService> logger,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _urlDowloadService = urlDowloadService;
        _logger = logger;

        using var scope = scopeFactory.CreateScope();
        _redisService = scope.ServiceProvider.GetRequiredService<IRedisService>();

        if (!int.TryParse(configuration["QueueCapacity"], out _channelCapacity))
        {
            _channelCapacity = 99;
        }

        _channel = Channel.CreateBounded<UrlMessage>(new BoundedChannelOptions(_channelCapacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        });
    }

    public async Task ConsumeNewUrlsAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _redisService.StreamReadAndAcknowledgeGroupAsync(UrlAppConstants.NewUrlsStreamKey, cancellationToken))
        {
            try
            {
                _logger.LogInformation("ConsumeNewUrlsAsync Message received, Id: {RedisId} " +
                                       "Content: {Url} Sender: {Status}",
                                       message.RedisId, message.Url, message.Status);

                if (Uri.TryCreate(message.Url, UriKind.RelativeOrAbsolute, out var uri))
                {
                    await _redisService.StreamAddAsync(UrlAppConstants.PendingUrlsStreamKey, [message]);
                    _logger.LogInformation("...ConsumeNewUrlsAsync Added to Pending: {url}", message.Url);
                }
                else
                {
                    await _redisService.StreamAddAsync(UrlAppConstants.InvalidUrlsStreamKey, [message]);
                    _logger.LogInformation("...ConsumeNewUrlsAsync Added to Invalid: {url}", message.Url);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("OnMessage exception: {Message}", ex.Message);
            }
        }
    }

    public async Task ConsumePendingUrlsAsync(CancellationToken cancellationToken)
    {
        await foreach (var message in _redisService.StreamReadAndAcknowledgeGroupAsync(UrlAppConstants.PendingUrlsStreamKey, cancellationToken))
        {
            try
            {
                _logger.LogInformation("ConsumePendingUrlsAsyncTryQueueAsync, Id: {RedisId} " +
                                       "Content: {Url} Sender: {Status}",
                                       message.RedisId, message.Url, message.Status);

                if (_channel.Reader.Count < _channelCapacity)
                {
                    await _channel.Writer.WriteAsync(message);
                    _logger.LogInformation("...ConsumePendingUrlsAsync Added: {uri} queue: {Count}/{Capacity}", message.Url, _channel.Reader.Count, _channelCapacity);
                }
                else
                {
                    _logger.LogInformation("...ConsumePendingUrlsAsync Queue Full Skipped: {uri} queue: {Count}/{Capacity}", message.Url, _channel.Reader.Count, _channelCapacity);
                }
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError("OnMessage exception: {Message}", ex.Message);
            }
        }
    }

    public async Task ProcessDownloadQueueAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_channel.Reader.Count > 0)
            {
                var message = await _channel.Reader.ReadAsync(cancellationToken);
                _ = DownloadUrlAsync(message.Url, cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Prevent throwing if stoppingToken was signaled
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred executing task.");
        }
    }

    private async Task DownloadUrlAsync(string url, CancellationToken cancellationToken)
    {
        await Task.Run(async () =>
        {
            _logger.LogInformation("DownloadUrlAsync Downloading...: {uri}", url);
            await _urlDowloadService.DownloaFile(new Uri(url));
            _logger.LogInformation("...DownloadUrlAsync Downloaded: {uri}", url);
        }, cancellationToken);
    }
}
