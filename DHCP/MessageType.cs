using System;

namespace DHCP
{
    /// <summary>
    /// Code ID of DHCP and BOOTP options as defined in RFC 2132
    /// </summary>
    public enum MessageType : Byte
    {
        DISCOVER = 1,
        OFFER = 2,
        REQUEST = 3,
        DECLINE = 4,
        ACK = 5,
        NAK = 6,
        RELEASE = 7,
        INFORM = 8,
    };
}
