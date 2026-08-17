using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Poc.Domain.Shared.Queue;

public interface ITaskQueue
{
	int Capacity { get; }

	int Count { get; }
	
	bool IsFull { get; }

	ValueTask QueueAsync(Func<CancellationToken, ValueTask> item, CancellationToken cancellationToken);

	ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
	
	bool TryQueueAsync(Func<CancellationToken, ValueTask> item);
	
	bool TryDequeueAsync(out Func<CancellationToken, ValueTask>? item);
}
