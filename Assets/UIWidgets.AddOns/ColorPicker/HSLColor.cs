using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;

namespace UIWidgets.AddOns
{
    public class HSLColor
    {
        HSVColor col;
        public float alpha;
        public float hue;
        public float lightness;
        public float saturation;

        public static HSLColor fromAHSL(float alpha, float hue, float saturation, float lightness)
        {
            return new HSLColor(alpha, hue, saturation, lightness);
        }

        public HSLColor(float alpha, float hue, float saturation, float lightness)
        {
            //fromAHSL
            this.alpha = alpha;
            this.hue = hue;
            this.lightness = lightness;
            this.saturation = saturation;
        }

        public Color toColor()
        {
            //未実装
            float chroma = this.saturation * this.lightness;
            float secondary = chroma * (1.0f - (((this.hue / 60.0f) % 2.0f) - 1.0f).abs());
            float match = this.lightness - chroma;
            //ColorUtils
            //return ColorUtils._colorFromHue(this.alpha, this.hue, chroma, secondary, match);

            //return ColorUtils._colorFromHue(this.alpha, this.hue, chroma, secondary, match);
            //new ColorUtils()
            throw new System.NotImplementedException();

        }

        public HSLColor withSaturation(float saturation)
        {
            return HSLColor.fromAHSL(this.alpha, this.hue, saturation, this.lightness);
        }
        public HSLColor withLightness(float lightness)
        {
            return HSLColor.fromAHSL(this.alpha, this.hue, this.saturation, lightness);
        }

    }
}