using System;

namespace DHCP
{
    // Immutable DTO that contains the log information.
    public class LogEntry
    {
        public LoggingEventType Severity { get; }
        public Object What { get; }
        public DateTime When { get; }

        public LogEntry(LoggingEventType severity, Object message, DateTime? when = null)
        {
            Severity = severity;
            What = message ?? throw new ArgumentNullException(nameof(message));
            When = when ?? DateTime.Now;
        }
    }
}