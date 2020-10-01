using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace DHCP
{
    public class ServerOptions
    {
        public Boolean ListenOnly { get; }
        public TimeSpan LeaseTime { get; }
        public IPAddress BindIp { get; }
        public IPAddress NetworkMask { get; }
        public IPAddress Gateway { get; }
        public IPAddress NetworkAddress { get; }
        public IPAddress BroadCastAddress { get; }
        public IPAddress PrimaryDns { get; }
        public IPAddress SecondaryDns { get; }

        public ServerOptions(IPAddress bindIp, IPAddress networkMask, IPAddress gateway, IPAddress primaryDns, IPAddress secondaryDns, TimeSpan? leaseTime = null, Boolean listenOnly = false)
        {
            BindIp = bindIp;
            NetworkMask = networkMask;
            Gateway = gateway;
            PrimaryDns = primaryDns;
            SecondaryDns = secondaryDns;
            ListenOnly = listenOnly;
            NetworkAddress = Helpers.GetNetworkAddress(BindIp, NetworkMask);
            BroadCastAddress = Helpers.GetBroadcastAddress(NetworkAddress, NetworkMask);
            if (leaseTime == null)
                LeaseTime = TimeSpan.FromHours(1);
            if (LeaseTime.TotalSeconds > UInt16.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(LeaseTime), $"{nameof(leaseTime)} should be lower than {TimeSpan.FromSeconds(UInt16.MaxValue)}");
        }

    }
}
