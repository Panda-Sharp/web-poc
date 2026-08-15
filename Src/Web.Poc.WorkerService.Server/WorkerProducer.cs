using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.WorkerService.Server;

public class WorkerProducer : BackgroundService
{
    private static readonly string[] Urls =
    [
        "https://github.com/Panda-Sharp/web-poc",
        "https://raw.githubusercontent.com/reactiveui/refit/main/images/logo.png"
    ];

    private readonly IHubContext<UrlHub, IUrl> _clockHub;
    private readonly ILogger<WorkerProducer> _logger;

    public WorkerProducer(
        IHubContext<UrlHub, IUrl> clockHub,
        ILogger<WorkerProducer> logger)
    {
        _clockHub = clockHub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            foreach (var url in Urls)
            {
                await _clockHub.Clients.All.ShowUrl(url); // DateTime.Now
                await Task.Delay(1000, cancellationToken);
            }

            await Task.Delay(1000, cancellationToken);
        }
    }
}
