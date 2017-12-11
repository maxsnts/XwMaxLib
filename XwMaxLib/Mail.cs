using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace XwMaxLib.Mail
{
    public class Mail
    {
        //*************************************************************************************************
        public static void SendMail(string fromName, string fromMail, string to, string subject, string message, bool isHtml)
        {
            if (Convert.ToBoolean(ConfigurationManager.AppSettings["DEVELOPMENT"]) == true)
                fromName = "DEV - " + fromName;

            if (ConfigurationManager.AppSettings["ENVIRONMENT"] == "DEVELOPMENT")
                fromName = "DEV - " + fromName;

            SmtpClient oSMTP = new SmtpClient();
            oSMTP.Host = ConfigurationManager.AppSettings["SMTPHOST"];

            string from = fromName + "<" + fromMail + ">";

            if (message.Contains("EXCEPTION:"))
            {
                int i = message.IndexOf("EXCEPTION:", StringComparison.Ordinal);
                i = message.IndexOf("\n", i + 1, StringComparison.Ordinal);
                int n = message.IndexOf("\n", i + 1, StringComparison.Ordinal);
                subject += ": " + message.Substring(i + 1, n - i) + "...";
            }

            subject = subject.Replace("\n", " ");
            subject = subject.Replace("\r", " ");

            MailMessage oMail = new MailMessage(from, to, subject, message);
            oMail.IsBodyHtml = isHtml;
            oMail.BodyEncoding = System.Text.Encoding.UTF8;

            string username = ConfigurationManager.AppSettings["SMTPUSER"];
            string password = ConfigurationManager.AppSettings["SMTPPASS"];

            if (!String.IsNullOrEmpty(username))
                oSMTP.Credentials = new NetworkCredential(username, password);

            oSMTP.Send(oMail);
        }

        //**********************************************************************************************************************
        public static bool IsMailValid(string address)
        {
            try
            {
                MailAddress addr = new MailAddress(address);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //**********************************************************************************************************************
        public static string ValidatePassword(string password, int min, int max, bool lower, bool upper, bool digit, string diff = "")
        {
            bool minLengthRequirements = password.Length >= min;
            bool maxLengthRequirements = password.Length <= max;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;
            bool isDifferentFrom = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c)) hasUpperCaseLetter = true;
                else if (char.IsLower(c)) hasLowerCaseLetter = true;
                else if (char.IsDigit(c)) hasDecimalDigit = true;
            }

            if (password != diff)
                isDifferentFrom = true;

            string result = string.Empty;

            if (!minLengthRequirements)
                result += String.Format(",MIN_{0}", min);
            if (!maxLengthRequirements)
                result += String.Format(",MAX_{0}", max);
            if (!hasUpperCaseLetter && upper)
                result += ",NEED_UPPER";
            if (!hasLowerCaseLetter && lower)
                result += ",NEED_LOWER";
            if (!hasDecimalDigit && digit)
                result += ",NEED_DIGIT";
            if (!isDifferentFrom && diff!="")
                result += ",NEED_DIFF";

            return (result == string.Empty) ? "GOOD" : result.Remove(0,1);
        }
    }
}
