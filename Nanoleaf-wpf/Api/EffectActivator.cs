using NLog;
using System;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Effects;

namespace Winleafs.Wpf.Api
{
    public static class EffectActivator
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static async Task ActivateEffect(Device device, string effectName, int brightness)
        {
            try
            {
                var client = NanoleafClient.GetClientForDevice(device);
                var customEffects = CustomEffects.GetCustomEffectsForDevice(device);

                //DO NOT change the order of disabling effects, then setting brightness and then enabling effects
                if (customEffects.HasActiveEffects(effectName))
                {
                    await customEffects.DeactivateAllEffects();
                }

                await client.StateEndpoint.SetBrightnessAsync(brightness);

                if (customEffects.EffectIsCustomEffect(effectName))
                {
                    var customEffect = customEffects.GetCustomEffect(effectName);

                    if (!customEffect.IsActive())
                    {
                        await customEffect.Activate();
                    }
                }
                else
                {
                    await client.EffectsEndpoint.SetSelectedEffectAsync(effectName);
                }

            }
            catch (Exception e)
            {
                _logger.Error(e, $"Enabling effect failed for device {device.Name} with trigger effect {effectName}");
            }
        }
    }
}
