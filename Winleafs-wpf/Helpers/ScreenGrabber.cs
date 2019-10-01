using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Winleafs.Wpf.Helpers
{
    public class ScreenGrabber
    {
        public static Bitmap CaptureScreen(Rectangle screenBounds)
        {
            //Note: these objects need to be initialized new everytime

            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(screenBounds.Width,
                                           screenBounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(screenBounds.X,
                                        screenBounds.Y,
                                        0,
                                        0,
                                        new Size(screenBounds.Width, screenBounds.Height),
                                        CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }

        /// <summary>
        /// Calculates the average color from the given bitmap
        /// Use <param name="minDiversion"> to drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
        /// </summary>
        /// <param name="bitmap">The input bitmap</param>
        /// <param name="screenBounds">Rectangle starting at 0, 0 and of equal height and width as the bitmap</param>
        /// <param name="minDiversion">Drop pixels that do not differ by at least minDiversion between color values (white, gray or black)</param>
        public static Color CalculateAverageColor(Bitmap bitmap, Rectangle screenBounds, int minDiversion = 50)
        {
            //Initialize bitmap data
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, screenBounds.Width, screenBounds.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

            //Variable initialization all outside the loop, since initializing is slower than allocating
            int red = 0;
            int green = 0;
            int blue = 0;
            int index = 0;
            int dropped = 0; // keep track of dropped pixels
            long totalRed = 0, totalGreen = 0, totalBlue = 0;
            int bppModifier = 4; //bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            int stride = bitmapData.Stride;
            IntPtr Scan0 = bitmapData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < screenBounds.Height; y++)
                {
                    for (int x = 0; x < screenBounds.Width; x++)
                    {
                        index = (y * stride) + x * bppModifier;
                        red = p[index + 2];
                        green = p[index + 1];
                        blue = p[index];

                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totalRed += red;
                            totalGreen += green;
                            totalBlue += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            bitmap.Dispose();

            int count = screenBounds.Width * screenBounds.Height - dropped;
            count++; //We increase count by 1 to make sure it is not 0. The difference in color when increasing count by 1 is neglectable

            return Color.FromArgb((int)(totalRed / count), (int)(totalGreen / count), (int)(totalBlue / count));
        }
    }
}
