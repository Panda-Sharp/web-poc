using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared.Extensions;
using Web.Poc.Domain.Shared.Queue;

namespace Web.Poc.WorkerService.Client;

public class UrlDownloaderWorker : BackgroundService
{
	private readonly IItemQueue<Uri> _urlQueue;
	private readonly ITaskQueue _urlDownloadTaskQueue;
	private readonly IUrlDowloadService _urlDowloadService;
	private readonly ILogger<UrlDownloaderWorker> _logger;

	public UrlDownloaderWorker(
		IItemQueue<Uri> urlQueue,
		ITaskQueue urlDownloadTaskQueue,
		IUrlDowloadService urlDowloadService,
		ILogger<UrlDownloaderWorker> logger)
	{
		_urlQueue = urlQueue;
		_urlDownloadTaskQueue = urlDownloadTaskQueue;
		_urlDowloadService = urlDowloadService;
		_logger = logger;
	}

	protected override Task ExecuteAsync(CancellationToken cancellationToken)
	{
		_logger.Log(typeof(UrlDownloaderWorker), "Is running...");

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

				_logger.Log(typeof(UrlDownloaderWorker), "Adding...: {uri}", [uri]);

				await _urlDownloadTaskQueue.QueueAsync((cancellationToken) => DownloadUrlAsync(cancellationToken, uri), cancellationToken);

				_logger.Log(typeof(UrlDownloaderWorker), "...Added: {uri}", [uri]);
			}
			catch (OperationCanceledException)
			{
				// Prevent throwing if stoppingToken was signaled
			}
			catch (Exception ex)
			{
				_logger.LogError(typeof(UrlDownloaderWorker), "Error occurred executing task work item.", ex);
			}
		}
	}


	private async ValueTask DownloadUrlAsync(CancellationToken cancellationToken, Uri uri)
    {
        Random rnd = new();
        while (!cancellationToken.IsCancellationRequested)
		{
			// A simple blocking consumer with no cancellation.
			await Task.Run(async () =>
			{
				_logger.Log(typeof(UrlDownloaderWorker), "Downloading...: {uri}", [uri]);

				await Task.Delay(rnd.Next(1000, 5000));
				//await _urlDowloadService.DownloaFile(url);

				_logger.Log(typeof(UrlDownloaderWorker), "...Downloaded: {uri}", [uri]);

			}, cancellationToken);
		}
	}
}
