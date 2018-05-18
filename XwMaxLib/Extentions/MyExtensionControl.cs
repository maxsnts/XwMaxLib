using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XwMaxLib.Extentions
{
    public static class MyExtensionControl
    {
        //*************************************************************************************************
        public static void ShowBalloon(this Control window, ToolTipIcon icon, string title, string text)
        {
            ToolTip balloon = new ToolTip();
            balloon.IsBalloon = true;
            balloon.UseFading = true;
            balloon.UseAnimation = true;
            balloon.ShowAlways = true;
            balloon.ToolTipIcon = icon;
            balloon.ToolTipTitle = title;
            balloon.SetToolTip(window, " ");
            balloon.Show(text, window, 2500);
            window.Focus();
        }
    }
}
