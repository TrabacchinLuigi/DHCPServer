using System;
using System.Collections.Generic;
using System.Text;

namespace DHCP
{
    enum OptionKind : Byte
    {
        /* RFC 1497 Vendor Extensions */

        Pad = 0,
        End = 255,

        SubnetMask = 1,
        TimeOffset = 2,
        Router = 3,
        TimeServer = 4,
        NameServer = 5,
        DomainNameServer = 6,
        LogServer = 7,
        CookieServer = 8,
        LPRServer = 9 /*TODO: what is an LRP ?*/,
        ImpressServer = 10,
        ResourceLocationServer = 11,
        HostName = 12,
        BootFileSize = 13,
        MeritDumpFile = 14,
        DomainName = 15,
        SwapServer = 16,
        RootPath = 17,
        ExtensionsPath = 18,

        /* IP Layer Parameters per Host */

        IpForwarding = 19,
        NonLocalSourceRouting = 20,
        PolicyFilter = 21,
        MaximumDatagramReassemblySize = 22,
        DefaultIpTimeToLive = 23,
        PathMtuAgingTimeout = 24,
        PathMtuPlateauTable = 25,

        /* IP Layer Parameters per Interface */

        InterfaceMtu = 26,
        AllSubnetsAreLocal = 27,
        BroadcastAddress = 28,
        PerformMaskDiscovery = 29,
        MaskSupplier = 30,
        PerformRouterDiscovery = 31,
        RouterSolicitationAddress = 32,
        StaticRoute = 33,

        /* Link Layer Parameters per Interface */

        TrailerEncapsulation = 34,
        ArpCacheTimeout = 35,
        EthernetEncapsulation = 36,

        /* TCP Parameters */

        TcpDefaultTtl = 37,
        TcpKeepaliveInterval = 38,
        TcpKeepaliveGarbage = 39,

        /* Application and Service Parameters */

        NetworkInformationServiceDomain = 40,
        NetworkInformationServers = 41,
        NetworkTimeProtocolServers = 42,
        VendorSpecificInformation = 43,
        NetbiosOverTcpIpNameServer = 44,
        NetbiosOverTcpIpDatagramDistributionServer = 4,
        NetbiosOverTcpIpNodeType = 46,
        NetbiosOverTcpIpScope = 47,
        XWindowSystemFontServer = 48,
        XWindowSystemDisplayManager = 49,
        NetworkInformationServicePlusDomain = 64,
        NetworkInformationServicePlusServers = 65,
        MobileIpHomeAgent = 68,
        SmtpServer = 69,
        Pop3Server = 70,
        NntpServer = 71,
        DefaultWwwServer = 72,
        DefaultFingerServer = 73,
        DefaultIrcServer = 74,
        StreettalkServer = 75,
        StreettalkDirectoryAssistanceServer = 76,

        /* DHCP Extensions */

        RequestedIpAddress = 50,
        IpAddressLeaseTime = 51,
        OptionOverload = 52,
        TftpServerName = 66,
        BootfileName = 67,
        DhcpMessageType = 53,
        ServerIdentifier = 54,
        ParameterRequestList = 55,
        Message = 56,
        MaximumDhcpMessageSize = 57,
        RenewalT1TimeValue = 58,
        RebindingT2TimeValue = 59,
        VendorClassIdentifier = 60,
        ClientIdentifier = 61

    };
}
