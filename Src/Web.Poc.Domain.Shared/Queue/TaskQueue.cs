using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Web.Poc.Domain.Shared.Queue;

// https://learn.microsoft.com/en-us/dotnet/api/system.collections.concurrent.concurrentqueue-1
// https://learn.microsoft.com/en-us/dotnet/standard/collections/thread-safe/blockingcollection-overview
// https://learn.microsoft.com/en-us/dotnet/core/extensions/channels
// https://learn.microsoft.com/en-us/dotnet/core/extensions/queue-service
public class TaskQueue : ITaskQueue
{
	private readonly Channel<Func<CancellationToken, ValueTask>> _channel;

	//private readonly BlockingCollection<Uri> _urlBlockingCollection = [];
	//private readonly BlockingCollection<Uri> _urlDownloadBlockingCollection = new(10);

	public int Capacity { get; }

	public int Count => _channel.Reader.Count;

	public bool IsFull => Count >= Capacity;

	public TaskQueue(int capacity)
	{
		Capacity = capacity;
		BoundedChannelOptions options = new(capacity)
		{
			FullMode = BoundedChannelFullMode.Wait
		};
		_channel = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
	}

	public async ValueTask QueueAsync(
		Func<CancellationToken, ValueTask> item,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(item);

		await _channel.Writer.WriteAsync(item, cancellationToken);
	}

	public async ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(
		CancellationToken cancellationToken)
	{
		Func<CancellationToken, ValueTask>? item =
			await _channel.Reader.ReadAsync(cancellationToken);

		return item;
	}

	public bool TryQueueAsync(
		Func<CancellationToken, ValueTask> item)
	{
		ArgumentNullException.ThrowIfNull(item);

		return _channel.Writer.TryWrite(item);
	}

	public bool TryDequeueAsync(out Func<CancellationToken, ValueTask>? item)
	{
		return _channel.Reader.TryRead(out item);
	}
}
