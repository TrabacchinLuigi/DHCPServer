using System;
using System.Net;
using System.Net.Sockets;
using Utility;

namespace DHCPClient
{
    class Program
    {
        static void Main(String[] args)
        {
            Console.WriteLine("Client!");

            var consoleLogger = new ConsoleLogger();
            try
            {
                var loggerAdapter = new ConsoleLoggerAdapter(consoleLogger);
                var client = new DHCP.Client(
                    loggerAdapter,
                    new DHCP.ClientOptions(IPAddress.Any)
                );

                using (var cts = new System.Threading.CancellationTokenSource())
                {
                    var mainTask = client.RunAsync(cts.Token);
                    while (true)
                    {
                        if (mainTask.IsFaulted) throw mainTask.Exception;
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
