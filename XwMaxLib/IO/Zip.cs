/*
 * This does, not make much sence... but i anted to isolate the code so that i 
 * can change the used zip library without changing the calling code
 */

using System.IO;
using System.IO.Compression;

namespace XwMaxLib.IO
{
    public class Zip
    {
        //**********************************************************************************************************
        public static void CompressFile(string source, string destination)
        {
            using (ZipArchive archive = ZipFile.Open(destination, ZipArchiveMode.Update))
            {
                archive.CreateEntryFromFile(source, Path.GetFileName(source), CompressionLevel.Optimal);
            }
        }

        //**********************************************************************************************************
        public static void DecompressFile(string source, string destination)
        {
            ZipFile.ExtractToDirectory(source, destination);
        }

        //**********************************************************************************************************
        public static void CompressDirectory(string source, string destination)
        {
            ZipFile.CreateFromDirectory(source, destination, CompressionLevel.Optimal, false);
        }
    }
}
