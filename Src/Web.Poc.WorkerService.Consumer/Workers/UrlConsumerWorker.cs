using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.WorkerService.Consumer.Services;

namespace Web.Poc.WorkerService.Consumer.Workers;

public class UrlConsumerWorker : BackgroundService
{
    private readonly IConsumerService _consumerService;
    private readonly ILogger<UrlConsumerWorker> _logger;

    public UrlConsumerWorker(
        IConsumerService consumerService,
        ILogger<UrlConsumerWorker> logger)
    {
        _consumerService = consumerService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlConsumerWorker Is running...");

        while (!cancellationToken.IsCancellationRequested)
        {
            await _consumerService.ConsumeNewUrlsAsync(cancellationToken);
        }
    }
}
