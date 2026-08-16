using Bogus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;
using Web.Poc.Domain.Shared;

namespace Web.Poc.WorkerService.Server;

public class WorkerProducer : BackgroundService
{
	private static readonly string[] Urls =
	[
		"https://github.com/Panda-Sharp/web-poc",
		"https://raw.githubusercontent.com/reactiveui/refit/main/images/logo.png"
	];

	private readonly IHubContext<UrlHub, IUrl> _clockHub;
	private readonly ILogger<WorkerProducer> _logger;

	public WorkerProducer(
		IHubContext<UrlHub, IUrl> clockHub,
		ILogger<WorkerProducer> logger)
	{
		_clockHub = clockHub;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken cancellationToken)
	{
		while (!cancellationToken.IsCancellationRequested)
		{
			_logger.Log("Worker running ...");

			for (int i = 0; i < 10; i++)
			{
				var faker = new Faker();
				var urls = Enumerable.Range(1, 5)
				  .Select(_ => faker.Internet.UrlWithPath());

				await _clockHub.Clients.All.OnAddUrls(urls); // DateTime.Now
				await Task.Delay(1000, cancellationToken);
			}
		}
	}
}
