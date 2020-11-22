using System.Collections.Generic;
using Unity.UIWidgets.material;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using UnityEngine;
using Color = Unity.UIWidgets.ui.Color;

namespace UIWidgets.AddOns
{
    public class Utils
    {
        public static bool useWhiteForeground(Color color, float bias = 1f)
        {
            int v = Mathf.Sqrt(Mathf.Pow(color.red, 2) * 0.299f +
                    Mathf.Pow(color.green, 2) * 0.587f +
                    Mathf.Pow(color.blue, 2) * 0.114f)
                .round();
            return v < 130f * bias ? true : false;
        }

        public static HSLColor hsvToHsl(HSVColor color)
        {
            float s = 0f;
            float l = 0f;
            l = (2 - color.saturation) * color.value / 2;
            if (l != 0)
            {
                if (l == 1)
                    s = 0f;
                else if (l < 0.5f)
                    s = color.saturation * color.value / (l * 2);
                else
                    s = color.saturation * color.value / (2 - l * 2);
            }
            return new HSLColor(
              color.alpha,
              color.hue,
              s.clamp(0f, 1f),
              l.clamp(0f, 1f)
            );
        }

        /// reference: https://en.wikipedia.org/wiki/HSL_and_HSV#HSL_to_HSV
        public static HSVColor hslToHsv(HSLColor color)
        {
            float s = 0f;
            float v = 0f;

            v = color.lightness +
                color.saturation *
                    (color.lightness < 0.5 ? color.lightness : 1 - color.lightness);
            if (v != 0) s = 2 - 2 * color.lightness / v;

            return HSVColor.fromAHSV(
              color.alpha,
              color.hue,
              s.clamp(0f, 1f),
              v.clamp(0f, 1f)
            );
        }

        public static List<Color> _defaultColors = new List<Color>
        {
          Colors.deepPurple,
          Colors.blue,
          Colors.blue,
          Colors.lightBlue,
          Colors.cyan,
          Colors.teal,
          Colors.green,
          Colors.lightGreen,
          Colors.lime,
          Colors.yellow,
          Colors.amber,
          Colors.orange,
          Colors.deepOrange,
          Colors.brown,
          Colors.grey,
          Colors.blueGrey,
          Colors.black
        };
    }
}