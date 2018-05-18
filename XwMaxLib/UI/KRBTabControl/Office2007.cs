using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;
using KRBColorTable;

namespace ColorSnapIn
{
    public sealed class Office2007 : ToolStripProfessionalRenderer
    {
        #region Static Members of the class

        private static readonly int _marginInset;
        private static readonly Blend _statusStripBlend;

        #endregion

        #region Static Constructor

        static Office2007()
        {
            _marginInset = 2;
            // One time creation of the blend for the status strip gradient brush
            _statusStripBlend = new Blend();
            _statusStripBlend.Positions = new float[] { 0.0F, 0.2F, 0.3F, 0.4F, 0.8F, 1.0F };
            _statusStripBlend.Factors = new float[] { 0.3F, 0.4F, 0.5F, 1.0F, 0.8F, 0.7F };
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Initialize a new instance of the Office2007 class.
        /// </summary>
        public Office2007()
            : base(new GrayStyles())
        {
            this.ColorTable.UseSystemColors = false;
        }

        /// <summary>
        /// Initializes a new instance of the Office2007 class.
        /// </summary>
        /// <param name="professionalColorTable">A <see cref="KRBProfessionalColors"/> to be used for painting.</param>
        public Office2007(KRBProfessionalColors professionalColorTable)
            : base(professionalColorTable)
        { }

        #endregion

        #region Override Methods

        /// <summary>
        /// Raises the RenderArrow event.
        /// </summary>
        /// <param name="e">A ToolStripArrowRenderEventArgs that contains the event data.</param>
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == false)
            {
                KRBProfessionalColors colorTable = ColorTable as KRBProfessionalColors;
                if (colorTable != null)
                {
                    if ((e.Item.Owner.GetType() == typeof(MenuStrip)) && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.MenuItemText != Color.Empty)
                        {
                            e.ArrowColor = colorTable.MenuItemText;
                        }
                    }
                    if ((e.Item.Owner.GetType() == typeof(StatusStrip)) && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.StatusStripText != Color.Empty)
                        {
                            e.ArrowColor = colorTable.StatusStripText;
                        }
                    }
                }
            }
            base.OnRenderArrow(e);
        }
        
        /// <summary>
        /// Raises the RenderItemText event.
        /// </summary>
        /// <param name="e">A ToolStripItemTextRenderEventArgs that contains the event data.</param>
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == false)
            {
                KRBProfessionalColors colorTable = ColorTable as KRBProfessionalColors;
                if (colorTable != null)
                {
                    if ((e.ToolStrip is MenuStrip) && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.MenuItemText != Color.Empty)
                        {
                            e.TextColor = colorTable.MenuItemText;
                        }
                    }
                    if ((e.ToolStrip is StatusStrip) && (e.Item.Selected == false) && e.Item.Pressed == false)
                    {
                        if (colorTable.StatusStripText != Color.Empty)
                        {
                            e.TextColor = colorTable.StatusStripText;
                        }
                    }
                }
            }
            base.OnRenderItemText(e);
        }
        
        /// <summary>
        /// Raises the RenderToolStripContentPanelBackground event. 
        /// </summary>
        /// <param name="e">An ToolStripContentPanelRenderEventArgs containing the event data.</param>
        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            // Must call base class, otherwise the subsequent drawing does not appear!
            base.OnRenderToolStripContentPanelBackground(e);
            if (ColorTable.UseSystemColors == false)
            {
                // Cannot paint a zero sized area
                if ((e.ToolStripContentPanel.Width > 0) &&
                    (e.ToolStripContentPanel.Height > 0))
                {
                    using (LinearGradientBrush backBrush = new LinearGradientBrush(e.ToolStripContentPanel.ClientRectangle,
                                                                                   ColorTable.ToolStripContentPanelGradientBegin,
                                                                                   ColorTable.ToolStripContentPanelGradientEnd,
                                                                                   LinearGradientMode.Vertical))
                    {
                        e.Graphics.FillRectangle(backBrush, e.ToolStripContentPanel.ClientRectangle);
                    }
                }
            }
        }
        
        /// <summary>
        /// Raises the RenderSeparator event. 
        /// </summary>
        /// <param name="e">An ToolStripSeparatorRenderEventArgs containing the event data.</param>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == false)
            {
                e.Item.ForeColor = ColorTable.RaftingContainerGradientBegin;
            }
            base.OnRenderSeparator(e);
        }
        
        /// <summary>
        /// Raises the RenderToolStripBackground event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderToolStripBackground(e);
            }
            else
            {
                if (e.ToolStrip is StatusStrip)
                {
                    // We do not paint the top two pixel lines, so are drawn by the status strip border render method
                    //RectangleF backRectangle = new RectangleF(0, 1.5f, e.ToolStrip.Width, e.ToolStrip.Height - 2);
                    RectangleF backRectangle = new RectangleF(0, 0, e.ToolStrip.Width, e.ToolStrip.Height);

                    // Cannot paint a zero sized area
                    if ((backRectangle.Width > 0) && (backRectangle.Height > 0))
                    {
                        using (LinearGradientBrush backBrush = new LinearGradientBrush(backRectangle,
                                                                                       ColorTable.StatusStripGradientBegin,
                                                                                       ColorTable.StatusStripGradientEnd,
                                                                                       LinearGradientMode.Vertical))
                        {
                            backBrush.Blend = _statusStripBlend;
                            e.Graphics.FillRectangle(backBrush, backRectangle);
                        }
                    }
                }
                else
                {
                    base.OnRenderToolStripBackground(e);
                }
            }
        }
        
        /// <summary>
        /// Raises the RenderImageMargin event. 
        /// </summary>
        /// <param name="e">An ToolStripRenderEventArgs containing the event data.</param>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (ColorTable.UseSystemColors == true)
            {
                base.OnRenderToolStripBackground(e);
            }
            else
            {
                if ((e.ToolStrip is ContextMenuStrip) ||
                    (e.ToolStrip is ToolStripDropDownMenu))
                {
                    // Start with the total margin area
                    Rectangle marginRectangle = e.AffectedBounds;

                    // Do we need to draw with separator on the opposite edge?
                    bool bIsRightToLeft = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

                    marginRectangle.Y += _marginInset;
                    marginRectangle.Height -= _marginInset * 2;

                    // Reduce so it is inside the border
                    if (bIsRightToLeft == false)
                    {
                        marginRectangle.X += _marginInset;
                    }
                    else
                    {
                        marginRectangle.X += _marginInset / 2;
                    }

                    // Draw the entire margine area in a solid color
                    using (SolidBrush backBrush = new SolidBrush(
                        ColorTable.ImageMarginGradientBegin))
                        e.Graphics.FillRectangle(backBrush, marginRectangle);
                }
                else
                {
                    base.OnRenderImageMargin(e);
                }
            }
        }

        #endregion
    }
}