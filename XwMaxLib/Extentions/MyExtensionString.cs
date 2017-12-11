using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionString
    {
        //*************************************************************************************************
        public static string GetMd5Hash(this string value)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(data).Replace("-", "").ToUpper();
            }
        }

        //*************************************************************************************************
        public static string GetSha1Hash(this string value)
        {
            using (SHA1 hash = SHA1.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(data).Replace("-", "").ToUpper();
            }
        }

        //*************************************************************************************************
        public static string GetSha256Hash(this string value)
        {
            using (SHA256 hash = SHA256.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(data).Replace("-", "").ToUpper();
            }
        }

        //*************************************************************************************************
        public static string GetSha512Hash(this string value)
        {
            using (SHA512 hash = SHA512.Create())
            {
                byte[] data = hash.ComputeHash(Encoding.UTF8.GetBytes(value));
                return BitConverter.ToString(data).Replace("-", "").ToUpper();
            }
        }

        //*************************************************************************************************
        public static Guid ToGuid(this string value)
        {
            if (value.IsEmpty())
                return Guid.Empty;
            else
                return new Guid(value);
        }

        //*************************************************************************************************
        public static string Left(this string s, int chars)
        {
            if (s.Length <= chars)
                return s;

            return s.Substring(0, chars);
        }

        //*************************************************************************************************
        public static string Right(this string s, int chars)
        {
            int c = chars;
            if (s.Length < c)
                c = s.Length;
            return s.Substring(s.Length - c);
        }

        //*************************************************************************************************
        public static string[] Split(this string s, string seperator, bool removeEmpty = true)
        {
            return s.Split(new string[] { seperator }, (removeEmpty) ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        //*************************************************************************************************
        public static string RightOf(this string s, string find)
        {
            int indexOf = s.IndexOf(find);
            if (indexOf == -1)
                return string.Empty;
            int start = indexOf + find.Length;
            int num = s.Length - start;
            return s.Substring(start, num);
        }

        //*************************************************************************************************
        public static bool IsEmpty(this string s, bool IgnoreWhiteSpace = true)
        {
            if (IgnoreWhiteSpace)
                return String.IsNullOrEmpty(s);
            else
                return String.IsNullOrWhiteSpace(s);
        }

        //*************************************************************************************************
        public static string EncodeAsciiTo64(this string s)
        {
            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(s);
            return System.Convert.ToBase64String(toEncodeAsBytes);
        }

        //*************************************************************************************************
        public static string DecodeAsciiFrom64(this string s)
        {
            byte[] encodedDataAsBytes = System.Convert.FromBase64String(s);
            return System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
        }

        //*************************************************************************************************
        ///<summary>This captures the content of GROUPS as defined in the Regex with "( )"</summary>
        public static string Capture(this string s, string regex, bool errorOnMultipleCaptures = true, int index = 1)
        {
            Match m = Regex.Match(s, regex);
            if (m.Success)
            {
                if (errorOnMultipleCaptures)
                {
                    if (m.Groups.Count > 2)
                        throw new Exception("String.Capture returned more than value");
                }
                return m.Groups[index].Value;
            }
            return "";
        }

        //*************************************************************************************************
        public static string RemoveDiacritics(this string s)
        {
            string stFormD = s.Normalize(NormalizationForm.FormD);
            StringBuilder sb = new StringBuilder();

            for (int ich = 0; ich < stFormD.Length; ich++)
            {
                UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(stFormD[ich]);
            }
            return (sb.ToString().Normalize(NormalizationForm.FormC));
        }

        //*************************************************************************************************
        public static string Capitalize(this string s)
        {
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }

        //*************************************************************************************************
        public static int CountUpperCase(this string s)
        {
            int count = 0;
            for (int i = 0; i < s.Length; i++)
                if (char.IsUpper(s[i]))
                    count++;
            return count;
        }

        //*************************************************************************************************
        public static string GetSQLlikeString(this string s)
        {
            s = s.Trim();
            s = s.Replace("%", "%%");
            s = s.Replace("*", "%");

            if (s.StartsWith("\"") && s.EndsWith("\""))
            {
                s = s.Remove(0, 1);
                s = s.Remove(s.Length - 1, 1);
                return s;
            }

            s = s.ToLower();

            s = Regex.Replace(s, "[áàâãäa]", "[áàâãäa]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "[éèêëe]", "[éèêëe]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "[íìîïi]", "[íìîïi]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "[óòôõöo]", "[óòôõöo]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "[úùûüu]", "[úùûüu]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "[cç]", "[cç]", RegexOptions.IgnoreCase);
            s = Regex.Replace(s, "['´`\"]", "_", RegexOptions.IgnoreCase);

            if (s.StartsWith("?")) //not sure how to spell the word?
            {
                s = Regex.Replace(s, "[sz]", "[sz]", RegexOptions.IgnoreCase);
                s = s.Remove(0, 1);
            }

            s = s.Insert(0, "%");
            s = s + "%";

            return s;
        }

        //*************************************************************************************************
        public static string GetRegexDiacritics(this string s)
        {
            string replaced = s;
            replaced = Regex.Replace(replaced, "[áàâãäa]", "[áàâãäa]", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, "[éèêëe]", "[éèêëe]", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, "[íìîïi]", "[íìîïi]", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, "[óòôõöo]", "[óòôõöo]", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, "[úùûüu]", "[úùûüu]", RegexOptions.IgnoreCase);
            replaced = Regex.Replace(replaced, "[cç]", "[cç]", RegexOptions.IgnoreCase);
            return replaced;
        }

        //*************************************************************************************************
        public static bool ToBoolOrDefault(this object s, bool defaultValue)
        {
            if (Boolean.TryParse(s.ToString(), out bool i))
            {
                return i;
            }
            else
            {
                if (Int32.TryParse(s.ToString(), out int n))
                {
                    if (n == 0)
                        return false;
                    else
                        return true;
                }
            }

            return defaultValue;
        }
    }
}
