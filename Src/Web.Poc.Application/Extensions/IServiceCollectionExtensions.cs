using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Web.Poc.Application.Contracts;

namespace Web.Poc.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpClient()
            .AddTransient<IUrlDowloadService, UrlDowloadService>();
    }
}
