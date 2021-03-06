﻿using System;
using System.Drawing;

namespace XwMaxLib.Extensions
{
    public static class MyExtensionRectangle
    {
        public static int GetMidX(this Rectangle rect)
        {
            double mid = rect.Left + rect.Width / 2;
            return (int)Math.Round(mid, 0, MidpointRounding.ToEven);
        }

        public static int GetMidY(this Rectangle rect)
        {
            double mid = rect.Top + rect.Height / 2;
            return (int)Math.Round(mid, 0, MidpointRounding.ToEven);
        }

        public static Point GetCenter(this Rectangle rect)
        {
            return new Point(rect.GetMidX(), rect.GetMidY());
        }

        public static Point GetTopLeft(this Rectangle rect)
        {
            return new Point(rect.Left, rect.Top);
        }

        public static Point GetTopRight(this Rectangle rect)
        {
            return new Point(rect.Right, rect.Top);
        }

        public static Point GetBottomRight(this Rectangle rect)
        {
            return new Point(rect.Right, rect.Bottom);
        }

        public static Point GetBottomLeft(this Rectangle rect)
        {
            return new Point(rect.Left, rect.Bottom);
        }

        public static float GetRatio(this Rectangle rect)
        {
            return (float)rect.Width / (float)rect.Height;
        }

        public static Rectangle Center(this Rectangle rect, Rectangle withRect)
        {
            Point withC = withRect.GetCenter();
            Point rectC = rect.GetCenter();
            rect.Offset(withC.X - rectC.X, withC.Y - rectC.Y);
            return rect;
        }

        public static Rectangle Fit(this Rectangle rect, Rectangle inRect, bool allowScaleUp)
        {
            if (allowScaleUp == false &&
                rect.Width <= inRect.Width &&
                rect.Height <= inRect.Height)
                return rect;

            float sW = (float)inRect.Width / (float)rect.Width;
            float sH = (float)inRect.Height / (float)rect.Height;
            float scale = (sW < sH) ? sW : sH;
            int infalteX = (int)Math.Round((rect.Width - rect.Width * scale) / 2, 0, MidpointRounding.ToEven);
            int infalteY = (int)Math.Round((rect.Height - rect.Height * scale) / 2, 0, MidpointRounding.ToEven);
            rect.Inflate(-infalteX, -infalteY);
            return rect; 
        }

        public static Rectangle AlignLeft(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(withRect.Left - rect.Left, 0);
            return rect;
        }

        public static Rectangle AlignRight(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(withRect.Right - rect.Right, 0);
            return rect;
        }

        public static Rectangle AlignTop(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(0, withRect.Top - rect.Top);
            return rect;
        }

        public static Rectangle AlignBottom(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(0, withRect.Bottom - rect.Bottom);
            return rect;
        }

        public static Rectangle AlignCenter(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(withRect.GetCenter().X - rect.GetCenter().X, 0);
            return rect;
        }

        public static Rectangle AlignMiddle(this Rectangle rect, Rectangle withRect)
        {
            rect.Offset(0, withRect.GetCenter().Y - rect.GetCenter().Y);
            return rect;
        }

        public static Rectangle InflatePercentage(this Rectangle rect, float percent)
        {
            double infX = ((double)rect.Width * percent / 100f) / 2;
            double infY = ((double)rect.Height * percent / 100f) / 2 ;
            rect.Inflate((int)Math.Round(infX, 0, MidpointRounding.ToEven), (int)Math.Round(infY, 0, MidpointRounding.ToEven));
            return rect;
        }
    }
}
