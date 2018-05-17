using System;
using System.Management;

namespace XwMaxLib
{
    public class WMI
    {
        private ManagementScope _oScope = null;

        //*****************************************************************************************************
        public WMI()
        {
            
        }

        //*****************************************************************************************************
        public WMI(string server, string username, string password, String sRoot)
        {
            Connect(server, username, password, sRoot);
        }
        
        //*****************************************************************************************************
        public void Connect(string server, string username, string password, String sRoot)
        {
            _oScope = new ManagementScope(String.Format(@"\\{0}\root\{1}", server, sRoot));
            _oScope.Options.Authentication = AuthenticationLevel.PacketPrivacy;
            _oScope.Options.Username = username;
            _oScope.Options.Password = password;
            _oScope.Connect();
        }

        //*****************************************************************************************************
        public ManagementObjectCollection RunQuery(string sQuery, params object[] oParams)
        {
            if (oParams != null && oParams.Length > 0)
                sQuery = String.Format(sQuery, oParams);
            ManagementObjectSearcher oSearcher = new ManagementObjectSearcher(_oScope, new ObjectQuery(sQuery));
            oSearcher.Options.Rewindable = true;
            oSearcher.Options.ReturnImmediately = true;
            return oSearcher.Get();
        }

        //*****************************************************************************************************
        public ManagementObject GetFirstObject(string sQuery, params object[] oParams)
        {
            ManagementObjectCollection oColl = this.RunQuery(sQuery, oParams);
            ManagementObjectCollection.ManagementObjectEnumerator oEnum = oColl.GetEnumerator();
            if (!oEnum.MoveNext())
                return null;
            else
                return oEnum.Current as ManagementObject;
        }

        //*****************************************************************************************************
        public ManagementObject GetObject(string sObjectPath, params object[] oParams)
        {
            if (oParams != null && oParams.Length > 0)
                sObjectPath = String.Format(sObjectPath, oParams);
            ManagementObject oObject = new ManagementObject(_oScope, new ManagementPath(sObjectPath), null);
            oObject.Get();
            return oObject;
        }

        //*****************************************************************************************************
        public ManagementClass GetClass(string sClassType)
        {
            ManagementClass oClass = new ManagementClass(_oScope, new ManagementPath(sClassType), null);
            return oClass;
        }

        //*****************************************************************************************************
        public bool IsConnected()
        {
            if (_oScope == null)
                return false;

            return _oScope.IsConnected;
        }
    }
}
