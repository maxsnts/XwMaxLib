using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel.Design;
using KRBTabControlNS.Win32;
using XwMaxLib.UI.KRBTabControl;

namespace KRBTabControlNS.CustomTab
{
    [Designer(typeof(KRBTabControlDesigner))]
    public class KRBTabControl : TabControl
    {
        #region Enum
        
        public enum UpDown32Style
        {
            BlackGlass,
            OfficeBlue,
            OfficeOlive,
            OfficeSilver,
            KRBBlue
        };

        public enum TabAlignments
        {
            Bottom,
            Top
        };

        public enum TabStyle
        {
            Sequence,
            KRBStyle
        };

        public enum TabHeaderStyle
        {
            Solid,
            Hatch,
            Texture
        };

        public enum TabCloseImage
        {
            Normal,
            Hover,
            Pressed
        };

        /// <summary>
        /// Specifies the four styles of raised or inset rectangles. Lines can have either
        /// Ridge or Groove appearance only.
        /// </summary>
        internal enum ThreeDStyle
        {
            /// <summary>
            /// Inset, groove appearance.
            /// </summary>
            Groove,
            /// <summary>
            /// Inset appearance. Applies only to rectangles, not to lines.
            /// </summary>
            Inset,
            /// <summary>
            /// Raised appearance. Applies only to rectangles, not to lines.
            /// </summary>
            Raised,
            /// <summary>
            /// Raised, ridge appearance.
            /// </summary>
            Ridge,
        }
        
        #endregion

        #region Symbolic Constants

        private readonly int _value = SystemInformation.Border3DSize.Width;      // Sistemde kullanılan çerçeve kalınlığı

        #endregion

        #region Instance Members

        private Rectangle _imgCloseRectangle;
        private GradientTab _tabGradient;
        private Hatcher _hatcher;
        private TabPageSelector _tabSelector = null;
        private TabPageExPool _tabPageExPool;
        private Custom3DBorder _tabBorder;
        private TabStyle _tabStyles = TabStyle.KRBStyle;                //Initializer
        private Scroller _myScroller = null;                            //Initializer
        private UpDown32 _upDown32 = null;                              //Initializer
        private Color _backgroundColor = SystemColors.Info;             //Initializer
        private Color _borderColor = Color.Gray;                        //Initializer
        private Color _tabBorderColor = SystemColors.ControlDark;       //Initializer
        private bool _isDrawHeader = true;                              //Initializer
        private bool _headerVisibility = true;                          //Initializer
        private TabAlignments _alignments = TabAlignments.Top;          //Initializer
        private TabHeaderStyle _headerStyle = TabHeaderStyle.Solid;     //Initializer
        private UpDown32Style _upDownStyle = UpDown32Style.BlackGlass;  //Initializer
        public TabCloseImage _tabCloseBtn = TabCloseImage.Normal;       //Initializer
        private bool _mouseImgProcessing = true;                        //Initializer
        private Cursor _myCursor = null;                                //Initializer
        private Image _backgroundImage = null;                          //Initializer
        private ArrowWindow _upArrow = null;                            //Initializer
        private ArrowWindow _downArrow = null;                          //Initializer

        #endregion

        #region Events
        public event CancelEventHandler TabClosing;
        #endregion

        #region Constructor

        public KRBTabControl()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.UserMouse, true);

            this.Size = new Size()  //Object Initializer
            {
                Width = 300,
                Height = 200
            };

            this.ItemSize = new Size()
            {
                Width = 73,
                Height = 28
            };

            _tabGradient = new GradientTab(Color.White, Color.Gainsboro, LinearGradientMode.Horizontal, Color.Black, Color.Black, FontStyle.Regular);    //Instantiate
            _tabGradient.GradientChanged += new EventHandler(_tabGradient_GradientChanged);

