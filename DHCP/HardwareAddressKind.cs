using System;
using System.Collections.Generic;
using System.Text;

namespace DHCP
{
    /// <summary>
    /// RFC1700, hardware types
    /// </summary>
    public enum HardwareAddressKind
    {
        Ethernet = 1,
        Experimental = 2,
        AX25 = 3,
        ProNetTokenRing = 4,
        Chaos = 5,
        Tokenring = 6,
        Arcnet = 7,
        FDDI = 8,
        Lanstar = 9
    }
}
