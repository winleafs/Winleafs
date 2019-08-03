using NLog;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Timers;
using System.Windows.Forms;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Helpers
{
    public class ScreenGrabber
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static object _colorLockObject = new object();
        private static object _bitmapLockObject = new object();

        private static System.Timers.Timer _timer;
        private static Rectangle _screenBounds;

        private static Bitmap _bitmap;
        private static BitmapData _bitmapData;
        private static readonly Color _whiteColor = Color.FromArgb(System.Windows.Media.Brushes.White.Color.A, System.Windows.Media.Brushes.White.Color.R, System.Windows.Media.Brushes.White.Color.G, System.Windows.Media.Brushes.White.Color.B);

        public ScreenGrabber(Device device)
        {
            var timerRefreshRate = 1000;

            if (device.ScreenMirrorRefreshRatePerSecond > 0 && device.ScreenMirrorRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / device.ScreenMirrorRefreshRatePerSecond;
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            _screenBounds = MonitorHelper.GetScreenBounds(device.ScreenMirrorMonitorIndex);
        }

        private static Bitmap CaptureScreen()
        {
            //Note: these objects need to be initialized new everytime

            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(_screenBounds.Width,
                                           _screenBounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(_screenBounds.X,
                                        _screenBounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            return bmpScreenshot;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            try
            {
                SetBitmap(CaptureScreen());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during screen grabbing");
            }
        }

        /// We do not use readerwriterlock since we most likely only have a few readers and the writer is more important, but readerwriterlock gives priority to readers
        /// Instead, we just use a lock which acts as a monitor
        private static void SetBitmap(Bitmap bitmap)
        {
            lock (_bitmapLockObject)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose(); //Dispose the old bitmap first
                    _bitmapData = null; //Also reset bitmap data such that the calculate method reintializes it from the new bitmap
                }
                
                _bitmap = bitmap;
            }
        }

        public static Color GetAverageScreenColor()
        {
            return CalculateAverageColor(0, 0, _screenBounds.Width, _screenBounds.Height);
        }

        public static void Start()
        {
            _timer.Start();
        }

        public static void Stop()
        {
            _timer.Stop();
        }

        public static Color CalculateAverageColor(int startX, int startY, int width, int height)
        {
            lock (_bitmapLockObject)
            {
                if (_bitmap != null)
                {
                    if (_bitmapData == null)
                    {
                        //Initialize bitmap data
                        _bitmapData = _bitmap.LockBits(new Rectangle(0, 0, _screenBounds.Width, _screenBounds.Height), ImageLockMode.ReadOnly, _bitmap.PixelFormat);
                    }

                    //Variable initialization all outside the loop, since initializing is slower than allocating
                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    int index = 0;
                    int minDiversion = 50; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
                    int dropped = 0; // keep track of dropped pixels
                    long totalRed = 0, totalGreen = 0, totalBlue = 0;
                    int bppModifier = 4; //bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;
                                        
                    int stride = _bitmapData.Stride;
                    IntPtr Scan0 = _bitmapData.Scan0;

                    unsafe
                    {
                        byte* p = (byte*)(void*)Scan0;

                        for (int y = startY; y < height; y++)
                        {
                            for (int x = startX; x < width; x++)
                            {
                                index = (y * stride) + x * bppModifier;
                                red = p[index + 2];
                                green = p[index + 1];
                                blue = p[index];

                                Debug.WriteLine($"{red} {green} {blue}");

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

                    int count = width * height - dropped;
                    count++; //We increase count by 1 to make sure it is not 0. The difference in color when increasing count by 1 is neglectable

                    return Color.FromArgb((int)(totalRed / count), (int)(totalGreen / count), (int)(totalBlue / count));
                }
            }

            return _whiteColor;
        }
    }
}
