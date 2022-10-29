using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace XwMaxLib.Converter
{
    public static class Base36
    {
        static string strBase = "abcdefghijklmnopqrstuvwxyz1234567890";
        static char[] chaBase = strBase.ToArray();

        public static string Encode(string value)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            BigInteger input = new BigInteger(bytes);

            StringBuilder sb = new StringBuilder();
            while (input != 0)
            {
                BigInteger bi = BigInteger.DivRem(input, 36, out BigInteger remainder);
                sb.Append(chaBase[(int)remainder]);
                input = bi;
            }
            return sb.ToString();
        }

        public static string Decode(string value)
        {
            BigInteger result = 0;
            int pos = 0;
            foreach (char c in value.ToLower())
            {
                result += strBase.IndexOf(c) * BigInteger.Pow(36, pos++);
            }
            return Encoding.UTF8.GetString(result.ToByteArray());
        }
    }
}
