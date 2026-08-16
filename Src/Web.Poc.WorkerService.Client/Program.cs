using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Web.Poc.Application.Extensions;
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

		//builder.Services.AddSingleton<ITaskQueue>(_ =>
		//{
		//	if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
		//	{
		//		queueCapacity = 100;
		//	}

		//	return new TaskQueue(queueCapacity);
		//});
		builder.Services.AddSingleton<ITaskQueue, TaskQueue>();
		builder.Services.AddHostedService<UrlDownloaderWorker>();

		var host = builder.Build();
		host.Run();
	}
}
