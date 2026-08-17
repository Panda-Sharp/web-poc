using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
}
