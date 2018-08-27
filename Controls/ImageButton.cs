using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;

namespace Channy.Controls2.Controls {
    public class ImageButton : System.Windows.Controls.Button {
        public ImageButton() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ImageButton") });
            Template = (ControlTemplate)Resources["ImageButtonTemplate"];
        }

        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty MouseOverImageProperty = DependencyProperty.Register("MouseOverImage", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty ClickImageProperty = DependencyProperty.Register("ClickImage", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));
        public static readonly DependencyProperty DisabledImageProperty = DependencyProperty.Register("DisabledImage", typeof(ImageSource), typeof(ImageButton), new FrameworkPropertyMetadata(null));

        public ImageSource Image {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public ImageSource MouseOverImage {
            get { return (ImageSource)GetValue(MouseOverImageProperty); }
            set { SetValue(MouseOverImageProperty, value); }
        }

        public ImageSource ClickImage {
            get { return (ImageSource)GetValue(ClickImageProperty); }
            set { SetValue(ClickImageProperty, value); }
        }

        public ImageSource DisabledImage {
            get { return (ImageSource)GetValue(DisabledImageProperty); }
            set { SetValue(DisabledImageProperty, value); }
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
    }
}
