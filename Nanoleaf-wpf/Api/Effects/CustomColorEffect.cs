using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Winleafs.Api;
using Color = System.Drawing.Color;

namespace Winleafs.Wpf.Api.Effects
{
    public class CustomColorEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;
        private readonly Color _color;

        public static string Name => $"{CustomEffectsCollection.EffectNamePreface}Custom Color";

        public CustomColorEffect(INanoleafClient nanoleafClient, Color color)
        {
            _nanoleafClient = nanoleafClient;
            _color = color;
        }

        public Task Activate()
        {
            return _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(
                Convert.ToInt32(_color.GetHue()), 
                Convert.ToInt32(_color.GetSaturation() * 100));
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
