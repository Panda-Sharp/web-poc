using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Infrastructure.Queue;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlDownloaderWorker : BackgroundService
{
    private readonly ITaskQueue _urlDownloadTaskQueue;
    private readonly ILogger<UrlDownloaderWorker> _logger;

    public UrlDownloaderWorker(
        ITaskQueue urlDownloadTaskQueue,
        ILogger<UrlDownloaderWorker> logger)
    {
        _urlDownloadTaskQueue = urlDownloadTaskQueue;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlDownloaderWorker Is running...");

        return ProcessTaskQueueAsync(cancellationToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_urlDownloadTaskQueue.TryDequeueAsync(out var downloadUrlAsync))
                {
                    _logger.LogInformation("TryDequeueAsync...:");
                    downloadUrlAsync?.Invoke(cancellationToken);
                    _logger.LogInformation("...TryDequeueAsync");
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
    }
}
