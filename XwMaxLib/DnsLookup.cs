using System;
using System.Net;
using System.Net.Sockets;

namespace XwMaxLib.DNS
{
    public class DnsLookup
    {
        //******************************************************************************************************
        public static IPAddress ResolveHost(String sHost)
        {
            IPHostEntry oIpHostInfo = System.Net.Dns.GetHostEntry(sHost);
            IPAddress oIpAddress = IPAddress.Loopback;
            foreach (IPAddress oInterface in oIpHostInfo.AddressList)
                if (oInterface.AddressFamily == AddressFamily.InterNetwork)
                    oIpAddress = oInterface;
            return oIpAddress;
        }
    }
}
