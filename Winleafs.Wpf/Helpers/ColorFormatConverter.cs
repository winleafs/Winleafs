using System;
using System.Drawing;
using Winleafs.Models.Models.Effects;

namespace Winleafs.Wpf.Helpers
{
    public static class ColorFormatConverter
    {
        public static System.Windows.Media.Color ToMediaColor(float hue, float saturation, float brightness)
        {
            var color = ToDrawingColor(hue, saturation, brightness);
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }

        /// <summary>
        /// Converts HSB values to Drawing Color object
        /// </summary>
        /// <param name="hue">Hue between 0 and 360</param>
        /// <param name="saturation">Saturation between 0 and 100</param>
        /// <param name="brightness">Brightness between 0 and 100</param>
        /// Source: http://www.splinter.com.au/converting-hsv-to-rgb-colour-using-c/
        public static Color ToDrawingColor(float hue, float saturation, float brightness)
        {
            //Normalize input
            saturation = saturation / 100;
            brightness = brightness / 100;

            double R, G, B;
            if (brightness <= 0)
            {
                R = G = B = 0;
            }
            else if (saturation <= 0)
            {
                R = G = B = brightness;
            }
            else
            {
                double hf = hue / 60.0;
                int i = (int)Math.Floor(hf);
                double f = hf - i;
                double pv = brightness * (1 - saturation);
                double qv = brightness * (1 - saturation * f);
                double tv = brightness * (1 - saturation * (1 - f));
                switch (i)
                {

                    // Red is the dominant color

                    case 0:
                    // Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.
                    case 6:
                    case -1:
                        R = brightness;
                        G = tv;
                        B = pv;
                        break;

                    // Green is the dominant color
                    case 1:
                        R = qv;
                        G = brightness;
                        B = pv;
                        break;
                    case 2:
                        R = pv;
                        G = brightness;
                        B = tv;
                        break;

                    // Blue is the dominant color
                    case 3:
                        R = pv;
                        G = qv;
                        B = brightness;
                        break;
                    case 4:
                        R = tv;
                        G = pv;
                        B = brightness;
                        break;

                    // Red is the dominant color
                    case 5:
                        R = brightness;
                        G = pv;
                        B = qv;
                        break;

                    // The color is not defined, we should throw an error.
                    default:
                        R = G = B = brightness; // Just pretend its black/white
                        break;
                }
            }

            var r = Clamp((int)(R * 255.0));
            var g = Clamp((int)(G * 255.0));
            var b = Clamp((int)(B * 255.0));
            return Color.FromArgb(255, r, g, b);
        }

        private static int Clamp(int i)
        {
            if (i < 0) return 0;
            if (i > 255) return 255;
            return i;
        }

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
