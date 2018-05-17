using System.Web;

namespace XwMaxLib.Net
{
    public class Net
    {
        //*************************************************************************************************
        public static string GetUserIP(HttpContext context = null)
        {
            HttpContext ctx = context;
            if (ctx == null)
                ctx = HttpContext.Current;
            string IP = ctx.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (IP == null)
                IP = ctx.Request.ServerVariables["REMOTE_ADDR"];
            return IP;
        }
    }
}
