using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Web.Poc.WorkerService.Server;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder.Services.AddSignalR();
		builder.Services.AddHostedService<WorkerProducer>();

		var app = builder.Build();

		app.MapHub<UrlHub>("/hubs/clock");

		app.Run();
	}
}
