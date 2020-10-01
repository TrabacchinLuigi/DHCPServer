using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DHCP
{
    public static class Helpers
    {
        public static IPAddress GetBroadcastAddress(IPAddress networkAddress, IPAddress subnetMask)
        {
            if (networkAddress is null) throw new ArgumentNullException(nameof(networkAddress));
            if (subnetMask is null) throw new ArgumentNullException(nameof(subnetMask));
            var ipAdressBytes = networkAddress.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new Byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (Byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        public static IPAddress GetNetworkAddress(IPAddress address, IPAddress subnetMask)
        {
            if (address is null) throw new ArgumentNullException(nameof(address));
            if (subnetMask is null) throw new ArgumentNullException(nameof(subnetMask));
            var ipAdressBytes = address.GetAddressBytes();
            var subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            var broadcastAddress = new Byte[ipAdressBytes.Length];
            for (var i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (Byte)(ipAdressBytes[i] & (subnetMaskBytes[i]));
            }
            return new IPAddress(broadcastAddress);
        }

        public static UInt32 GetNetworkLegth(IPAddress subnetMask)
        {
            if (subnetMask is null) throw new ArgumentNullException(nameof(subnetMask));
            var wildCardBytes = subnetMask.GetAddressBytes()
                 .Select(x => (Byte)(x ^ 255));
            if (BitConverter.IsLittleEndian)
                wildCardBytes = wildCardBytes.Reverse();
            var length = BitConverter.ToUInt32(wildCardBytes.ToArray(), 0);
            return length + 1;
        }

        public static IPAddress ToIpAddress(UInt32 num)
        {
            var bytes = BitConverter.GetBytes(num);
            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        public static UInt32 ToUint(IPAddress address)
        {
            if (address is null) throw new ArgumentNullException(nameof(address));
            var bytes = address.GetAddressBytes().AsEnumerable();
            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse();
            return BitConverter.ToUInt32(bytes.ToArray(), 0);
        }
    }
}
