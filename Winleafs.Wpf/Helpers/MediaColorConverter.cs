using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Winleafs.Wpf.Helpers
{
	public static class MediaColorConverter
	{
		public static uint ToRgb(Color color)
		{
			var rgb = (uint)(color.R << 16);
			rgb += (uint)(color.G << 8);
			rgb += (uint)color.B;

			return rgb;
		}

		public static Color FromRgb(uint argb)
		{
			var b = (byte)(argb & 255);
			var g = (byte)((argb >> 8) & 255);
			var r = (byte)((argb >> 16) & 255);

			return Color.FromArgb(255, r, g, b);
		}

	}
}
