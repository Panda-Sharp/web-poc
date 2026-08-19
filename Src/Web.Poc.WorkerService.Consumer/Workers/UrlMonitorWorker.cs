using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Infrastructure.Queue;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlMonitorWorker : BackgroundService
{
    private readonly IItemQueue<Uri> _urlQueue;
    private readonly ITaskQueue _urlDownloadTaskQueue;
    private readonly IUrlDowloadService _urlDowloadService;
    private readonly ILogger<UrlMonitorWorker> _logger;

    public UrlMonitorWorker(
        IItemQueue<Uri> urlQueue,
        ITaskQueue urlDownloadTaskQueue,
        ILogger<UrlMonitorWorker> logger,
        IUrlDowloadService urlDowloadService)
    {
        _urlQueue = urlQueue;
        _urlDownloadTaskQueue = urlDownloadTaskQueue;
        _urlDowloadService = urlDowloadService;
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
                var uri = await _urlQueue.DequeueAsync(cancellationToken);
                if (uri == null)
                {
                    continue;
                }
                _logger.LogInformation("Try Adding...: {uri}", uri);

                if (!_urlDownloadTaskQueue.TryQueueAsync((ct) => DownloadUrlAsync(uri, ct)))
                {
                    continue;
                }
                _logger.LogInformation("...Added: {uri}", uri);
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


    private async ValueTask DownloadUrlAsync(Uri uri, CancellationToken cancellationToken)
    {
        // TODO: delete me
        //Random rnd = new();
        while (!cancellationToken.IsCancellationRequested)
        {
            // A simple blocking consumer with no cancellation.
            await Task.Run(async () =>
            {
                _logger.LogInformation("Downloading...: {uri}", uri);

                // TODO: delete me
                //await Task.Delay(rnd.Next(1000, 5000));

                await _urlDowloadService.DownloaFile(uri);

                _logger.LogInformation("...Downloaded: {uri}", uri);

            }, cancellationToken);
        }
    }
}
