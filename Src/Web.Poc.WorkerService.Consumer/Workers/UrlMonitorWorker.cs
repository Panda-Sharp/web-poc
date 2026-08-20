using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.WorkerService.Consumer.Services;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlMonitorWorker : BackgroundService
{
    private readonly IConsumerService _consumerService;
    private readonly ILogger<UrlMonitorWorker> _logger;

    public UrlMonitorWorker(
        IConsumerService consumerService,
        ILogger<UrlMonitorWorker> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlMonitorWorker Is running...");

        while (!cancellationToken.IsCancellationRequested)
        {
            await _consumerService.ConsumePendingUrlsAsync(cancellationToken);
        }
    }
}
