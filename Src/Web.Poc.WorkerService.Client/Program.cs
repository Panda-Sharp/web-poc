using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Poc.Application.Extensions;

namespace Web.Poc.WorkerService.Client;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services.AddApplication(builder.Configuration);
        builder.Services.AddHostedService<WorkerConsumer>();

        var host = builder.Build();

        host.Run();
    }
}
