using System.Collections.Generic;
using System.Threading.Tasks;
using Winleafs.Models;
using Winleafs.Nanoleaf;

namespace Winleafs.Wpf.Api.Effects
{
    public class TurnOffEffect : ICustomEffect
    {
        private readonly INanoleafClient _nanoleafClient;

        public TurnOffEffect(INanoleafClient nanoleafClient)
        {
            _nanoleafClient = nanoleafClient;
        }

        /// <inheritdoc />
        public Task Activate()
        {
            return _nanoleafClient.StateEndpoint.SetStateWithStateCheckAsync(false);
        }

        /// <inheritdoc />
        public Task Deactivate()
        {
            //Do nothing since this is not a continious effect
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public bool IsActive()
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsContinuous()
        {
            return false;
        }

        /// <inheritdoc />
        public List<System.Drawing.Color> GetColors()
        {
            return new List<System.Drawing.Color> { ICustomEffect.DefaultColor };
        }

        public string GetName()
        {
            return $"{UserSettings.EffectNamePreface}Turn lights off";
        }
    }
}
