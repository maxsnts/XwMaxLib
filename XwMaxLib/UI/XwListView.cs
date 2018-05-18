using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;
using System.Drawing.Drawing2D;

namespace XwMaxLib.UI
{
    public class XwListView : ListView
    {
        private enum DataType
        {
            Unknown,
            Text,
            Image,
            ImageIndex,
            Progress,
            Image_Text,
            ImageIndex_Text
        }

        private struct DataItem
        {
            public DataType type;
            public string text;
            public int imageIndex;
            public double progress;
            public Image image;
        }

        private Dictionary<ListViewItem.ListViewSubItem, DataItem> data = new Dictionary<ListViewItem.ListViewSubItem, DataItem>();

        //***********************************************************************************
        public XwListView()
        {
            this.View = System.Windows.Forms.View.Details;
            this.OwnerDraw = true;
            this.FullRowSelect = true;
            this.DoubleBuffered = true;
            this.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            this.DrawItem += new DrawListViewItemEventHandler(this_DrawItem);
            this.DrawSubItem += new DrawListViewSubItemEventHandler(this_DrawSubItem);
            this.DrawColumnHeader += new DrawListViewColumnHeaderEventHandler(this_DrawColumnHeader);
        }

        //***********************************************************************************
        private void this_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        //***********************************************************************************
        public ColumnHeader InsertColumn(string text, int width)
        {
            return Columns.Add(text, width);
        }

        //***********************************************************************************
        public ListViewItem InsertItem()
        {
            ListViewItem item = Items.Add(string.Empty);
            DataItem dataitem = new DataItem();
            dataitem.type = DataType.Unknown;
            data[item.SubItems[0]] = dataitem;
            for (int i = 1; i < Columns.Count; i++)
            {
                DataItem datasubitem = new DataItem();
                datasubitem.type = DataType.Unknown;
                data[item.SubItems.Add(string.Empty)] = datasubitem;
            }
            return item;
        }

        //***********************************************************************************
        private void SetSubItemData(ListViewItem Item, int SubItemIndex, DataType datatype, string text, int imageindex, Image image, double progress)
        {
            DataItem dataitem = new DataItem();
            dataitem.type = datatype;
            dataitem.text = text;
            dataitem.imageIndex = imageindex;
            dataitem.image = image;
            dataitem.progress = progress;
            data[Item.SubItems[SubItemIndex]] = dataitem;
            Item.SubItems[SubItemIndex].Text = text;
        }

        //***********************************************************************************
        public void SetSubItemText(ListViewItem Item, int SubItemIndex, string text)
        {
            SetSubItemData(Item, SubItemIndex, DataType.Text, text, -1, null, 0.0);
        }

        //***********************************************************************************
        public void SetSubItemImage(ListViewItem Item, int SubItemIndex, string text, int imageindex)
        {
            SetSubItemData(Item, SubItemIndex, DataType.ImageIndex_Text, text, imageindex, null, 0.0);
        }

        //***********************************************************************************
        public void SetSubItemImage(ListViewItem Item, int SubItemIndex, string text, Image image)
        {
            SetSubItemData(Item, SubItemIndex, DataType.Image_Text, text, -1, image, 0.0);
        }

        //***********************************************************************************
        public void SetSubItemProgress(ListViewItem Item, int SubItemIndex, double progress)
        {
            SetSubItemData(Item, SubItemIndex, DataType.Progress, "progress", -1, null, progress);
        }

        //***********************************************************************************
        private void this_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //e.DrawFocusRectangle(); 
        }

        //***********************************************************************************
        private void this_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (!data.ContainsKey(e.SubItem))
                return;

            DataItem dataitem = data[e.SubItem];
            
            e.DrawBackground();

            if (e.Item.Selected)
                e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);

            if (dataitem.type == DataType.Text)
            {
                e.Graphics.DrawString(dataitem.text, e.SubItem.Font, (e.Item.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText, e.Bounds.Left + 1, e.Bounds.Top + 1);
            }

            if (dataitem.type == DataType.ImageIndex_Text)
            {
                if (SmallImageList != null)
                {
                    if (dataitem.imageIndex > -1 && dataitem.imageIndex < SmallImageList.Images.Count)
                    {
                        Image image = SmallImageList.Images[dataitem.imageIndex];
                        e.Graphics.DrawImage(image, new Point(e.Bounds.Left + 1, e.Bounds.Top + 1));
                    }
                }
                e.Graphics.DrawString(dataitem.text, e.SubItem.Font, (e.Item.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText, e.Bounds.Left + 16, e.Bounds.Top + 1);
            }

            if (dataitem.type == DataType.Image_Text)
            {
                if (dataitem.image != null)
                    e.Graphics.DrawImage(dataitem.image, new Point(e.Bounds.Left + 1, e.Bounds.Top + 1));
                e.Graphics.DrawString(dataitem.text, e.SubItem.Font, (e.Item.Selected) ? SystemBrushes.HighlightText : SystemBrushes.WindowText, e.Bounds.Left + 16, e.Bounds.Top + 1);
            }

            if (dataitem.type == DataType.Progress)
            {
                Rectangle rect = e.Bounds;
                rect.Y += 1;
                rect.Height -= 3;
                rect.Width -= 2;

                Rectangle rectProgress = rect;
                rectProgress.Width = (int)rect.Width * (int)dataitem.progress / 100;

                e.Graphics.FillRectangle(new SolidBrush(Color.LightYellow), rect);

                e.Graphics.FillRectangle(
                    new LinearGradientBrush(
                        new Point(e.Bounds.Left + 1, e.Bounds.Top + 1),
                        new Point(e.Bounds.Left + 1, e.Bounds.Bottom - 1),
                        Color.Lime,
                        Color.LimeGreen),
                    rectProgress);

                e.Graphics.DrawRectangle(new Pen(Color.Black), rect);

                StringFormat strFormat = new StringFormat();
                strFormat.Alignment = StringAlignment.Center;
                e.Graphics.DrawString(String.Format("{0:0.000}%", dataitem.progress), e.SubItem.Font, SystemBrushes.WindowText, rect.Left + rect.Width / 2, rect.Top, strFormat);
            }
        }

        //***********************************************************************************
        public void DeleteByKey(string key)
        {
            BeginUpdate();
            ListViewItem[] items = Items.Find(key, false);
            foreach (ListViewItem i in items)
            {
                foreach (ListViewItem.ListViewSubItem s in i.SubItems)
                    data.Remove(s);
            }
            Items.RemoveByKey(key);
            EndUpdate();
        }

        //***********************************************************************************
        public void DeleteAll()
        {
            Items.Clear();
            data.Clear();
        }
    }
}
