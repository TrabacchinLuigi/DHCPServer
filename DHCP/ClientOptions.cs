using System.Net;

namespace DHCP
{
    public class ClientOptions
    {
        public IPAddress BindTo { get; }

        public ClientOptions(IPAddress bindTo)
        {
            BindTo = bindTo ?? throw new System.ArgumentNullException(nameof(bindTo));
        }
    }
}