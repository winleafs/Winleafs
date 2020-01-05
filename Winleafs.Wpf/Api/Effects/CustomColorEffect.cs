using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
using Color = System.Drawing.Color;

namespace Winleafs.Wpf.Api.Effects
{
    public class CustomColorEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;

        public Color Color { get; set; }

        private readonly string _effectName;

        public CustomColorEffect(INanoleafClient nanoleafClient, Color color, string effectName)
        {
            _nanoleafClient = nanoleafClient;
            Color = color;
            _effectName = effectName;
        }

        /// <inheritdoc />
        public Task Activate()
        {
            return _nanoleafClient.StateEndpoint.SetHueAndSaturationAsync(
                Convert.ToInt32(Color.GetHue()), 
                Convert.ToInt32(Color.GetSaturation() * 100));
        }

        /// <inheritdoc />
        public Task Deactivate()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool IsContinuous()
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsActive()
        {
            return false;
        }

        /// <inheritdoc />
        public List<Color> GetColors()
        {
            return new List<Color> { Color };
        }

        public string GetName()
        {
            return UserCustomColorEffect.DisplayName(_effectName);
        }
    }
}
