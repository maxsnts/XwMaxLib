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
            sFile = Path.Combine(sFile, $"{DateTime.Now.ToString("yyyy-MM-dd")}.log");
            File.AppendAllText(sFile, (sMsg + "\r\n"), System.Text.Encoding.Default);
        }
    }
}
