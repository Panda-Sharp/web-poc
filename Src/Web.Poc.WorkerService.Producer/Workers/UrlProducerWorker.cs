using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared.Extensions;
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
        _logger.Log(typeof(UrlProducerWorker), "Is running ...");

        await TrySendUrlsAsync(cancellationToken);
    }

    private async Task TrySendUrlsAsync(CancellationToken cancellationToken)
    {
        Random rnd = new();
        int page = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            //var urls = UrlsHelper.GetFromFaker();
            var urls = UrlsHelper.GetFromCsv(page);
            var urlsText = string.Join(",", urls.Take(100));

            _logger.Log(typeof(UrlProducerWorker), "Sending...: {urls}", [urlsText]);
            await _urlHub.Clients.All.OnAddUrls(urls); // DateTime.Now
            _logger.Log(typeof(UrlProducerWorker), "...Sent: {urls}", [urlsText]);

            page++;

            await Task.Delay(rnd.Next(500, 1500), cancellationToken);
        }
    }
}
