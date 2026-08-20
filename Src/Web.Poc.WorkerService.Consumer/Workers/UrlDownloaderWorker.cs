using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.WorkerService.Consumer.Services;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlDownloaderWorker : BackgroundService
{
    private readonly IConsumerService _consumerService;
    private readonly ILogger<UrlDownloaderWorker> _logger;

    public UrlDownloaderWorker(
        IConsumerService consumerService,
        ILogger<UrlDownloaderWorker> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlDownloaderWorker Is running...");

        while (!cancellationToken.IsCancellationRequested)
        {
            await _consumerService.ProcessDownloadQueueAsync(cancellationToken);
        }
    }
}
