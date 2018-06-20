using System;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionObject
    {
        //*************************************************************************************************
        public static bool IsIn(this object value, params object[] values)
        {
            foreach (object i in values)
            {
                if (i.Equals(value))
                    return true;
            }

            return false;
        }

        //*************************************************************************************************
        public static int ToIntOrDefault(this object s, int defaultValue)
        {
            int i = 0;
            if (Int32.TryParse(s.ToString(), out i) == true)
                return i;
            else
                return defaultValue;
        }

        //*************************************************************************************************
        public static double ToDoubleOrDefault(this object s, double defaultValue)
        {
            if (Double.TryParse(s.ToString(), out double i) == true)
                return i;
            else
                return defaultValue;
        }


        //*************************************************************************************************
        public static Int64 ToInt64OrDefault(this object s, Int64 defaultValue)
        {
            Int64 i = 0;
            if (Int64.TryParse(s.ToString(), out i) == true)
                return i;
            else
                return defaultValue;
        }

        //*************************************************************************************************
        public static string ToStringOrDefault(this object s, string defaultValue = "")
        {
            if (s == null)
                return defaultValue;

            return s.ToString();
        }

        //*************************************************************************************************
        public static int GetEnumValueInt(this object s)
        {
            return ((int)s);
        }

        //*************************************************************************************************
        public static string GetEnumValueString(this object s)
        {
            return ((int)s).ToString();
        }

    }
}
