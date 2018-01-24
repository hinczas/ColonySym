using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static Color Increment(this Color col)
        {
            int r = col.R;
            int g = col.G;
            int b = col.B;

            r = r >= 255 ? 255 : r + 1;
            g = g >= 255 ? 255 : g + 1;
            b = b >= 255 ? 255 : b + 1;

            Color tmp = Color.FromArgb(255, r, g, b);
            return tmp;
        }
        public static Color Decrease(this Color col)
        {
            int r = col.R;
            int g = col.G;
            int b = col.B;

            r = r <= 0 ? 0 : r - 1;
            g = g <= 0 ? 0 : g - 1;
            b = b <= 0 ? 0 : b - 1;

            Color tmp = Color.FromArgb(255, r, g, b);
            return tmp;
        }
    }
}
