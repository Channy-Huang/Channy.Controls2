using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Channy.Controls2.Controls {
    public class ToolTip : System.Windows.Controls.ToolTip {
        public ToolTip() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ToolTip") });
            MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(ToolTipEx_MouseLeftButtonUp);
            Loaded += new RoutedEventHandler(ToolTipEx_Loaded);
            timer.Tick += Timer_Tick;
            InitializeStyle();
        }

        public void ShowToolTip(FrameworkElement target, string message, double? maxWidth = null) {
            PlacementTarget = target;
            Content = message;

            if (maxWidth == null) {
                MaxWidth = target.ActualWidth;
            } else {
                MaxWidth = maxWidth.Value;
            }
            IsOpen = true;

            target.Focus();
        }

        private void Timer_Tick(object sender, EventArgs e) {
            IsOpen = false;
        }

        public static readonly DependencyProperty DurationProperty = DependencyProperty.Register("Duration", typeof(TimeSpan), typeof(ToolTip), new PropertyMetadata(TimeSpan.FromSeconds(5)));
        public TimeSpan Duration {
            get { return (TimeSpan)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        protected override void OnOpened(RoutedEventArgs e) {
            timer.Interval = Duration;
            timer.Start();
            base.OnOpened(e);
        }

        protected override void OnClosed(RoutedEventArgs e) {
            timer.Stop();
            base.OnClosed(e);
        }

        void ToolTipEx_Loaded(object sender, RoutedEventArgs e) {
            HwndSource source = (HwndSource)PresentationSource.FromVisual(Window.GetWindow(this));
            if (source != null) {
                source.AddHook(new HwndSourceHook(WndProc));
            }
        }

        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
            switch (msg) {
                case (int)WinApi.Messages.WM_LBUTTONDOWN:
                    if (IsOpen) {
                        IsOpen = false;
                        //handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        void ToolTipEx_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            IsOpen = false;
        }

        private Image icon = null;
        private ToolTipStyles toolTipStyle = ToolTipStyles.Error;

        public enum ToolTipStyles {
            Alert,
            Error,
            None
        }

        public ToolTipStyles ToolTipStyle {
            get { return toolTipStyle; }
            set {
                toolTipStyle = value;
                if (toolTipStyle == ToolTipStyles.Alert) {
                    icon.Source = new BitmapImage(new Uri(@"pack://application:,,,/Channy.Controls2;component/Images/alert.png"));
                } else if (toolTipStyle == ToolTipStyles.Error) {
                    icon.Source = new BitmapImage(new Uri(@"pack://application:,,,/Channy.Controls2;component/Images/error2.png"));
                } else if (toolTipStyle == ToolTipStyles.None) {
                    icon.Source = null;
                }
            }
        }

        public override void OnApplyTemplate() {
            base.OnApplyTemplate();

            icon = (Image)Template.FindName("icon", this);
            if (icon == null) {
                throw new Exception("Cannot initialize object 'icon'");
            }
            //isInitializationComplete = true;
        }

        private void InitializeStyle() {
            Style = (Style)Resources["ToolTipStyle"];
            ApplyTemplate();
        }

        private DispatcherTimer timer = new DispatcherTimer();
    }
}
