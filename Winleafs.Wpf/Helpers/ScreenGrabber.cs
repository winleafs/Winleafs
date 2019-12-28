using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Helpers
{
    public static class ScreenGrabber
    {
        private static readonly Timer _timer;
        private static readonly Rectangle _screenBounds;

        private static BitmapData _bitmapData;
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
            //Note: these objects need to be initialized new everytime

            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(_screenBounds.Width,
                                           _screenBounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(_screenBounds.X, _screenBounds.Y,
                                        0, 0,
                                        new Size(_screenBounds.Width, _screenBounds.Height),
                                        CopyPixelOperation.SourceCopy);

            lock (_lockObject)
            {
                _bitmapData = bmpScreenshot.LockBits(new Rectangle(0, 0, _screenBounds.Width, _screenBounds.Height), ImageLockMode.ReadOnly, bmpScreenshot.PixelFormat);
            }            
        }

        /// <summary>
        /// Calculates the average color from the given bitmap
        /// Use <param name="minDiversion"> to drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
        /// </summary>
        public static List<Color> CalculateAverageColor(List<Rectangle> areasToCapture, int minDiversion = 50)
        {
            var colors = new List<Color>();
            int bppModifier = 4; //bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            lock (_lockObject)
            {
                int stride = _bitmapData.Stride;
                IntPtr Scan0 = _bitmapData.Scan0;

                unsafe
                {
                    byte* p = (byte*)(void*)Scan0;

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
                            for (int x = 0; x < area.X + area.Width; x++)
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

                        int count = area.Width * area.Height - dropped;
                        count++; //We increase count by 1 to make sure it is not 0. The difference in color when increasing count by 1 is neglectable

                        colors.Add(Color.FromArgb((int)(totalRed / count), (int)(totalGreen / count), (int)(totalBlue / count)));
                    }
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
            _timer.Stop();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            CaptureScreen();
        }
    }
}
