using NLog;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows.Forms;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Helpers
{
    public static class ScreenGrabber
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static object _colorLockObject = new object();
        private static object _bitmapLockObject = new object();

        private static System.Timers.Timer _timer;
        private static Rectangle _screenBounds;

        private static Bitmap _bitmap;
        private static Color _color;

#pragma warning disable S3963 // "static" fields should be initialized inline
        static ScreenGrabber()
        {
            var timerRefreshRate = 1000;

            if (UserSettings.Settings.ScreenMirrorRefreshRatePerSecond > 0 && UserSettings.Settings.ScreenMirrorRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.ScreenMirrorRefreshRatePerSecond;
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            // We select monitors via the new API but the MonitorInfo class works via the Windows Forms diaply name.
            // Hence we retrieve the selected WIndows Forms monitor via our settings and then by comapring the DevicePath
            var monitors = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets();
            var formsMonitors = WindowsDisplayAPI.Display.GetDisplays();

            var selectedMonitor = formsMonitors.FirstOrDefault(monitor => monitor.DevicePath.Equals(monitors[UserSettings.Settings.ScreenMirrorMonitorIndex].DevicePath));

            var monitorInfo = new MonitorInfo();
            EnumDisplaySettings(selectedMonitor.DisplayName, -1, ref monitorInfo);

            _screenBounds = new Rectangle(monitorInfo.dmPositionX, monitorInfo.dmPositionY, monitorInfo.dmPelsWidth, monitorInfo.dmPelsHeight);
        }
#pragma warning restore S3963 // "static" fields should be initialized inline

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

                SetColor(CalculateAverageColor());
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during screen grabbing");
            }
        }

        #region Getter and setters with locks
        /// We do not use readerwriterlock since we most likely only have a few readers and the writer is more important, but readerwriterlock gives priority to readers
        /// Instead, we just use a lock which acts as a monitor
        private static void SetBitmap(Bitmap bitmap)
        {
            lock (_bitmapLockObject)
            {
                _bitmap = bitmap;
            }
        }

        public static Bitmap GetBitmap()
        {
            lock (_bitmapLockObject)
            {
                return _bitmap;
            }
        }

        private static void SetColor(Color color)
        {
            lock (_colorLockObject)
            {
                _color = color;
            }
        }

        public static Color GetColor()
        {
            lock (_colorLockObject)
            {
                return _color;
            }
        }
        #endregion

        public static void Start()
        {
            _timer.Start();
        }

        public static void Stop()
        {
            _timer.Stop();
        }

        private static Color CalculateAverageColor()
        {
            //Variable initialization all outside the loop, since initializing is slower than allocating
            var bm = GetBitmap();

            int red = 0;
            int green = 0;
            int blue = 0;
            int idx = 0;
            int minDiversion = 50; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long totalRed = 0, totalGreen = 0, totalBlue = 0;
            int bppModifier = 4; //bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; //cutting corners, will fail on anything else but 32 and 24 bit images. In this case the bitmap is always 32 bit.

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, _screenBounds.Width, _screenBounds.Height), ImageLockMode.ReadOnly, bm.PixelFormat);

            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < _screenBounds.Height; y++)
                {
                    for (int x = 0; x < _screenBounds.Width; x++)
                    {
                        idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];

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

            bm.Dispose(); //Dispose must stay here, moving it above the unsafe block causes crashes

            int count = _screenBounds.Width * _screenBounds.Height - dropped;
            count++; //We increase count by 1 to make sure it is not 0. The difference in color when increasing count by 1 is neglectable

            return Color.FromArgb((int)(totalRed / count), (int)(totalGreen / count), (int)(totalBlue / count));
        }


        [DllImport("user32.dll")]
#pragma warning disable S4214 // "P/Invoke" methods should not be visible
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref MonitorInfo monitorInfo);
#pragma warning restore S4214 // "P/Invoke" methods should not be visible

        [StructLayout(LayoutKind.Sequential)]
        public struct MonitorInfo
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}
