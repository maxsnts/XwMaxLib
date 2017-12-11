using System;
using System.IO;

namespace XwMaxLib.Log
{
    public class Log
    {
        //**************************************************************************************************
        public static void WriteOnDailyFile(string folder, string sMsg)
        {
            string sFile = folder;
            if (!Directory.Exists(sFile))
                Directory.CreateDirectory(sFile);
            sFile = String.Format(@"{0}\{1}.log", sFile, DateTime.Now.ToString("yyyy-MM-dd"));
            File.AppendAllText(sFile, (sMsg + "\r\n"), System.Text.Encoding.Default);
        }
    }
}
