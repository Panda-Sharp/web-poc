using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Infrastructure.Queue;

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
            .WithUrl(AppConstants.HubConnection)
            .Build();

        _connection.On<IEnumerable<string>>(AppConstants.UrlSentEvent, OnAddUrls);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlConsumerWorker Is running...");

        await TryToConnectAsync(cancellationToken);
    }

    private async Task TryToConnectAsync(CancellationToken cancellationToken)
    {
        // Loop is here to wait until the server is running
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Connecting...");
                // TODO: There can be case when connection dropped and requires reconnect.
                // Not sure if start covers it (don't think so),
                // so it's worth to check if reconnect logic is required too.
                await _connection.StartAsync(cancellationToken);
                _logger.LogInformation("...Connected");
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
            _logger.LogInformation("Adding...: {url}", url);
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out var uri))
            {
                _ = _urlQueue.QueueAsync(uri, CancellationToken.None);
            }
            _logger.LogInformation("...Added: {url}", url);
        }
    }
}
