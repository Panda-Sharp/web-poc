using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared.Extensions;
using Web.Poc.Domain.Shared.Queue;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlConsumerWorker : BackgroundService, IUrl
{
    private readonly IItemQueue<Uri> _urlQueue;
    private readonly ILogger<UrlConsumerWorker> _logger;
    private readonly HubConnection _connection;

    public UrlConsumerWorker(
        IItemQueue<Uri> urlQueue,
        ILogger<UrlConsumerWorker> logger)
    {
        _urlQueue = urlQueue;
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl(AppConstants.HubConsumerUrl)
            .Build();

        _connection.On<IEnumerable<string>>(AppConstants.UrlSentEvent, OnAddUrls);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.Log(typeof(UrlConsumerWorker), "Is running...");

        await TryToConnectAsync(cancellationToken);
    }

    private async Task TryToConnectAsync(CancellationToken cancellationToken)
    {
        // Loop is here to wait until the server is running
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.Log(typeof(UrlConsumerWorker), "Connecting...");
                await _connection.StartAsync(cancellationToken);
                _logger.Log(typeof(UrlConsumerWorker), "...Connected");
                break;
            }
            catch
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task OnAddUrls(IEnumerable<string> urls)
    {
        foreach (var url in urls)
        {
            _logger.Log(typeof(UrlConsumerWorker), "Adding...: {url}", [url]);
            if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                _ = _urlQueue.QueueAsync(uri, CancellationToken.None);
            }
            _logger.Log(typeof(UrlConsumerWorker), "...Added: {url}", [url]);
        }
    }
}
