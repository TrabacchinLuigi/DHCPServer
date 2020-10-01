using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace DHCP
{
    public class Server
    {
        public const UInt16 Port = 67;
        private readonly ILogger _logger;
        private readonly ServerOptions _serverOptions;
        private UInt32 _initialAddress;
        private readonly Dictionary<PhysicalAddress, (String State, IPAddress Ip, DateTime? BindTime)> Leasing
            = new Dictionary<PhysicalAddress, (String, IPAddress, DateTime?)>();

        public Server(ILogger logger, ServerOptions serverOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serverOptions = serverOptions ?? throw new ArgumentNullException(nameof(serverOptions));
            _initialAddress = Helpers.ToUint(serverOptions.NetworkAddress);
        }

        private UInt32 GetNextIp()
        {
            var next = ++_initialAddress;
            var bindedIp = Helpers.ToUint(_serverOptions.BindIp);
            if (next == bindedIp) next++;
            return next;
        }

        public  Task RunAsync(CancellationToken cancellationToken)
            => Task.Run(() => Run(cancellationToken));

        public void Run(CancellationToken cancellationToken)
        {
            //ThreadPool.
            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                //socket.Blocking = false;
                socket.Bind(new IPEndPoint(_serverOptions.BindIp, Port));
                var buffer = new Byte[576];
                var callerEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, 0);
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var readBytes = socket.ReceiveFrom(buffer, ref callerEndpoint);
                        if (Message.TryParse(new ReadOnlySpan<Byte>(buffer, 0, readBytes), out var message))
                        {
                            var messageType = message.Options[OptionKind.DhcpMessageType];
                            switch (messageType)
                            {
                                case MessageType.DISCOVER:
                                    HandleDiscover(message);
                                    break;
                                case MessageType.REQUEST:
                                    HandleRequest(message);
                                    break;
                                default:
                                    _logger.Log($"received a dhcp message of kind '{messageType}', this kind of message is not handled by current implementation", LoggingEventType.Warning);
                                    break;
                            }

                        }
                    }
                    catch (Exception ex) when (ex is SocketException)
                    { _logger.Log(ex); }
                }
            }
        }

        private void HandleDiscover(Message req)
        {
            _logger.Log($"Discover request received from {req.ClientHardwareAddress}");
            if (_serverOptions.ListenOnly) return;
            var ip = Helpers.ToIpAddress(GetNextIp());
            Leasing.TryAdd(req.ClientHardwareAddress, ("OFFERED", ip, null));
            SendOffer(req);
        }

        private void HandleRequest(Message req)
        {
            _logger.Log($"request received from {req.ClientHardwareAddress}");
            if (_serverOptions.ListenOnly) return;
            var oldLease = Leasing[req.ClientHardwareAddress];
            Leasing[req.ClientHardwareAddress] = (State: "Bound", oldLease.Ip, BindTime: DateTime.Now);

            SendAck(req);
        }

        private void SendOffer(Message req)
        {
            var offeringIp = Leasing[req.ClientHardwareAddress].Ip;

            _logger.Log($"offering {offeringIp} to {req.ClientHardwareAddress}");
            var answer = new Message()
            {
                Operation = Operation.BOOTREPLY,
                HardwareAddressType = HardwareAddressKind.Ethernet,
                HardwareLength = 6,
                Hops = 0,
                SessionId = req.SessionId,
                Seconds = 0,
                Flags = req.Flags,
                ClientIpAddress = IPAddress.Any,
                YourIpAddress = offeringIp,
                ServerIdentifierAddress = _serverOptions.BindIp,
                GatewayOrRelayAddress = req.GatewayOrRelayAddress,
                ClientHardwareAddress = req.ClientHardwareAddress,
                ServerHostName = "",
                BootFileName = ""
            };
            answer.Options.Add(OptionKind.DhcpMessageType, MessageType.OFFER);
            answer.Options.Add(OptionKind.SubnetMask, _serverOptions.NetworkMask);
            answer.Options.Add(OptionKind.Router, _serverOptions.Gateway);
            answer.Options.Add(OptionKind.DomainNameServer, new IPAddress[] { _serverOptions.PrimaryDns, _serverOptions.SecondaryDns });
            answer.Options.Add(OptionKind.IpAddressLeaseTime, Convert.ToUInt32(_serverOptions.LeaseTime.TotalSeconds));
            answer.Options.Add(OptionKind.ServerIdentifier, _serverOptions.BindIp);
            answer.Options.Add(OptionKind.ParameterRequestList, req.Options[OptionKind.ParameterRequestList]);

            Send(_serverOptions.BroadCastAddress, answer);

        }

        private void Send(IPAddress address, Message message)
        {
            _logger.Log($"Sending data to {address}");
            var buffer = new Byte[576];
            message.TryWriteTo(buffer);
            using (var socket = new Socket(SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTo(buffer, new IPEndPoint(address, Client.Port));
            }
        }

        private void SendAck(Message req)
        {
            var ackIp = Leasing[req.ClientHardwareAddress].Ip;
            _logger.Log($"Acknowledging {ackIp} to {req.ClientHardwareAddress}");
            var answer = new Message
            {
                Operation = Operation.BOOTREPLY,
                HardwareAddressType = HardwareAddressKind.Ethernet,
                HardwareLength = 6,
                Hops = 0,
                SessionId = req.SessionId,
                Seconds = 0,
                Flags = req.Flags,
                ClientIpAddress = req.ClientIpAddress,
                YourIpAddress = ackIp,
                ServerIdentifierAddress = _serverOptions.BindIp,
                GatewayOrRelayAddress = req.GatewayOrRelayAddress,
                ClientHardwareAddress = req.ClientHardwareAddress,
                ServerHostName = "",
                BootFileName = ""
            };
            answer.Options.Add(OptionKind.DhcpMessageType, MessageType.ACK);
            answer.Options.Add(OptionKind.SubnetMask, _serverOptions.NetworkMask);
            answer.Options.Add(OptionKind.Router, _serverOptions.Gateway);
            answer.Options.Add(OptionKind.DomainNameServer, new IPAddress[] { _serverOptions.PrimaryDns, _serverOptions.SecondaryDns });
            answer.Options.Add(OptionKind.IpAddressLeaseTime, Convert.ToUInt32(_serverOptions.LeaseTime.TotalSeconds));
            answer.Options.Add(OptionKind.ServerIdentifier, _serverOptions.BindIp);
            answer.Options.Add(OptionKind.ParameterRequestList, req.Options[OptionKind.ParameterRequestList]);
        }
    }
}