using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Web.Poc.Domain.Shared.Queue;

public class ItemQueue<T> : IItemQueue<T> where T : class
{
	private readonly Channel<T> _channel;

	public int Count => _channel.Reader.Count;

	public ItemQueue(int capacity)
	{
		BoundedChannelOptions options = new(capacity)
		{
			FullMode = BoundedChannelFullMode.Wait
		};
		_channel = Channel.CreateBounded<T>(options);
	}

	public ItemQueue()
	{
		_channel = Channel.CreateUnbounded<T>();
	}

	public async ValueTask QueueAsync(
		T item,
		CancellationToken cancellationToken)
	{
		ArgumentNullException.ThrowIfNull(item);

		await _channel.Writer.WriteAsync(item, cancellationToken);
	}

	public async ValueTask<T?> DequeueAsync(CancellationToken cancellationToken)
	{
		var item = await _channel.Reader.ReadAsync(cancellationToken);

		return item;
	}
}
