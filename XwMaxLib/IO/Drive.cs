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
        public static Int64 GetDiskFreeSpace(string path)
        {
            DriveInfo drive = new DriveInfo(path);
            return drive.TotalFreeSpace;
        }

        //********************************************************************************************************
        public static Int64 GetDiskFreeSpaceInMb(string path)
        {
            return GetDiskFreeSpace(path) / 1024 / 1024;
        }

    }
}
