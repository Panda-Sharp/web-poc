using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared;
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
		_logger.LogInformation("{Name} is running...", nameof(UrlDownloaderWorker));

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

				_logger.Log("_urlDownloadBlockingCollection adding: {uri} ...", uri);

				await _urlDownloadTaskQueue.QueueAsync((cancellationToken) => DownloadUrlAsync(cancellationToken, uri), cancellationToken);

				_logger.Log("... url: {url} _urlDownloadBlockingCollection", uri);
			}
			catch (OperationCanceledException)
			{
				// Prevent throwing if stoppingToken was signaled
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred executing task work item.");
			}
		}
	}


	private async ValueTask DownloadUrlAsync(CancellationToken cancellationToken, Uri uri)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_logger.Log("DownloadUrlAsync: {uri} ...", uri);

			// A simple blocking consumer with no cancellation.
			await Task.Run(async () =>
			{
				_logger.Log("DownloadUrlAsync: {uri} ...", uri);

				Random rnd = new();
				await Task.Delay(rnd.Next(1000, 5000));
				//await _urlDowloadService.DownloaFile(url);

				_logger.Log("... DownloadUrlAsync: {uri} downloaded", uri);

			}, cancellationToken);
		}
	}
}
