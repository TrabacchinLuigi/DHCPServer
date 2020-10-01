using System;
using System.Collections.Generic;
using System.Text;

namespace DHCP
{
    static class LoggerExtensions
    {
        public static void Log(this ILogger logger, String message, LoggingEventType loggingEventType)
            => logger.Log(new LogEntry(loggingEventType, message));

        public static void Log(this ILogger logger, String message)
            => logger.Log(new LogEntry(LoggingEventType.Information, message));

        public static void Log(this ILogger logger, Exception exception)
            => logger.Log(new LogEntry(LoggingEventType.Error, exception));

    }
}
