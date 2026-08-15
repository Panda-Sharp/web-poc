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

            await _clockHub.Clients.All.ShowUrl(DateTime.Now.ToString());
            await Task.Delay(1000, cancellationToken);
        }
    }
}
