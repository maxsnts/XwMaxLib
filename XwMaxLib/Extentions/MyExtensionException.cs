using System;
using System.Data.Common;
using System.Text;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionException
    {
        //*************************************************************************************************
        public static void PreserveCallStack(this Exception exception)
        {
            typeof(System.Exception).GetMethod("InternalPreserveStackTrace", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(exception, null);
        }

        //*************************************************************************************************
        /*
        public static string ProcessException(this Exception ex, bool innerEx)
        {
            if (ex == null)
                return string.Empty;

            StringBuilder message = null;
            if (innerEx == false)
                message = new StringBuilder("\r\n=================================== EXCEPTION: ================================\r\n", 1024);
            else
                message = new StringBuilder("\r\n----------------------------------- INNER EX: ---------------------------------\r\n", 1024);

            message.AppendLine(ex.Message);

            if (ex is DbObjectsException)
            {
                message.AppendLine("--------------------------------- SQL COMMAND: --------------------------------");
                message.AppendLine(((DbObjectsException)ex).CommandText);
            }
            
            if (ex is XwDbException)
            {
                message.AppendLine("\r\n-------------------------------- SQL COMMAND: --------------------------------");
                message.AppendLine(((XwDbException)ex).Command);
            }

            message.AppendLine("---------------------------------- STACK TRACE: -------------------------------");
            if (ex.StackTrace != null)
            {
                message.AppendLine(ex.StackTrace.ToString());
                message = message.Replace("at ", "\r\nat ");
                message = message.Replace(" in ", "\r\nin ");
            }

            if (ex != ex.InnerException)
            {
                message.Append(ProcessException(ex.InnerException, true));
            }

            return message.ToString();
        }
        */
    }
}
