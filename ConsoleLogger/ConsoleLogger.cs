using System;

namespace Utility
{
    public class ConsoleLogger
    {
        public void Log(Severity severity, String what, DateTime when)
        {
            lock (Console.Out)
            {
                var previousForegroundColor = Console.ForegroundColor;
                var previousBackgroundColor = Console.BackgroundColor;
                Console.BackgroundColor = GetBackground(severity);
                Console.ForegroundColor = GetForeground(severity);
                Console.Write(' ');
                Console.Write(severity);
                Console.Write(' ');
                Console.ForegroundColor = previousForegroundColor;
                Console.BackgroundColor = previousBackgroundColor;
                Console.Write(' ');
                Console.Write(when);
                Console.Write(' ');
                Console.Write(what);
                Console.WriteLine();
            }
        }

        private ConsoleColor GetForeground(Severity severity)
        {
            switch (severity)
            {
                case Severity.Debug:
                    return ConsoleColor.DarkGray;
                case Severity.Information:
                case Severity.Warning:
                case Severity.Error:
                case Severity.Fatal:
                    return ConsoleColor.White;
                default:
                    throw new ArgumentException(nameof(severity));
            }
        }

        private ConsoleColor GetBackground(Severity severity)
        {
            switch (severity)
            {
                case Severity.Debug:
                    return ConsoleColor.Gray;
                case Severity.Information:
                    return ConsoleColor.Cyan;
                case Severity.Warning:
                    return ConsoleColor.DarkYellow;
                case Severity.Error:
                    return ConsoleColor.Red;
                case Severity.Fatal:
                    return ConsoleColor.Magenta;
                default:
                    throw new ArgumentException(nameof(severity));
            }
        }
    }
}
