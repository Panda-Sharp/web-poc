using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Web.Poc.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddSerilog(this IServiceCollection services, IConfiguration configuration)
    {
        var logFileName = configuration["LogFileName"];
        if (string.IsNullOrWhiteSpace(logFileName))
        {
            logFileName = "mylog";
        }

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {SourceContext} {Message} " +
                                             "{NewLine}{Exception}")
            .WriteTo.File($"logs/{logFileName}.log",
                          rollingInterval: RollingInterval.Day,
                          restrictedToMinimumLevel: LogEventLevel.Warning)
            .CreateLogger();

        services
            .AddLogging(loggingBuilder => loggingBuilder
            .ClearProviders()
            .AddSerilog(dispose: true));
    }
}
