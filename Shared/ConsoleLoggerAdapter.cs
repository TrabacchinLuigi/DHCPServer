using System;

namespace Utility
{
    public class ConsoleLoggerAdapter : DHCP.ILogger
    {
        private readonly ConsoleLogger _adapted;

        public ConsoleLoggerAdapter(ConsoleLogger adapted)
        {
            _adapted = adapted ?? throw new ArgumentNullException(nameof(adapted));
        }

        public void Log(DHCP.LogEntry entry)
        {
            if (entry is null) throw new ArgumentNullException(nameof(entry));
            _adapted.Log((Severity)entry.Severity, entry.What.ToString(), entry.When);
        }
    }
}