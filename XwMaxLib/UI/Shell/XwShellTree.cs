using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ShellDll;
using System.Runtime.InteropServices;

namespace XwMaxLib.UI.Shell
{
    public class XwShellTree : TreeView
    {
        private const int DBT_DEVTYP_HANDLE = 0x00000006;
        private const int DBT_DEVTYP_DEVICEINTERFACE = 0x00000005;
        private const int DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000;
        private const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            public byte dbch_data;
            public byte dbch_data1;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, uint Flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint UnregisterDeviceNotification(IntPtr hHandle);

        private TreeNode desktop = null;
        private TreeNode computer = null;
        private string CurrentDirectory = string.Empty;



        //***********************************************************************************************
        public XwShellTree()
        {
            ShellImageList.SetSmallImageList(this);
            BeforeExpand += new TreeViewCancelEventHandler(this_BeforeExpand);
            AfterSelect += new TreeViewEventHandler(this_AfterSelect);
        }

        //***********************************************************************************************
        public string GetCurrentDirectory()
        {
            return CurrentDirectory;
        }
        
        //***********************************************************************************************
        public void Load()
        {
            BeginUpdate();
            Nodes.Clear();
            desktop = new TreeNode(System.Environment.SpecialFolder.Desktop.ToString());
            desktop.Tag = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            desktop.ImageIndex = desktop.SelectedImageIndex = ShellImageList.GetSpecialFolderImageIndex(ShellAPI.CSIDL.DESKTOP);
            Nodes.Add(desktop);
            SelectedNode = desktop;

            TreeNode documents = new TreeNode(System.Environment.SpecialFolder.MyDocuments.ToString());
            documents.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            documents.ImageIndex = documents.SelectedImageIndex = ShellImageList.GetFileImageIndex(documents.Tag.ToString(), FileAttributes.Directory | FileAttributes.System);
            desktop.Nodes.Add(documents);
            LoadNode(documents, true);

            computer = new TreeNode(System.Environment.SpecialFolder.MyComputer.ToString());
            computer.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer);
            computer.ImageIndex = computer.SelectedImageIndex = ShellImageList.GetSpecialFolderImageIndex(ShellAPI.CSIDL.DRIVES);
            desktop.Nodes.Add(computer);

            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (string.Compare(drive.Name, "A:\\", true) == 0 && drive.DriveType == DriveType.Removable)
                    continue;

                if (string.Compare(drive.Name, "B:\\", true) == 0 && drive.DriveType == DriveType.Removable)
                    continue;
                
                string name = String.Format("{0} ({1})", (drive.IsReady)?drive.VolumeLabel:drive.DriveType.ToString(), drive.Name.Replace("\\", ""));
                TreeNode drvNode = new TreeNode(name);
                drvNode.Tag = drive.RootDirectory.FullName;
                drvNode.ImageIndex = drvNode.SelectedImageIndex = ShellImageList.GetFileImageIndex(drive.Name, FileAttributes.Device);
                computer.Nodes.Add(drvNode);

                if (drive.IsReady)
                {
                    DirectoryInfo checkD = new DirectoryInfo(drive.RootDirectory.FullName);
                    DirectoryInfo[] check = checkD.GetDirectories();
                    if (check.Length > 0)
                    {
                        TreeNode dummy = new TreeNode(string.Empty);
                        drvNode.Nodes.Add(dummy);
                    }
                }
            }

            LoadNode(desktop, true);
            desktop.Expand();
            computer.Expand();
            EndUpdate();

