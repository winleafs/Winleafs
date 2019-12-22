using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api.Events;
using Winleafs.Wpf.Api.Layouts;

namespace Winleafs.Wpf.Api
{
    /// <summary>
    /// Class that orchestrates which effects and events should be played per device
    /// </summary>
    public class Orchestrator
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public Device Device { get; set; }

        public ScheduleTimer ScheduleTimer { get; set; }

        public PanelLayout PanelLayout { get; set; }

        private readonly CustomEffectsCollection _customEffects;
        private readonly EventTriggersCollection _eventTriggersCollection;

        private List<Action> _effectChangedCallbacks;

        public Orchestrator(Device device)
        {
            Device = device;

            ScheduleTimer = new ScheduleTimer(this);
            PanelLayout = new PanelLayout(Device);
            _eventTriggersCollection = new EventTriggersCollection(this);
            _effectChangedCallbacks = new List<Action>();

            // Custom effect initialization must come after panel layout initialization as custom screen mirror effect needs the panel layout
            _customEffects = new CustomEffectsCollection(Device, this); 

            if (device.OperationMode == OperationMode.Schedule)
            {
                ScheduleTimer.StartTimer();
            }
            else if (device.OperationMode == OperationMode.Manual)
            {
                Task.Run(() => ActivateEffect(Device.ManualEffect, Device.ManualBrightness));
            }
        }

        /// <summary>
        /// Try to set the operation mode for a device
        /// Only the override is able to stop the override, effects and events may not remove the manual mode
        /// First stops all active effects and events before switching to the new effect or event claiming operation mode
        /// </summary>
        public async Task<bool> TrySetOperationMode(OperationMode operationMode, bool isFromOverride = false, bool sync = false)
        {
            if (!isFromOverride && Device.OperationMode == OperationMode.Manual)
            {
                return false;
            }

            Device.OperationMode = operationMode;

            //Stop all things that can activate an effect
            await StopAllEffectsAndEvents();

            //If its a schedule, then the schedule timer can start again. The events and override manage their own effect activation
            if (Device.OperationMode == OperationMode.Schedule)
            {
                ScheduleTimer.StartTimer(sync);
            }

            //Finally, trigger effect changed callback, but not for manual operation mode, since that one is responsible for setting the effect and triggering the callbacks itself
            if (Device.OperationMode != OperationMode.Manual)
            {
                TriggerEffectChangedCallbacks();
            }            

            return true;
        }

        /// <summary>
        /// Stops all currently running effects and events
        /// </summary>
        private async Task StopAllEffectsAndEvents()
        {
            ScheduleTimer.StopTimer();
            _eventTriggersCollection.StopAllEvents();

            if (_customEffects.HasActiveEffects())
            {
                await _customEffects.DeactivateAllEffects();
            }
        }

        /// <summary>
        /// Activates an effect by name and brightness. This can be a custom effect (e.g. screen mirror) or a effect available on the Nanoleaf device
        /// First deactivates any custom effects before enabling the new effect
        /// </summary>
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

        /// <summary>
        /// Returns the list of custom effects for the device
        /// </summary>
        public List<ICustomEffect> GetCustomEffects()
        {
            return _customEffects.GetCustomEffects();
        }

        /// <summary>
        /// Get the current effect name, not equal the description as shown in the view
        /// </summary>
        public string GetActiveEffectName()
        {
            switch (Device.OperationMode)
            {
                case OperationMode.Manual:
                    return Device.ManualEffect;

                case OperationMode.Event:
                    var activeEvent = _eventTriggersCollection.EventTriggers.FirstOrDefault(e => e.IsActive());

                    return activeEvent?.GetTrigger().GetEffectName();

                case OperationMode.Schedule:
                    var activeTimeTrigger = UserSettings.Settings.GetActiveTimeTriggerForDevice(Device.Name);

                    return activeTimeTrigger?.GetEffectName();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Get the currently active brightness
        /// </summary>
        /// <returns>The currently active brightness, -1 if there is no current brightness</returns>
        public int GetActiveBrightness()
        {
            switch (Device.OperationMode)
            {
                case OperationMode.Manual:
                    return Device.ManualBrightness;

                case OperationMode.Event:
                    var activeEvent = _eventTriggersCollection.EventTriggers.FirstOrDefault(e => e.IsActive());

                    if (activeEvent != null)
                    {
                        return activeEvent.GetTrigger().GetBrightness();
                    }
                    return -1;

                case OperationMode.Schedule:
                    var activeTimeTrigger = UserSettings.Settings.GetActiveTimeTriggerForDevice(Device.Name);

                    if (activeTimeTrigger != null)
                    {
                        return activeTimeTrigger.Brightness;
                    }
                    return -1;

                default:
                    return -1;
            }
        }

        /// <summary>
        /// Returns the instance of a custom effect, null if it does not exist
        /// </summary>
        public ICustomEffect GetCustomEffectFromName(string effectName)
        {
            try
            {
                return _customEffects.GetCustomEffect(effectName);
            }
            catch
            {
                return null;
            }
        }

        public void AddEffectChangedCallback(Action callback)
        {
            _effectChangedCallbacks.Add(callback);
        }

        public void TriggerEffectChangedCallbacks()
        {
            foreach (var callback in _effectChangedCallbacks)
            {
                callback();
            }
        }
    }
}
