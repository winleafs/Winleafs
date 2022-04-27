using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Effects.ScreenMirrorEffects;
using Winleafs.Models;
using Winleafs.Nanoleaf;

namespace Winleafs.Wpf.Api.Effects
{
    public class CustomEffectsCollection
    {
        private Dictionary<string, ICustomEffect> _customEffects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEffectsCollection"/> class.
        /// </summary>
        /// <param name="orchestrator">The orchestrator instance currently in use.</param>
        public CustomEffectsCollection(Orchestrator orchestrator)
        {
            var nanoleafClient = NanoleafClient.GetClientForDevice(orchestrator.Device);

            _customEffects = new Dictionary<string, ICustomEffect>();

            var customColorEffects = UserSettings.Settings.CustomEffects;

            if (customColorEffects?.Count > 0)
            {
                _customEffects = customColorEffects.Select(customColorEffect => new CustomColorEffect(nanoleafClient, customColorEffect.Color, customColorEffect.EffectName))
                    .ToDictionary(x => x.GetName(), x => x as ICustomEffect);
            }

            //We will not translate effect names since their names are identifiers
            if (ScreenMirrrorSupportDeviceTypes.Types.Contains(orchestrator.PanelLayout.DeviceType))
            {
                //Only add screen mirror to supported devices
                var screenMirrorEffect = new ScreenMirrorEffect(orchestrator.Device, orchestrator.PanelLayout, nanoleafClient);
                _customEffects.Add(screenMirrorEffect.GetName(), screenMirrorEffect);
            }

            var turnOffEffect = new TurnOffEffect(nanoleafClient);
            _customEffects.Add(turnOffEffect.GetName(), turnOffEffect);
        }

        /// <summary>
        /// Determines if the given <paramref name="effectName"/> is a custom effect.
        /// </summary>
        /// <param name="effectName">The name of the effect to be searched for.</param>
        /// <returns>
        /// A boolean to indicate if the effect is a custom effect, true if it is.
        /// </returns>
        public bool EffectIsCustomEffect(string effectName)
        {
            if (string.IsNullOrWhiteSpace(effectName))
            {
                return false;
            }

            return _customEffects.ContainsKey(effectName);
        }

        public ICustomEffect GetCustomEffect(string effectName)
        {
            return string.IsNullOrWhiteSpace(effectName) ? null : _customEffects[effectName];
        }

        /// <summary>
        /// Returns true when any other effect next to the given effectName is active,
        /// used when switching effects. If no effectName is given, returns true when any effect is active
        /// </summary>
        public bool HasActiveEffects(string effectName = "")
        {
            return _customEffects.Any(keyValuePair =>
                !keyValuePair.Key.Equals(effectName) && keyValuePair.Value.IsActive());
        }

        /// <summary>
        /// Disables all custom that are currently active effects.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        public async Task DeactivateAllEffects()
        {
            foreach (var activeEffect in _customEffects.Where(keyValuePair => keyValuePair.Value.IsActive()))
            {
                await activeEffect.Value.Deactivate();
            }
        }

        /// <summary>
        /// Returns the list of custom effects for the device
        /// </summary>
        public List<ICustomEffect> GetCustomEffects()
        {
            return _customEffects.Values.ToList();
        }

        /// <summary>
        /// Returns true when any <see cref="ScreenMirrorEffect"/> is active.
        /// </summary>
        public bool HasActiveScreenMirrorEffect()
        {
            return _customEffects.Values.Any(customEffect => customEffect.IsActive() && customEffect is ScreenMirrorEffect);
        }
    }
}
