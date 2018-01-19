using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Channy.Controls2.Controls {
    public class Window : System.Windows.Window {
        static Window() {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(Window), new FrameworkPropertyMetadata(typeof(Window)));
        }

        public Window() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/Window") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ScrollBar") });
            Template = (ControlTemplate)Resources["WindowExTemplate"];
            RegisterEvents();
            base.WindowStyle = WindowStyle;
            base.AllowsTransparency = AllowsTransparency;
        }

        #region Attributes
        public bool Draggable { get; set; } = true;

        private new WindowStyle WindowStyle = WindowStyle.None;
        private new bool AllowsTransparency = true;

        public int Radius {
            get { return radius; }
        }

        public ControlBoxStyles ControlBoxStyle {
            get {
                return controlBoxStyle;
            }
            set {
                controlBoxStyle = value;
                if (isInitializationComplete) {
                    SetControlBoxStyle(ControlBoxStyle);
                }
            }
        }

        public Color ControlGridBackColor {
            get { return controlGridBackColor; }
            set {
                controlGridBackColor = value;
                if (isInitializationComplete) {
                    SetControlGridBackColor(value);
                }
            }
        }

        public bool ShowTitle {
            get {
                return showTitle;
            }
            set {
                showTitle = value;
                if (title != null) {
                    title.Visibility = Visibility.Collapsed;
                }
            }
        }

        public double BottomBannerHeight {
            get { return bottomBannerHeight; }
            set {
                if (value < 0) {
                    bottomBannerHeight = 0;
                } else if (bottomBannerHeight > Height - 40) {
                    bottomBannerHeight = Height - 40;
                } else {
                    bottomBannerHeight = value;
                }
                if (isInitializationComplete) {
                    SetBottomBanner(BottomBannerHeight);
                }
            }
        }

        public bool ReserveTitleBar {
            get { return reserveTitleBar; }
            set {
                reserveTitleBar = value;
                if (contentPresenter != null) {
                    SetReserveTitleBar(value);
                }
            }
        }

        /// <summary>
        /// Gets or Sets the base color of the window. When glass effect is enabled, this makes no sense.
        /// </summary>
        public Color BaseColor {
            get { return baseColor; }
            set {
                baseColor = value;
                if (isInitializationComplete) {
                    SetBackColor(value);
                }
            }
        }

        /// <summary>
        /// Sets or Gets if the icon is displayed on the top left of the window
        /// </summary>
        public bool ShowIcon {
            get { return showIcon; }
            set {
                showIcon = value;
                if (isInitializationComplete) {
                    if (!ShowIcon) {
                        logo.Visibility = Visibility.Collapsed;
                    } else {
                        logo.Visibility = Visibility.Visible;
                    }
                }
            }
        }

        public bool HideOnClose { get; set; } = false;
        #endregion

        public enum ControlBoxStyles {
            MinMaxClose,
            Close,
            MinClose,
            None
        }

        public void BringToFront() {
            WindowHelper.GlobalActivate(this);
        }

        private bool IsGlassEffectEnabled {
            get {
                return GetGlassEffectEnabled();
            }
        }

        private void RegisterEvents() {
            SizeChanged += new SizeChangedEventHandler(WindowEx_SizeChanged);
            Loaded += new RoutedEventHandler(WindowEx_Loaded);
        }

        private void UnregisterEvents() {
            SizeChanged -= new SizeChangedEventHandler(WindowEx_SizeChanged);
            Loaded -= new RoutedEventHandler(WindowEx_Loaded);
        }

        private int radius = 5;
        private bool isInitializationComplete = false;
        private bool showIcon = true;

        private byte alphaCache = 255;

        private ControlBoxStyles controlBoxStyle = ControlBoxStyles.MinMaxClose;

        private System.Windows.Controls.Button close = null;
        private System.Windows.Controls.Button min = null;
        private System.Windows.Controls.Button max = null;
        private Grid controlBox = null;
        private Line minMaxLine = null;
        private Line maxCloseLine = null;
        private Grid bottomBanner = null;
        private Border controlGridBorder = null;
        private Border border = null;
        private Image logo = null;
        private TextBlock title = null;
        private Border titleBar = null;
        private ContentPresenter contentPresenter = null;

        private bool reserveTitleBar = true;
        private bool showTitle = true;

        private double bottomBannerHeight = 35.0;

        private Color controlGridBackColor = Color.FromArgb(128, 255, 255, 255);
        private Color baseColor = Color.FromArgb(255, 234, 234, 234);

        private IntPtr blurRegionHandle = IntPtr.Zero;
        private const int HitTestCornerThreshold = 8;
        private const int HitTestBorderThreshold = 4;
        //private const int TitleHeight = 36;
        //private int ControlBoxWidth = 93;
        private WinApi.RECT clientRect = new WinApi.RECT();

        protected override void OnClosed(EventArgs e) {
            if (blurRegionHandle != IntPtr.Zero) {
                WinApi.DeleteObject(blurRegionHandle);
            }
            UnregisterEvents();
            MouseLeftButtonDown -= new MouseButtonEventHandler(WindowEx_MouseLeftButtonDown);
            if (min != null) {
                min.Click -= new RoutedEventHandler(min_Click);
                min = null;
            }
            if (max != null) {
                max.Click -= new RoutedEventHandler(max_Click);
                max = null;
            }
            if (close != null) {
                close.Click -= new RoutedEventHandler(close_Click);
                close = null;
            }
            controlBox = null;
            minMaxLine = null;
            maxCloseLine = null;
            //brush = null;
            bottomBanner = null;
            controlGridBorder = null;
            border = null;
            logo = null;
            base.OnClosed(e);
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case (int)WinApi.Messages.WM_DWMCOMPOSITIONCHANGED:
                    if (IsGlassEffectEnabled) {
                        alphaCache = BaseColor.A;
                        baseColor.A = 1;
                        BaseColor = baseColor;
                        EnableBlur();
                    } else {
                        baseColor.A = alphaCache;
                        BaseColor = baseColor;
                    }
                    break;
                case (int)WinApi.Messages.WM_GETMINMAXINFO:
                    WinApi.WmGetMinMaxInfo(hwnd, lParam);
                    handled = true;
                    break;
                case (int)WinApi.Messages.WM_NCHITTEST:
                    int x = unchecked((short)(long)lParam);
                    int y = unchecked((short)((long)lParam >> 16));
                    Point pos = PointFromScreen(new Point(x, y));
                    Point topLeft = PointFromScreen(new Point(Left, Top));
                    x = (int)pos.X;
                    y = (int)pos.Y;
                    int left = (int)topLeft.X;
                    int top = (int)topLeft.Y;
                    if (WindowState == WindowState.Maximized) {
                        top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.X;
                        left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Y;
                    }

                    if (y <= HitTestCornerThreshold && x <= HitTestCornerThreshold) { // Topleft
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTTOPLEFT);
                    } else if (y <= HitTestBorderThreshold && ActualWidth - x <= HitTestCornerThreshold) { // Topright
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTTOPRIGHT);
                    } else if (ActualHeight - y <= HitTestCornerThreshold && ActualWidth - x <= HitTestCornerThreshold) { // Bottomright
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTBOTTOMRIGHT);
                    } else if (ActualHeight - y <= HitTestCornerThreshold && x <= HitTestCornerThreshold) { // Bottomleft
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTBOTTOMLEFT);
                    } else if (y <= HitTestBorderThreshold) { // Top
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTTOP);
                    } else if (ActualWidth - x <= HitTestBorderThreshold) { // Right
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTRIGHT);
                    } else if (ActualHeight - y <= HitTestBorderThreshold) { // Bottom
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTBOTTOM);
                    } else if (x <= HitTestBorderThreshold) { // Left
                        handled = true;
                        return new IntPtr((int)WinApi.HitTest.HTLEFT);
                    } else {
                        if (Draggable && IsInTitleArea(x, y)) {
                            handled = true;
                            return new IntPtr((int)WinApi.HitTest.HTCAPTION);
                        } else {
                            break;
                        }
                    }
                case (int)WinApi.Messages.WM_SIZING:
                    int edge = wParam.ToInt32();
                    WinApi.RECT rect = new WinApi.RECT();
                    rect = (WinApi.RECT)Marshal.PtrToStructure(lParam, typeof(WinApi.RECT));
                    Point rectLt = PointFromScreen(new Point(rect.left, rect.top));
                    Point rectRb = PointFromScreen(new Point(rect.right, rect.bottom));
                    Point windowLt = PointToScreen(new Point(Left, Top));
                    Point windowRb = PointToScreen(new Point(Left + ActualWidth, Top + ActualHeight));
                    if (!double.IsNaN(MinHeight)) {
                        if (rectRb.Y - rectLt.Y <= MinHeight) {
                            if (edge == (int)WinApi.Sizing.WMSZ_TOP
                                || edge == (int)WinApi.Sizing.WMSZ_TOPLEFT
                                || edge == (int)WinApi.Sizing.WMSZ_TOPRIGHT) {
                                rect.top = rect.bottom - (int)(MinHeight * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
                            } else if (edge == (int)WinApi.Sizing.WMSZ_BOTTOM
                                || edge == (int)WinApi.Sizing.WMSZ_BOTTOMLEFT
                                || edge == (int)WinApi.Sizing.WMSZ_BOTTOMRIGHT) {
                                rect.bottom = rect.top + (int)(MinHeight * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
                            }

                            handled = true;
                        }
                    }

                    if (!double.IsNaN(MinWidth)) {
                        int minWidthOnScreen = (int)new Point(MinWidth, 0).X;
                        if (rectRb.X - rectLt.X <= MinWidth) {
                            if (edge == (int)WinApi.Sizing.WMSZ_LEFT
                                || edge == (int)WinApi.Sizing.WMSZ_TOPLEFT
                                || edge == (int)WinApi.Sizing.WMSZ_BOTTOMLEFT) {
                                rect.left = rect.right - (int)(MinWidth * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
                            } else if (edge == (int)WinApi.Sizing.WMSZ_RIGHT
                                || edge == (int)WinApi.Sizing.WMSZ_TOPRIGHT
                                || edge == (int)WinApi.Sizing.WMSZ_BOTTOMRIGHT) {
                                rect.right = rect.left + (int)(MinWidth * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
                            }

                            handled = true;
                        }
                    }

                    if (handled) {
                        Marshal.StructureToPtr(rect, lParam, true);
                        return new IntPtr(1);
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private static bool GetGlassEffectEnabled() {
            Version ver = Environment.OSVersion.Version;
            if (ver.Major < 6) {
                return false;
            } else if (ver.Major == 6) {
                if (ver.Minor == 0 || ver.Minor == 1) { // Vista or Win 7
                    WinApi.DwmIsCompositionEnabled(out bool enabled);
                    return enabled;
                }

                /* 
                 * If the invoker is not manifested
                 * See https://msdn.microsoft.com/en-us/library/windows/desktop/ms724833(v=vs.85).aspx
                 */
                if (WindowHelper.IsWindows10()) {
                    return true;
                }
            } else if (ver.Major == 10) {
                return true;
            }
            return false;
        }

        private bool IsInTitleArea(int x, int y) {
            //var pos = PointFromScreen(new Point(x, y));
            var point = new Point(x, y);
            var posTitle = TranslatePoint(point, titleBar);
            var posControlBox = TranslatePoint(point, controlBox);

            if (posTitle.X < 0 || posTitle.X > titleBar.ActualWidth || posTitle.Y < 0 || posTitle.Y > titleBar.ActualHeight) {
                return false;
            }

            if (posControlBox.X > 0 && posControlBox.X <= controlBox.ActualWidth && posControlBox.Y > 0 && posControlBox.Y <= controlBox.ActualHeight) {
                return false;
            }

            //if (y <= TitleHeight && x <= ActualWidth - ControlBoxWidth) {
            //    return true;
            //}

            return true;
        }

        private void WindowEx_SizeChanged(object sender, SizeChangedEventArgs e) {
            WinApi.GetClientRect(new WindowInteropHelper(this).Handle, out clientRect);
            if (IsLoaded && IsGlassEffectEnabled) {
                EnableBlur();
            }
        }

        private void WindowEx_Loaded(object sender, RoutedEventArgs e) {
            InitializeControlObjects();

            if (IsGlassEffectEnabled) {
                alphaCache = BaseColor.A;
                baseColor.A = 1;
                BaseColor = baseColor;
                EnableBlur();
            } else {
                baseColor.A = alphaCache;
                BaseColor = baseColor;
            }

            BringToFront();
        }

        private void EnableBlur() {
            Version ver = Environment.OSVersion.Version;
            if (ver.Major < 6) {
                return;
            } else if (ver.Major == 10 || WindowHelper.IsWindows10()) { // Win 10
                WinApi.AccentPolicy accentPolicy = new WinApi.AccentPolicy() {
                    AccentState = WinApi.AccentState.ACCENT_ENABLE_BLURBEHIND,
                };

                int size = Marshal.SizeOf(accentPolicy);
                IntPtr accentPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accentPolicy, accentPtr, false);
                WinApi.WindowCompositionAttributeData data = new WinApi.WindowCompositionAttributeData() {
                    Data = accentPtr,
                    SizeOfData = size,
                    Attribute = WinApi.WindowCompositionAttribute.WCA_ACCENT_POLICY
                };
                IntPtr handle = new WindowInteropHelper(this).Handle;
                WinApi.SetWindowCompositionAttribute(handle, ref data);
                Marshal.FreeHGlobal(accentPtr);
            } else if (ver.Major == 6) {
                if (ver.Minor == 0 || ver.Minor == 1) { // Vista or Win 7
                    if (blurRegionHandle != IntPtr.Zero) {
                        WinApi.DeleteObject(blurRegionHandle);
                    }
                    Point pos = border.TranslatePoint(new Point(0, 0), this);
                    int left = (int)pos.X;
                    int top = (int)pos.Y;
                    double w = border.Width;
                    blurRegionHandle = WinApi.CreateRectRgn(left, top, clientRect.Width - left * 2, clientRect.Height - left * 2);

                    WinApi.DWM_BLURBEHIND blur = new WinApi.DWM_BLURBEHIND() {
                        fEnable = true,
                        dwFlags = WinApi.DWM_BLURBEHIND.DWM_BB_ENABLE | WinApi.DWM_BLURBEHIND.DWM_BB_BLURREGION,
                        hRegionBlur = blurRegionHandle
                    };
                    IntPtr handle = new WindowInteropHelper(this).Handle;
                    WinApi.DwmEnableBlurBehindWindow(handle, blur);
                }
            }
        }

        private void InitializeControlObjects() {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            if (source != null) {
                source.AddHook(new HwndSourceHook(WndProc));
            }

            MouseLeftButtonDown += new MouseButtonEventHandler(WindowEx_MouseLeftButtonDown);
            border = (Border)Template.FindName("WindowExBorder", this);
            if (border != null) {
                SetBackColor(BaseColor);
            } else {
                throw new Exception("Cannot initialize object 'WindowExBorder'");
            }

            logo = (Image)Template.FindName("Logo", this);
            if (logo != null) {
                if (!ShowIcon) {
                    logo.Visibility = Visibility.Collapsed;
                }
            } else {
                throw new Exception("Cannot initialize object 'Logo'");
            }

            controlGridBorder = (Border)Template.FindName("ControlGridBorder", this);
            if (controlGridBorder == null) {
                throw new Exception("Cannot initialize object 'ControlGridBorder'");
            }
            SetControlGridBackColor(ControlGridBackColor);

            titleBar = (Border)Template.FindName("PART_TitleBar", this);
            if (titleBar == null) {
                throw new Exception("Cannot initialize object 'PART_TitleBar'");
            }

            controlBox = (Grid)Template.FindName("PART_ControlBox", this);
            if (controlBox == null) {
                throw new Exception("Cannot initialize object 'PART_ControlBox'");
            }

            bottomBanner = (Grid)Template.FindName("BottomBanner", this);
            if (bottomBanner != null) {
                SetBottomBanner(BottomBannerHeight);
            } else {
                throw new Exception("Cannot initialize object 'BottomBanner'");
            }

            contentPresenter = (ContentPresenter)Template.FindName("ContentPresenter", this);
            if (contentPresenter == null) {
                throw new Exception("Cannot initialize object 'ContentPresenter'");
            } else {
                SetReserveTitleBar(ReserveTitleBar);
            }

            close = (System.Windows.Controls.Button)Template.FindName("CloseButton", this);
            if (close != null) {
                close.Click += new RoutedEventHandler(close_Click);
            } else {
                throw new Exception("Cannot initialize object 'CloseBox'");
            }

            maxCloseLine = (Line)Template.FindName("Line2", this);
            if (maxCloseLine == null) {
                throw new Exception("Cannot initialize object 'MaxCloseSeparator'");
            }

            max = (System.Windows.Controls.Button)Template.FindName("MaxButton", this);
            if (max != null) {
                max.Click += new RoutedEventHandler(max_Click);
            } else {
                throw new Exception("Cannot initialize object 'MaxBox'");
            }

            minMaxLine = (Line)Template.FindName("Line1", this);
            if (minMaxLine == null) {
                throw new Exception("Cannot initialize object 'MinMaxSeparator'");
            }

            min = (System.Windows.Controls.Button)Template.FindName("MinButton", this);
            if (min != null) {
                min.Click += new RoutedEventHandler(min_Click);
            } else {
                throw new Exception("Cannot initialize object 'MinBox'");
            }
            SetControlBoxStyle(ControlBoxStyle);

            title = (TextBlock)Template.FindName("Title", this);
            if (title == null) {
                throw new Exception("Cannot initialize object 'Title'");
            } else {
                if (!showTitle) {
                    title.Visibility = Visibility.Collapsed;
                }
            }

            isInitializationComplete = true;
        }

        private void SetBackColor(Color color) {
            border.Background = new SolidColorBrush(color);
            Brush brush = new SolidColorBrush();
        }

        private void SetBottomBanner(double height) {
            bottomBanner.Height = height;
            if (height == 0) {
                controlGridBorder.CornerRadius = new CornerRadius(0, 0, Radius, Radius);
            } else {
                controlGridBorder.CornerRadius = new CornerRadius(0);
            }
        }

        private void SetReserveTitleBar(bool reserve) {
            if (reserve) {
                contentPresenter.SetValue(Grid.RowProperty, 1);
                contentPresenter.SetValue(Grid.RowSpanProperty, 2);
                controlGridBorder.SetValue(Grid.RowProperty, 1);
                controlGridBorder.SetValue(Grid.RowSpanProperty, 1);
            } else {
                contentPresenter.SetValue(Grid.RowProperty, 0);
                contentPresenter.SetValue(Grid.RowSpanProperty, 3);
                controlGridBorder.SetValue(Grid.RowProperty, 0);
                controlGridBorder.SetValue(Grid.RowSpanProperty, 2);
            }
        }

        private void SetControlGridBackColor(Color color) {
            double offset = 5 / Height;
            LinearGradientBrush brush = new LinearGradientBrush();
            brush.StartPoint = new Point(0, 0);
            brush.EndPoint = new Point(0, 1);
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 0.0));
            brush.GradientStops.Add(new GradientStop(color, offset));
            brush.GradientStops.Add(new GradientStop(color, 1 - offset));
            brush.GradientStops.Add(new GradientStop(Color.FromArgb(0, color.R, color.G, color.B), 1));

            controlGridBorder.Background = brush;
        }

        private void SetControlBoxStyle(ControlBoxStyles style) {
            if (style == ControlBoxStyles.None) {
                controlBox.Visibility = Visibility.Hidden;
            } else if (style == ControlBoxStyles.Close) {
                controlBox.Visibility = Visibility.Visible;
                min.Visibility = Visibility.Hidden;
                minMaxLine.Visibility = Visibility.Hidden;
                max.Visibility = Visibility.Hidden;
                maxCloseLine.Visibility = Visibility.Hidden;
                close.Visibility = Visibility.Visible;
            } else if (style == ControlBoxStyles.MinClose) {
                controlBox.Visibility = Visibility.Visible;
                min.Visibility = Visibility.Visible;
                minMaxLine.Visibility = Visibility.Visible;
                max.Visibility = Visibility.Collapsed;
                maxCloseLine.Visibility = Visibility.Collapsed;
                close.Visibility = Visibility.Visible;
            } else if (style == ControlBoxStyles.MinMaxClose) {
                controlBox.Visibility = Visibility.Visible;
                min.Visibility = Visibility.Visible;
                minMaxLine.Visibility = Visibility.Visible;
                max.Visibility = Visibility.Visible;
                maxCloseLine.Visibility = Visibility.Visible;
                close.Visibility = Visibility.Visible;
            }
        }

        void min_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        void max_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else {
                WindowState = WindowState.Maximized;
            }
        }

        void close_Click(object sender, RoutedEventArgs e) {
            if (HideOnClose) {
                Hide();
            } else {
                Close();
            }
        }

        void WindowEx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (Draggable && e.ButtonState == MouseButtonState.Pressed) {
                DragMove();
            }
        }

        protected override void OnStateChanged(EventArgs e) {
            base.OnStateChanged(e);
            if (WindowState != WindowState.Minimized) {
                BringToFront();
            }
        }
    }
}
