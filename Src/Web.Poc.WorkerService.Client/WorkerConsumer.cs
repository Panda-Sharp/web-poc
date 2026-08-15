using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Web.Poc.Application.Contracts;

namespace Web.Poc.WorkerService.Client;

public class WorkerConsumer : BackgroundService, IUrl
{
    private readonly IUrlDowloadService _urlDowloadService;
    private readonly ILogger<WorkerConsumer> _logger;
    private readonly HubConnection _connection;

    public WorkerConsumer(
        IUrlDowloadService urlDowloadService,
        ILogger<WorkerConsumer> logger)
    {
        _urlDowloadService = urlDowloadService;
        _logger = logger;

        _connection = new HubConnectionBuilder()
            .WithUrl(AppConstants.HubUrl)
            .Build();

        _connection.On<string>(AppConstants.UrlSentEvent, ShowUrl);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Loop is here to wait until the server is running
        while (true)
        {
            try
            {
                await _connection.StartAsync(cancellationToken);
                break;
            }
            catch
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    public async Task ShowUrl(string url)
    {
        _logger.LogInformation("{url}", url); // DateTime.Now

        await _urlDowloadService.DownloaFile(url);
    }
}
