using System.Threading;
using System.Threading.Tasks;

namespace Web.Poc.WorkerService.Consumer.Services
{
    public interface IConsumerService
    {
        Task ConsumeNewUrlsAsync(CancellationToken cancellationToken);

        Task ConsumePendingUrlsAsync(CancellationToken cancellationToken);

        Task ProcessDownloadQueueAsync(CancellationToken cancellationToken);
    }
}