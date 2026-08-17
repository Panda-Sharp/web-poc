using System.Threading;
using System.Threading.Tasks;

namespace Web.Poc.Domain.Shared.Queue;

public interface IItemQueue<T> where T : class
{
	int Count { get; }

	ValueTask QueueAsync(T message, CancellationToken cancellationToken);

	ValueTask<T?> DequeueAsync(CancellationToken cancellationToken);

	bool TryQueueAsync(T item);

	bool TryDequeueAsync(out T? item);
}