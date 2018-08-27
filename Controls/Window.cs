using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/PredefinedColors") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/Window") });
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ScrollBar") });

            Template = (ControlTemplate)Resources["WindowExTemplate"];
            base.WindowStyle = WindowStyle.None;
            base.AllowsTransparency = true;
            Background = Brushes.Transparent;
            RegisterEvents();
        }

        #region Dependency properties
        public static readonly DependencyProperty ControlBoxStyleProperty = DependencyProperty.Register("ControlBoxStyle", typeof(ControlBoxStyles), typeof(Window), new FrameworkPropertyMetadata(ControlBoxStyles.MinMaxClose));
        public ControlBoxStyles ControlBoxStyle {
            get { return (ControlBoxStyles)GetValue(ControlBoxStyleProperty); }
            set { SetValue(ControlBoxStyleProperty, value); }
        }

        public static readonly new DependencyProperty AllowsTransparencyProperty = DependencyProperty.Register("AllowsTransparency", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(false));
        public new bool AllowsTransparency {
            get { return (bool)GetValue(AllowsTransparencyProperty); }
        }

        public static readonly new DependencyProperty WindowStyleProperty = DependencyProperty.Register("WindowStyle", typeof(WindowStyle), typeof(Window), new FrameworkPropertyMetadata(WindowStyle.None));
        public new WindowStyle WindowStyle {
            get { return (WindowStyle)GetValue(WindowStyleProperty); }
        }

        public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(Window), new FrameworkPropertyMetadata(new CornerRadius(0)));
        public CornerRadius CornerRadius {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }

        public static readonly DependencyProperty BaseBackgroundProperty = DependencyProperty.Register("BaseBackground", typeof(Brush), typeof(Window), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(1, 0, 0, 0))));
        /// <summary>
        /// The downmost layer of background, Background is on top of it.
        /// </summary>
        public Brush BaseBackground {
            get { return (Brush)GetValue(BaseBackgroundProperty); }
            set { SetValue(BaseBackgroundProperty, value); }
        }
        public static readonly DependencyProperty ContentPresenterBackgroundProperty = DependencyProperty.Register("ContentPresenterBackground", typeof(SolidColorBrush), typeof(Window), new FrameworkPropertyMetadata(new SolidColorBrush(Color.FromArgb(192, 234, 234, 234))));
        /// <summary>
        /// The topmost layer of background, Background is on bottom of it.
        /// With a top margin of the height of the title bar, and with a bottom margin of the bottom banner.
        /// </summary>
        public SolidColorBrush ContentPresenterBackground {
            get { return (SolidColorBrush)GetValue(ContentPresenterBackgroundProperty); }
            set { SetValue(ContentPresenterBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ShowIconProperty = DependencyProperty.Register("ShowIcon", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool ShowIcon {
            get { return (bool)GetValue(ShowIconProperty); }
            set { SetValue(ShowIconProperty, value); }
        }

        public static readonly DependencyProperty ShowTitleProperty = DependencyProperty.Register("ShowTitle", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool ShowTitle {
            get { return (bool)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }

        public static readonly DependencyProperty BottomBannerHeightProperty = DependencyProperty.Register("BottomBannerHeight", typeof(double), typeof(Window), new FrameworkPropertyMetadata(40.0));
        public double BottomBannerHeight {
            get { return (double)GetValue(BottomBannerHeightProperty); }
            set { SetValue(BottomBannerHeightProperty, value); }
        }

        public static readonly DependencyProperty EnableBottomBannerProperty = DependencyProperty.Register("EnableBottomBanner", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool EnableBottomBanner {
            get { return (bool)GetValue(EnableBottomBannerProperty); }
            set { SetValue(EnableBottomBannerProperty, value); }
        }

        public static readonly DependencyProperty ReserveTitleBarSpaceProperty = DependencyProperty.Register("ReserveTitleBarSpace", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool ReserveTitleBarSpace {
            get { return (bool)GetValue(ReserveTitleBarSpaceProperty); }
            set { SetValue(ReserveTitleBarSpaceProperty, value); }
        }

        public static readonly DependencyProperty DraggableProperty = DependencyProperty.Register("Draggable", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool Draggable {
            get { return (bool)GetValue(DraggableProperty); }
            set { SetValue(DraggableProperty, value); }
        }

        public static readonly DependencyProperty HideOnCloseProperty = DependencyProperty.Register("HideOnClose", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(false));
        public bool HideOnClose {
            get { return (bool)GetValue(HideOnCloseProperty); }
            set { SetValue(HideOnCloseProperty, value); }
        }

        public static readonly DependencyProperty EnableGlassEffectProperty = DependencyProperty.Register("EnableGlassEffect", typeof(bool), typeof(Window), new FrameworkPropertyMetadata(true));
        public bool EnableGlassEffect {
            get { return (bool)GetValue(EnableGlassEffectProperty); }
            set { SetValue(EnableGlassEffectProperty, value); }
        }
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

        public static bool IsGlassEffectAvailable => GetDwmAvailable();

        private void RegisterEvents() {
            SizeChanged += new SizeChangedEventHandler(WindowEx_SizeChanged);
            Loaded += new RoutedEventHandler(WindowEx_Loaded);
            MouseLeftButtonDown += new MouseButtonEventHandler(WindowEx_MouseLeftButtonDown);
        }

        private void UnregisterEvents() {
            SizeChanged -= WindowEx_SizeChanged;
            Loaded -= WindowEx_Loaded;
            MouseLeftButtonDown -= WindowEx_MouseLeftButtonDown;
        }

        public static T FindVisualChild<T>(DependencyObject parent, string childName = "")
            where T : DependencyObject {
            // Confirm parent and childName are valid. 
            if (parent == null) {
                return null;
            }

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++) {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null) {
                    // recursively drill down the tree
                    foundChild = FindVisualChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child. 
                    if (foundChild != null) break;
                } else if (!string.IsNullOrEmpty(childName)) {
                    // If the child's name is set for search
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName) {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                } else {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject {
            // get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            // we’ve reached the end of the tree
            if (parentObject == null) return null;

            // check if the parent matches the type we’re looking for
            if (parentObject is T parent) {
                return parent;
            } else {
                // use recursion to proceed with next level
                return FindVisualParent<T>(parentObject);
            }
        }

        private System.Windows.Controls.Button close = null;
        private System.Windows.Controls.Button min = null;
        private System.Windows.Controls.Button max = null;
        private Grid controlBox = null;
        private Border titleBarHitTester = null;
        private Grid body = null;

        private IntPtr blurRegionHandle = IntPtr.Zero;
        private const int HitTestCornerThreshold = 8;
        private const int HitTestBorderThreshold = 4;
        private WinApi.RECT clientRect = new WinApi.RECT();
        
        protected override void OnClosed(EventArgs e) {
            if (blurRegionHandle != IntPtr.Zero) {
                WinApi.DeleteObject(blurRegionHandle);
            }
            UnregisterEvents();
            if (min != null) {
                min.Click -= Min_Click;
                min = null;
            }
            if (max != null) {
                max.Click -= Max_Click;
                max = null;
            }
            if (close != null) {
                close.Click -= Close_Click;
                close = null;
            }
            controlBox = null;
            base.OnClosed(e);
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case (int)WinApi.Messages.WM_DWMCOMPOSITIONCHANGED:
                    if (IsGlassEffectAvailable && EnableGlassEffect) {
                        EnableBlur();
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
                        }
                        break;
                    }
                case (int)WinApi.Messages.WM_NCLBUTTONDBLCLK:
                    if (!Draggable || ControlBoxStyle != ControlBoxStyles.MinMaxClose) {
                        handled = true;
                    }
                    break;
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

        private static bool GetDwmAvailable() {
            try {
                WinApi.DwmIsCompositionEnabled(out bool enabled);
                return enabled;
            } catch {
                return false;
            }
        }

        private bool IsInTitleArea(int x, int y) {
            var point = new Point(x, y);
            var posTitle = TranslatePoint(point, titleBarHitTester);
            var posControlBox = TranslatePoint(point, controlBox);

            if (posTitle.X < 0 || posTitle.X > titleBarHitTester.ActualWidth || posTitle.Y < 0 || posTitle.Y > titleBarHitTester.ActualHeight) {
                return false;
            }

            if (posControlBox.X > 0 && posControlBox.X <= controlBox.ActualWidth && posControlBox.Y > 0 && posControlBox.Y <= controlBox.ActualHeight) {
                return false;
            }

            if (ReserveTitleBarSpace) {
                return true;
            }

            var posBody = TranslatePoint(point, body);
            if (posBody.X > 0 && posBody.X <= body.ActualWidth && posBody.Y > 0 && posBody.Y <= body.ActualHeight) {
                var result = VisualTreeHelper.HitTest(body, posBody);
                if (result == null) {
                    return false;
                }

                if (result.VisualHit == null) {
                    return true;
                }

                //if (!(result.VisualHit is Control)) {
                //    return true;
                //}
                
                if (FindVisualParent<ContentPresenter>(result.VisualHit) == null) {
                    return true;
                }

                return false;
            }

            return true;
        }

        private void WindowEx_SizeChanged(object sender, SizeChangedEventArgs e) {
            WinApi.GetClientRect(new WindowInteropHelper(this).Handle, out clientRect);
            if (IsLoaded && IsGlassEffectAvailable && EnableGlassEffect) {
                EnableBlur();
            }
        }

        private void WindowEx_Loaded(object sender, RoutedEventArgs e) {
            InitializeControlObjects();
            if (IsGlassEffectAvailable && EnableGlassEffect) {
                EnableBlur();
            }
            BringToFront();
        }

        private void EnableBlur() {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (WinApi.IsWindows10OrGreater) {
                WinApi.AccentPolicy accentPolicy = new WinApi.AccentPolicy() {
                    AccentState = WinApi.AccentState.ACCENT_ENABLE_BLURBEHIND | WinApi.AccentState.ACCENT_ENABLE_GRADIENT | WinApi.AccentState.ACCENT_TRANSPARENTGRADIENT,
                };
                int size = Marshal.SizeOf(accentPolicy);
                IntPtr accentPtr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(accentPolicy, accentPtr, false);
                WinApi.WindowCompositionAttributeData data = new WinApi.WindowCompositionAttributeData() {
                    Data = accentPtr,
                    SizeOfData = size,
                    Attribute = WinApi.WindowCompositionAttribute.WCA_ACCENT_POLICY
                };
                WinApi.SetWindowCompositionAttribute(handle, ref data);
                Marshal.FreeHGlobal(accentPtr);
            } else if (WinApi.IsWindowsVistaOrGreater) {
                Point pos = TranslatePoint(new Point(0, 0), this);
                int left = (int)pos.X;
                int top = (int)pos.Y;
                double w = Width;
                blurRegionHandle = WinApi.CreateRectRgn(left, top, clientRect.Width - left * 2, clientRect.Height - left * 2);

                WinApi.DWM_BLURBEHIND blur = new WinApi.DWM_BLURBEHIND() {
                    fEnable = true,
                    dwFlags = WinApi.DWM_BLURBEHIND.DWM_BB_ENABLE | WinApi.DWM_BLURBEHIND.DWM_BB_BLURREGION,
                    hRegionBlur = blurRegionHandle
                };
                WinApi.DwmEnableBlurBehindWindow(handle, blur);
                if (blurRegionHandle != IntPtr.Zero) {
                    WinApi.DeleteObject(blurRegionHandle);
                }
            }
        }

        private void InitializeControlObjects() {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(this);
            if (source != null) {
                source.AddHook(new HwndSourceHook(WndProc));
            }

            body = (Grid)Template.FindName("PART_Body", this);
            
            titleBarHitTester = (Border)Template.FindName("PART_TitleBarHitTester", this);
            if (titleBarHitTester == null) {
                throw new Exception("Cannot initialize object 'PART_TitleBarHitTester'");
            }

            controlBox = (Grid)Template.FindName("PART_ControlBox", this);
            if (controlBox == null) {
                throw new Exception("Cannot initialize object 'PART_ControlBox'");
            }

            close = (System.Windows.Controls.Button)Template.FindName("PART_CloseButton", this);
            if (close != null) {
                close.Click += new RoutedEventHandler(Close_Click);
            } else {
                throw new Exception("Cannot initialize object 'PART_CloseBox'");
            }

            max = (System.Windows.Controls.Button)Template.FindName("PART_MaxButton", this);
            if (max != null) {
                max.Click += new RoutedEventHandler(Max_Click);
            } else {
                throw new Exception("Cannot initialize object 'PART_MaxBox'");
            }

            min = (System.Windows.Controls.Button)Template.FindName("PART_MinButton", this);
            if (min != null) {
                min.Click += new RoutedEventHandler(Min_Click);
            } else {
                throw new Exception("Cannot initialize object 'PART_MinBox'");
            }
        }

        private void Min_Click(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Max_Click(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            } else {
                WindowState = WindowState.Maximized;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            if (HideOnClose) {
                Hide();
            } else {
                Close();
            }
        }

        private void WindowEx_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
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
