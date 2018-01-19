using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Channy.Controls2.Controls
{
    public class Button : System.Windows.Controls.Button {
        public Button() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/Button") });
            InitializeStyle();
        }

        ~Button() {
            Dispose(false);
        }

        public enum RoundCorner {
            Left,
            Top,
            Right,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            All,
            None
        }

        public string Caption {
            get {
                if (isInitializationComplete) {
                    return caption.Text;
                } else {
                    return "";
                }
            }
            set {
                if (isInitializationComplete) {
                    if (value != null) {
                        caption.Text = value;
                    } else {
                        caption.Text = "";
                    }
                }
            }
        }

        public ImageSource Icon {
            get { return icon.Source; }
            set {
                if (icon != null) {
                    icon.Source = value;
                }
            }
        }

        public Thickness IconMargin {
            get { return icon.Margin; }
            set {
                if (icon != null) {
                    icon.Margin = value;
                }
            }
        }

        public RoundCorner Corner {
            get { return corner; }
            set {
                corner = value;
                if (isInitializationComplete) {
                    switch (corner) {
                        case RoundCorner.All:
                            border.CornerRadius = new CornerRadius(radius);
                            break;
                        case RoundCorner.Bottom:
                            border.CornerRadius = new CornerRadius(0, 0, radius, radius);
                            break;
                        case RoundCorner.BottomLeft:
                            border.CornerRadius = new CornerRadius(0, 0, 0, radius);
                            break;
                        case RoundCorner.BottomRight:
                            border.CornerRadius = new CornerRadius(0, 0, radius, 0);
                            break;
                        case RoundCorner.Left:
                            border.CornerRadius = new CornerRadius(radius, 0, 0, radius);
                            break;
                        case RoundCorner.None:
                            border.CornerRadius = new CornerRadius(0);
                            break;
                        case RoundCorner.Right:
                            border.CornerRadius = new CornerRadius(0, radius, radius, 0);
                            break;
                        case RoundCorner.Top:
                            border.CornerRadius = new CornerRadius(radius, radius, 0, 0);
                            break;
                        case RoundCorner.TopLeft:
                            border.CornerRadius = new CornerRadius(radius, 0, 0, 0);
                            break;
                        case RoundCorner.TopRight:
                            border.CornerRadius = new CornerRadius(0, radius, 0, 0);
                            break;
                    }
                }
            }
        }

        private bool disposed = false;
        private TextBlock caption = null;
        private int radius = 4;
        private Border border = null;
        private Image icon = null;
        private RoundCorner corner = RoundCorner.None;

        private bool isInitializationComplete = false;

        public override void OnApplyTemplate() {
            caption = (TextBlock)Template.FindName("Caption", this);
            if (caption == null) {
                throw new Exception("Cannot initialize object 'Caption'");
            }

            border = (Border)Template.FindName("Border", this);
            if (border == null) {
                throw new Exception("Cannot initialize object 'Border'");
            }

            icon = (Image)Template.FindName("Icon", this);
            if (icon == null) {
                throw new Exception("Cannot initialize object 'Icon'");
            }
            isInitializationComplete = true;
            base.OnApplyTemplate();
        }

        private void InitializeStyle() {
            Style = (Style)Resources["ButtonStyle"];
            ApplyTemplate();
        }

        public void Dispose() {
            Dispose(true);
        }

        protected void Dispose(bool disposing) {
            if (!disposed) {
                caption = null;
            }
            disposed = true;
        }
    }
}
