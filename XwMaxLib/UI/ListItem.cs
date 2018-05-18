using System;
using System.Collections.Generic;
using System.Text;

namespace XwMaxLib.Objects
{
    public class ListItem
    {
        public int ID;
        public string Text;

        public ListItem(int id, string text)
        {
            ID = id;
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
