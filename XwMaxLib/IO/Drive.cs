using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace XwMaxLib.IO
{
    public class Drive
    {
        //********************************************************************************************************
        /// <summary>
        /// Gets the drive type of the given path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>DriveType of path</returns>
        public static DriveType GetPathDriveType(string path)
        {
            //OK, so UNC paths aren't 'drives', but this is still handy
            if (path.StartsWith(@"\\")) return DriveType.Network;
            DriveInfo di = new DriveInfo(path);
            return di.DriveType;
        }


        //********************************************************************************************************
        [DllImport("Shlwapi.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private extern static bool PathFileExists(StringBuilder path);
        public static bool FileExists(string path)
        {
            // A StringBuilder is required for interops calls that use strings
            StringBuilder builder = new StringBuilder();
            builder.Append(path);
            return PathFileExists(builder);
        }

        //********************************************************************************************************
        public static long GetDiskFreeSpace(string path)
        {
            DriveInfo drive = new DriveInfo(path);
            return drive.TotalFreeSpace;
        }

        //********************************************************************************************************
        public static long GetDiskFreeSpaceInMb(string path)
        {
            return GetDiskFreeSpace(path) / 1024 / 1024;
        }

        //*************************************************************************************************
        public static string GetFileSize(double byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= (1024 * 1024 * 1024))
                size = string.Format("{0:##.00}", byteCount / (1024 * 1024 * 1024)) + " Gb";
            else if (byteCount >= (1024 * 1024))
                size = string.Format("{0:##.00}", byteCount / (1024 * 1024)) + " Mb";
            else if (byteCount >= 1024)
                size = string.Format("{0:##.00}", byteCount / 1024) + " Kb";
            else if (byteCount < 1024)
                size = string.Format("{0:##}", byteCount) + " By";

            return size;
        }

    }
}
