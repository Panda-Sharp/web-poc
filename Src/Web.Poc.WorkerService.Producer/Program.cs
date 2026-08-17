using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Web.Poc.Application.Contracts;
using Web.Poc.WorkerService.Producer.Hubs;
using Web.Poc.WorkerService.Producer.Workers;

namespace Web.Poc.WorkerService.Producer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddSignalR();
        builder.Services.AddHostedService<UrlProducerWorker>();

        var app = builder.Build();

        app.MapHub<UrlHub>(AppConstants.HubProducerUrl);

        app.Run();
    }
}
