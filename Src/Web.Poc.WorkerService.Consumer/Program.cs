using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Web.Poc.Application.Extensions;
using Web.Poc.Domain.Shared.Extensions;
using Web.Poc.Domain.Shared.Queue;

namespace Web.Poc.WorkerService.Client;

public class Program
{
	public static void Main(string[] args)
	{
		var builder = Host.CreateApplicationBuilder(args);

		builder.Services.AddApplication(builder.Configuration);

		builder.Services.AddSingleton<IItemQueue<Uri>, ItemQueue<Uri>>();
		builder.Services.AddHostedService<UrlConsumerWorker>();

		builder.Services.AddTaskQueue(builder.Configuration);
		builder.Services.AddHostedService<UrlDownloaderWorker>();

		var host = builder.Build();
		host.Run();
	}
}
