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

	public int Count => _channel.Reader.Count;

	public TaskQueue(int capacity)
	{
		BoundedChannelOptions options = new(capacity)
		{
			FullMode = BoundedChannelFullMode.Wait
		};
		_channel = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);
	}

	public TaskQueue()
	{
		_channel = Channel.CreateUnbounded<Func<CancellationToken, ValueTask>>();
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
}
