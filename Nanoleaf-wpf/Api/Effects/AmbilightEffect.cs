using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Winleafs.Api;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Api.Effects
{
    public class AmbilightEffect : ICustomEffect
    {
        private INanoleafClient _nanoleafClient;
        private System.Timers.Timer _timer;
        private Rectangle _screenBounds;

        public AmbilightEffect(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;

            var timerRefreshRate = 1000;

            if (UserSettings.Settings.AmbilightRefreshRatePerSecond > 0 && UserSettings.Settings.AmbilightRefreshRatePerSecond <= 10)
            {
                timerRefreshRate = 1000 / UserSettings.Settings.AmbilightRefreshRatePerSecond;
            }

            _timer = new System.Timers.Timer(timerRefreshRate);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;

            var monitorInfo = new MonitorInfo();
            EnumDisplaySettings(Screen.AllScreens[UserSettings.Settings.AmbilightMonitorIndex].DeviceName, -1, ref monitorInfo);

            _screenBounds = new Rectangle(monitorInfo.dmPositionX, monitorInfo.dmPositionY, monitorInfo.dmPelsWidth, monitorInfo.dmPelsHeight);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => SetColor());
        }

        private async Task SetColor()
        {
            var color = CalculateAverageColor(CaptureScreen());
            var hue = (int)color.GetHue();
            var sat = (int)(color.GetSaturation() * 100);

            await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat);
        }

        private Bitmap CaptureScreen()
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

        private Color CalculateAverageColor(Bitmap bm)
        {
            int width = bm.Width;
            int height = bm.Height;
            int red = 0;
            int green = 0;
            int blue = 0;
            int minDiversion = 50; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
            int dropped = 0; // keep track of dropped pixels
            long[] totals = new long[] { 0, 0, 0 };
            int bppModifier = bm.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4; // cutting corners, will fail on anything else but 32 and 24 bit images

            BitmapData srcData = bm.LockBits(new Rectangle(0, 0, bm.Width, bm.Height), ImageLockMode.ReadOnly, bm.PixelFormat);

            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];
                        if (Math.Abs(red - green) > minDiversion || Math.Abs(red - blue) > minDiversion || Math.Abs(green - blue) > minDiversion)
                        {
                            totals[2] += red;
                            totals[1] += green;
                            totals[0] += blue;
                        }
                        else
                        {
                            dropped++;
                        }
                    }
                }
            }

            bm.Dispose();

            int count = width * height - dropped;
            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return Color.FromArgb(avgR, avgG, avgB);
        }

        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref MonitorInfo monitorInfo);

        public string GetName()
        {
            return $"{CustomEffects.EffectNamePreface}Ambilight";
        }

        public async Task Activate()
        {
            _timer.Start();
        }

        public async Task Deactivate()
        {
            _timer.Stop();
            Thread.Sleep(500); //Give the last command the time to complete, 500 is based on testing and a high value (better safe then sorry)
        }

        public bool IsContinuous()
        {
            return true;
        }

        public bool IsActive()
        {
            return _timer.Enabled;
        }

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