using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Web.Poc.Domain.Shared.Queue;

namespace Web.Poc.Domain.Shared.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddTaskQueue(this IServiceCollection services, IConfiguration configuration)
    {
        if (!int.TryParse(configuration["QueueCapacity"], out var queueCapacity))
        {
            queueCapacity = 100;
        }

        services
            .AddSingleton<ITaskQueue>(_ => new TaskQueue(queueCapacity));
    }

    public static void AddApplicationLogging(this IServiceCollection services, IConfiguration configuration)
    {
        var logFileName = configuration["LogFileName"];
        if (string.IsNullOrWhiteSpace(logFileName))
        {
            logFileName = "myapp.log";
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File($"logs/{logFileName}", 
                          rollingInterval: RollingInterval.Day, 
                          restrictedToMinimumLevel: LogEventLevel.Warning)
            .CreateLogger();

        services
            .AddLogging(loggingBuilder => loggingBuilder
                            .ClearProviders() 
                            .AddSerilog(dispose: true));
    }
}
