using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Services.UrlDowload;

namespace Web.Poc.Application.Services.Redis;

public interface IRedisService
{
    Task StreamAddAsync(string streamKey, IEnumerable<UrlMessage> payload);

    IAsyncEnumerable<UrlMessage> StreamReadGroupAsync(string streamKey);

    IAsyncEnumerable<UrlMessage> StreamReadAndAcknowledgeGroupAsync(string streamKey, CancellationToken cancellationToken);
}
