using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Channy.Controls2.Controls {
    public class ImageButton : System.Windows.Controls.Button {
        //public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnImageChanged)));
        //public static readonly DependencyProperty MouseOverImageProperty = DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(ImageButton));
        //public static readonly DependencyProperty ClickImageProperty = DependencyProperty.Register("ClickImage", typeof(ImageSource), typeof(ImageButton));

        public ImageSource Image {
            get {
                //return (ImageSource)GetValue(ImageProperty);
                return image.Source;
            }
            set {
                //SetValue(ImageProperty, value);
                image.Source = value;
            }
        }

        public ImageSource MouseOverImage {
            get {
                return mouseHoverImage.Source;
                //return (ImageSource)GetValue(MouseOverImageProperty);
            }
            set {
                //SetValue(MouseOverImageProperty, value);
                mouseHoverImage.Source = value;
            }
        }

        public ImageSource ClickImage {
            get {
                return clickImage.Source;
                //return (ImageSource)GetValue(ClickImageProperty);
            }
            set {
                //SetValue(ClickImageProperty, value);
                clickImage.Source = value;
            }
        }

        public ImageSource DisabledImage {
            get {
                return disabledImage.Source;
            }
            set {
                disabledImage.Source = value;
            }
        }

        public ImageButton() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ImageButton") });
            InitializeStyle();
        }

        private Image image;
        private Image mouseHoverImage;
        private Image clickImage;
        private Image disabledImage;

        public override void OnApplyTemplate() {
            image = (Image)Template.FindName("Normal", this);
            if (image == null) {
                throw new Exception("Cannot initialize object 'Normal'");
            }

            mouseHoverImage = (Image)Template.FindName("MouseOver", this);
            if (mouseHoverImage == null) {
                throw new Exception("Cannot initialize object 'MouseOver'");
            }

            clickImage = (Image)Template.FindName("MouseClick", this);
            if (clickImage == null) {
                throw new Exception("Cannot initialize object 'MouseClick'");
            }

            disabledImage = (Image)Template.FindName("Disabled", this);
            if (disabledImage == null) {
                throw new Exception("Cannot initialize object 'Disabled'");
            }

            base.OnApplyTemplate();
        }

        private void InitializeStyle() {
            Style = (Style)Resources["ImageButtonStyle"];
            ApplyTemplate();
        }
    }
}
