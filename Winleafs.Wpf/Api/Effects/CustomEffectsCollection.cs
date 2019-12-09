using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;

namespace Winleafs.Wpf.Api.Effects
{
    public class CustomEffectsCollection
    {
        private Dictionary<string, ICustomEffect> _customEffects;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEffectsCollection"/> class.
        /// </summary>
        /// <param name="device">The device to be used to generate a <see cref="NanoleafClient"/></param>
        /// <param name="orchestrator">The orchestrator instance currently in use.</param>
        public CustomEffectsCollection(Device device, Orchestrator orchestrator)
        {
            var nanoleafClient = NanoleafClient.GetClientForDevice(device);

            _customEffects = new Dictionary<string, ICustomEffect>();

            var customColorEffects = UserSettings.Settings.CustomEffects;

            if (customColorEffects != null && customColorEffects.Any())
            {
                foreach (var customColorEffect in customColorEffects)
                {
                    var effect = new CustomColorEffect(nanoleafClient, customColorEffect.Color, customColorEffect.EffectName);
                    _customEffects.Add(effect.GetName(), effect);
                }
            }

            //We will not translate effect names since their names are identifiers
            var screenMirrorEffect = new ScreenMirrorEffect(device, orchestrator, nanoleafClient);
            _customEffects.Add(screenMirrorEffect.GetName(), screenMirrorEffect);

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
            return _customEffects.ContainsKey(effectName);
        }

        public ICustomEffect GetCustomEffect(string effectName)
        {
            return _customEffects[effectName];
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
        /// Gets the <see cref="ICustomEffect"/>s stored in this collection
        /// as a <see cref="Effect"/> instance.
        /// </summary>
        /// <returns>
        /// The custom formats formatted as a <see cref="Effect"/>
        /// </returns>
        public List<Effect> GetCustomEffectAsEffects()
        {
            //TODO: remove 
            return _customEffects.Keys.OrderBy(name => name)
                .Select(name => new Effect { Name = name })
                .ToList();
        }

        /// <summary>
        /// Returns the list of custom effects for the device
        /// </summary>
        public List<ICustomEffect> GetCustomEffects()
        {
            return _customEffects.Values.ToList();
        }
    }
}
