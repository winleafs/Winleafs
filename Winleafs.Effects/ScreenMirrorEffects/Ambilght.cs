using Winleafs.Effects.Helpers;
using Winleafs.Models;
using Winleafs.Nanoleaf;

namespace Winleafs.Effects.ScreenMirrorEffects
{
    public class Ambilght : IScreenMirrorEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly List<Rectangle> _captureArea; //A list since that is what the screengrabber expects as input

        public Ambilght(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;

            var screenBounds = ScreenBoundsHelper.GetScreenBounds(UserSettings.Settings.ScreenMirrorMonitorIndex);

            _captureArea = new List<Rectangle>
            {
                //Which area of the screenshot we need to look at, in the case of Ambilight, we need to look at the whole screenshot.
                //So start at 0, 0 and then use the width and height of the screen being captured
                new Rectangle(0, 0, screenBounds.Width, screenBounds.Height )
            };
        }

        /// <summary>
        /// Applies the ambilight effect to the lights. Ambilight is the average color of the whole screen
        /// Sets the color of the nanoleaf with the logging disabled.
        /// Seeing as a maximum of 10 requests per second can be set this will generate a lot of unwanted log data.
        /// See https://github.com/StijnOostdam/Winleafs/issues/40.
        /// </summary>
        public async Task ApplyEffect()
        {
            var colors = ScreenGrabber.CalculateAverageColor(_captureArea);

            if (colors == null)
            {
                //This can happen when before the first screen shot is taken when the effect is enabled
                return;
            }

            var hue = (int)colors[0].GetHue(); //Safe since we always have 1 area
            var sat = (int)(colors[0].GetSaturation() * 100); //Safe since we always have 1 area

            await _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(hue, sat, disableLogging: true);
        }

        /*
         * Saved code if we ever want to introduce brightness control again:

            //For brightness calculation see: https://stackoverflow.com/a/596243 and https://www.w3.org/TR/AERT/#color-contrast
            //We do not use Color.GetBrightness() since that value is always null because we use Color.FromArgb in the screengrabber.
            //Birghtness can be maximum 100
            var brightness = Math.Min(100, (int)(0.299 * color.R + 0.587 * color.G + 0.114 * color.B));

            await _nanoleafClient.StateEndpoint.SetHueSaturationAndBrightnessAsync(hue, sat, brightness, disableLogging: true);
         */
    }
}
