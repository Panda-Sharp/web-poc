using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared;

namespace Web.Poc.WorkerService.Client;

public class WorkerConsumer : BackgroundService, IUrl
{
	// https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview
	private readonly BlockingCollection<Uri> _urlBlockingCollection = [];
	private readonly BlockingCollection<Uri> _urlDownloadBlockingCollection = new(10);


	// https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1

	private readonly IUrlDowloadService _urlDowloadService;
	private readonly ILogger<WorkerConsumer> _logger;
	private readonly HubConnection _connection;

	public WorkerConsumer(
		IUrlDowloadService urlDowloadService,
		ILogger<WorkerConsumer> logger)
	{
		_urlDowloadService = urlDowloadService;
		_logger = logger;

		_connection = new HubConnectionBuilder()
			.WithUrl(AppConstants.HubUrl)
			.Build();

		_connection.On<IEnumerable<string>>(AppConstants.UrlSentEvent, AddUrls);
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		// Loop is here to wait until the server is running
		while (true)
		{
			try
			{
				await _connection.StartAsync(cancellationToken);
				break;
			}
			catch
			{
				await Task.Delay(1000, cancellationToken);
			}
		}

		while (!_urlDownloadBlockingCollection.IsAddingCompleted &&
			   !_urlBlockingCollection.IsCompleted)
		{
			_urlBlockingCollection.TryTake(out var uri);
			if (uri == null)
			{
				continue;
			}

			_logger.Log("_urlDownloadBlockingCollection adding: {uri} ...", uri);
			_urlDownloadBlockingCollection.TryAdd(uri);
			_logger.Log("... url: {url} _urlDownloadBlockingCollection", uri);
		}

		while (!cancellationToken.IsCancellationRequested)
		{
			// A simple blocking consumer with no cancellation.
			await Task.Run(() =>
			{
				// Blocks if dataItems.Count == 0.
				// IOE means that Take() was called on a completed collection.
				// Some other thread can call CompleteAdding after we pass the
				// IsCompleted check but before we call Take.
				// In this example, we can simply catch the exception since the
				// loop will break on the next iteration.
				while (!_urlDownloadBlockingCollection.IsCompleted)
				{
					_urlDownloadBlockingCollection.TryTake(out var uri);
					if (uri == null)
					{
						continue;
					}

					_ = DownloadUrlAsync(uri);
				}

				_logger.Log("_urlBlockingCollection No more items to take.");

			}, cancellationToken);
		}
	}

	public async Task AddUrls(IEnumerable<string> urls)
	{
		foreach (var url in urls)
		{
			_logger.Log("_urlBlockingCollection adding: {url} ...", url);
			if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
			{
				_urlBlockingCollection.TryAdd(uri);
			}
			_logger.Log("... _urlBlockingCollection: {url} Added", url);
		}
	}

	private async Task DownloadUrlAsync(Uri uri)
	{
		_logger.Log("DownloadUrlAsync: {uri} ...", uri);

		Random rnd = new();
		await Task.Delay(rnd.Next(1000, 5000));
		//await _urlDowloadService.DownloaFile(url);

		_logger.Log("... DownloadUrlAsync: {uri} downloaded", uri);
	}

}
