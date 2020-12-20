using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Timers;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Helpers
{
    public static class ScreenGrabber
    {
        private static readonly Timer _timer;
        private static readonly Rectangle _screenBounds;

        private static int _bitsPerPixel;
        private static int _bitmapStride;
        private static byte[] _pixels;
        private static object _lockObject;

        static ScreenGrabber()
        {
            var timerRefreshRate = 1000;

            if (UserSettings.Settings.ScreenMirrorRefreshRatePerSecond > 0 && UserSettings.Settings.ScreenMirrorRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.ScreenMirrorRefreshRatePerSecond;
            }

            _timer = new Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            _screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);

            _lockObject = new object();
        }

        private static void CaptureScreen()
        {
            lock (_lockObject)
            {
                //Create a new bitmap.
                var bitmap = new Bitmap(_screenBounds.Width,
                                               _screenBounds.Height,
                                               PixelFormat.Format32bppArgb);

                // Create a graphics object from the bitmap.
                var gfxScreenshot = Graphics.FromImage(bitmap);

                // Take the screenshot from the upper left corner to the right bottom corner.
                gfxScreenshot.CopyFromScreen(_screenBounds.X, _screenBounds.Y,
                                            0, 0,
                                            new Size(_screenBounds.Width, _screenBounds.Height),
                                            CopyPixelOperation.SourceCopy);

                //Copy the bit map to a memory safe byte array and set all needed variables
                var bitmapData = bitmap.LockBits(new Rectangle(0, 0, _screenBounds.Width, _screenBounds.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);

                _bitmapStride = bitmapData.Stride;
                _bitsPerPixel = bitmap.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
                _pixels = new byte[Math.Abs(bitmapData.Stride * bitmapData.Height)];
                Marshal.Copy(bitmapData.Scan0, _pixels, 0, _pixels.Length);

                //Free memory
                bitmap.UnlockBits(bitmapData);
            }            
        }

        /// <summary>
        /// Calculates the average color for each of the given <paramref name="areasToCapture"/>.
        /// Use <param name="minDiversion"> to drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
        /// </summary>
        public static List<Color> CalculateAverageColor(IEnumerable<Rectangle> areasToCapture, int minDiversion = 50)
        {
            if (_pixels == null)
            {
                //This can happen when before the first screen shot is taken when the effect is enabled
                return null;
            }

            var colors = new List<Color>();

            lock (_lockObject)
            {
                foreach (var area in areasToCapture)
                {
                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    int index = 0;
                    int dropped = 0; // keep track of dropped pixels
                    long totalRed = 0, totalGreen = 0, totalBlue = 0;

                    for (int y = area.Y; y < area.Y + area.Height; y++)
                    {
                        for (int x = area.X; x < area.X + area.Width; x++)
                        {
                            index = (y * _bitmapStride) + x * _bitsPerPixel; //Find the location of the byte of the current pixel that is being analyzed
                            blue = _pixels[index];
                            green = _pixels[index + 1];
                            red = _pixels[index + 2];

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

                    int count = area.Width * area.Height - dropped;
                    count++; //We increase count by 1 to make sure it is not 0. The difference in color when increasing count by 1 is neglectable

                    colors.Add(Color.FromArgb((int)(totalRed / count), (int)(totalGreen / count), (int)(totalBlue / count)));
                }
            }            

            return colors;
        }

        public static void Start()
        {
            _timer.Start();
        }

        public static void Stop()
        {
            //Force a garbage collection to clean up the last bitmaps
            GC.Collect();

            _timer.Stop();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CaptureScreen();
        }
    }
}
