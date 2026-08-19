using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.WorkerService.Producer.Helpers;
using Web.Poc.WorkerService.Producer.Hubs;

namespace Web.Poc.WorkerService.Producer.Workers;

public class UrlProducerWorker : BackgroundService
{
    private readonly IHubContext<UrlHub, IUrl> _urlHub;
    private readonly ILogger<UrlProducerWorker> _logger;

    public UrlProducerWorker(
        IHubContext<UrlHub, IUrl> urlHub,
        ILogger<UrlProducerWorker> logger)
    {
        _urlHub = urlHub;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("UrlProducerWorker Is waiting for a client ...");

        while (!UrlHub.IsConnected)
        {
            await Task.Delay(100, cancellationToken);
        }

        _logger.LogInformation("UrlProducerWorker Is running ...");

        await TrySendUrlsAsync(cancellationToken);
    }

    private async Task TrySendUrlsAsync(CancellationToken cancellationToken)
    {
        Random rnd = new();
        int page = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            var urls = UrlsHelper.GetFromCsv(page);
            var urlsToLog = urls.Take(100).ToArray();

            _logger.LogInformation("Sending...: {urls}", urlsToLog);
            await _urlHub.Clients.All.OnAddUrls(urls); // DateTime.Now
            _logger.LogInformation("...Sent: {urls}", urlsToLog);

            page++;

            // this is to simulate batch of urls sent at diffent time
            await Task.Delay(rnd.Next(500, 1500), cancellationToken);
        }
    }
}
