using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Poc.Application.Extensions;
using Web.Poc.Infrastructure.Extensions;
using Web.Poc.WorkerService.Consumer.Services;
using Web.Poc.WorkerService.Consumer.Workers;

namespace Web.Poc.WorkerService.Consumer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddSerilog(builder.Configuration);
        builder.Services.AddRedisPubSub(builder.Configuration);

        builder.Services.AddSingleton<IConsumerService, ConsumerService>();
        builder.Services.AddHostedService<UrlConsumerWorker>();
        builder.Services.AddHostedService<UrlMonitorWorker>();
        builder.Services.AddHostedService<UrlDownloaderWorker>();

        var host = builder.Build();
        host.Run();
    }
}
