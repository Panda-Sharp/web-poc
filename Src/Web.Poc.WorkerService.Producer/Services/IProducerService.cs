using System.Collections.Generic;
using System.Threading.Tasks;
using Web.Poc.Application.Services.UrlDowload;

namespace Web.Poc.WorkerService.Producer.Services
{
    public interface IProducerService
    {
        IAsyncEnumerable<UrlMessage> ConsumeAsync(string key);
        Task PublishAsync();
    }
}