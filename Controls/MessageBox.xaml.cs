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
            Loaded += MessageBox_Loaded;
        }

        private void MessageBox_Loaded(object sender, RoutedEventArgs e) {
            BottomBannerHeight = BottomBanner.ActualHeight + 10;
        }

        public enum MessageBoxStyle { Simple, OK, YesNo }

        public enum MessageBoxType { None, Info, Error, Question, Success }

        private bool isDialog = false;

        private void SetMessageBoxStyle(MessageBoxStyle style) {
            if (style == MessageBoxStyle.Simple) {
                BottomBannerHeight = 15;
                GridLengthConverter converter = new GridLengthConverter();
                BottomBanner.Height = (GridLength)converter.ConvertFromString("15");
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

        public static bool? ShowDialog(System.Windows.Window parent, string message, string title, MessageBoxType type) {
            return ShowDialog(parent, message, title, null, MessageBoxStyle.OK, type);
        }

        public static bool? ShowDialog(System.Windows.Window parent, string message, string title, ImageSource icon, MessageBoxStyle style, MessageBoxType type, Style buttonStyle = null) {
            MessageBox messageBox = new MessageBox() {
                isDialog = true,
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
                    messageBox.BaseColor = window.BaseColor;
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
                    messageBox.BaseColor = window.BaseColor;
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
                    messageBox.BaseColor = window.BaseColor;
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

        private void OK_Click(object sender, RoutedEventArgs e) {
            if (isDialog) {
                DialogResult = true;
            }
            Close();
        }
        #endregion
    }
}
