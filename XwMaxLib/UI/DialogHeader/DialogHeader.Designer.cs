using System.ComponentModel;
namespace XwMaxLib.UI
{
    partial class DialogHeader
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Title = new System.Windows.Forms.Label();
            this.Description = new System.Windows.Forms.Label();
            this.Icon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).BeginInit();
            this.SuspendLayout();
            // 
            // Title
            // 
            this.Title.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Title.AutoEllipsis = true;
            this.Title.BackColor = System.Drawing.Color.Transparent;
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(62, 8);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(544, 13);
            this.Title.TabIndex = 0;
            this.Title.Text = "Title";
            // 
            // Description
            // 
            this.Description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.Description.AutoEllipsis = true;
            this.Description.BackColor = System.Drawing.Color.Transparent;
            this.Description.Location = new System.Drawing.Point(74, 25);
            this.Description.Name = "Description";
            this.Description.Size = new System.Drawing.Size(539, 17);
            this.Description.TabIndex = 1;
            this.Description.Text = "Description";
            // 
            // Icon
            // 
            this.Icon.BackColor = System.Drawing.Color.Transparent;
            this.Icon.Location = new System.Drawing.Point(12, 8);
            this.Icon.Name = "Icon";
            this.Icon.Size = new System.Drawing.Size(32, 32);
            this.Icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Icon.TabIndex = 2;
            this.Icon.TabStop = false;
            // 
            // DialogHeader
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Icon);
            this.Controls.Add(this.Description);
            this.Controls.Add(this.Title);
            this.Name = "DialogHeader";
            this.Size = new System.Drawing.Size(621, 50);
            this.Load += new System.EventHandler(this.DialogHeader_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.DialogHeader_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.Icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label Description;
        private System.Windows.Forms.PictureBox Icon;
    }
}
