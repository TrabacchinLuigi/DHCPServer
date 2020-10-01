using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;

namespace DHCP
{
    class Message
    {
        public Operation Operation { get; set; }
        /// hardware addr type= 1 for 10mb ethernet 
        public HardwareAddressKind HardwareAddressType { get; set; }
        public Byte HardwareLength { get; set; }
        public Byte Hops { get; set; }
        public UInt32 SessionId { get; set; }
        public UInt16 Seconds { get; set; }
        public UInt16 Flags { get; set; }
        public IPAddress ClientIpAddress { get; set; }
        public IPAddress YourIpAddress { get; set; }
        public IPAddress ServerIdentifierAddress { get; set; }
        public IPAddress GatewayOrRelayAddress { get; set; }
        public PhysicalAddress ClientHardwareAddress { get; set; }
        public String ServerHostName { get; set; }
        public String BootFileName { get; set; }
        public UInt32 MagicCookie { get; set; }
        public Dictionary<OptionKind, Object> Options { get; } = new Dictionary<OptionKind, Object>();

        public static Boolean  TryParse(ReadOnlySpan<Byte> bytes, out Message parsedMessage)
        {
            parsedMessage = null;
            try
            {
                parsedMessage = new Message(bytes);
                return true;
            }
            catch (ArgumentException ) { return false; }
        }

        public Message() { }
        public Message(ReadOnlySpan<Byte> bytes) : this()
        {
            var index = 0;
            Operation = (Operation)bytes[index];
            HardwareAddressType = (HardwareAddressKind)bytes[++index];
            HardwareLength = bytes[++index]; // hardware addr length= 6 for 10mb ethernet
            Hops = bytes[++index]; // relay hop count
            SessionId = BitConverter.ToUInt32(bytes.Slice(++index, 4)); // session id; initialized by client
            Seconds = BitConverter.ToUInt16(bytes.Slice(index += 4, 2)); // seconds since client began address acquistion
            Flags = BitConverter.ToUInt16(bytes.Slice(index += 2, 2)); // 
            ClientIpAddress = new IPAddress(bytes.Slice(index += 2, 4)); // client IP when BOUND; RENEW; REBINDING state
            YourIpAddress = new IPAddress(bytes.Slice(index += 4, 4)); // 'your' client IP
            ServerIdentifierAddress = new IPAddress(bytes.Slice(index += 4, 4)); // next server to use in boostrap; returned in OFFER & ACK
            GatewayOrRelayAddress = new IPAddress(bytes.Slice(index += 4, 4)); // gateway/relay agent IP
            ClientHardwareAddress = new PhysicalAddress(bytes.Slice(index += 4, HardwareLength).ToArray()); // client hardware address
            ServerHostName = Encoding.UTF8.GetString(bytes.Slice(index += 16, 64));
            BootFileName = Encoding.UTF8.GetString(bytes.Slice(index += 64, 128));
            MagicCookie = BitConverter.ToUInt32(bytes.Slice(index += 128, 4)); // contains 99; 130; 83; 99

            var optionsSlice = bytes.Slice(index += 4);
            while (optionsSlice.Length > 0)
            {
                var optionId = (OptionKind)optionsSlice[0];

                if (optionId == OptionKind.End)
                {
                    break;
                }
                else if (optionId == OptionKind.Pad)
                {
                    optionsSlice = optionsSlice.Slice(1); // NOP
                }
                else
                {
                    var valueLength = optionsSlice[1];
                    switch (optionId)
                    {
                        case OptionKind.DhcpMessageType:
                            var messageType = (MessageType)optionsSlice[2];
                            Options.Add(optionId, messageType);
                            break;
                        default:
                            var value = optionsSlice.Slice(2, valueLength).ToArray();
                            Options.Add(optionId, value);
                            break;
                    }
                    optionsSlice = optionsSlice.Slice(valueLength + 2);
                }
            }
        }
        public Boolean TryWriteTo(Byte[] buffer)
        {
            try
            {
                buffer[0] = (Byte)Operation;
                buffer[1] = (Byte)HardwareAddressType;
                buffer[2] = HardwareLength;
                buffer[3] = Hops;
                BitConverter.TryWriteBytes(new Span<Byte>(buffer, 4, 4), SessionId);
                BitConverter.TryWriteBytes(new Span<Byte>(buffer, 8, 2), Seconds);
                BitConverter.TryWriteBytes(new Span<Byte>(buffer, 10, 2), Flags);
                ClientIpAddress.TryWriteBytes(new Span<Byte>(buffer, 12, 4), out _);
                YourIpAddress.TryWriteBytes(new Span<Byte>(buffer, 16, 4), out _);
                ServerIdentifierAddress.TryWriteBytes(new Span<Byte>(buffer, 20, 4), out _);
                GatewayOrRelayAddress.TryWriteBytes(new Span<Byte>(buffer, 24, 4), out _);
                ClientHardwareAddress.GetAddressBytes().CopyTo(new Span<Byte>(buffer, 28, 16));
                Encoding.UTF8.GetBytes(ServerHostName).CopyTo(new Span<Byte>(buffer, 44, 64));
                Encoding.UTF8.GetBytes(BootFileName).CopyTo(new Span<Byte>(buffer, 108, 128));
                BitConverter.TryWriteBytes(new Span<Byte>(buffer, 236, 4), MagicCookie);
                var index = 240;
                foreach (var option in Options)
                {
                    buffer[index] = (Byte)option.Key;
                    Byte dataLength = 0;
                    switch (option.Value)
                    {
                        case IPAddress iPAddress:
                            buffer[index + 1] = dataLength = 4;
                            iPAddress.TryWriteBytes(new Span<Byte>(buffer, index + 2, dataLength), out _);
                            break;
                        case IPAddress[] iPAddresses:
                            buffer[index + 1] = dataLength = (Byte)(4 * iPAddresses.Length);
                            var offset = 0;
                            foreach (var iPAddress in iPAddresses)
                            {
                                iPAddress.TryWriteBytes(new Span<Byte>(buffer, index + 2 + offset, dataLength), out _);
                                offset += 4;
                            }
                            break;
                        case Byte[] bytes:
                            buffer[index + 1] = dataLength = (Byte)bytes.Length;
                            bytes.CopyTo(new Span<Byte>(buffer, index + 2, dataLength));
                            break;
                        case Byte b:
                            buffer[index + 1] = dataLength = 1;
                            buffer[index + 2] = b;
                            break;
                        case MessageType messageType:
                            buffer[index + 1] = dataLength = 1;
                            buffer[index + 2] = (Byte)messageType;
                            break;
                        case UInt32 uintVal:
                            buffer[index + 1] = dataLength = 4;
                            BitConverter.TryWriteBytes(new Span<Byte>(buffer, index + 2, dataLength), uintVal);
                            break;
                        default:
                            throw new NotSupportedException();
                    }
                    index += dataLength + 2;
                }
            }
            catch { return false; }
            return true;
        }
    }
}