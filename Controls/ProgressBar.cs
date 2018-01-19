using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Channy.Controls2.Controls {
    public class ProgressBar : System.Windows.Controls.ProgressBar {
        public ProgressBar() {
            Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = Common.GetResourceUri("Channy/ProgressBar") });
            InitializeStyle();
        }

        private void InitializeStyle() {
            Style = (Style)Resources["ProgressBarStyle"];
            ApplyTemplate();
        }
    }
}
