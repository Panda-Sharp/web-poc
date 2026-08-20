using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System;
using Web.Poc.Application.Contracts;
using Web.Poc.Application.Services.Redis;
using Web.Poc.Application.Services.UrlDowload;

namespace Web.Poc.Application.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddHttpClient()
            .AddTransient<IUrlDowloadService, UrlDowloadService>();
    }

    public static IServiceCollection AddRedisPubSub(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration["REDIS_CONNECTION_STRING"];
        if (string.IsNullOrWhiteSpace(redisConnectionString))
        {
            throw new ArgumentException("The app hasn't been configured for Redis yet.");
        }

        var connection = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(connection);
        services.AddScoped<IRedisService, RedisService>();
        return services;
    }
}
