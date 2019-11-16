using System;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Models;
using Color = System.Drawing.Color;

namespace Winleafs.Wpf.Api.Effects
{
    public class CustomColorEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        public Color Color { get; set; }

        public static string Name => $"{UserSettings.EffectNamePreface}Custom Color";

        public CustomColorEffect(INanoleafClient nanoleafClient, Color color)
        {
            _nanoleafClient = nanoleafClient;
            Color = color;
        }

        public Task Activate()
        {
            return _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(
                Convert.ToInt32(Color.GetHue()), 
                Convert.ToInt32(Color.GetSaturation() * 100));
        }

        public Task Deactivate()
        {
            return Task.CompletedTask;
        }

        public bool IsContinuous()
        {
            return false;
        }

        public bool IsActive()
        {
            return false;
        }
    }
}
