using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Web.Poc.Domain.Shared.Extensions;

public static class LoggerExtensions
{
    private static void Log(this ILogger logger, string message)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation($"{DateTime.Now}: {message}");
        }
    }

    private static void Log(this ILogger logger, string message, params object[] args)
    {
        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation($"{DateTime.Now}: {message}", args);
        }
    }

    public static void Log(
        this ILogger logger,
        Type callerType,
        string message,
        object[]? args = null,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerPath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string path = callerType?.Name ?? Path.GetFileName(callerPath);
        string logMessage = $"[{path} / {callerMember ?? string.Empty}: {lineNumber}] {message ?? string.Empty}";

        if (args == null)
        {
            Log(logger, logMessage);
        }
        else
        {
            Log(logger, logMessage, args);
        }
    }

    public static void LogError(
        this ILogger logger,
        Type callerType,
        string message,
        Exception exception,
        object[]? args = null,
        [CallerMemberName] string callerMember = "",
        [CallerFilePath] string callerPath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        string path = callerType?.Name ?? Path.GetFileName(callerPath);
        string logMessage = $"[{path} / {callerMember ?? string.Empty}: {lineNumber}] {message ?? string.Empty} = ex: {exception?.Message ?? string.Empty}";

        if (args == null)
        {
            Log(logger, logMessage);
        }
        else
        {
            Log(logger, logMessage, args);
        }
    }
}
