using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Channy.Controls2.Controls {
    public class ToggleButton : System.Windows.Controls.Primitives.ToggleButton {
        public ToggleButton() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ToggleButton") });
            Background = new SolidColorBrush(Color.FromRgb(82, 212, 104));
            BorderBrush = new SolidColorBrush(Color.FromRgb(65, 201, 85));
            FocusVisualStyle = null;
            Template = Resources["AnimatedToggleButton"] as ControlTemplate;
        }
    }
}