            _hatcher = new Hatcher(Color.White, Color.Gainsboro, HatchStyle.DashedVertical);
            _hatcher.HatchChanged += new EventHandler(_hatcher_HatchChanged);

            
            this.AllowDrop = true;  // For drag and drop tab pages.You can change this from control's property.
        }

        #endregion

        #region Property
        
        /// <summary>
        /// Tab Kontrolümüzün görünümünü ayarlar.
        /// </summary>
        [Description("Tab Kontrolümüzün görünümünü ayarlar")]
        [DefaultValue(typeof(TabStyle), "KRBStyle")]
        [Browsable(true)]
        public TabStyle TabStyles
        {
            get { return _tabStyles; }
            set
            {
                if (!value.Equals(_tabStyles))
                {
                    _tabStyles = value;

                    Invalidate();
                    Update();
                }
            }
        }

        /// <summary>
        /// Tab Kontrolünün sınır çizgi rengini ayarlar.
        /// </summary>
        [Description("Tab Kontrolünün sınır çizgi rengini ayarlar")]
        [DefaultValue(typeof(Color), "Gray")]
        [Browsable(true)]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (!value.Equals(_borderColor))
                {
                    _borderColor = value;
                    
                    Invalidate();
                    Update();
                }
            }
        }

        /// <summary>
        /// TabPage sınır çizgi rengini ayarlar.
        /// </summary>
        [Description("TabPage sınır çizgi rengini ayarlar")]
        [DefaultValue(typeof(Color), "ControlDark")]
        [Browsable(true)]
        public Color TabBorderColor
        {
            get { return _tabBorderColor; }
            set
            {
                if (!value.Equals(_tabBorderColor))
                {
                    _tabBorderColor = value;
                    
                    Invalidate();
                    Update();
                }
            }
        }

        /// <summary>
        /// Tab başlığının hangi stil'de çizileceğini ayarlar.
        /// </summary>
        [Description("Tab başlığının hangi stil'de çizileceğini ayarlar")]
        [DefaultValue(typeof(TabHeaderStyle), "Solid")]
        [Browsable(true)]
        public TabHeaderStyle HeaderStyle
        {
            get { return _headerStyle; }
            set 
            {
                if (!value.Equals(_headerStyle))
                {
                    _headerStyle = value;

                    Invalidate();
                    Update();
                }
            }
        }
        
        /// <summary>
        /// Tab Kontrolü'nü belirtilen stil de hizalar.
        /// </summary>
        [Description("Tab Kontrolü'nü belirtilen stil de hizalar")]
        [DefaultValue(typeof(TabAlignments), "Top")]
        [Browsable(true)]
        public TabAlignments Alignments
        {
            get { return _alignments; }
            set
            {
                if (!value.Equals(_alignments))
                {
                    _alignments = value;

                    if (Enum.GetName(typeof(TabAlignments), _alignments) == "Top")
                        base.Alignment = TabAlignment.Top;
                    else
                        base.Alignment = TabAlignment.Bottom;
                }
            }
        }

        /// <summary>
        /// Tab kontrolünün görünüm ayarlarını düzenler.
        /// </summary>
        [Description("Tab kontrolünün görünüm ayarlarını düzenler")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public GradientTab TabGradient
        {
            get { return _tabGradient; }
            set
            {
                if (!value.Equals(_tabGradient))
                    _tabGradient = value;
            }
        }

        /// <summary>
        /// Tab kontrolünün arkaplan görünüm ayarlarını düzenler.
        /// </summary>
        [Description("Tab kontrolünün arkaplan görünüm ayarlarını düzenler")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [Browsable(true)]
        public Hatcher BackgroundHatcher
        {
            get { return _hatcher; }
            set 
            {
                if (!value.Equals(_hatcher))
                    _hatcher = value;
            }
        }
        
        /// <summary>
        /// Tab kontrolünün arkaplan rengini ayarlar.
        /// </summary>
        [Description("Tab kontrolünün arkaplan rengini ayarlar")]
        [DefaultValue(typeof(Color), "Info")]
        [Browsable(true)]
        public Color BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (!value.Equals(_backgroundColor))
                {
                    _backgroundColor = value;
                    
                    if (_headerStyle == TabHeaderStyle.Solid)
                    {
                        Invalidate();
                        Update();
                    }
                }
            }
        }

        /// <summary>
        /// UpDown kontrolünün görünümünü ayarlar.
        /// </summary>
        [Description("UpDown kontrolünün görünümünü ayarlar")]
        [DefaultValue(typeof(UpDown32Style), "BlackGlass")]
        [Browsable(true)]
        public UpDown32Style UpDownStyle
        {
            get { return _upDownStyle; }
            set
            {
                if (!value.Equals(_upDownStyle))
                {
                    _upDownStyle = value;

                    if (_upDown32 != null)
                    {
                        if (_myScroller != null)
                        {
                            _myScroller.ScrollLeft -= new EventHandler(ScrollLeft);
                            _myScroller.ScrollRight -= new EventHandler(ScrollRight);
                            _myScroller.Dispose();
                            _myScroller = null;
                        }

                        _myScroller = new Scroller(_upDownStyle);
                        _myScroller.ScrollLeft += new EventHandler(ScrollLeft);
                        _myScroller.ScrollRight += new EventHandler(ScrollRight);

                        IntPtr parentHwnd = User32.GetParent(_myScroller.Handle);

                        if (parentHwnd != this.Handle)
                            User32.SetParent(_myScroller.Handle, this.Handle);

                        this.OnResize(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// Tab kontrolünün başlığının görünüp görünmemesini ayarlayabilirsiniz.
        /// </summary>
        [Description("Tab kontrolünün başlığının görünüp görünmemesini ayarlayabilirsiniz")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool HeaderVisibility
        {
            get { return _headerVisibility; }
            set
            {
                if (!value.Equals(_headerVisibility))
                {
                    _headerVisibility = value;

                    if (value == true)
                        this.Multiline = false;
                    else
                        this.Multiline = true;

                    this.UpdateStyles();
                }
            }
        }


        /// <summary>
        /// Tab kontrolünün başlık arkaplanının çizilip çizilmemesini ayarlayabilirsiniz.
        /// </summary>
        [Description("Tab kontrolünün başlık arkaplanının çizilip çizilmemesini ayarlayabilirsiniz")]
        [DefaultValue(true)]
        [Browsable(true)]
        public bool IsDrawHeader
        {
            get { return _isDrawHeader; }
            set 
            {
                if (!value.Equals(_isDrawHeader))
                {
                    _isDrawHeader = value;

                    if (!_isDrawHeader)
                    {
                        this.DrawMode = TabDrawMode.Normal;

                        typeof(Control).InvokeMember("SetStyle", System.Reflection.BindingFlags.DeclaredOnly |
                            System.Reflection.BindingFlags.Public |
                            System.Reflection.BindingFlags.NonPublic |
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.InvokeMethod, null, this, new object[] { ControlStyles.SupportsTransparentBackColor, true });
                    }
                    else
                        this.DrawMode = TabDrawMode.OwnerDrawFixed;

                    this.UpdateStyles();
                }
            }
        }

        [Browsable(true)]
        public new Image BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }
            set
            {
                if (value != null)
                {
                    if (!value.Equals(_backgroundImage))
                    {
                        _backgroundImage = value;

                        if (_headerStyle == TabHeaderStyle.Texture)
                        {
                            Invalidate();
                            Update();
                        }
                    }
                }
                else
                {
                    _backgroundImage = null;

                    if (_headerStyle == TabHeaderStyle.Texture)
                    {
                        Invalidate();
                        Update();
                    }
                }
            }
        }
        
        [Editor(typeof(TabpageExCollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        public new TabPageCollection TabPages
        {
            get
            {
                return base.TabPages;
            }
        }

        public new Size ItemSize
        {
            get { return base.ItemSize; }
            set
            {
                if (!value.Equals(base.ItemSize))
                {
                    if (value.Height < 26)
                        value.Height = 26;
                    else if (value.Height > 80)
                        value.Height = 80;

                    base.ItemSize = value;

                    Invalidate();
                    Update();
                }
            }
        }

        public override System.Drawing.Rectangle DisplayRectangle
        {
            get
            {
                if (!_headerVisibility)
                {
                    return new Rectangle(1, 1, this.Width - 2, this.Height - 2);    // Başlık yok.
                }
                else
                {
                    switch (_alignments)
                    {
                        case TabAlignments.Top:
                            return new Rectangle(1, ItemSize.Height + 6, this.Width - 2, this.Height - ItemSize.Height - 7);
                        default:
                            return new Rectangle(1, 1, this.Width - 2, this.Height - ItemSize.Height - 6);
                    }
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                if (!_isDrawHeader)
                    return Color.Transparent;

                return base.BackColor;
            }
        }

        [Browsable(false)]
        public new TabDrawMode DrawMode
        {
            get { return base.DrawMode; }
            set 
            {
                if (!value.Equals(base.DrawMode))
                {
                    if (_tabStyles == TabStyle.Sequence)
                        base.DrawMode = TabDrawMode.Normal;
                    else
                        base.DrawMode = TabDrawMode.OwnerDrawFixed;
                }
            }
        }

        [Browsable(false)]
        public new TabAlignment Alignment
        {
            get { return base.Alignment; }
            set
            {
                if (!value.Equals(base.Alignment))
                {
                    base.Alignment = value;

                    Invalidate();
                    Update();
                }
            }
        }

        [DefaultValue(false)]
        [Browsable(false)]
        public new bool Multiline
        {
            get { return base.Multiline; }
            set
            {
                if (!value.Equals(base.Multiline))
                {
                    if (_headerVisibility)
                        base.Multiline = false;
                    else
                        base.Multiline = true;
                }
            }
        }

        [Browsable(false)]
        public new TabAppearance Appearance
        {
            get { return base.Appearance; }
        }

        [Browsable(false)]
        public new TabSizeMode SizeMode
        {
            get { return base.SizeMode; }
        }

        [Browsable(false)]
        public new bool HotTrack
        {
            get { return base.HotTrack; }
        }

        #endregion

        #region Override Methods

        protected override void OnResize(EventArgs e)
        {
            if (!this.Multiline && _upDown32 != null)
            {
                Rectangle rctTabArea = this.DisplayRectangle;

                if (this.Alignments == TabAlignments.Top)
                {
                    _myScroller.Location = new Point(this.Width - _myScroller.Width - 4, rctTabArea.Top - _myScroller.Height - 8);
                }
                else
                {
                    _myScroller.Location = new Point(this.Width - _myScroller.Width - 4, rctTabArea.Bottom + 8);
                }

                this.Invalidate();
            }

            base.OnResize(e);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (Visible)
                Draw(pe.Graphics);
        }

        protected override void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            gfbevent.UseDefaultCursors = this._myCursor == null ? true : false;

            if (gfbevent.Effect == DragDropEffects.Move)
                Cursor.Current = this._myCursor;
            else
            {
                Cursor.Current = Cursors.No;

                if (_upArrow != null && _upArrow.Visible)
                    _upArrow.Visible = false;

                if (_downArrow != null && _downArrow.Visible)
                    _downArrow.Visible = false;
            }
        }

        protected override void OnDragOver(DragEventArgs drgevent)
        {
            if (!DesignMode)
            {
                TabPageEx over = OverTab();

                if (drgevent.KeyState == 1 && over != null)
                {
                    drgevent.Effect = DragDropEffects.Move;

                    int draggingTabIndex = this.SelectedIndex;
                    int overringTabIndex = this.TabPages.IndexOf(over);

                    if (overringTabIndex != draggingTabIndex)
                    {
                        Rectangle rctOver = this.GetTabRect(overringTabIndex);

                        if (overringTabIndex == 0)
                            rctOver.Inflate(-4, 0);

                        if (_alignments == TabAlignments.Top)
                            rctOver.Y += 3;
                        else
                            rctOver.Y -= 4;

                        if (_upArrow == null)
                            _upArrow = new ArrowWindow(KRBTabResources.UpArrow, 0.5);

                        if (_downArrow == null)
                            _downArrow = new ArrowWindow(KRBTabResources.DownArrow, 0.5);

                        Point upArrowPoint = Point.Empty;
                        Point downArrowPoint = Point.Empty;

                        if (overringTabIndex > draggingTabIndex)
                        {
                            if (_alignments == TabAlignments.Top)   // Bottom düzenlenecek
                            {
                                if (_tabStyles == TabStyle.Sequence)
                                {
                                    downArrowPoint = new Point(rctOver.Right - _downArrow.Width / 2, rctOver.Top + 2 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Right - _upArrow.Width / 2, rctOver.Bottom - 3);
                                }
                                else
                                {
                                    downArrowPoint = new Point(rctOver.Right - _downArrow.Width / 2 + 1, rctOver.Top + 2 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Right - _upArrow.Width / 2 + 1, rctOver.Bottom - 10);
                                }
                            }
                            else
                            {
                                if (_tabStyles == TabStyle.Sequence)
                                {
                                    downArrowPoint = new Point(rctOver.Right - _downArrow.Width / 2, rctOver.Top + 5 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Right - _upArrow.Width / 2, rctOver.Bottom - 1);
                                }
                                else
                                {
                                    downArrowPoint = new Point(rctOver.Right - _downArrow.Width / 2 + 1, rctOver.Top + 11 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Right - _upArrow.Width / 2 + 1, rctOver.Bottom - 3);
                                }
                            }
                        }
                        else
                        {
                            if (_alignments == TabAlignments.Top)   // Bottom düzenlenecek
                            {
                                if (_tabStyles == TabStyle.Sequence)
                                {
                                    downArrowPoint = new Point(rctOver.Left - _downArrow.Width / 2, rctOver.Top + 2 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Left - _upArrow.Width / 2, rctOver.Bottom - 3);
                                }
                                else
                                {
                                    downArrowPoint = new Point(rctOver.Left - _downArrow.Width / 2 + 1, rctOver.Top + 2 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Left - _upArrow.Width / 2 + 1, rctOver.Bottom - 10);
                                }
                            }
                            else
                            {
                                if (_tabStyles == TabStyle.Sequence)
                                {
                                    downArrowPoint = new Point(rctOver.Left - _downArrow.Width / 2, rctOver.Top + 5 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Left - _upArrow.Width / 2, rctOver.Bottom - 1);
                                }
                                else
                                {
                                    downArrowPoint = new Point(rctOver.Left - _downArrow.Width / 2 + 1, rctOver.Top + 11 - _downArrow.Height);
                                    upArrowPoint = new Point(rctOver.Left - _upArrow.Width / 2 + 1, rctOver.Bottom - 3);
                                }
                            }
                        }

                        _upArrow.Location = this.PointToScreen(upArrowPoint);
                        _downArrow.Location = this.PointToScreen(downArrowPoint);

                        if (!_upArrow.Visible)
                            _upArrow.Visible = true;

                        if (!_downArrow.Visible)
                            _downArrow.Visible = true;
                    }
                    else
                        drgevent.Effect = DragDropEffects.None;
                }
                else
                    drgevent.Effect = DragDropEffects.None;
            }

            base.OnDragOver(drgevent);
        }

        protected override void OnDragDrop(DragEventArgs drgevent)
        {
            if (!DesignMode)
            {
                TabPageEx over = OverTab();

                if (over != null)
                {
                    if (drgevent.Data.GetDataPresent(typeof(TabPageEx)))
                    {
                        TabPageEx draggingTab = drgevent.Data.GetData(typeof(TabPageEx)) as TabPageEx;

                        if (draggingTab != over)
                        {
                            int overringTabIndex = this.TabPages.IndexOf(over);

                            this.TabPages.Remove(draggingTab);
                            this.TabPages.Insert(overringTabIndex, draggingTab);

                            // Select our dragging tab.
                            this.SelectedTab = draggingTab;

                            _upArrow.Hide();
                            _downArrow.Hide();
                        }
                    }
                }
            }
            
            base.OnDragDrop(drgevent);
        }

        byte process = 0;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_imgCloseRectangle.Contains(e.Location))
            {
                if (process == 1)
                    return;
                else
                {
                    _tabCloseBtn = TabCloseImage.Hover;
                    
                    _mouseImgProcessing = false;
                    
                    Invalidate();
                    process++;
                }
            }
            else
            {
                if (!_mouseImgProcessing)
                {
                    _tabCloseBtn = TabCloseImage.Normal;

                    _mouseImgProcessing = true;
                    
                    Invalidate();
                    process = 0;
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnKeyDown(KeyEventArgs ke)
        {
            if (ke.KeyCode == Keys.Left || ke.KeyCode == Keys.Right)
                this.Invalidate();

            base.OnKeyDown(ke);
        }
        
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_imgCloseRectangle.Contains(e.Location))
                {
                    _tabCloseBtn = TabCloseImage.Pressed;

                    Invalidate();
                }
                else if (!DesignMode && this.TabCount > 1)
                {
                    Rectangle draggingTab = this.GetTabRect(this.SelectedIndex);

                    if (this.SelectedIndex == 0)
                        if (draggingTab.X + 4 > e.X)
                            return;

                    if (_alignments == TabAlignments.Top)
                        draggingTab.Y += 3;
                    else
                        draggingTab.Y -= 3;

                    if (draggingTab.Contains(e.X, e.Y))
                    {
                        string filePath = System.IO.Path.Combine(Application.StartupPath, "Drag.cur");
                        this._myCursor = GetCustomCursor(filePath);
                        
                        this.DoDragDrop((TabPageEx)this.SelectedTab, DragDropEffects.Move);
                    }
                }
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_imgCloseRectangle.Contains(e.Location))
                    _tabCloseBtn = TabCloseImage.Hover;
                else
                    _tabCloseBtn = TabCloseImage.Normal;

                Invalidate();
            }

            base.OnMouseUp(e);
        }

        public void CloseTabByButton(TabPage tab)
        {
            SelectTab(tab);
            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, _imgCloseRectangle.X + 1, _imgCloseRectangle.Y+1, 0));
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (DesignMode)
                {
                    if (this.TabCount >= 1)
                    {
                        if (_imgCloseRectangle.Contains(e.Location))
                        {
                            _tabCloseBtn = TabCloseImage.Normal;

                            this.SelectedTab.Dispose();

                            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                            selectionService.SetSelectedComponents(new IComponent[] { this }, SelectionTypes.Auto);
                        }
                        else
                        {
                            TabPageEx over = OverTab();

                            if (over != null)
                                this.SelectedTab = over;
                        }
                    }
                }
                else
                {
                    if (this.TabCount >= 1)
                    {
                        if (_imgCloseRectangle.Contains(e.Location))
                        {
                            TabPage closingTab = this.SelectedTab;
                            
                            CancelEventArgs cancel = new CancelEventArgs();
                            if (TabClosing != null)
                                TabClosing(this, cancel);

                            if (cancel.Cancel == false)
                            {
                                if (this.SelectedTab != null && this.SelectedTab == closingTab)
                                    this.SelectedTab.Dispose();
                            }

                            _tabCloseBtn = TabCloseImage.Normal;
                        }
                        else
                        {
                            TabPageEx over = OverTab();

                            if (over != null)
                                this.SelectedTab = over;
                        }
                    }
                }
            }
            /*TODO
            else if (e.Button == MouseButtons.Right)
            {
                if (!DesignMode)
                {
                    if (_tabSelector == null)
                        _tabSelector = new TabPageSelector(this);
                }
            }
            */

            base.OnMouseClick(e);
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (DesignMode)
                {
                    if (this.TabCount >= 1)
                    {
                        if (_imgCloseRectangle.Contains(e.Location))
                        {
                            _tabCloseBtn = TabCloseImage.Normal;

                            this.SelectedTab.Dispose();

                            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                            selectionService.SetSelectedComponents(new IComponent[] { this }, SelectionTypes.Auto);
                        }
                        else
                        {
                            TabPageEx over = OverTab();

                            if (over != null)
                                this.SelectedTab = over;
                        }
                    }
                }
                else
                {
                    if (this.TabCount >= 1)
                    {
                        if (_imgCloseRectangle.Contains(e.Location))
                        {
                            _tabCloseBtn = TabCloseImage.Normal;

                            this.SelectedTab.Dispose();
                        }
                        else
                        {
                            TabPageEx over = OverTab();

                            if (over != null)
                                this.SelectedTab = over;
                        }
                    }
                }
            }

            base.OnMouseDoubleClick(e);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (_upDown32 != null)
            {
                if (this.SelectedIndex == 0)
                {
                    _myScroller._leftScroller.Enabled = false;
                    _myScroller._rightScroller.Enabled = true;
                }
                else if (this.SelectedIndex == this.TabCount - 1)
                {
                    _myScroller._leftScroller.Enabled = true;
                    _myScroller._rightScroller.Enabled = false;
                }
                else
                {
                    _myScroller._leftScroller.Enabled = true;
                    _myScroller._rightScroller.Enabled = true;
                }
            }

            base.OnSelectedIndexChanged(e);
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            if (e.Control is TabPageEx && e.Control.Text == String.Empty)
            {
                TabPageEx adding = e.Control as TabPageEx;

                int value = 0;

                foreach (TabPage tab in this.TabPages)
                {
                    if (tab.Name.Contains("tabPageEx"))
                    {
                        try
                        {
                            int current = Convert.ToInt32(tab.Name.Substring(9, tab.Name.Length - 9));
                            value = Math.Max(value, current);
                        }
                        catch { }
                    }
                }

                adding.Name = "tabPageEx" + (value + 1).ToString();
                adding.Text = adding.Name;
            }

            base.OnControlAdded(e);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == (int)User32.Msgs.WM_PARENTNOTIFY)
            {
                if ((ushort)(m.WParam.ToInt32() & 0xFFFF) == (int)User32.Msgs.WM_CREATE)
                {
                    IntPtr handle = FindUpDownControl();

                    if (handle != IntPtr.Zero)
                    {
                        if (_upDown32 != null)
                            _upDown32.ReleaseHandle();

                        _upDown32 = new UpDown32(handle);

                        if (_myScroller != null)
                        {
                            _myScroller.ScrollLeft -= new EventHandler(ScrollLeft);
                            _myScroller.ScrollRight -= new EventHandler(ScrollRight);
                            _myScroller.Dispose();
                            _myScroller = null;
                        }

                        _myScroller = new Scroller(_upDownStyle);
                        _myScroller.ScrollLeft += new EventHandler(ScrollLeft);
                        _myScroller.ScrollRight += new EventHandler(ScrollRight);

                        this.OnResize(EventArgs.Empty);
                        
                        IntPtr parentHwnd = User32.GetParent(_myScroller.Handle);

                        if (parentHwnd != m.HWnd)
                            User32.SetParent(_myScroller.Handle, m.HWnd);

                        if (this.SelectedIndex == 0)
                            _myScroller._leftScroller.Enabled = false;
                        else if (this.SelectedIndex == this.TabCount - 1)
                            _myScroller._rightScroller.Enabled = false;
                    }
                }
            }
            else if (m.Msg == (int)User32.Msgs.WM_PAINT)
            {
                if (_upDown32 != null)
                {
                    if (this.TabCount > 1 && !this.Multiline)
                    {
                        int width = 0;

                        for (int i = 0; i < this.TabCount; i++)
                            width += GetTabRect(i).Width;

                        if (width <= _myScroller.Left)
                            _myScroller.Visible = false;
                        else
                            _myScroller.Visible = true;
                    }
                    else if (this.TabCount == 1 && !this.Multiline)
                        _myScroller.Visible = false;
                }
            }
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _tabGradient.GradientChanged -= _tabGradient_GradientChanged;
                _tabGradient.Dispose();

                _hatcher.HatchChanged -= _hatcher_HatchChanged;
                _hatcher.Dispose();

                if (_tabSelector != null)
                    _tabSelector.Dispose();
                if (_tabPageExPool != null)
                    _tabPageExPool.Dispose();
                if (_tabBorder != null)
                    _tabBorder.Dispose();
                if (_myCursor != null)
                    _myCursor.Dispose();
                if (_upArrow != null)
                    _upArrow.Dispose();
                if (_downArrow != null)
                    _downArrow.Dispose();
                if (_myScroller != null)
                {
                    _myScroller.ScrollLeft -= new EventHandler(ScrollLeft);
                    _myScroller.ScrollRight -= new EventHandler(ScrollRight);
                    _myScroller.Dispose();
                }
                if (_upDown32 != null)
                {
                    _upDown32.ReleaseHandle();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Virtual Methods

        protected virtual void Draw(Graphics gfx)
        {
            DrawBorder(gfx);

            // Draw Tab Header Background and Fill the tabpage border line.
            if (this.SelectedTab != null && _headerVisibility)
            {
                if (IsDrawHeader)
                {
                    Rectangle rct = this.ClientRectangle;
                    Rectangle rctTabArea = this.DisplayRectangle;

                    rct.Inflate(-1, -1);

                    SolidBrush solidBrush = null;
                    HatchBrush hatchBrush = null;
                    TextureBrush textureBrush = null;

                    switch (_alignments)
                    {
                        case TabAlignments.Top:
                            if (_headerStyle == TabHeaderStyle.Solid)
                            {
                                solidBrush = new SolidBrush(_backgroundColor);
                                gfx.FillRectangle(solidBrush, rct.Left, rct.Top, rct.Width, rctTabArea.Top - 6);    // KRBStyle
                                solidBrush.Dispose();
                            }
                            else if (_headerStyle == TabHeaderStyle.Hatch)
                            {
                                hatchBrush = new HatchBrush(_hatcher.HatchType, _hatcher.ForeColor, _hatcher.BackColor);
                                gfx.FillRectangle(hatchBrush, rct.Left, rct.Top, rct.Width, rctTabArea.Top - 6);    // KRBStyle
                                hatchBrush.Dispose();
                            }
                            else
                            {
                                if (_backgroundImage != null)
                                {
                                    textureBrush = new TextureBrush(_backgroundImage, WrapMode.TileFlipXY);
                                    gfx.FillRectangle(textureBrush, rct.Left, rct.Top, rct.Width, rctTabArea.Top - 6);    // KRBStyle
                                    textureBrush.Dispose();
                                }
                            }
                            break;
                        case TabAlignments.Bottom://düzenlenecek
                            if (_headerStyle == TabHeaderStyle.Solid)
                            {
                                solidBrush = new SolidBrush(_backgroundColor);
                                gfx.FillRectangle(solidBrush, rct.Left, rctTabArea.Bottom + 5, rct.Width, rct.Height - rctTabArea.Bottom - 4);    // KRBStyle
                                solidBrush.Dispose();
                            }
                            else if (_headerStyle == TabHeaderStyle.Hatch)
                            {
                                hatchBrush = new HatchBrush(_hatcher.HatchType, _hatcher.ForeColor, _hatcher.BackColor);
                                gfx.FillRectangle(hatchBrush, rct.Left, rctTabArea.Bottom + 5, rct.Width, rct.Height - rctTabArea.Bottom - 4);    // KRBStyle
                                hatchBrush.Dispose();
                            }
                            else
                            {
                                if (_backgroundImage != null)
                                {
                                    textureBrush = new TextureBrush(_backgroundImage, WrapMode.TileFlipXY);
                                    gfx.FillRectangle(textureBrush, rct.Left, rctTabArea.Bottom + 5, rct.Width, rct.Height - rctTabArea.Bottom - 4);    // KRBStyle
                                    textureBrush.Dispose();
                                }
                            }
                            break;
                    }
                }

                //Draw Tabs
                for (int i = 0; i < this.TabCount; i++)
                    DrawTabs(gfx, i);
            }
        }

        protected virtual void DrawTabs(Graphics gfx, int nIndex)
        {
            Rectangle currentTab = this.GetTabRect(nIndex);

            /* Eğer ilk tabın indisi 0 ise Rectangle'ın Left location değerini belirtilen oranda artırıyoruz.
            Aynı zamanda Tab genişliğinide gene belirlenen oran kadar küçültüyoruz. */
            if (nIndex == 0)
            {
                currentTab.X += 4;
                currentTab.Width -= 4;
            }

            if (_alignments == TabAlignments.Top)
                currentTab.Y += 3;
            else
                currentTab.Y -= 4;

            if (nIndex == this.SelectedIndex)
            {
                LinearGradientBrush pathBrush = null;

                Rectangle rct = this.ClientRectangle;
                Rectangle rctTabArea = this.DisplayRectangle;

                rct.Inflate(-1, -1);
                rctTabArea.Inflate(_value - 1, _value - 1);

                Image closeImg = GetCloseTabImage();

                switch (_alignments)
                {
                    case TabAlignments.Top:
                        
                        _imgCloseRectangle = new Rectangle()  // Object Initializer
                        {
                            X = currentTab.Right - (closeImg.Width + 5),
                            Y = (currentTab.Height - closeImg.Height) / 2 + 4,
                            Width = closeImg.Width,
                            Height = closeImg.Height,
                        };
                        
                        using (Pen pen1 = new Pen(_tabBorderColor))
                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            // Create an open figure
                            gp.AddLine(rct.Left, rctTabArea.Top, rct.Left, rctTabArea.Top - 4);
                            gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top);
                            gp.AddLine(currentTab.Right - 1, currentTab.Top, currentTab.Right - 1, rctTabArea.Top - 4);
                            gp.AddLine(rct.Right, rctTabArea.Top - 4, rct.Right, rctTabArea.Top);

                            pathBrush = new LinearGradientBrush(gp.GetBounds(), _tabGradient.ColorStart, _tabGradient.ColorEnd, _tabGradient.GradientStyle);
                            gfx.FillPath(pathBrush, gp);

                            gp.Reset();
                            gp.AddLine(rct.Left, rctTabArea.Top - 4, currentTab.Left, rctTabArea.Top - 4);
                            gp.AddLine(currentTab.Left, currentTab.Top, currentTab.Right - 1, currentTab.Top);
                            gp.AddLine(currentTab.Right - 1, rctTabArea.Top - 4, rct.Right - 1, rctTabArea.Top - 4);

                            // Create another figure
                            gp.StartFigure();
                            gp.AddLine(rct.Left, rctTabArea.Top, rct.Right - 1, rctTabArea.Top);

                            gfx.DrawPath(pen1, gp);
                            gfx.DrawImageUnscaled(closeImg, _imgCloseRectangle);
                            closeImg.Dispose();
                        }
                        break;
                    case TabAlignments.Bottom:

                        _imgCloseRectangle = new Rectangle()  // Object Initializer
                        {
                            X = currentTab.Right - (closeImg.Width + 5),
                            Y = currentTab.Top + (currentTab.Height - closeImg.Height) / 2 + 3,
                            Width = closeImg.Width,
                            Height = closeImg.Height,
                        };
                            
                        using (Pen pen1 = new Pen(_tabBorderColor))
                        using (GraphicsPath gp = new GraphicsPath())
                        {
                            // Create an open figure
                            gp.AddLine(rct.Left, rctTabArea.Bottom - 1, rct.Left, rctTabArea.Bottom + 3);
                            gp.AddLine(currentTab.Left, rctTabArea.Bottom + 3, currentTab.Left, currentTab.Bottom);
                            gp.AddLine(currentTab.Right - 1, currentTab.Bottom, currentTab.Right - 1, rctTabArea.Bottom + 3);
                            gp.AddLine(rct.Right, rctTabArea.Bottom + 3, rct.Right, rctTabArea.Bottom - 1);

                            pathBrush = new LinearGradientBrush(gp.GetBounds(), _tabGradient.ColorEnd, _tabGradient.ColorStart, _tabGradient.GradientStyle);
                            gfx.FillPath(pathBrush, gp);

                            gp.Reset();
                            gp.AddLine(rct.Left, rctTabArea.Bottom + 3, currentTab.Left, rctTabArea.Bottom + 3);
                            gp.AddLine(currentTab.Left, currentTab.Bottom, currentTab.Right - 1, currentTab.Bottom);
                            gp.AddLine(currentTab.Right - 1, rctTabArea.Bottom + 3, rct.Right - 1, rctTabArea.Bottom + 3);

                            // Create another figure
                            gp.StartFigure();
                            gp.AddLine(rct.Left, rctTabArea.Bottom - 1, rct.Right - 1, rctTabArea.Bottom - 1);

                            gfx.DrawPath(pen1, gp);
                            gfx.DrawImageUnscaled(closeImg, _imgCloseRectangle);
                            closeImg.Dispose();
                        }
                        break;
                }

                pathBrush.Dispose();
            }
            else
            {
                if (_tabStyles == TabStyle.Sequence)
                {
                    Rectangle rctTabArea = this.DisplayRectangle;

                    rctTabArea.Inflate(_value - 1, _value - 1);

                    Pen pen = new Pen(_tabBorderColor);
                    Color First = _tabGradient.ColorStart;
                    SolidBrush pathBrush = new SolidBrush(First);

                    if (nIndex - this.SelectedIndex == 1)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 2);
                                gp.AddLine(currentTab.Left, currentTab.Top + 2, currentTab.Right - 3, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Top + 4, currentTab.Right - 1, rctTabArea.Top - 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, currentTab.Top + 2, currentTab.Right - 3, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Top + 4, currentTab.Right - 1, rctTabArea.Top - 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                        else
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right - 3, currentTab.Bottom - 2, currentTab.Right - 1, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Right - 1, currentTab.Bottom - 4, currentTab.Right - 1, rctTabArea.Bottom + 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, currentTab.Bottom - 2, currentTab.Right - 3, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Bottom - 4, currentTab.Right - 1, rctTabArea.Bottom + 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                    }
                    else if (nIndex - this.SelectedIndex > 1)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 4);
                                gp.AddLine(currentTab.Left, currentTab.Top + 3, currentTab.Left + 1, currentTab.Top + 2);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right - 3, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Top + 4, currentTab.Right - 1, rctTabArea.Top - 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, currentTab.Top + 3, currentTab.Left + 1, currentTab.Top + 2);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right - 3, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Top + 4, currentTab.Right - 1, rctTabArea.Top - 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                        else
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Left, currentTab.Bottom - 3, currentTab.Left + 1, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right - 3, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Bottom - 4, currentTab.Right - 1, rctTabArea.Bottom + 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, currentTab.Bottom - 3, currentTab.Left + 1, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right - 3, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right - 1, currentTab.Bottom - 4, currentTab.Right - 1, rctTabArea.Bottom + 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                    }
                    else if (nIndex - this.SelectedIndex == -1)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right, currentTab.Top + 2, currentTab.Right, rctTabArea.Top - 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right, currentTab.Top + 2);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                        else
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right, currentTab.Bottom - 2, currentTab.Right, rctTabArea.Bottom + 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right, currentTab.Bottom - 2);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                    }
                    else if (nIndex - this.SelectedIndex < -1)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right - 2, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right, currentTab.Top + 4, currentTab.Right, rctTabArea.Top - 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, rctTabArea.Top - 4, currentTab.Left, currentTab.Top + 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Top + 2, currentTab.Right - 2, currentTab.Top + 2);
                                gp.AddLine(currentTab.Right - 2, currentTab.Top + 2, currentTab.Right, currentTab.Top + 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                        else
                        {
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Create an open figure
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right - 2, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right, currentTab.Bottom - 4, currentTab.Right, rctTabArea.Bottom + 4);

                                gfx.FillPath(pathBrush, gp);

                                gp.Reset();
                                gp.AddLine(currentTab.Left, rctTabArea.Bottom + 4, currentTab.Left, currentTab.Bottom - 4);
                                gp.AddLine(currentTab.Left + 2, currentTab.Bottom - 2, currentTab.Right - 2, currentTab.Bottom - 2);
                                gp.AddLine(currentTab.Right - 2, currentTab.Bottom - 2, currentTab.Right, currentTab.Bottom - 4);

                                gfx.DrawPath(pen, gp);
                            }
                        }
                    }

                    pathBrush.Dispose();
                    pen.Dispose();
                }
                else
                {
                    _tabBorder = new Custom3DBorder(gfx);   //Draw 3D Border Line

                    if (nIndex > this.SelectedIndex)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            _tabBorder.Draw3DLine(currentTab.Right, currentTab.Top + 3, currentTab.Right, currentTab.Bottom - 11, ThreeDStyle.Groove, 1);
                        }
                        else
                        {
                            _tabBorder.Draw3DLine(currentTab.Right, currentTab.Bottom - 5, currentTab.Right, currentTab.Top + 13, ThreeDStyle.Groove, 1);
                        }
                    }
                    else if (nIndex < this.SelectedIndex && nIndex != 0)
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            _tabBorder.Draw3DLine(currentTab.Left, currentTab.Top + 3, currentTab.Left, currentTab.Bottom - 11, ThreeDStyle.Groove, 1);
                        }
                        else
                        {
                            _tabBorder.Draw3DLine(currentTab.Left, currentTab.Bottom - 5, currentTab.Left, currentTab.Top + 13, ThreeDStyle.Groove, 1);
                        }
                    }
                }
            }

            //Draw Tab's Icon Image at the ImageList.
            if (this.ImageList != null && !this.TabPages[nIndex].ImageIndex.Equals(-1))
            {
                if (this.TabPages[nIndex].ImageIndex <= this.ImageList.Images.Count - 1)
                {
                    Image img = this.ImageList.Images[this.TabPages[nIndex].ImageIndex];
                    Rectangle currentImgRct;

                    if (_alignments == TabAlignments.Top)
                    {
                        currentImgRct = new Rectangle() //Object Initializer
                        {
                            X = currentTab.X + 5,
                            Y = (currentTab.Height - img.Height) / 2 + 4,
                            Width = img.Width,
                            Height = img.Height
                        };

                        currentTab = new Rectangle()
                        {
                            X = currentTab.X + 10 + img.Width,
                            Y = currentTab.Y,
                            Width = currentTab.Width - (10 + img.Width),
                            Height = currentTab.Height
                        };
                    }
                    else
                    {
                        currentImgRct = new Rectangle() //Object Initializer
                        {
                            X = currentTab.X + 5,
                            Y = currentTab.Bottom - (img.Height + 6),
                            Width = img.Width,
                            Height = img.Height
                        };

                        currentTab = new Rectangle()
                        {
                            X = currentTab.X + 10 + img.Width,
                            Y = currentTab.Y + 7,
                            Width = currentTab.Width - (10 + img.Width),
                            Height = currentTab.Height - 6
                        };
                    }

                    gfx.DrawImageUnscaled(img, currentImgRct);
                    img.Dispose();
                }
            }
            else
            {
                if (_alignments == TabAlignments.Top)
                {
                    currentTab = new Rectangle()
                    {
                        X = currentTab.X + 10,
                        Y = currentTab.Y,
                        Width = currentTab.Width - 10,
                        Height = currentTab.Height
                    };
                }
                else
                {
                    currentTab = new Rectangle()
                    {
                        X = currentTab.X + 10,
                        Y = currentTab.Y + 7,
                        Width = currentTab.Width - 10,
                        Height = currentTab.Height - 6
                    };
                }
            }

            //Draw Text on the Tabs
            using (SolidBrush brush = new SolidBrush(Color.FromName(this.Enabled ? nIndex == this.SelectedIndex ? _tabGradient.TabPageSelectedTextColor.Name : _tabGradient.TabPageTextColor.Name : SystemColors.InactiveBorder.Name)))
            {
                using (StringFormat format = new StringFormat())
                {
                    if (nIndex == this.SelectedIndex)
                    {
                        format.Alignment = StringAlignment.Near;
                    }
                    else
                    {
                        currentTab.X -= 10;
                        currentTab.Width += 10;
                        format.Alignment = StringAlignment.Center;
                    }

                    format.LineAlignment = StringAlignment.Center;
                    Font currentFont = this.TabPages[nIndex].Font;

                    if (nIndex == this.SelectedIndex)
                        currentFont = new Font(currentFont, _tabGradient.SelectedTabFontStyle);

                    gfx.DrawString(this.TabPages[nIndex].Text, currentFont, brush, currentTab, format);
                }
            }
        }

        protected virtual void DrawBorder(Graphics gfx)
        {
            if (_isDrawHeader)
                ControlPaint.DrawBorder(gfx, this.ClientRectangle, _borderColor, ButtonBorderStyle.Solid);
            else
            {
                if (_headerVisibility && this.TabCount > 0)
                {
                    Rectangle rct = this.ClientRectangle;
                    Rectangle rctTabArea = this.DisplayRectangle;

                    using (Pen pen = new Pen(_borderColor))
                    using (Pen pen2 = new Pen(_tabBorderColor))
                    {
                        if (_alignments == TabAlignments.Top)
                        {
                            gfx.DrawLine(pen2, rct.X, rctTabArea.Y - 5, rct.X, rctTabArea.Y - 1);
                            gfx.DrawLine(pen, rct.X, rctTabArea.Y, rct.X, rct.Bottom - 1);
                            gfx.DrawLine(pen, rct.X, rct.Bottom - 1, rct.Width - 1, rct.Bottom - 1);
                            gfx.DrawLine(pen, rct.Width - 1, rct.Bottom - 1, rct.Width - 1, rctTabArea.Y);
                            gfx.DrawLine(pen2, rct.Width - 1, rctTabArea.Y - 1, rct.Width - 1, rctTabArea.Y - 5);
                        }
                        else
                        {
                            gfx.DrawLine(pen2, rct.X, rctTabArea.Bottom + 4, rct.X, rctTabArea.Bottom);
                            gfx.DrawLine(pen, rct.X, rctTabArea.Bottom - 1, rct.X, rct.Y);
                            gfx.DrawLine(pen, rct.X, rct.Y, rct.Width - 1, rct.Y);
                            gfx.DrawLine(pen, rct.Width - 1, rct.Y, rct.Width - 1, rctTabArea.Bottom - 1);
                            gfx.DrawLine(pen2, rct.Width - 1, rctTabArea.Bottom, rct.Width - 1, rctTabArea.Bottom + 4);
                        }
                    }
                }
                else
                    ControlPaint.DrawBorder(gfx, this.ClientRectangle, _borderColor, ButtonBorderStyle.Solid);
            }
        }

        #endregion

        #region Helper Methods

        private void _tabGradient_GradientChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void _hatcher_HatchChanged(object sender, EventArgs e)
        {
            Invalidate();
            Update();
        }

        private void ScrollRight(object sender, EventArgs e)
        {
            if (this.TabCount <= 1)
                return;
            if (GetTabRect(this.TabCount - 1).Right <= this._myScroller.Left)
            {
                _myScroller._rightScroller.Enabled = false;
                return;
            }
            if (!_myScroller._leftScroller.Enabled)
                _myScroller._leftScroller.Enabled = true;

            int scrollPos = Math.Max(0, (ScrollPosition() + 1) * 0x10000);

            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_HSCROLL, (IntPtr)(scrollPos | 0x4), IntPtr.Zero);
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_HSCROLL, (IntPtr)(scrollPos | 0x8), IntPtr.Zero);

            if (GetTabRect(this.TabCount - 1).Right <= this._myScroller.Left)
                _myScroller._rightScroller.Enabled = false;

            this.Invalidate();
            this.Update();
        }

        private void ScrollLeft(object sender, EventArgs e)
        {
            if (this.TabCount <= 1)
                return;
            if (!_myScroller._rightScroller.Enabled)
                _myScroller._rightScroller.Enabled = true;

            int scrollPos = Math.Max(0, (ScrollPosition() - 1) * 0x10000);

            if (scrollPos == 0)
                _myScroller._leftScroller.Enabled = false;

            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_HSCROLL, (IntPtr)(scrollPos | 0x4), IntPtr.Zero);
            User32.SendMessage(this.Handle, (int)User32.Msgs.WM_HSCROLL, (IntPtr)(scrollPos | 0x8), IntPtr.Zero);

            this.Invalidate();
            this.Update();
        }
        
        private IntPtr FindUpDownControl()
        {
            return User32.FindWindowEx(this.Handle, IntPtr.Zero, "msctls_updown32", "\0");
        }

        private Cursor GetCustomCursor(string filename)
        {
            Cursor custom = null;

            IntPtr cPointer;

            try
            {
                cPointer = User32.LoadCursorFromFile(filename);

                if (!IntPtr.Zero.Equals(cPointer))
                    custom = new Cursor(cPointer);
            }
            catch
            {
                return null;
            }

            return custom;
        }

        private int ScrollPosition()
        {
            int value = -1;
            Rectangle tabRect;

            do
            {
                tabRect = GetTabRect(value + 1);
                value++;
            } while (tabRect.Left < 0 && value < this.TabCount);

            return value;
        }

        private TabPageEx OverTab()
        {
            TabPageEx over = null;

            Point pt = this.PointToClient(Cursor.Position);

            User32.TCHITTESTINFO mouseInfo = new User32.TCHITTESTINFO(pt, User32.TabControlHitTest.TCHT_ONITEM);
            int currentTabIndex = User32.SendMessage(this.Handle, User32._TCM_HITTEST, IntPtr.Zero, ref mouseInfo);

            if (currentTabIndex > -1)
            {
                Rectangle currentTabRct = this.GetTabRect(currentTabIndex);

                if (currentTabIndex == 0)
                    currentTabRct.X += 4;

                if (_alignments == TabAlignments.Top)
                {
                    if (_tabStyles == TabStyle.Sequence)
                    {
                        currentTabRct.Y += 5;

                        if (currentTabIndex - this.SelectedIndex < -1)
                        {
                            if ((currentTabRct.Right - 5 == pt.X) && (currentTabRct.Top == pt.Y) && (currentTabIndex == 0))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Left + 1 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Top + 1 == pt.Y))
                                return null;
                        }
                        if (currentTabIndex - this.SelectedIndex == -1)
                        {
                            if ((currentTabRct.Left + 1 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Top + 1 == pt.Y))
                                return null;
                        }
                        else if (currentTabIndex - this.SelectedIndex == 1)
                        {
                            if ((currentTabRct.Right - 2 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Top + 1 == pt.Y))
                                return null;
                        }
                        else if (currentTabIndex - this.SelectedIndex > 1)
                        {
                            if ((currentTabRct.Left == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 2 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Top == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Top + 1 == pt.Y))
                                return null;
                        }
                    }
                    else
                    {
                        currentTabRct.Y += 3;
                    }
                }
                else
                {
                    if (_tabStyles == TabStyle.Sequence)
                    {
                        currentTabRct.Y -= 5;

                        if (currentTabIndex - this.SelectedIndex < -1)
                        {
                            if ((currentTabRct.Right - 5 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y) && (currentTabIndex == 0))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Left + 1 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Bottom - 2 == pt.Y))
                                return null;
                        }
                        if (currentTabIndex - this.SelectedIndex == -1)
                        {
                            if ((currentTabRct.Left + 1 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Left == pt.X) && (currentTabRct.Bottom - 2 == pt.Y))
                                return null;
                        }
                        else if (currentTabIndex - this.SelectedIndex == 1)
                        {
                            if ((currentTabRct.Right - 2 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Bottom - 2 == pt.Y))
                                return null;
                        }
                        else if (currentTabIndex - this.SelectedIndex > 1)
                        {
                            if ((currentTabRct.Left == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 2 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Bottom - 1 == pt.Y))
                                return null;
                            else if ((currentTabRct.Right - 1 == pt.X) && (currentTabRct.Bottom - 2 == pt.Y))
                                return null;
                        }
                    }
                    else
                    {
                        currentTabRct.Y -= 3;
                    }
                }

                if (currentTabRct.Contains(pt))
                    over = this.TabPages[currentTabIndex] as TabPageEx;
            }

            return over;
        }
        
        private Bitmap GetCloseTabImage()
        {
            Bitmap bmp;
            switch (_tabCloseBtn)
            {
                case TabCloseImage.Hover:
                    bmp = KRBTabResources.TabHover;
                    break;
                case TabCloseImage.Pressed:
                    bmp = KRBTabResources.TabPressed;
                    break;
                default:
                    bmp = KRBTabResources.TabNormal;
                    break;
            }

            return bmp;
        }

        #endregion

        #region General Methods

        public void HideTab(TabPageEx TabPage)
        {
            if (_tabPageExPool == null)
                _tabPageExPool = new TabPageExPool();

            if (this.TabPages.Contains(TabPage))
            {
                _tabPageExPool.Add(TabPage);
                this.TabPages.Remove(TabPage);
            }
        }

        public void ShowTab(TabPageEx TabPage)
        {
            if (_tabPageExPool != null)
            {
                if (_tabPageExPool.Contains(TabPage))
                {
                    this.TabPages.Add(TabPage);
                    _tabPageExPool.Remove(TabPage);
                }
            }
        }

        #endregion

        [Editor(typeof(GradientTabEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(GradientTabConverter))]
        public class GradientTab : IDisposable
        {
            #region Event

            public event EventHandler GradientChanged;

            #endregion

            #region Instance Members

            private Color _colorStart = Color.White;
            private Color _colorEnd = Color.Gainsboro;
            private Color _tabPageTextColor = Color.Black;
            private Color _tabPageSelectedTextColor = Color.Black;
            private LinearGradientMode _gradientStyle = LinearGradientMode.Horizontal;
            private FontStyle _selectedTabFontStyle = FontStyle.Regular;

            #endregion

            #region Constructor

            public GradientTab() { }

            public GradientTab(Color first, Color second, LinearGradientMode lnrMode, Color textColor, Color selectedTextColor, FontStyle fontStyle)
            {
                _colorStart = first;
                _colorEnd = second;
                _gradientStyle = lnrMode;
                _tabPageTextColor = textColor;
                _tabPageSelectedTextColor = selectedTextColor;
                _selectedTabFontStyle = fontStyle;
            }

            #endregion

            #region Property

            /// <summary>
            /// Başlangıç rengi.
            /// </summary>
            [Description("Başlangıç rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "White")]
            [Browsable(true)]
            public Color ColorStart
            {
                get { return _colorStart; }
                set
                {
                    if (!value.Equals(_colorStart))
                    {
                        _colorStart = value;
                         
                    }
                }
            }

            /// <summary>
            /// Bitiş rengi.
            /// </summary>
            [Description("Bitiş rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "LightBlue")]
            [Browsable(true)]
            public Color ColorEnd
            {
                get { return _colorEnd; }
                set
                {
                    if (!value.Equals(_colorEnd))
                    {
                        _colorEnd = value;
                        OnGradientChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// Gradient stili.
            /// </summary>
            [Description("Gradient stili")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(LinearGradientMode), "Horizontal")]
            [Browsable(true)]
            public LinearGradientMode GradientStyle
            {
                get { return _gradientStyle; }
                set
                {
                    if (!value.Equals(_gradientStyle))
                    {
                        _gradientStyle = value;
                        OnGradientChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// Seçili olmayan tabımızın yazı rengi.
            /// </summary>
            [Description("Seçili olmayan tabımızın yazı rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "Black")]
            [Browsable(true)]
            public Color TabPageTextColor
            {
                get { return _tabPageTextColor; }
                set
                {
                    if (!value.Equals(_tabPageTextColor))
                    {
                        _tabPageTextColor = value;
                        OnGradientChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// O an için seçili olan tabımızın yazı rengi.
            /// </summary>
            [Description("O an için seçili olan tabımızın yazı rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "Black")]
            [Browsable(true)]
            public Color TabPageSelectedTextColor
            {
                get { return _tabPageSelectedTextColor; }
                set
                {
                    if (!value.Equals(_tabPageSelectedTextColor))
                    {
                        _tabPageSelectedTextColor = value;
                        OnGradientChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// O an için seçili olan tabımızın yazı sitili.
            /// </summary>
            [Description("O an için seçili olan tabımızın yazı sitili")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(FontStyle), "Regular")]
            [Browsable(true)]
            public FontStyle SelectedTabFontStyle
            {
                get { return _selectedTabFontStyle; }
                set
                {
                    if (!value.Equals(_selectedTabFontStyle))
                    {
                        _selectedTabFontStyle = value;
                        OnGradientChanged(EventArgs.Empty);
                    }
                }
            }

            #endregion

            #region Helper Methods

            private void OnGradientChanged(EventArgs e)
            {
                if (GradientChanged != null)
                    GradientChanged(this, e);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        class GradientTabConverter : ExpandableObjectConverter
        {
            #region Override Methods

            //All the CanConvertTo() method needs to is check that the target type is a string.
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;
                else
                    return base.CanConvertTo(context, destinationType);
            }

            //ConvertTo() simply checks that it can indeed convert to the desired type.
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return ToString(value);
                else
                    return base.ConvertTo(context, culture, value, destinationType);
            }

            /* The exact same process occurs in reverse when converting a GradientFill object to a string.
            First the Properties window calls CanConvertFrom(). If it returns true, the next step is to call
            the ConvertFrom() method. */
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                else
                    return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                    return FromString(value);
                else
                    return base.ConvertFrom(context, culture, value);
            }

            #endregion

            #region Helper Methods

            private string ToString(object value)
            {
                GradientTab gradient = value as GradientTab;    // Gelen object tipimizi GradientTab tipine dönüştürüyoruz ve ayıklama işlemine başlıyoruz.
                ColorConverter converter = new ColorConverter();
                return String.Format("{0}, {1}, {2}, {3}, {4}, {5}",
                    converter.ConvertToString(gradient.ColorStart), converter.ConvertToString(gradient.ColorEnd), gradient.GradientStyle, converter.ConvertToString(gradient.TabPageTextColor), converter.ConvertToString(gradient.TabPageSelectedTextColor), gradient.SelectedTabFontStyle);
            }

            private GradientTab FromString(object value)
            {
                string[] result = ((string)value).Split(',');
                if (result.Length != 6)
                    throw new ArgumentException("Could not convert to value");

                try
                {
                    GradientTab gradient = new GradientTab();

                    // Retrieve the colors
                    ColorConverter converter = new ColorConverter();
                    gradient.ColorStart = (Color)converter.ConvertFromString(result[0]);
                    gradient.ColorEnd = (Color)converter.ConvertFromString(result[1]);
                    gradient.GradientStyle = (LinearGradientMode)Enum.Parse(typeof(LinearGradientMode), result[2], true);
                    gradient.TabPageTextColor = (Color)converter.ConvertFromString(result[3]);
                    gradient.TabPageSelectedTextColor = (Color)converter.ConvertFromString(result[4]);
                    gradient.SelectedTabFontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), result[5], true);

                    return gradient;
                }
                catch (Exception)
                {
                    throw new ArgumentException("Could not convert to value");
                }
            }

            #endregion
        }

        class GradientTabEditor : UITypeEditor
        {
            #region Override Methods

            public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override void PaintValue(PaintValueEventArgs e)
            {
                GradientTab gradient = e.Value as GradientTab;
                using (LinearGradientBrush brush = new LinearGradientBrush(e.Bounds, gradient.ColorStart, gradient.ColorEnd, gradient.GradientStyle))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }

            #endregion
        }

        [Editor(typeof(HatcherEditor), typeof(UITypeEditor))]
        [TypeConverter(typeof(HatcherConverter))]
        public class Hatcher : IDisposable
        {
            #region Event

            public event EventHandler HatchChanged;

            #endregion

            #region Instance Members

            private Color _foreColor = Color.White;
            private Color _backColor = Color.Gainsboro;
            private System.Drawing.Drawing2D.HatchStyle _hatchType = System.Drawing.Drawing2D.HatchStyle.DashedVertical;
            private string _hatchStyle = "DashedVertical";

            #endregion

            #region Constructor

            public Hatcher() { }

            public Hatcher(Color foreColor, Color backColor, HatchStyle type)
            {
                _foreColor = foreColor;
                _backColor = backColor;
                _hatchType = type;
            }

            #endregion

            #region Property
            
            /// <summary>
            /// Arkaplan ön stil rengi.
            /// </summary>
            [Description("Arkaplan ön stil rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "White")]
            [Browsable(true)]
            public Color ForeColor
            {
                get { return _foreColor; }
                set 
                {
                    if (!value.Equals(_foreColor))
                    {
                        _foreColor = value;
                        OnHatchChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// Arkaplan stil rengi.
            /// </summary>
            [Description("Arkaplan stil rengi")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue(typeof(Color), "Gainsboro")]
            [Browsable(true)]
            public Color BackColor
            {
                get { return _backColor; }
                set 
                {
                    if (!value.Equals(_backColor))
                    {
                        _backColor = value;
                        OnHatchChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// Hatch style.
            /// </summary>
            [Browsable(false)]
            public System.Drawing.Drawing2D.HatchStyle HatchType
            {
                get { return _hatchType; }
                set 
                {
                    if (!value.Equals(_hatchType))
                    {
                        _hatchType = value;
                        OnHatchChanged(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            /// Hatch style.
            /// </summary>
            [Description("Hatch style")]
            [RefreshProperties(RefreshProperties.Repaint)]
            [NotifyParentProperty(true)]
            [DefaultValue("DashedVertical")]
            [Browsable(true)]
            [TypeConverter(typeof(HatchStyleConverter))]
            public string HatchStyle
            {
                get { return _hatchStyle; }
                set
                {
                    if (!value.Equals(_hatchStyle))
                    {
                        _hatchStyle = value;
                        _hatchType = (HatchStyle)Enum.Parse(typeof(HatchStyle), _hatchStyle, true);
                        OnHatchChanged(EventArgs.Empty);
                    }
                }
            }
            
            #endregion

            #region Helper Methods

            private void OnHatchChanged(EventArgs e)
            {
                if (HatchChanged != null)
                    HatchChanged(this, e);
            }

            #endregion
            
            #region IDisposable Members

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        class HatcherConverter : ExpandableObjectConverter
        {
            #region Override Methods

            //All the CanConvertTo() method needs to is check that the target type is a string.
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return true;
                else
                    return base.CanConvertTo(context, destinationType);
            }

            //ConvertTo() simply checks that it can indeed convert to the desired type.
            public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                    return ToString(value);
                else
                    return base.ConvertTo(context, culture, value, destinationType);
            }

            /* The exact same process occurs in reverse when converting a GradientFill object to a string.
            First the Properties window calls CanConvertFrom(). If it returns true, the next step is to call
            the ConvertFrom() method. */
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                if (sourceType == typeof(string))
                    return true;
                else
                    return base.CanConvertFrom(context, sourceType);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                if (value is string)
                    return FromString(value);
                else
                    return base.ConvertFrom(context, culture, value);
            }
            
            #endregion

            #region Helper Methods
            
            private string ToString(object value)
            {
                Hatcher hatch = value as Hatcher;    // Gelen object tipimizi Hatcher tipine dönüştürüyoruz ve ayıklama işlemine başlıyoruz.
                
                ColorConverter converter = new ColorConverter();

                return String.Format("{0}, {1}, {2}",
                    converter.ConvertToString(hatch.ForeColor), converter.ConvertToString(hatch.BackColor), hatch.HatchStyle);
            }

            private Hatcher FromString(object value)
            {
                string[] result = ((string)value).Split(',');
                if (result.Length != 3)
                    throw new ArgumentException("Could not convert to value");

                try
                {
                    Hatcher hatch = new Hatcher();

                    // Retrieve the colors
                    ColorConverter converter = new ColorConverter();

                    hatch.ForeColor = (Color)converter.ConvertFromString(result[0]);
                    hatch.BackColor = (Color)converter.ConvertFromString(result[1]);
                    hatch.HatchStyle = result[2];

                    return hatch;
                }
                catch (Exception)
                {
                    throw new ArgumentException("Could not convert to value");
                }
            }

            #endregion
        }

        class HatcherEditor : UITypeEditor
        {
            #region Override Methods

            public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            {
                return true;
            }

            public override void PaintValue(PaintValueEventArgs e)
            {
                Hatcher hatch = e.Value as Hatcher;
                using (HatchBrush brush = new HatchBrush(hatch.HatchType, hatch.ForeColor, hatch.BackColor))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds);
                }
            }

            #endregion
        }

        class HatchStyleConverter : StringConverter // Bug! enumeration of HatchStyle,multiple values at the result.
        {
            #region Instance Members
            
            // Cache the collection of values so you don't need to re-create it each time.
            private static StandardValuesCollection _collection;

            #endregion

            #region Override Methods

            public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
            {
                return true;    // PropertyGrid shows a Combobox to us.
            }

            public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
            {
                return true;    // If you select the true,PropertyGrid  does not allow change the value.It means Select Only.
            }

            // Provide the list of HatchStyle standart values.
            public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
            {
                if (_collection == null)
                {
                    List<string> hatchStyle = new List<string>();

                    foreach (string current in Enum.GetNames(typeof(HatchStyle)))
                    {
                        hatchStyle.Add(current);
                    }
                    // Now wrap the real values in the StandarValuesCollection object.
                    _collection = new TypeConverter.StandardValuesCollection(hatchStyle);
                }

                return _collection;
            }
            
            #endregion
        }

        class KRBTabControlDesigner : System.Windows.Forms.Design.ParentControlDesigner
        {
            #region Instance Members

            private DesignerVerbCollection _verbs;
            private DesignerActionListCollection _actionLists;

            private IDesignerHost _designerHost;
            private ISelectionService _selectionService;
            private IComponentChangeService _changeService;

            #endregion

            #region Constructor

            public KRBTabControlDesigner()
                : base() { }

            #endregion

            #region Property
            
            public override DesignerActionListCollection ActionLists
            {
                get
                {
                    if (_actionLists == null)
                    {
                        _actionLists = new DesignerActionListCollection();
                        _actionLists.Add(new KRBTabControlActionList((KRBTabControl)Control));
                    }

                    return _actionLists;
                }
            }
           
            public override DesignerVerbCollection Verbs
            {
                get
                {
                    if (_verbs == null)
                    {
                        _verbs = new DesignerVerbCollection();

                        DesignerVerb[] addVerbs = new DesignerVerb[]
                        {
                            new DesignerVerb("Add Tab", new EventHandler(OnAddTab)),
                            new DesignerVerb("Remove Tab", new EventHandler(OnRemoveTab))
                        };

                        _verbs.AddRange(addVerbs);
                    }

                    return _verbs;
                }
            }

            #endregion

            #region Override Methods

            public override void Initialize(IComponent component)
            {
                base.Initialize(component);

                _designerHost = (IDesignerHost)GetService(typeof(IDesignerHost));
                _selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                // Update your designer verb whenever ComponentChanged event occurs.
                if (_changeService != null)
                    _changeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
            }

            protected override void PostFilterProperties(System.Collections.IDictionary properties)
            {
                properties.Remove("RightToLeft");
                properties.Remove("RightToLeftLayout");
                base.PostFilterProperties(properties);
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == (int)User32.Msgs.WM_NCHITTEST)
                {
                    if (m.Result.ToInt32() == User32._htTransparent)
                        m.Result = (IntPtr)User32._htClient;
                }
            }

            protected override bool GetHitTest(Point point)
            {
                if (_selectionService.PrimarySelection == this.Control)
                {
                    Point p = this.Control.PointToClient(point);

                    User32.TCHITTESTINFO hti = new User32.TCHITTESTINFO(p, User32.TabControlHitTest.TCHT_ONITEM);

                    Message m = new Message();
                    m.HWnd = this.Control.Handle;
                    m.Msg = User32._TCM_HITTEST;

                    IntPtr lParam = System.Runtime.InteropServices.Marshal.AllocHGlobal(System.Runtime.InteropServices.Marshal.SizeOf(hti));
                    System.Runtime.InteropServices.Marshal.StructureToPtr(hti, lParam, false);
                    m.LParam = lParam;

                    base.WndProc(ref m);
                    System.Runtime.InteropServices.Marshal.FreeHGlobal(lParam);

                    if (m.Result.ToInt32() != -1)
                        return hti.flags != User32.TabControlHitTest.TCHT_NOWHERE;
                }

                return false;
            }

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);

                if (_changeService != null)
                    _changeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
            }

            #endregion

            #region Helper Methods

            private void OnAddTab(Object sender, EventArgs e)
            {
                KRBTabControl parentControl = (KRBTabControl)Control;
                TabPageCollection oldTabs = parentControl.TabPages;

                RaiseComponentChanging(TypeDescriptor.GetProperties(parentControl)["TabPages"]);
                TabPageEx newTab = (TabPageEx)_designerHost.CreateComponent(typeof(TabPageEx));
                newTab.Text = newTab.Name;
                parentControl.TabPages.Add(newTab);

                RaiseComponentChanged(TypeDescriptor.GetProperties(parentControl)["TabPages"], oldTabs, parentControl.TabPages);
                parentControl.SelectedTab = newTab;
            }

            private void OnRemoveTab(Object sender, EventArgs e)
            {
                KRBTabControl parentControl = (KRBTabControl)Control;

                if (parentControl.SelectedIndex < 0)
                    return;

                TabPageCollection oldTabs = parentControl.TabPages;

                RaiseComponentChanging(TypeDescriptor.GetProperties(parentControl)["TabPages"]);
                _designerHost.DestroyComponent(parentControl.SelectedTab);

                RaiseComponentChanged(TypeDescriptor.GetProperties(parentControl)["TabPages"], oldTabs, parentControl.TabPages);
                _selectionService.SetSelectedComponents(new IComponent[] { parentControl }, SelectionTypes.Auto);
            }

            private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
            {
                KRBTabControl parentControl = (KRBTabControl)Control;

                foreach (DesignerVerb verb in Verbs)
                {
                    if (verb.Text == "Remove Tab")
                    {
                        switch (parentControl.TabPages.Count)
                        {
                            case 0:
                                verb.Enabled = false;
                                break;
                            default:
                                verb.Enabled = true;
                                break;
                        }

                        break;
                    }
                }
            }

            #endregion
        }

        class KRBTabControlActionList : DesignerActionList
        {
            #region Instance Members

            private KRBTabControl _linkedControl;
            private IDesignerHost _host;
            private IComponentChangeService _changeService;
            private DesignerActionUIService _designerService;
            private bool _isSupportedAlphaColor = false;

            #endregion

            #region Constructor

            public KRBTabControlActionList(KRBTabControl control)
                : base(control)
            {
                _linkedControl = control;
                _host = (IDesignerHost)GetService(typeof(IDesignerHost));
                _changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
                _designerService = (DesignerActionUIService)GetService(typeof(DesignerActionUIService));

                this.AutoShow = true;   // When this control added design area, Smart Tags open automatically.
            }

            #endregion

            #region Property - DesignerActionPropertyItem

            public TabStyle TabStyles
            {
                get { return _linkedControl.TabStyles; }
                set
                {
                    GetPropertyByName("TabStyles").SetValue(_linkedControl, value);
                }
            }

            public UpDown32Style UpDownStyle
            {
                get { return _linkedControl.UpDownStyle; }
                set
                {
                    GetPropertyByName("UpDownStyle").SetValue(_linkedControl, value);
                }
            }

            public TabAlignments Alignments
            {
                get { return _linkedControl.Alignments; }
                set
                {
                    GetPropertyByName("Alignments").SetValue(_linkedControl, value);
                }
            }

            public Color BorderColor
            {
                get { return _linkedControl.BorderColor; }
                set
                {
                    GetPropertyByName("BorderColor").SetValue(_linkedControl, value);
                }
            }

            public TabHeaderStyle HeaderStyle
            {
                get { return _linkedControl.HeaderStyle; }
                set
                {
                    GetPropertyByName("HeaderStyle").SetValue(_linkedControl, value);
                }
            }

            public Color ForeColor
            {
                get { return _linkedControl.BackgroundHatcher.ForeColor; }
                set
                {
                    if (!value.Equals(_linkedControl.BackgroundHatcher.ForeColor))
                    {
                        Hatcher oldHatcher = _linkedControl.BackgroundHatcher;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("Fore Color");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("BackgroundHatcher"));

                            GetChildPropertyTabHeaderByName("ForeColor").SetValue(_linkedControl.BackgroundHatcher, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("BackgroundHatcher"), oldHatcher, _linkedControl.BackgroundHatcher);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            public Color BackColor
            {
                get { return _linkedControl.BackgroundHatcher.BackColor; }
                set
                {
                    if (!value.Equals(_linkedControl.BackgroundHatcher.BackColor))
                    {
                        Hatcher oldHatcher = _linkedControl.BackgroundHatcher;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("Back Color");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("BackgroundHatcher"));

                            GetChildPropertyTabHeaderByName("BackColor").SetValue(_linkedControl.BackgroundHatcher, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("BackgroundHatcher"), oldHatcher, _linkedControl.BackgroundHatcher);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            [TypeConverter(typeof(HatchStyleConverter))]
            public string HatchStyle
            {
                get { return _linkedControl.BackgroundHatcher.HatchStyle; }
                set
                {
                    if (!value.Equals(_linkedControl.BackgroundHatcher.HatchStyle))
                    {
                        Hatcher oldHatcher = _linkedControl.BackgroundHatcher;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("Hatch Style");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("BackgroundHatcher"));

                            GetChildPropertyTabHeaderByName("HatchStyle").SetValue(_linkedControl.BackgroundHatcher, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("BackgroundHatcher"), oldHatcher, _linkedControl.BackgroundHatcher);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            public Color BackgroundColor
            {
                get { return _linkedControl.BackgroundColor; }
                set
                {
                    GetPropertyByName("BackgroundColor").SetValue(_linkedControl, value);
                }
            }

            public Image BackgroundImage
            {
                get { return _linkedControl.BackgroundImage; }
                set
                {
                    GetPropertyByName("BackgroundImage").SetValue(_linkedControl, value);
                }
            }

            public Color FirstColor
            {
                get { return _linkedControl.TabGradient.ColorStart; }
                set
                {
                    if (!value.Equals(_linkedControl.TabGradient.ColorStart))
                    {
                        GradientTab oldGradient = _linkedControl.TabGradient;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("First Color");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("TabGradient"));

                            GetChildPropertyTabItemByName("ColorStart").SetValue(_linkedControl.TabGradient, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("TabGradient"), oldGradient, _linkedControl.TabGradient);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            public Color SecondColor
            {
                get { return _linkedControl.TabGradient.ColorEnd; }
                set
                {
                    if (!value.Equals(_linkedControl.TabGradient.ColorEnd))
                    {
                        GradientTab oldGradient = _linkedControl.TabGradient;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("Second Color");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("TabGradient"));

                            GetChildPropertyTabItemByName("ColorEnd").SetValue(_linkedControl.TabGradient, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("TabGradient"), oldGradient, _linkedControl.TabGradient);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            public LinearGradientMode GradientMode
            {
                get { return _linkedControl.TabGradient.GradientStyle; }
                set
                {
                    if (!value.Equals(_linkedControl.TabGradient.GradientStyle))
                    {
                        GradientTab oldGradient = _linkedControl.TabGradient;

                        // Start the transaction.
                        DesignerTransaction transaction = _host.CreateTransaction("Gradient Mode");

                        try
                        {
                            _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("TabGradient"));

                            GetChildPropertyTabItemByName("GradientStyle").SetValue(_linkedControl.TabGradient, value);

                            _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("TabGradient"), oldGradient, _linkedControl.TabGradient);

                            // Commit the transaction.
                            transaction.Commit();
                        }
                        catch
                        {
                            transaction.Cancel();
                        }
                        finally
                        {
                            _designerService.Refresh(_linkedControl);
                        }
                    }
                }
            }

            public Color TabBorderColor
            {
                get { return _linkedControl.TabBorderColor; }
                set
                {
                    GetPropertyByName("TabBorderColor").SetValue(_linkedControl, value);
                }
            }

            public bool IsSupportedAlphaColor
            {
                get { return _isSupportedAlphaColor; }
                set
                {
                    if (!value.Equals(_isSupportedAlphaColor))
                        _isSupportedAlphaColor = value;
                }
            }

            #endregion

            #region Override Methods

            public override DesignerActionItemCollection GetSortedActionItems()
            {
                DesignerActionItemCollection items = new DesignerActionItemCollection();

                // Creating the action list headers.
                items.Add(new DesignerActionHeaderItem("Tab Control Appearance"));
                items.Add(new DesignerActionHeaderItem("Tab Header Appearance"));
                items.Add(new DesignerActionHeaderItem("Tab Item Appearance"));
                items.Add(new DesignerActionHeaderItem("Commands"));
                items.Add(new DesignerActionHeaderItem("Information"));

                items.Add(new DesignerActionPropertyItem("TabStyles", "Tab Styles", "Tab Control Appearance",
                    "Tab Kontrolümüzün görünümünü ayarlar"));

                items.Add(new DesignerActionPropertyItem("Alignments", "Tab Alignments", "Tab Control Appearance",
                    "Tab Kontrolü'nü belirtilen stil de hizalar"));

                items.Add(new DesignerActionPropertyItem("HeaderStyle", "Tab Header Styles", "Tab Control Appearance",
                    "Tab başlığının hangi stil'de çizileceğini ayarlar"));

                items.Add(new DesignerActionPropertyItem("UpDownStyle", "UpDown32 Styles", "Tab Control Appearance",
                    "UpDown kontrolünün görünümünü ayarlar"));
                
                items.Add(new DesignerActionPropertyItem("BorderColor", "Tab Control Border Color", "Tab Control Appearance",
                    "Tab Kontrolünün sınır çizgi rengini ayarlar"));

                items.Add(new DesignerActionPropertyItem("ForeColor", "Fore Color", "Tab Header Appearance",
                                "Arkaplan ön stil rengi"));

                items.Add(new DesignerActionPropertyItem("BackColor", "Back Color", "Tab Header Appearance",
                                "Arkaplan stil rengi"));

                items.Add(new DesignerActionPropertyItem("HatchStyle", "Hatch Style", "Tab Header Appearance",
                                "Hatch style"));

                items.Add(new DesignerActionPropertyItem("BackgroundColor", "Background Color", "Tab Header Appearance",
                            "Tab kontrolünün arkaplan rengini ayarlar"));

                items.Add(new DesignerActionPropertyItem("BackgroundImage", "Background Image", "Tab Header Appearance",
                            "Tab kontrolünün arkaplan resmini ayarlar"));

                items.Add(new DesignerActionMethodItem(this,
                    "HeaderDraw", "Header Draw " + (_linkedControl.IsDrawHeader ? "ON" : "OFF"), "Tab Header Appearance",
                    "Tab kontrolünün başlık arkaplanının çizilip çizilmemesini ayarlayabilirsiniz", false));
                
                items.Add(new DesignerActionMethodItem(this,
                    "HeaderVisibility", "Header Visibility " + (_linkedControl.HeaderVisibility ? "ON" : "OFF"), "Tab Header Appearance",
                    "Tab kontrolünün başlığının görünüp görünmemesini ayarlayabilirsiniz", false));
                
                items.Add(new DesignerActionPropertyItem("FirstColor", "First Color", "Tab Item Appearance",
                    "Başlangıç rengi"));

                items.Add(new DesignerActionPropertyItem("SecondColor", "Second Color", "Tab Item Appearance",
                    "Bitiş rengi"));

                items.Add(new DesignerActionPropertyItem("GradientMode", "Gradient Mode", "Tab Item Appearance",
                    "Gradient stili"));

                items.Add(new DesignerActionPropertyItem("TabBorderColor", "TabPage Border Color", "Tab Item Appearance",
                    "TabPage sınır çizgi rengini ayarlar"));
                
                items.Add(new DesignerActionPropertyItem("IsSupportedAlphaColor", "Support Alpha Color", "Tab Item Appearance",
                    "Alfa renk desteği"));

                items.Add(new DesignerActionMethodItem(this,
                    "RandomizeColors", "Randomize Colors", "Tab Item Appearance",
                    "Rastgele renk belirle", true));

                items.Add(new DesignerActionMethodItem(this,
                    "AddTab", "Add Tab", "Commands",
                    "Yeni bir tab page ekler", false));

                DesignerActionMethodItem methodRemove = new DesignerActionMethodItem(this, "RemoveTab", "Remove Tab", "Commands",
                    "Seçili olan tab page'i kaldırır", false);

                if (_linkedControl.TabCount > 0)
                {
                    if (!items.Contains(methodRemove))
                        items.Add(methodRemove);
                }
                else
                {
                    if (items.Contains(methodRemove))
                        items.Remove(methodRemove);
                }

                items.Add(new DesignerActionTextItem("X: " + _linkedControl.Location.X + ", " + "Y: " + _linkedControl.Location.Y, "Information"));
                items.Add(new DesignerActionTextItem("Width: " + _linkedControl.Size.Width + ", " + "Height: " + _linkedControl.Size.Height, "Information"));

                return items;
            }

            #endregion

            #region Helper Methods

            private PropertyDescriptor GetPropertyByName(String propName)
            {
                if (propName != null)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(_linkedControl)[propName];

                    if (prop != null)
                        return prop;
                    else
                        throw new ArgumentException("Property name not found.", propName);
                }
                else
                    throw new ArgumentNullException("Property name must not blank.");
            }

            private PropertyDescriptor GetChildPropertyTabItemByName(String childPropName)
            {
                if (childPropName != null)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(_linkedControl.TabGradient)[childPropName];

                    if (prop != null)
                        return prop;
                    else
                        throw new ArgumentException("Child property name not found.", childPropName);
                }
                else
                    throw new ArgumentNullException("Child property name must not blank.");
            }

            private PropertyDescriptor GetChildPropertyTabHeaderByName(String childPropName)
            {
                if (childPropName != null)
                {
                    PropertyDescriptor prop = TypeDescriptor.GetProperties(_linkedControl.BackgroundHatcher)[childPropName];

                    if (prop != null)
                        return prop;
                    else
                        throw new ArgumentException("Child property name not found.", childPropName);
                }
                else
                    throw new ArgumentNullException("Child property name must not blank.");
            }
            
            #endregion

            #region General Methods - DesignerActionMethodItem

            public void RandomizeColors()
            {
                Random rand = new Random();

                if (!IsSupportedAlphaColor)
                {
                    FirstColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
                    SecondColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255));
                }
                else
                {
                    FirstColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255), rand.Next(255));
                    SecondColor = Color.FromArgb(rand.Next(255), rand.Next(255), rand.Next(255), rand.Next(255));
                }
            }
            
            public void HeaderDraw()
            {
                bool oldValue = _linkedControl.IsDrawHeader;

                // Start the transaction.
                DesignerTransaction transaction = _host.CreateTransaction("Header Draw");

                try
                {
                    _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("IsDrawHeader"));
                    _linkedControl.IsDrawHeader = !_linkedControl.IsDrawHeader;
                    _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("IsDrawHeader"), oldValue, _linkedControl.IsDrawHeader);

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
                finally
                {
                    _designerService.Refresh(_linkedControl);
                }
            }

            public void HeaderVisibility()
            {
                bool oldValue = _linkedControl.HeaderVisibility;

                // Start the transaction.
                DesignerTransaction transaction = _host.CreateTransaction("Header Visibility");

                try
                {
                    _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("HeaderVisibility"));
                    _linkedControl.HeaderVisibility = !_linkedControl.HeaderVisibility;
                    _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("HeaderVisibility"), oldValue, _linkedControl.HeaderVisibility);

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
                finally
                {
                    _designerService.Refresh(_linkedControl);
                }
            }

            public void AddTab()
            {
                TabPageCollection oldTabs = _linkedControl.TabPages;

                // Start the transaction.
                DesignerTransaction transaction = _host.CreateTransaction("Add Tab");

                try
                {
                    _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("TabPages"));
                    TabPageEx newTab = (TabPageEx)_host.CreateComponent(typeof(TabPageEx));
                    newTab.Text = newTab.Name;
                    _linkedControl.TabPages.Add(newTab);
                    _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("TabPages"), oldTabs, _linkedControl.TabPages);
                    _linkedControl.SelectedTab = newTab;

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
                finally
                {
                    _designerService.Refresh(_linkedControl);
                }
            }

            public void RemoveTab()
            {
                if (_linkedControl.SelectedIndex < 0)
                    return;

                TabPageCollection oldTabs = _linkedControl.TabPages;

                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));

                // Start the transaction.
                DesignerTransaction transaction = _host.CreateTransaction("Remove Tab");

                try
                {
                    _changeService.OnComponentChanging(_linkedControl, GetPropertyByName("TabPages"));
                    _host.DestroyComponent(_linkedControl.SelectedTab);

                    _changeService.OnComponentChanged(_linkedControl, GetPropertyByName("TabPages"), oldTabs, _linkedControl.TabPages);
                    selectionService.SetSelectedComponents(new IComponent[] { _linkedControl }, SelectionTypes.Auto);

                    // Commit the transaction.
                    transaction.Commit();
                }
                catch
                {
                    transaction.Cancel();
                }
                finally
                {
                    _designerService.Refresh(_linkedControl);
                }
            }

            #endregion
        }

        class TabpageExCollectionEditor : CollectionEditor
        {
            #region Constructor

            public TabpageExCollectionEditor(Type type)
                : base(type) { }

            #endregion

            #region Override Methods

            protected override Type CreateCollectionItemType()
            {
                return typeof(TabPageEx);
            }

            protected override void DestroyInstance(object instance)
            {
                base.DestroyInstance(instance);

                ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
                selectionService.SetSelectedComponents(new IComponent[] { (KRBTabControl)Context.Instance }, SelectionTypes.Auto);
            }

            #endregion
        }

        class TabPageExPool : System.Collections.CollectionBase, IDisposable
        {
            #region Indexer

            public TabPageEx this[int index]
            {
                get
                {
                    return (TabPageEx)this.List[index];
                }
                set
                {
                    this.List[index] = value;
                }
            }

            #endregion

            #region General Methods

            public void Add(TabPageEx TabPage)
            {
                if (this.List.Contains(TabPage))
                    throw new ArgumentException("Bu tab zaten var.");
                else
                    this.List.Add(TabPage);
            }

            public void Remove(TabPageEx TabPage)
            {
                if (this.List.Contains(TabPage))
                    this.List.Remove(TabPage);
            }

            public bool Contains(TabPageEx TabPage)
            {
                if (this.List.Contains(TabPage))
                    return true;
                else
                    return false;
            }

            public int IndexOf(TabPageEx TabPage)
            {
                return this.List.IndexOf(TabPage);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                foreach (TabPageEx tab in this)
                    tab.Dispose();

                GC.SuppressFinalize(this);
            }

            #endregion
        }

        /// <summary>
        /// Add show/hide capability to a KRBTabControl.When user right-clicks the parent control's header a popup is shown.
        /// </summary>
        class TabPageSelector : IDisposable
        {
            #region Symbolic Constants

            private const int _chkListWidth = 110;
            private const int _chkListHeight = 110;

            #endregion

            #region Instance Members

            private KRBTabControl _linkedControl;
            private CheckedListBox _checkedList;
            private ToolStripDropDown _popup;

            #endregion

            #region Constructor

            public TabPageSelector()
            {
                _checkedList = new CheckedListBox();
                _checkedList.CheckOnClick = true;
                _checkedList.SelectedValueChanged += new EventHandler(OnSelectedValueChanged);

                ToolStripControlHost host = new ToolStripControlHost(_checkedList);
                host.Margin = new Padding(0);
                host.Padding = new Padding(0);
                host.AutoSize = false;

                _popup = new ToolStripDropDown();
                _popup.Padding = new Padding(0);
                _popup.Items.Add(host);
            }

            public TabPageSelector(KRBTabControl control)
                : this()    // Process the first overloading constructor.
            {
                _linkedControl = control;
                _linkedControl.MouseClick += new MouseEventHandler(OnMouseClick);
            }


            #endregion

            #region Helper Methods

            private void OnSelectedValueChanged(object sender, EventArgs e)
            {
                TabPageEx TabPage = _checkedList.SelectedItem as TabPageEx;

                switch (_checkedList.GetItemCheckState(_checkedList.SelectedIndex))
                {
                    case CheckState.Checked:
                        _linkedControl.ShowTab(TabPage);
                        break;
                    case CheckState.Unchecked:
                        _linkedControl.HideTab(TabPage);
                        break;
                }

                LoadAllItem();
            }

            private void OnMouseClick(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Right)
                {
                    LoadAllItem();

                    int valueW = _checkedList.PreferredSize.Width;
                    int valueH = (_checkedList.Items.Count * 16) + 7;

                    _checkedList.Width = valueW < _chkListWidth ? valueW : _chkListWidth;
                    _checkedList.Height = valueH < _chkListHeight ? valueH : _chkListHeight;

                    _popup.Show(_linkedControl.PointToScreen(new Point(e.X, e.Y)));
                }
            }

            private void LoadAllItem()
            {
                _checkedList.Items.Clear();

                foreach (TabPage tab in _linkedControl.TabPages)
                    _checkedList.Items.Add(tab, true);

                if (_linkedControl._tabPageExPool != null)
                {
                    for (int i = 0; i < _linkedControl._tabPageExPool.Count; i++)
                        _checkedList.Items.Add(_linkedControl._tabPageExPool[i], false);
                }
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                if (_linkedControl != null)
                    _linkedControl.MouseClick -= new MouseEventHandler(OnMouseClick);

                if (_checkedList != null)
                {
                    _checkedList.SelectedValueChanged -= new EventHandler(OnSelectedValueChanged);
                    _checkedList.Dispose();
                }
                if (_popup != null)
                    _popup.Dispose();

                GC.SuppressFinalize(this);
            }

            #endregion
        }

        class Custom3DBorder : IDisposable
        {
            #region Instance Members

            /// <summary>
            /// This is a private member variable that contains the associated
            /// Graphics object. It is not part of the programming interface of
            /// the Custom3DBorder class.
            /// </summary>
            private Graphics _graphics;

            #endregion

            #region Constructor

            /// <summary>
            /// This constructor initializes a new instance of the Custom3DBorder class with the
            /// specified Graphics object. Grid points are based on a grid where the lines
            /// are between the pixels, instead of through the centers of the pixels.
            /// </summary>
            /// <param name="graphics">Graphics object that is the drawing surface on which
            /// this Custom3DBorder object draws.</param>
            public Custom3DBorder(Graphics graphics)
            {
                _graphics = graphics;
            }

            #endregion

            #region Helper Methods

            /// <summary>
            /// Draws a line connecting the two points specified by the coordinate pairs. If the line
            /// is horizontal, the pixels are drawn just below the specified grid line. If the line is
            /// vertical, the pixels are drawn just to the right of the the specified grid line. If the
            /// line is diagonal, the pixels are drawn from corner to corner of a rectangle where the
            /// corners are made up of the two points. The pixels will fall just inside the rectangle.
            /// </summary>
            /// <param name="pen">Pen object that determines the color, width, and style of the line.</param>
            /// <param name="p1">Point structure that specifies the first grid point to connect.</param>
            /// <param name="p2">Point structure that specifies the second grid point to connect.</param>
            private void DrawLine(Pen pen, Point p1, Point p2)
            {
                Point pp1 = new Point(p1.X, p1.Y);
                Point pp2 = new Point(p2.X, p2.Y);

                // to simplify the modifications that we have to make
                // if the second point is to the left of the first point,
                // switch the two points.
                if (pp2.X < pp1.X)
                {
                    Point tempPoint = pp2;
                    pp2 = pp1;
                    pp1 = tempPoint;
                }

                if (pp1.Y == pp2.Y)  // if line is horizontal
                {
                    pp2.X--;
                }
                else if (pp1.X == pp2.X)  // if line is vertical
                {
                    pp2.Y--;
                }
                else if (pp2.Y > pp1.Y) // if line is descending down and to right
                {
                    pp2.X--;
                    pp2.Y--;
                }
                else if (pp2.Y < pp1.Y) // if line is ascending up and to right
                {
                    pp1.Y--;
                    pp2.X--;
                }
                _graphics.DrawLine(pen, pp1, pp2);
            }

            #endregion

            #region General Methods

            /// <summary>
            /// Draws an inset or raised line connecting the two points specified by
            /// the coordinate pairs. This method draws only horizontal or vertical
            /// lines. If the line is not horizontal or vertical, this method throws an
            /// ArgumentException. If the line is horizontal, the pixels are
            /// drawn just below the specified grid line. If the line is
            /// vertical, the pixels are drawn just to the right of the
            /// the specified grid line.
            /// </summary>
            /// <param name="p1">Point structure that specifies the first grid point to connect.</param>
            /// <param name="p2">Point structure that specifies the second grid point to connect.</param>
            /// <param name="style">Specifies the raised or inset style.</param>
            /// <param name="depth">Specifies the depth of the raised or inset line or rectangle.
            /// Valid values are 1 and 2.</param>
            public void Draw3DLine(Point p1, Point p2, ThreeDStyle style, int depth)
            {
                if (p1.X != p2.X && p1.Y != p2.Y)
                    throw new ArgumentException();
                if (depth != 1 && depth != 2)
                    throw new ArgumentException();

                if (depth == 1)
                {
                    switch (style)
                    {
                        case ThreeDStyle.Inset:
                        case ThreeDStyle.Groove:
                            if (p1.Y == p2.Y)
                            {
                                DrawLine(SystemPens.ControlDark, p1, p2);
                                Point pl1 = new Point(p1.X, p1.Y + 1);
                                Point pl2 = new Point(p2.X, p2.Y + 1);
                                DrawLine(SystemPens.ControlLightLight, pl1, pl2);
                            }
                            else
                            {
                                DrawLine(SystemPens.ControlDark, p1, p2);
                                Point pl1 = new Point(p1.X + 1, p1.Y);
                                Point pl2 = new Point(p2.X + 1, p2.Y);
                                DrawLine(SystemPens.ControlLightLight, pl1, pl2);
                            }
                            break;
                        case ThreeDStyle.Raised:
                        case ThreeDStyle.Ridge:
                            if (p1.Y == p2.Y)
                            {
                                DrawLine(SystemPens.ControlLightLight, p1, p2);
                                Point pd1 = new Point(p1.X, p1.Y + 1);
                                Point pd2 = new Point(p2.X, p2.Y + 1);
                                DrawLine(SystemPens.ControlDark, pd1, pd2);
                            }
                            else
                            {
                                DrawLine(SystemPens.ControlLightLight, p1, p2);
                                Point pd1 = new Point(p1.X + 1, p1.Y);
                                Point pd2 = new Point(p2.X + 1, p2.Y);
                                DrawLine(SystemPens.ControlDark, pd1, pd2);
                            }
                            break;
                    }
                }
                else if (depth == 2)
                {
                    switch (style)
                    {
                        case ThreeDStyle.Inset:
                        case ThreeDStyle.Groove:
                            if (p1.Y == p2.Y)
                            {
                                DrawLine(SystemPens.ControlDarkDark, p1, p2);
                                Point pp1 = new Point(p1.X, p1.Y + 1);
                                Point pp2 = new Point(p2.X, p2.Y + 1);
                                DrawLine(SystemPens.ControlDark, pp1, pp2);
                                pp1.Y++;
                                pp2.Y++;
                                DrawLine(SystemPens.ControlLight, pp1, pp2);
                                pp1.Y++;
                                pp2.Y++;
                                DrawLine(SystemPens.ControlLightLight, pp1, pp2);
                            }
                            else
                            {
                                DrawLine(SystemPens.ControlDarkDark, p1, p2);
                                Point pp1 = new Point(p1.X + 1, p1.Y);
                                Point pp2 = new Point(p2.X + 1, p2.Y);
                                DrawLine(SystemPens.ControlDark, pp1, pp2);
                                pp1.X++;
                                pp2.X++;
                                DrawLine(SystemPens.ControlLight, pp1, pp2);
                                pp1.X++;
                                pp2.X++;
                                DrawLine(SystemPens.ControlLightLight, pp1, pp2);
                            }
                            break;
                        case ThreeDStyle.Raised:
                        case ThreeDStyle.Ridge:
                            if (p1.Y == p2.Y)
                            {
                                DrawLine(SystemPens.ControlLightLight, p1, p2);
                                Point pp1 = new Point(p1.X, p1.Y + 1);
                                Point pp2 = new Point(p2.X, p2.Y + 1);
                                DrawLine(SystemPens.ControlLight, pp1, pp2);
                                pp1.Y++;
                                pp2.Y++;
                                DrawLine(SystemPens.ControlDark, pp1, pp2);
                                pp1.Y++;
                                pp2.Y++;
                                DrawLine(SystemPens.ControlDarkDark, pp1, pp2);
                            }
                            else
                            {
                                DrawLine(SystemPens.ControlLightLight, p1, p2);
                                Point pp1 = new Point(p1.X + 1, p1.Y);
                                Point pp2 = new Point(p2.X + 1, p2.Y);
                                DrawLine(SystemPens.ControlLight, pp1, pp2);
                                pp1.X++;
                                pp2.X++;
                                DrawLine(SystemPens.ControlDark, pp1, pp2);
                                pp1.X++;
                                pp2.X++;
                                DrawLine(SystemPens.ControlDarkDark, pp1, pp2);
                            }
                            break;
                    }
                }
            }

            /// <summary>
            /// Draws an inset or raised line connecting the two points specified by
            /// the coordinate pairs. This method draws only horizontal or vertical
            /// lines. If the line is not horizontal or vertical, this method throws an
            /// ArgumentException. If the line is horizontal, the pixels are
            /// drawn just below the specified grid line. If the line is
            /// vertical, the pixels are drawn just to the right of the
            /// the specified grid line.
            /// </summary>
            /// <param name="x1">x coordinate of the first grid point.</param>
            /// <param name="y1">y coordinate of the first grid point.</param>
            /// <param name="x2">x coordinate of the second grid point.</param>
            /// <param name="y2">y coordinate of the second grid point.</param>
            /// <param name="style">Specifies the raised or inset style.</param>
            /// <param name="depth">Specifies the depth of the raised or inset line or rectangle.
            /// Valid values are 1 and 2.</param>
            public void Draw3DLine(int x1, int y1, int x2, int y2, ThreeDStyle style, int depth)
            {
                this.Draw3DLine(new Point(x1, y1), new Point(x2, y2), style, depth);
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            #endregion
        }

        class ArrowWindow : Form
        {
            #region Instance Members

            private Icon _icon = null;      // Initializer

            #endregion

            #region Constructor

            public ArrowWindow()
            {
                this.Visible = false;
                this.ShowIcon = false;
                this.ControlBox = false;
                this.ShowInTaskbar = false;
                this.TransparencyKey = this.BackColor;
                this.AutoScaleMode = AutoScaleMode.Font;
                this.SizeGripStyle = SizeGripStyle.Hide;
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.None;
                this.StartPosition = FormStartPosition.Manual;
                this.BackgroundImageLayout = ImageLayout.Center;
            }

            public ArrowWindow(Icon arrowIcon, Double opacity)
                : this()
            {
                if (arrowIcon == null)
                    throw new ArgumentException();

                _icon = arrowIcon;
                this.Opacity = opacity;

                this.Size = new Size(_icon.Width, _icon.Height);    // Penceremizin boyutlarını iconun boyutlarına ayarlayalım.
            }

            #endregion

            #region Property

            public Icon ArrowIcon
            {
                get { return _icon; }
                set 
                {
                    if (!value.Equals(_icon))
                    {
                        _icon = value;

                        Invalidate();
                        Update();
                    }
                }
            }

            #endregion

            #region Override Methods

            protected override void OnPaint(PaintEventArgs e)
            {
                if (_icon != null)
                    e.Graphics.DrawIcon(_icon, this.ClientRectangle);

            }

            protected override void OnSizeChanged(EventArgs e)
            {
                if (_icon != null)
                    this.Size = new Size(_icon.Width, _icon.Height);
            }

            protected override void OnVisibleChanged(EventArgs e)
            {
                if (this.Visible)
                {
                    User32.SetWindowPos(this.Handle, new IntPtr((int)User32.SetWindowPosZ.HWND_TOPMOST), 0, 0, 0, 0,
                        User32.FlagsSetWindowPos.SWP_NOSIZE | User32.FlagsSetWindowPos.SWP_NOMOVE | User32.FlagsSetWindowPos.SWP_NOREDRAW | User32.FlagsSetWindowPos.SWP_NOACTIVATE);
                }
            }

            protected override void WndProc(ref Message m)
            {
                switch (m.Msg)
                {
                    case (int)User32.Msgs.WM_NCHITTEST:
                        m.Result = (IntPtr)User32._htTransparent;
                        break;
                }

                base.WndProc(ref m);
            }

            protected override bool ShowWithoutActivation
            {
                get
                {
                    return true;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_icon != null)
                        _icon.Dispose();
                }

                base.Dispose(disposing);
            }
            
            #endregion
        }

        class UpDown32 : NativeWindow
        {
            #region Constructor

            public UpDown32(IntPtr hwnd)
                : base()
            {
                this.AssignHandle(hwnd);
            }

            #endregion

            #region Override Methods

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                switch (m.Msg)
                {
                    case (int)User32.Msgs.WM_DESTROY:
                        this.ReleaseHandle();
                        break;
                    case (int)User32.Msgs.WM_NCDESTROY:
                        this.ReleaseHandle();
                        break;
                    case (int)User32.Msgs.WM_NCPAINT:
                        int style = User32.GetWindowLong(m.HWnd, (int)User32.WindowExStyles.GWL_STYLE);
                        if ((style & (int)User32.WindowStyles.WS_VISIBLE) == (int)User32.WindowStyles.WS_VISIBLE)
                        {
                            User32.SetWindowPos(m.HWnd, new IntPtr(0), 0, 0, 0, 0, User32.FlagsSetWindowPos.SWP_NOMOVE | User32.FlagsSetWindowPos.SWP_NOSIZE | User32.FlagsSetWindowPos.SWP_NOREDRAW | User32.FlagsSetWindowPos.SWP_NOACTIVATE);
                            User32.SetWindowLong(m.HWnd, (int)User32.WindowExStyles.GWL_STYLE, style & ~(int)User32.WindowStyles.WS_VISIBLE);
                        }
                        m.Result = (IntPtr)1; // indicate msg has been processed
                        break;
                }
            }
            
            #endregion
        }

        class Scroller : Control
        {
            #region Event

            public event EventHandler ScrollLeft;
            public event EventHandler ScrollRight;

            #endregion

            #region Instance Members

            public RolloverUpDown _leftScroller;
            public RolloverUpDown _rightScroller;

            #endregion

            #region Constructor

            public Scroller()
                : base()
            {
                this.SetStyle(ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
            }

            public Scroller(UpDown32Style upDownStyle)
                : this()
            {
                _leftScroller = new RolloverUpDown(true);
                _rightScroller = new RolloverUpDown(true);

                switch (upDownStyle)
                {
                    case UpDown32Style.BlackGlass:
                        _leftScroller.NormalImage = KRBTabResources.LeftNormalBlackGlass;
                        _leftScroller.HoverImage = KRBTabResources.LeftHoverBlackGlass;
                        _leftScroller.DownImage = KRBTabResources.LeftDownBlackGlass;
                        _rightScroller.NormalImage = KRBTabResources.RightNormalBlackGlass;
                        _rightScroller.HoverImage = KRBTabResources.RightHoverBlackGlass;
                        _rightScroller.DownImage = KRBTabResources.RightDownBlackGlass;
                        break;
                    case UpDown32Style.KRBBlue:
                        _leftScroller.NormalImage = KRBTabResources.LeftNormalKRBBlue;
                        _leftScroller.HoverImage = KRBTabResources.LeftHoverKRBBlue;
                        _leftScroller.DownImage = KRBTabResources.LeftDownKRBBlue;
                        _rightScroller.NormalImage = KRBTabResources.RightNormalKRBBlue;
                        _rightScroller.HoverImage = KRBTabResources.RightHoverKRBBlue;
                        _rightScroller.DownImage = KRBTabResources.RightDownKRBBlue;
                        break;
                    case UpDown32Style.OfficeBlue:
                        _leftScroller.NormalImage = KRBTabResources.LeftNormalOfficeBlue;
                        _leftScroller.HoverImage = KRBTabResources.LeftHoverOfficeBlue;
                        _leftScroller.DownImage = KRBTabResources.LeftDownOfficeBlue;
                        _rightScroller.NormalImage = KRBTabResources.RightNormalOfficeBlue;
                        _rightScroller.HoverImage = KRBTabResources.RightHoverOfficeBlue;
                        _rightScroller.DownImage = KRBTabResources.RightDownOfficeBlue;
                        break;
                    case UpDown32Style.OfficeOlive:
                        _leftScroller.NormalImage = KRBTabResources.LeftNormalOfficeOlive;
                        _leftScroller.HoverImage = KRBTabResources.LeftHoverOfficeOlive;
                        _leftScroller.DownImage = KRBTabResources.LeftDownOfficeOlive;
                        _rightScroller.NormalImage = KRBTabResources.RightNormalOfficeOlive;
                        _rightScroller.HoverImage = KRBTabResources.RightHoverOfficeOlive;
                        _rightScroller.DownImage = KRBTabResources.RightDownOfficeOlive;
                        break;
                    case UpDown32Style.OfficeSilver:
                        _leftScroller.NormalImage = KRBTabResources.LeftNormalOfficeSilver;
                        _leftScroller.HoverImage = KRBTabResources.LeftHoverOfficeSilver;
                        _leftScroller.DownImage = KRBTabResources.LeftDownOfficeSilver;
                        _rightScroller.NormalImage = KRBTabResources.RightNormalOfficeSilver;
                        _rightScroller.HoverImage = KRBTabResources.RightHoverOfficeSilver;
                        _rightScroller.DownImage = KRBTabResources.RightDownOfficeSilver;
                        break;
                }

                this.Size = new Size()
                {
                    Width = _leftScroller.NormalImage.Width * 2 + 1,
                    Height = _leftScroller.NormalImage.Height
                };

                _leftScroller.Location = new Point(0, 0);
                _rightScroller.Location = new Point(this.Width / 2 + 1, 0);

                _leftScroller.MouseClick += new MouseEventHandler(OnLeftScroll);
                _rightScroller.MouseClick += new MouseEventHandler(OnRightScroll);
                
                _leftScroller.MouseDoubleClick += new MouseEventHandler(OnLeftScroll);
                _rightScroller.MouseDoubleClick += new MouseEventHandler(OnRightScroll);
                
                this.Controls.Add(_leftScroller);
                this.Controls.Add(_rightScroller);
            }

            #endregion

            #region Property

            //protected override CreateParams CreateParams
            //{
            //    get
            //    {
            //        CreateParams cp = base.CreateParams;
            //        cp.ExStyle |= 0x20;

            //        return cp;
            //    }
            //}

            #endregion

            #region Override Methods
            
            //protected override void OnPaintBackground(PaintEventArgs pevent)
            //{
            //    if (BackColor == Color.Transparent)
            //    {
            //         // Do Nothing.
            //    }
            //    else
            //    {
            //        base.OnPaintBackground(pevent);
            //    }
            //}
            
            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _leftScroller.MouseClick -= new MouseEventHandler(OnLeftScroll);
                    _rightScroller.MouseClick -= new MouseEventHandler(OnRightScroll);
                    
                    _leftScroller.MouseDoubleClick -= new MouseEventHandler(OnLeftScroll);
                    _rightScroller.MouseDoubleClick -= new MouseEventHandler(OnRightScroll);

                    _leftScroller.Dispose();
                    _rightScroller.Dispose();
                }

                base.Dispose(disposing);
            }
            
            #endregion

            #region Helper Methods

            private void OnRightScroll(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (ScrollRight != null)
                        ScrollRight(this, EventArgs.Empty);
                }
            }

            private void OnLeftScroll(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (ScrollLeft != null)
                        ScrollLeft(this, EventArgs.Empty);
                }
            }

            #endregion
        }

        class RolloverUpDown : UpDownBase
        {
            #region Instance Members

            private Image _disabledImage;
            private Image _normalImage;
            private Image _hoverImage;
            private Image _downImage;

            #endregion

            #region Constructor

            public RolloverUpDown(bool value)
            {
                this.Caching = value;
            }

            #endregion

            #region Property

            public Image DisabledImage
            {
                get { return _disabledImage; }
                set 
                {
                    if (value != null)
                    {
                        if (!value.Equals(_disabledImage))
                            _disabledImage = value;
                    }
                    else
                    {
                        _disabledImage = null;
                    }
                }
            }

            public Image NormalImage
            {
                get { return _normalImage; }
                set 
                {
                    if (value != null)
                    {
                        if (!value.Equals(_normalImage))
                        {
                            _normalImage = value;

                            this.Size = new Size(_normalImage.Width, _normalImage.Height);
                        }
                    }
                    else
                    {
                        _normalImage = null;
                    }
                }
            }

            public Image HoverImage
            {
                get { return _hoverImage; }
                set 
                {
                    if (value != null)
                    {
                        if (!value.Equals(_hoverImage))
                            _hoverImage = value;
                    }
                    else
                    {
                        _hoverImage = null;
                    }
                }
            }

            public Image DownImage
            {
                get { return _downImage; }
                set 
                {
                    if (value != null)
                    {
                        if (!value.Equals(_downImage))
                            _downImage = value;
                    }
                    else
                    {
                        _downImage = null;
                    }
                }
            }
            
            #endregion
            
            #region Override Methods

            protected override void PaintDisabled(Graphics g)
            {
                if (_disabledImage != null)
                    g.DrawImageUnscaled(_disabledImage, 0, 0);
                else
                {
                    if (_normalImage != null)
                        ControlPaint.DrawImageDisabled(g, _normalImage, 0, 0, this.BackColor);
                }
            }

            protected override void PaintNormal(Graphics g)
            {
                if (_normalImage != null)
                    g.DrawImageUnscaled(_normalImage, 0, 0);
            }

            protected override void PaintHover(Graphics g)
            {
                if (_hoverImage != null)
                    g.DrawImageUnscaled(_hoverImage, 0, 0);
            }

            protected override void PaintDown(Graphics g)
            {
                if (_downImage != null)
                    g.DrawImageUnscaled(_downImage, 0, 0);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_disabledImage != null)
                        _disabledImage.Dispose();
                    if (_normalImage != null)
                        _normalImage.Dispose();
                    if (_hoverImage != null)
                        _hoverImage.Dispose();
                    if (_downImage != null)
                        _downImage.Dispose();
                }

                base.Dispose(disposing);
            }
            
            #endregion
        }

        abstract class UpDownBase : Control
        {
            #region Enum

            private enum States
            {
                Disabled,
                Normal,
                Hover,
                Down
            };

            #endregion

            #region Instance Members
            
            private Image _disabledImage;
            private Image _normalImage;
            private Image _hoverImage;
            private Image _downImage;

            private bool _caching = true;               // Initializer
            private States _btnState = States.Normal;   // Initializer

            #endregion

            #region Constructor

            public UpDownBase()
            {
                this.SetStyle(ControlStyles.UserPaint | ControlStyles.FixedWidth | ControlStyles.FixedHeight, true);
            }

            #endregion

            #region Property

            public bool Caching
            {
                get { return _caching; }
                set 
                {
                    if (!value.Equals(_caching))
                        _caching = value;
                }
            }
            
            #endregion

            #region Override Methods

            protected override void OnPaint(PaintEventArgs e)
            {
                if (!_caching)
                {
                    switch (_btnState)
                    {
                        case States.Down:
                            PaintDown(e.Graphics);
                            break;
                        case States.Hover:
                            PaintHover(e.Graphics);
                            break;
                        case States.Normal:
                            PaintNormal(e.Graphics);
                            break;
                        case States.Disabled:
                            PaintDisabled(e.Graphics);
                            break;
                    }
                }
                else
                {
                    switch (_btnState)
                    {
                        case States.Down:
                            CreateAndPaintCachedImage(_downImage,
                                new ClientPaintMethod(PaintDown), e.Graphics);
                            break;
                        case States.Hover:
                            CreateAndPaintCachedImage(_hoverImage,
                                new ClientPaintMethod(PaintHover), e.Graphics);
                            break;
                        case States.Normal:
                            CreateAndPaintCachedImage(_normalImage,
                                new ClientPaintMethod(PaintNormal), e.Graphics);
                            break;
                        case States.Disabled:
                            CreateAndPaintCachedImage(_disabledImage,
                                new ClientPaintMethod(PaintDisabled), e.Graphics);
                            break;
                    }
                }
            }
            
            protected override void OnMouseEnter(EventArgs e)
            {
                if (_btnState == States.Disabled)
                    return;

                _btnState = States.Hover;

                this.Invalidate();

                base.OnMouseEnter(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                if (_btnState == States.Disabled)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    _btnState = States.Down;
                    this.Invalidate();
                }

                base.OnMouseDown(e);
            }
            
            protected override void OnMouseUp(MouseEventArgs e)
            {
                if (_btnState == States.Disabled)
                    return;

                if (e.Button == MouseButtons.Left)
                {
                    _btnState = States.Hover;
                    this.Invalidate();
                }

                base.OnMouseUp(e);
            }

            protected override void OnMouseLeave(EventArgs e)
            {
                if (_btnState == States.Disabled)
                    return;

                _btnState = States.Normal;

                this.Invalidate();

                base.OnMouseLeave(e);
            }

            protected override void OnEnabledChanged(EventArgs e)
            {
                if (!this.Enabled)
                    _btnState = States.Disabled;
                else if (this.Enabled && _btnState == States.Disabled)
                    _btnState = States.Normal;

                this.Invalidate();

                base.OnEnabledChanged(e);
            }
            
            #endregion

            #region Abstract Methods

            protected abstract void PaintDisabled(Graphics g);

            protected abstract void PaintNormal(Graphics g);

            protected abstract void PaintHover(Graphics g);

            protected abstract void PaintDown(Graphics g);

            #endregion

            #region Helper Methods
            
            private delegate void ClientPaintMethod(Graphics g);
            
            private void CreateAndPaintCachedImage(
                Image image, ClientPaintMethod paintMethod, Graphics g)
            {
                if (image == null)
                {
                    image = new Bitmap(Width, Height);
                    Graphics bufferedGraphics = Graphics.FromImage(image);
                    paintMethod(bufferedGraphics);
                    bufferedGraphics.Dispose();
                }
                g.DrawImageUnscaled(image, 0, 0);
            }

            #endregion

            #region General Methods
            
            protected void Flush()
            {
                _disabledImage = null;
                _normalImage = null;
                _hoverImage = null;
                _downImage = null;
            }

            #endregion
        }
    }

    [Designer(typeof(System.Windows.Forms.Design.ScrollableControlDesigner))]
    public class TabPageEx : TabPage
    {
        #region Instance Members

        private string _text = null;

        #endregion

        #region Constructor

        public TabPageEx()
            : base()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);
        }

        public TabPageEx(string text)
            : this()
        {
            this.Text = text;
        }

        #endregion

        #region Property

        public object SomeUserObject = null;

        public new string Text
        {
            get
            {
                return _text;
            }
            set
            {
                if (!value.Equals(_text))
                {
                    base.Text = value;
                    base.Text = base.Text.Trim();
                    base.Text = base.Text.PadRight(base.Text.Length + 10);
                    _text = base.Text.TrimEnd();
                }
            }
        }
        
        #endregion

        #region Override Methods

        public override string ToString()
        {
            return this.Text;
        }
        
        #endregion
    }
}