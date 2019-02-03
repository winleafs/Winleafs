using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Winleafs.Api;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Effects
{
    public class AmbilightEffect
    {
        private static Dictionary<string, AmbilightEffect> _effectsForClients = new Dictionary<string, AmbilightEffect>();

        public static AmbilightEffect GetEffectForDevice(Device device)
        {
            if (!_effectsForClients.ContainsKey(device.IPAddress))
            {
                _effectsForClients.Add(device.IPAddress, new AmbilightEffect(device));
            }

            return _effectsForClients[device.IPAddress];
        }

        private INanoleafClient _nanoleafClient;
        private System.Timers.Timer _timer;

        public AmbilightEffect(Device device)
        {
            _nanoleafClient = NanoleafClient.GetClientForDevice(device);

            _timer = new System.Timers.Timer(100);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
        }

        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => SetColor());
        }

        public async Task SetColor()
        {
            var color = CalculateAverageColor(CaptureScreen());
            var hue = (int)color.GetHue();
            var sat = (int)(color.GetSaturation() * 100);

            await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat);
        }

        public Bitmap CaptureScreen()
        {
            //Create a new bitmap.
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
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
            int minDiversion = 15; // drop pixels that do not differ by at least minDiversion between color values (white, gray or black)
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

            int count = width * height - dropped;
            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            return Color.FromArgb(avgR, avgG, avgB);
        }
    }
}