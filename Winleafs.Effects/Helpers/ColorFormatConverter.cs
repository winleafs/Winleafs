using Winleafs.Models.Effects;

namespace Winleafs.Effects.Helpers
{
    public class ColorFormatConverter
    {
        public static uint ToRgb(System.Windows.Media.Color color)
        {
            var rgb = (uint)(color.R << 16);
            rgb += (uint)(color.G << 8);
            rgb += (uint)color.B;

            return rgb;
        }

        public static System.Windows.Media.Color ToMediaColor(uint argb)
        {
            var blue = (byte)(argb & 255);          // mask the lowest byte to get blue
            var green = (byte)((argb >> 8) & 255);  // shift 1 byte right then mask it to get green
            var red = (byte)((argb >> 16) & 255);   // shift 2 bytes right then mask it to get red

            return System.Windows.Media.Color.FromArgb(255, red, green, blue);
        }

        public static Palette ToPalette(uint Rgb)
        {
            double delta;
            double min;
            double hue = 0.0;
            double saturation;
            double brightness;

            var mediaColor = ToMediaColor(Rgb);
            min = Math.Min(Math.Min(mediaColor.R, mediaColor.G), mediaColor.B);
            brightness = Math.Max(Math.Max(mediaColor.R, mediaColor.G), mediaColor.B);
            delta = brightness - min;

            if (brightness == 0.0)
            {
                saturation = 0;
            }
            else
            {
                saturation = delta / brightness;
            }

            if (saturation == 0)
            {
                hue = 0.0;
            }

            else
            {
                if (mediaColor.R == brightness)
                {
                    hue = (mediaColor.G - mediaColor.B) / delta;
                }
                else if (mediaColor.G == brightness)
                {
                    hue = 2 + ((mediaColor.B - mediaColor.R) / delta);
                }
                else if (mediaColor.B == brightness)
                {
                    hue = 4 + (mediaColor.R - mediaColor.G) / delta;
                }

                hue *= 60;

                if (hue < 0.0)
                {
                    hue += 360;
                }
            }

            return new Palette
            {
                Hue = (int)Math.Floor(hue),
                Saturation = (int)Math.Floor(saturation),
                Brightness = (int)Math.Floor(brightness / 255)
            };
        }
    }
}
