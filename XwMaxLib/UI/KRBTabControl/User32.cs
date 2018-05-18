using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace KRBTabControlNS.Win32
{
    internal partial class User32
    {
        #region Struct

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public override string ToString()
            {
                return "{left=" + left.ToString() + ", " + "top=" + top.ToString() + ", " +
                    "right=" + right.ToString() + ", " + "bottom=" + bottom.ToString() + "}";
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct TCHITTESTINFO
        {
            public Point pt;
            public TabControlHitTest flags;

            public TCHITTESTINFO(TabControlHitTest hitTest)
                : this()
            {
                flags = hitTest;
            }

            public TCHITTESTINFO(Point point, TabControlHitTest hitTest)
                : this(hitTest)
            {
                pt = point;
            }

            public TCHITTESTINFO(int x, int y, TabControlHitTest hitTest)
                : this(hitTest)
            {
                pt = new Point(x, y);
            }
        }
        
        #endregion

        #region UnmanagedMethods

        [DllImport("gdi32")]
        internal static extern bool BitBlt(
          IntPtr hdcDest,     // handle to destination DC (device context)
          int nXDest,         // x-coord of destination upper-left corner
          int nYDest,         // y-coord of destination upper-left corner
          int nWidth,         // width of destination rectangle
          int nHeight,        // height of destination rectangle
          IntPtr hdcSrc,      // handle to source DC
          int nXSrc,          // x-coordinate of source upper-left corner
          int nYSrc,          // y-coordinate of source upper-left corner
          System.Int32 dwRop  // raster operation code
          );

        [DllImport("gdi32")]
        internal static extern IntPtr CreateDC(
          String driverName,
          String deviceName,
          String output,
          IntPtr lpInitData);

        [DllImport("gdi32")]
        internal static extern bool DeleteDC(
          IntPtr dc);

        [DllImport("uxtheme", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern Int32 SetWindowTheme(
            IntPtr hWnd,
            String textSubAppName,
            String textSubIdList);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            int lParam);
        
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SendMessage(
            IntPtr hwnd,
            int tMsg,
            IntPtr wParam,
            ref TCHITTESTINFO lParam);
        
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadCursorFromFile(string filename);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool ReleaseCapture();

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool IsWindowVisible(IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr SetParent(IntPtr hwndChild, IntPtr hwndParent);
        
        /// <summary>
        /// The GetParent function retrieves a handle to the specified window's parent or owner.
        /// </summary>
        /// <param name="hwnd">Handle to the window whose parent window handle is to be retrieved.</param>
        /// <returns>If the window is a child window, the return value is a handle to the parent window. If the window is a top-level window, the return value is a handle to the owner window. If the window is a top-level unowned window or if the function fails, the return value is NULL.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetParent(IntPtr hwnd);

        /// <summary>
        /// The FindWindowEx function retrieves a handle to a window whose class name and window name match the specified strings. The function searches child windows, beginning with the one following the specified child window.
        /// </summary>
        /// <param name="hwndParent">Handle to the parent window whose child windows are to be searched.</param>
        /// <param name="hwndChildAfter">Handle to a child window.</param>
        /// <param name="lpszClass">Specifies class name.</param>
        /// <param name="lpszWindow">Pointer to a null-terminated string that specifies the window name (the window's title).</param>
        /// <returns>If the function succeeds, the return value is a handle to the window that has the specified class and window names.If the function fails, the return value is NULL.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr FindWindowEx(
            IntPtr hwndParent,
            IntPtr hwndChildAfter,
            [MarshalAs(UnmanagedType.LPTStr)]
			string lpszClass,
            [MarshalAs(UnmanagedType.LPTStr)]
			string lpszWindow);

        /// <summary>
        /// The MoveWindow function changes the position and dimensions of the specified window.
        /// </summary>
        /// <param name="hwnd">Handle to the window.</param>
        /// <param name="X">Specifies the new position of the left side of the window.</param>
        /// <param name="Y">Specifies the new position of the top of the window.</param>
        /// <param name="nWidth">Specifies the new width of the window.</param>
        /// <param name="nHeight">Specifies the new height of the window.</param>
        /// <param name="bRepaint">If the bRepaint parameter is TRUE, the system sends the WM_PAINT message to the window procedure immediately after moving the window (that is, the MoveWindow function calls the UpdateWindow function). If bRepaint is FALSE, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</param>
        /// <returns>If the function succeeds, the return value is true.If the function fails, the return value is false.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool MoveWindow(
            IntPtr hwnd,
            int X,
            int Y,
            int nWidth,
            int nHeight,
            bool bRepaint);

        /// <summary>
        /// The InvalidateRect function adds a rectangle to the specified window's update region.
        /// </summary>
        /// <param name="hwnd">Handle to window.</param>
        /// <param name="rect">Rectangle coordinates.</param>
        /// <param name="bErase">Erase state.</param>
        /// <returns>If the function succeeds, the return value is true.If the function fails, the return value is false.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool InvalidateRect(
            IntPtr hwnd,
            ref Rectangle rect,
            bool bErase);

        /// <summary>
        /// The ValidateRect function validates the client area within a rectangle by removing the rectangle from the update region of the specified window.
        /// </summary>
        /// <param name="hwnd">Handle to window.</param>
        /// <param name="rect">Validation rectangle coordinates.</param>
        /// <returns>If the function succeeds, the return value is true.If the function fails, the return value is false.</returns>
        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool ValidateRect(
            IntPtr hwnd,
            ref Rectangle rect);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int GetWindowLong(
            IntPtr hWnd,
            int dwStyle);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetDC(
            IntPtr hwnd);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int ReleaseDC(
            IntPtr hwnd,
            IntPtr hdc);

        //[DllImport("user32", CharSet = CharSet.Auto)]
        //internal static extern int GetScrollPos(
        //    IntPtr hwnd,
        //    int nBar);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int GetClientRect(
            IntPtr hwnd,
            ref RECT rc);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int GetClientRect(
            IntPtr hwnd,
            [In, Out] ref Rectangle rect);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool GetWindowRect(
            IntPtr hWnd,
            [In, Out] ref Rectangle rect);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int GetWindowRect(
            IntPtr hwnd,
            ref RECT rc);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int ShowWindow(
            IntPtr hWnd,
            int nCmdShow);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern uint SetWindowLong(
            IntPtr hWnd,
            int nIndex,
            int dwNewLong);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern int SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndAfter,
            int X,
            int Y,
            int Width,
            int Height,
            FlagsSetWindowPos flags);

        [DllImport("user32", CharSet = CharSet.Auto)]
        internal static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X,
           int Y, int cx, int cy, uint uFlags);

        #endregion
    }
}