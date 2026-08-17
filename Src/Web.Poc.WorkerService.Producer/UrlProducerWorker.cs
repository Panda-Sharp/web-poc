using Bogus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared.Extensions;

namespace Web.Poc.WorkerService.Server;

public class UrlProducerWorker : BackgroundService
{
    private static readonly string[] Urls =
    [
        "https://github.com/Panda-Sharp/web-poc",
        "https://raw.githubusercontent.com/reactiveui/refit/main/images/logo.png"
    ];

    private readonly IHubContext<UrlHub, IUrl> _clockHub;
    private readonly ILogger<UrlProducerWorker> _logger;

    public UrlProducerWorker(
        IHubContext<UrlHub, IUrl> clockHub,
        ILogger<UrlProducerWorker> logger)
    {
        _clockHub = clockHub;
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
        while (!cancellationToken.IsCancellationRequested)
        {
            var urls = GetUrls();

            _logger.Log(typeof(UrlProducerWorker), "Sending...: {urls}", [string.Join(",",urls)]);
            await _clockHub.Clients.All.OnAddUrls(urls); // DateTime.Now
            _logger.Log(typeof(UrlProducerWorker), "...Sent: {urls}", [string.Join(",", urls)]);

            await Task.Delay(rnd.Next(500, 1500));
        }
    }

    private IEnumerable<string> GetUrls()
    {
        var faker = new Faker();
        var urls = Enumerable.Range(1, 10)
          .Select(_ => faker.Internet.UrlWithPath());


        return urls;
    }
}
