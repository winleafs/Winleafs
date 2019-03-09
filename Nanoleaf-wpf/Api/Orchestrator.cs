using NLog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api.Events;

namespace Winleafs.Wpf.Api
{
    /// <summary>
    /// Class which determines which effects should be played, there is one orchestrator per device
    /// </summary>
    public class Orchestrator
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Device Device { get; set; }
        public ScheduleTimer ScheduleTimer { get; set; }
        private CustomEffectsCollection _customEffects;
        private EventsCollection _events;

        public Orchestrator(Device device)
        {
            Device = device;

            _customEffects = new CustomEffectsCollection(Device);
            ScheduleTimer = new ScheduleTimer(this);
            _events = new EventsCollection(this);

            if (device.OperationMode == OperationMode.Schedule)
            {
                ScheduleTimer.StartTimer();
            }
            else if (device.OperationMode == OperationMode.Manual)
            {
                Task.Run(() => ActivateEffect(Device.OverrideEffect, Device.OverrideBrightness));
            }
        }

        public async Task<bool> TrySetOperationMode(OperationMode operationMode, bool isFromOverride = false)
        {
            //Only the override is able to stop the override, effects and events may not remove the manual mode
            if (!isFromOverride && Device.OperationMode == OperationMode.Manual)
            {
                return false;
            }

            Device.OperationMode = operationMode;

            //Stop all things that can activate an effect
            ScheduleTimer.StopTimer();
            _events.StopAllEvents();

            if (_customEffects.HasActiveEffects())
            {
                await _customEffects.DeactivateAllEffects();
            }

            //If its a schedule, then the schedule timer can start again. The events and override manage their own effect activation
            if (Device.OperationMode == OperationMode.Schedule)
            {
                ScheduleTimer.StartTimer();
            }

            return true;
        }

        public async Task ActivateEffect(string effectName, int brightness)
        {
            try
            {
                var client = NanoleafClient.GetClientForDevice(Device);

                //DO NOT change the order of disabling effects, then setting brightness and then enabling effects
                if (_customEffects.HasActiveEffects(effectName))
                {
                    await _customEffects.DeactivateAllEffects();
                }

                await client.StateEndpoint.SetBrightnessAsync(brightness);

                if (_customEffects.EffectIsCustomEffect(effectName))
                {
                    var customEffect = _customEffects.GetCustomEffect(effectName);

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
                _logger.Error(e, $"Enabling effect failed for device {Device.Name} with trigger effect {effectName}");
            }
        }

        public List<Effect> GetCustomEffectAsEffects()
        {
            return _customEffects.GetCustomEffectAsEffects();
        }
    }
}
