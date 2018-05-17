using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XwMaxLib.Extensions;

namespace Tester
{
    public partial class Test_Rectangle_Form : Form
    {
        public Test_Rectangle_Form()
        {
            InitializeComponent();

            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.DoubleBuffer,
                true);

        }

        private void Test_Rectangle_Form_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            
            //setup
            Rectangle bigHRect = new Rectangle(50, 50, 3000, 2500);
            Rectangle smallHRect = new Rectangle(100, 100, 300, 250);

            Rectangle bigVRect = new Rectangle(150, 150, 2000, 3000);
            Rectangle smallVRect = new Rectangle(600, 600, 200, 300);

            Rectangle percent = new Rectangle(600, 200, 100, 200);

            Rectangle canvas = this.ClientRectangle;
            canvas.Inflate(-20, -20);

            //center
            percent = percent.Center(canvas);
            bigHRect = bigHRect.Center(canvas);
            //smallHRect = smallHRect.Center(canvas);
            bigVRect = bigVRect.Center(canvas);
            //smallVRect = smallVRect.Center(canvas);

            //fit
            bigHRect = bigHRect.Fit(canvas, true);
            bigVRect = bigVRect.Fit(canvas, true);
            smallHRect = smallHRect.Fit(canvas, false);
            smallVRect = smallVRect.Fit(smallHRect, false);

            //align
            smallHRect = smallHRect.AlignCenter(canvas);
            smallVRect = smallVRect.AlignLeft(smallHRect);
            bigVRect = bigVRect.AlignRight(canvas);
            bigVRect = bigVRect.AlignBottom(canvas);
            smallVRect = smallVRect.AlignMiddle(canvas);

            percent = percent.InflatePercentage(50);

            //draw
            g.DrawRectangle(Pens.Fuchsia, canvas);
            g.DrawLine(Pens.Fuchsia, canvas.GetTopLeft(), canvas.GetBottomRight());
            g.DrawLine(Pens.Fuchsia, canvas.GetBottomLeft(), canvas.GetTopRight());

            g.DrawRectangle(Pens.Lime, bigHRect);
            g.DrawLine(Pens.Lime, bigHRect.GetTopLeft(), bigHRect.GetBottomRight());
            g.DrawLine(Pens.Lime, bigHRect.GetBottomLeft(), bigHRect.GetTopRight());

            g.DrawRectangle(Pens.Yellow, smallHRect);
            g.DrawLine(Pens.Yellow, smallHRect.GetTopLeft(), smallHRect.GetBottomRight());
            g.DrawLine(Pens.Yellow, smallHRect.GetBottomLeft(), smallHRect.GetTopRight());

            g.DrawRectangle(Pens.Cyan, bigVRect);
            g.DrawLine(Pens.Cyan, bigVRect.GetTopLeft(), bigVRect.GetBottomRight());
            g.DrawLine(Pens.Cyan, bigVRect.GetBottomLeft(), bigVRect.GetTopRight());

            g.DrawRectangle(Pens.Red, smallVRect);
            g.DrawLine(Pens.Red, smallVRect.GetTopLeft(), smallVRect.GetBottomRight());
            g.DrawLine(Pens.Red, smallVRect.GetBottomLeft(), smallVRect.GetTopRight());

            g.DrawRectangle(Pens.White, percent);
            g.DrawLine(Pens.White, percent.GetTopLeft(), percent.GetBottomRight());
            g.DrawLine(Pens.White, percent.GetBottomLeft(), percent.GetTopRight());
        }
    }
}
