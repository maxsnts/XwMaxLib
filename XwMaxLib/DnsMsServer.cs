using System;
using System.Management;

namespace XwMaxLib.DNS
{
    public class DnsMsServer
    {
        private WMI wmi = null;
        private string DnsServerName = string.Empty;

        //******************************************************************************************************
        public DnsMsServer(string server, string username, string password)
        {
            wmi = new WMI();
            wmi.Connect(server, username, password, "MicrosoftDNS");
            DnsServerName = GetDNSServerName();
        }

        //******************************************************************************************************
        public ManagementObjectCollection ListZones()
        {
            return wmi.RunQuery($"SELECT * FROM MicrosoftDNS_Zone");
            //AND Name = '{1}'
        }
        
        //******************************************************************************************************
        private string GetDNSServerName()
        {
            ManagementObject oServer = wmi.GetObject("MicrosoftDNS_Server.Name=\".\"");
            return oServer["Name"] as String;
        }
    }
}
