using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Channy.Controls2.Controls {
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window {
        public MessageBox() {
            InitializeComponent();
        }

        public static readonly DependencyProperty MessageBoxTypeProperty = DependencyProperty.Register("Type", typeof(MessageBoxType), typeof(MessageBox), new FrameworkPropertyMetadata(MessageBoxType.Info));
        public MessageBoxType Type {
            get { return (MessageBoxType)GetValue(MessageBoxTypeProperty); }
            set { SetValue(MessageBoxTypeProperty, value); }
        }

        public enum MessageBoxStyle { None, OK, YesNo }

        public enum MessageBoxType { None, Info, Error, Question, Success }

        private bool isModal = false;

        private void SetMessageBoxStyle(MessageBoxStyle style) {
            if (style == MessageBoxStyle.None) {
                BottomBanner.Height = 15;
                OK.Visibility = Visibility.Hidden;
                Yes.Visibility = Visibility.Hidden;
                No.Visibility = Visibility.Hidden;
            } else if (style == MessageBoxStyle.OK) {
                OK.Visibility = Visibility.Visible;
                OK.Focus();
                Yes.Visibility = Visibility.Hidden;
                No.Visibility = Visibility.Hidden;
            } else if (style == MessageBoxStyle.YesNo) {
                OK.Visibility = Visibility.Hidden;
                Yes.Visibility = Visibility.Visible;
                Yes.Focus();
                No.Visibility = Visibility.Visible;
            }
        }

        private void SetMessageBoxType(MessageBoxType type) {
            ImageSourceConverter converter = new ImageSourceConverter();
            if (type == MessageBoxType.Error) {
                Picture.Source = new BitmapImage(new Uri("pack://application:,,,/Channy.Controls2;component/Images/error.png"));
            } else if (type == MessageBoxType.Info) {
                Picture.Source = new BitmapImage(new Uri("pack://application:,,,/Channy.Controls2;component/Images/info.png"));
            } else if (type == MessageBoxType.Question) {
                Picture.Source = new BitmapImage(new Uri("pack://application:,,,/Channy.Controls2;component/Images/question.png"));
            } else if (type == MessageBoxType.Success) {
                Picture.Source = new BitmapImage(new Uri("pack://application:,,,/Channy.Controls2;component/Images/success.png"));
            }
        }

        #region Modal dialog
        public static bool? ShowDialog(System.Windows.Window parent, string message) {
            return ShowDialog(parent, message, "");
        }

        public static bool? ShowDialog(System.Windows.Window parent, string message, string title) {
            return ShowDialog(parent, message, title, MessageBoxType.None);
        }

        //public static bool? ShowDialog(System.Windows.Window parent, string message, string title, MessageBoxType type) {
        //    return ShowDialog(parent, message, title, null, MessageBoxStyle.OK, type);
        //}

        public static bool? ShowDialog(System.Windows.Window parent, string message, string title, ImageSource icon, MessageBoxStyle style, MessageBoxType type, Style buttonStyle = null) {
            MessageBox messageBox = new MessageBox() {
                isModal = true,
                Owner = parent
            };
            messageBox.Message.Text = message;
            messageBox.SetMessageBoxStyle(style);
            messageBox.Title = title;
            messageBox.Icon = icon;
            if (parent != null) {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                messageBox.Background = parent.Background;
                messageBox.FontSize = parent.FontSize;
                if (parent is Window window) {
                    messageBox.BaseBackground = window.BaseBackground;
                }
            } else {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            messageBox.SetMessageBoxType(type);
            if (buttonStyle != null) {
                messageBox.OK.Style = buttonStyle;
                messageBox.Yes.Style = buttonStyle;
                messageBox.No.Style = buttonStyle;
            }
            return messageBox.ShowDialog();
        }

        public static bool? ShowDialog(System.Windows.Window parent, string message, string title, MessageBoxType type) {
            MessageBox messageBox = new MessageBox() {
                isModal = true,
                Owner = parent,
                Type = type,
                Title = title
            };
            messageBox.Message.Text = message;
            if (parent != null) {
                //messageBox.Icon = parent.Icon;
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                messageBox.Background = parent.Background;
                messageBox.FontSize = parent.FontSize;
                if (parent is Window window) {
                    messageBox.BaseBackground = window.BaseBackground;
                    messageBox.EnableGlassEffect = window.EnableGlassEffect;
                }
            } else {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            return messageBox.ShowDialog();
        }
        #endregion

        #region Non-modal dialog
        public static void Show(System.Windows.Window parent, string message) {
            Show(parent, message, "", MessageBoxType.None);
        }

        public static void Show(System.Windows.Window parent, string message, string title, MessageBoxType type) {
            Show(parent, message, title, null, MessageBoxStyle.OK, type);
        }

        public static void Show(System.Windows.Window parent, string message, string title, ImageSource icon, MessageBoxStyle style, MessageBoxType type, Style buttonStyle = null) {
            MessageBox messageBox = new MessageBox() {
                Owner = parent,
            };

            messageBox.Message.Text = message;
            messageBox.SetMessageBoxStyle(style);
            messageBox.Title = title;
            messageBox.Icon = icon;
            if (parent != null) {
                messageBox.FontSize = parent.FontSize;
                messageBox.Background = parent.Background;
                if (parent is Window window) {
                    messageBox.BaseBackground = window.BaseBackground;
                }
            } else {
                messageBox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }
            messageBox.SetMessageBoxType(type);

            if (buttonStyle != null) {
                messageBox.OK.Style = buttonStyle;
                messageBox.Yes.Style = buttonStyle;
                messageBox.No.Style = buttonStyle;
            }
            messageBox.Show();
        }

        public static void Show(System.Windows.Window parent, string message, string title, ImageSource icon, MessageBoxStyle style, MessageBoxType type, double left, double top, Style buttonStyle = null) {
            MessageBox messageBox = new MessageBox() {
                Owner = parent,
            };

            messageBox.Message.Text = message;
            messageBox.SetMessageBoxStyle(style);
            messageBox.Title = title;
            messageBox.Icon = icon;
            messageBox.Left = left;
            messageBox.Top = top;
            if (parent != null) {
                messageBox.FontSize = parent.FontSize;
                messageBox.Background = parent.Background;
                if (parent is Window window) {
                    messageBox.BaseBackground = window.BaseBackground;
                }
            }
            messageBox.SetMessageBoxType(type);

            if (buttonStyle != null) {
                messageBox.OK.Style = buttonStyle;
                messageBox.Yes.Style = buttonStyle;
                messageBox.No.Style = buttonStyle;
            }
            messageBox.Show();
        }
        #endregion

        private void OK_Click(object sender, RoutedEventArgs e) {
            if (isModal) {
                DialogResult = true;
            }
            Close();
        }
    }
}
