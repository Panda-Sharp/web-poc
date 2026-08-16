using System;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Poc.Domain.Shared.Queue;

public interface ITaskQueue
{
	int Count { get; }

	ValueTask QueueAsync(Func<CancellationToken, ValueTask> item, CancellationToken cancellationToken);

	ValueTask<Func<CancellationToken, ValueTask>> DequeueAsync(CancellationToken cancellationToken);
}
