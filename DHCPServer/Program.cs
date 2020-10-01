using System;
using System.Net.Sockets;
using System.Net;
using Utility;

namespace DHCPServer
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Server!");

            var consoleLogger = new ConsoleLogger();
            try
            {
                var loggerAdapter = new ConsoleLoggerAdapter(consoleLogger);
                var server = new DHCP.Server(
                    loggerAdapter,
                    new DHCP.ServerOptions(
                        IPAddress.Parse("192.168.2.1"),
                        IPAddress.Parse("255.255.255.0"),
                        IPAddress.Parse("192.168.2.1"),
                        IPAddress.Parse("8.8.8.8"),
                        IPAddress.Parse("8.8.4.4"),
                        null, 
                        false
                    )
                );
                using (var cts = new System.Threading.CancellationTokenSource())
                {
                    var mainTask = server.RunAsync(cts.Token);
                    while (true)
                    {
                        var keyread = Console.ReadKey();
                        if (keyread.Key == ConsoleKey.Escape)
                        {
                            consoleLogger.Log(Severity.Information, "ShuttingDown", DateTime.Now);
                            cts.Cancel();
                            mainTask.Wait();
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                consoleLogger.Log(Severity.Fatal, ex.ToString(), DateTime.Now);
                throw;
            }
        }
    }
}
