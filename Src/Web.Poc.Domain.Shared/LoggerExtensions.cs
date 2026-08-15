using Microsoft.Extensions.Logging;

namespace Web.Poc.Domain.Shared;

public static class LoggerExtensions
{
	public static void Log(this ILogger logger, string message)
	{
		if (logger.IsEnabled(LogLevel.Information))
		{	
			logger.LogInformation($"{DateTime.Now}: {message}");
		}
	}

	public static void Log(this ILogger logger, string message, params object[] args)
	{
		if (logger.IsEnabled(LogLevel.Information))
		{
			logger.LogInformation($"{DateTime.Now}: {message}", args);
		}
	}
}
