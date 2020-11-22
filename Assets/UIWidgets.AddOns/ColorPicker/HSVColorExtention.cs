using System;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;

namespace UIWidgets.AddOns
{
    public class HSVColorExtention
    {
        public static HSVColor fromColor(Color color)
        {
            double red = (double)color.red / 0xFF;
            double green = (double)color.green / 0xFF;
            double blue = (double)color.blue / 0xFF;

            double max = Math.Max(red, Math.Max(green, blue));
            double min = Math.Min(red, Math.Min(green, blue));
            double delta = max - min;

            double alpha = (double)color.alpha / 0xFF;
            double hue = _getHue(red, green, blue, max, delta);
            double saturation = max == 0.0 ? 0.0 : delta / max;

            return HSVColor.fromAHSV((float)alpha, (float)hue, (float)saturation, (float)max);
        }

        static double _getHue(double red, double green, double blue, double max, double delta)
        {
            double hue = double.NaN;
            if (max == 0.0)
            {
                hue = 0.0;
            }
            else if (max == red)
            {
                hue = 60.0 * (((green - blue) / delta) % 6);
            }
            else if (max == green)
            {
                hue = 60.0 * (((blue - red) / delta) + 2);
            }
            else if (max == blue)
            {
                hue = 60.0 * (((red - green) / delta) + 4);
            }

            /// Set hue to 0.0 when red == green == blue.
            hue = double.IsNaN(hue) ? 0.0 : hue;
            return hue;
        }
    }
}