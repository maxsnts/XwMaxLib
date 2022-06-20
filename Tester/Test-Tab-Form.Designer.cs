namespace Tester
{
    partial class Test_Tab_Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ServerTabs = new KRBTabControlNS.CustomTab.KRBTabControl();
            this.SuspendLayout();
            // 
            // ServerTabs
            // 
            this.ServerTabs.AllowDrop = true;
            this.ServerTabs.BackgroundHatcher.BackColor = System.Drawing.Color.Gray;
            this.ServerTabs.BackgroundHatcher.HatchType = System.Drawing.Drawing2D.HatchStyle.DashedVertical;
            this.ServerTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ServerTabs.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.ServerTabs.ItemSize = new System.Drawing.Size(73, 28);
            this.ServerTabs.Location = new System.Drawing.Point(0, 0);
            this.ServerTabs.Name = "ServerTabs";
            this.ServerTabs.Size = new System.Drawing.Size(839, 512);
            this.ServerTabs.TabGradient.ColorEnd = System.Drawing.Color.LightSteelBlue;
            this.ServerTabs.TabGradient.GradientStyle = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.ServerTabs.TabIndex = 0;
            this.ServerTabs.TabStyles = KRBTabControlNS.CustomTab.KRBTabControl.TabStyle.Sequence;
            this.ServerTabs.UpDownStyle = KRBTabControlNS.CustomTab.KRBTabControl.UpDown32Style.OfficeBlue;
            // 
            // Test_Tab_Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 512);
            this.Controls.Add(this.ServerTabs);
            this.Name = "Test_Tab_Form";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Test_Tab_Form";
            this.Load += new System.EventHandler(this.Test_Tab_Form_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private KRBTabControlNS.CustomTab.KRBTabControl ServerTabs;
    }
}