using System;
using System.Collections.Generic;
using System.Text;

namespace DHCP
{
    public interface ILogger
    {
        void Log(LogEntry entry);
    }
}