            RegisterForDeviceChange(true);
        }

        //***********************************************************************************************
        public void LoadNode(TreeNode node, bool init)
        {
            if ((node == desktop || node == computer) && init == false)
                return;

            Cursor.Current = Cursors.WaitCursor;
            
            if (init == false)
                node.Nodes.Clear();

            DirectoryInfo nodeDirInfo = new DirectoryInfo(node.Tag.ToString());

            foreach (DirectoryInfo dir in nodeDirInfo.GetDirectories())
            {
                if ((dir.Attributes & FileAttributes.Hidden) != 0)
                    continue;

                TreeNode child = new TreeNode(dir.Name);
                child.ImageIndex = child.SelectedImageIndex = ShellImageList.GetFileImageIndex(dir.FullName, dir.Attributes);
                child.Tag = dir.FullName;
                node.Nodes.Add(child);

                try
                {
                    DirectoryInfo[] check = dir.GetDirectories();
                    if (check.Length > 0)
                    {
                        TreeNode dummy = new TreeNode(string.Empty);
                        child.Nodes.Add(dummy);
                    }
                }
                catch { }
            }

            Cursor.Current = Cursors.Default;
        }

        //***********************************************************************************************
        private void this_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            BeginUpdate();
            LoadNode(e.Node, false);
            EndUpdate();
        }

        //********************************************************************************************
        private void this_AfterSelect(object sender, TreeViewEventArgs e)
        {
            CurrentDirectory = e.Node.Tag.ToString();
        }

        //***********************************************************************************************
        public bool SelectPath(string path)
        {
            if (path == CurrentDirectory)
                return true;
            
            if (path.EndsWith(":")) //for drive letters
                path += "\\";
            
            string[] parts = path.Split('\\');

            BeginUpdate();
            TreeNode currentNode = computer;
            string currentPath = string.Empty;
            foreach(string s in parts)
            {
                currentPath = Path.Combine(currentPath, 
                    s + ((currentPath.Contains("\\")) ? "" : "\\"));

                foreach (TreeNode child in currentNode.Nodes)
                {
                    if (string.Compare(currentPath,child.Tag.ToString(), true)==0)
                    {
                        LoadNode(child, false);
                        currentNode = child;
                        child.Expand();
                        break;
                    }
                }
            }
            SelectedNode = null;
            SelectedNode = currentNode;
    
            EndUpdate();

            if (string.Compare(path, currentPath, true) == 0)
                return true;

            return false;
        }

        //***********************************************************************************************
        protected override void WndProc(ref Message m)
        {
            const int WM_DEVICECHANGE = 0x0219;
            const int DBT_DEVICEARRIVAL = 0x8000;
            const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
            const int DBT_DEVNODES_CHANGED = 0x0007;
            
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:

                    break;

                case DBT_DEVICEARRIVAL:

                    break;

                case DBT_DEVICEREMOVECOMPLETE:

                    break;

                case DBT_DEVNODES_CHANGED:

                    break;
            }
            base.WndProc(ref m);
        }

        //***********************************************************************************************
        private void RegisterForDeviceChange(bool register)
        {
            if (register)
            {
                DEV_BROADCAST_HANDLE data = new DEV_BROADCAST_HANDLE();
                data.dbch_devicetype = DBT_DEVTYP_HANDLE;
                data.dbch_reserved = 0;
                data.dbch_nameoffset = 0;
                data.dbch_hdevnotify = (IntPtr)0;
                int size = Marshal.SizeOf(data);
                data.dbch_size = size;
                IntPtr buffer = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(data, buffer, true);
                RegisterDeviceNotification(this.Handle, buffer, 0);
            }
            else
            {
                /*
                // close the directory handle
                if (mDirHandle != IntPtr.Zero)
                {
                    Native.CloseDirectoryHandle(mDirHandle);
                    //    string er = Marshal.GetLastWin32Error().ToString();
                }

                // unregister
                if (mDeviceNotifyHandle != IntPtr.Zero)
                {
                    Native.UnregisterDeviceNotification(mDeviceNotifyHandle);
                }


                mDeviceNotifyHandle = IntPtr.Zero;
                mDirHandle = IntPtr.Zero;

                mCurrentDrive = "";
                if (mFileOnFlash != null)
                {
                    mFileOnFlash.Close();
                    mFileOnFlash = null;
                }
                */
            }
        }
    }
}
