using System.Text.RegularExpressions;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionMatch
    {
        //*************************************************************************************************
        public static string GetNamedGroup(this Match match, string groupName, string defaultValue = "{NOTFOUND}")
        {
            if (!match.Success)
                return defaultValue;

            string key = "${" + groupName + "}";
            string value = match.Result(key);
            if (value == key)
                return defaultValue;
            return value;
        }
    }
}
