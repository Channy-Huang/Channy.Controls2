using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Channy.Controls2 {
    internal class Common {
        public static Uri GetResourceUri(string theme) {
            return new Uri(string.Format("/Channy.Controls2;component/Themes/{0}.xaml", theme), UriKind.Relative);
        }
    }
}
