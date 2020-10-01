using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DHCP
{
    public class Client
    {
        public const UInt16 Port = 68;
        private readonly ILogger _logger;
        private readonly ClientOptions _options;

        public Client(ILogger logger, ClientOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        public Task RunAsync(CancellationToken cancellationToken)
            => Task.Run(() => Run(cancellationToken));

        public void Run(CancellationToken cancellationToken)
        {
            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.Bind(new IPEndPoint(_options.BindTo, Port));
                var buffer = new Byte[1024];
                var callerEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                while (!cancellationToken.IsCancellationRequested)
                {
                    var readBytes = socket.ReceiveFrom(buffer, ref callerEndpoint);
                    try
                    {
                        for (var i = 0; i < readBytes; i++)
                        {
                            Console.Write(buffer[i]);
                            Console.Write(' ');
                        }
                        Console.WriteLine();
                    }
                    catch (Exception ex) when (ex is SocketException)
                    {
                        _logger.Log(ex);
                    }
                }
            }
        }
    }
}
