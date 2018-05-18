using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace XwMaxLib.UI
{
    public partial class DialogHeader : UserControl
    {
        private Color color1 = Color.FromArgb(227, 239, 255);
        private Color color2 = Color.FromArgb(197, 222, 255);
        private Color color3 = Color.FromArgb(173, 209, 255);
        private Color color4 = Color.FromArgb(193, 219, 255);
        private Pen border = new Pen(Color.FromArgb(128, 128, 128), 2);
        
        //**************************************************************************************
        public DialogHeader()
        {
            InitializeComponent();
            
        }

        //**************************************************************************************
        private void DialogHeader_Paint(object sender, PaintEventArgs e)
        {
            Rectangle top = new Rectangle(0, 0, this.Width, this.Height / 2);
            Rectangle bottom = new Rectangle(0, this.Height / 2, this.Width, this.Height);

            LinearGradientBrush t = new LinearGradientBrush(top, color1, color2, 90, false);
            LinearGradientBrush b = new LinearGradientBrush(bottom, color3, color4, 90, false);

            e.Graphics.FillRectangle(t, top);
            e.Graphics.FillRectangle(b, bottom);

            e.Graphics.DrawLine(border, 0, this.Height, this.Width, this.Height);
        }

        //**************************************************************************************
        private void DialogHeader_Load(object sender, System.EventArgs e)
        {
            this.Dock = DockStyle.Top;
            this.Height = 50;
        }

        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************
        //**************************************************************************************
        [
            Category("DialogHeader"),
            Description("First Gradient Color")
        ]
        public System.Drawing.Color Gradient1
        {
            set
            {
                this.color1 = value;
            }
            get
            {
                return this.color1;
            }
        }

        //--------------------------------------------------------------------------------------
        [
            Category("DialogHeader"),
            Description("Second Gradient Color")
        ]
        public System.Drawing.Color Gradient2
        {
            set
            {
                this.color2 = value;
            }
            get
            {
                return this.color2;
            }
        }

        //--------------------------------------------------------------------------------------
        [
            Category("DialogHeader"),
            Description("Third Gradient Color")
        ]
        public System.Drawing.Color Gradient3
        {
            set
            {
                this.color3 = value;
            }
            get
            {
                return this.color3;
            }
        }

        //--------------------------------------------------------------------------------------
        [
            Category("DialogHeader"),
            Description("Forth Gradient Color")
        ]
        public System.Drawing.Color Gradient4
        {
            set
            {
                this.color4 = value;
            }
            get
            {
                return this.color4;
            }
        }

        //--------------------------------------------------------------------------------------
        [
            Category("DialogHeader"),
            Description("HeaderTitle")
        ]
        public string HeaderTitle
        {
            get { return Title.Text; }
            set { Title.Text = value; }
        }

        //--------------------------------------------------------------------------------------
         [
            Category("DialogHeader"),
            Description("HeaderDescription")
        ]
        public string HeaderDescription
        {
            get { return Description.Text; }
            set { Description.Text = value; }
        }

         //--------------------------------------------------------------------------------------
         [
            Category("DialogHeader"),
            Description("HeaderImage")
        ]
         public Image HeaderImage
         {
             get { return Icon.Image; }
             set { Icon.Image = value; }
         }

         
    }
}
