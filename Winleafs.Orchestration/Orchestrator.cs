using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NLog;
using Winleafs.Effects;
using Winleafs.Effects.ScreenMirrorEffects;
using Winleafs.Layouts;
using Winleafs.Models;
using Winleafs.Models.Enums;
using Winleafs.Nanoleaf;
using Winleafs.Orchestration.Events;

namespace Winleafs.Orchestration
{
    /// <summary>
    /// Class that orchestrates which effects and events should be played per device
    /// </summary>
    public class Orchestrator
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();
        private static Type _turnOffEffectType = typeof(TurnOffEffect);

        public Device Device { get; set; }

        public ScheduleTimer ScheduleTimer { get; private set; }

        public PanelLayout PanelLayout { get; set; }

        public EventTriggersCollection EventTriggersCollection { get; private set; }

        private readonly CustomEffectsCollection _customEffects;

        private List<Action> _effectChangedCallbacks;

        public Orchestrator(Device device)
        {
            Device = device;

            ScheduleTimer = new ScheduleTimer(Device);
            PanelLayout = new PanelLayout(Device);
            EventTriggersCollection = new EventTriggersCollection(Device);
            _effectChangedCallbacks = new List<Action>();

            // Custom effect initialization must come after panel layout initialization as custom screen mirror effect needs the panel layout
            _customEffects = new CustomEffectsCollection(this); 

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
            if (_customEffects.HasActiveEffects())
            {
                await _customEffects.DeactivateAllEffects();
            }

            //If its a schedule, then the schedule timer can start again. The events and override manage their own effect activation
            if (Device.OperationMode == OperationMode.Schedule)
            {
                ScheduleTimer.StartTimer(sync);
            }          

            return true;
        }

        /// <summary>
        /// Activates an effect by name and brightness. This can be a custom effect (e.g. screen mirror) or a effect available on the Nanoleaf device
        /// First deactivates any custom effects before enabling the new effect
        /// </summary>
        public async Task ActivateEffect(string effectName, int brightness)
        {
            _logger.Info($"Orchestrator is activating effect {effectName} with brightness {brightness} for device {Device.IPAddress}");

            try
            {
                var client = NanoleafClient.GetClientForDevice(Device);

                //DO NOT change the order of disabling effects, then setting brightness and then enabling effects
                if (_customEffects.HasActiveEffects(effectName))
                {
                    await _customEffects.DeactivateAllEffects();
                }


                if (_customEffects.EffectIsCustomEffect(effectName))
                {
                    var customEffect = _customEffects.GetCustomEffect(effectName);

                    //Special case: no need to set the brightness if we are turning off the device
                    if (customEffect.GetType() != _turnOffEffectType)
                    {
                        await client.StateEndpoint.SetBrightnessAsync(brightness);
                    }

                    if (!customEffect.IsActive())
                    {
                        await customEffect.Activate();
                    }
                }
                else
                {
                    await client.StateEndpoint.SetBrightnessAsync(brightness);
                    await client.EffectsEndpoint.SetSelectedEffectAsync(effectName);
                }

                //Finally, trigger effect changed callback
                TriggerEffectChangedCallbacks();
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
        /// Returns true when any <see cref="ScreenMirrorEffect"/> is active.
        /// </summary>
        public bool HasActiveScreenMirrorEffect()
        {
            return _customEffects.HasActiveScreenMirrorEffect();
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
                    return EventTriggersCollection.ActiveTrigger?.EffectName;

                case OperationMode.Schedule:
                    return UserSettings.Settings.GetActiveTimeTriggerForDevice(Device.Name)?.GetEffectName();

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns true when any effect is active
        /// </summary>
        public bool HasActiveEffect()
        {
            return GetActiveEffectName() != null;
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
                    var activeTrigger = EventTriggersCollection.ActiveTrigger;

                    if (activeTrigger != null)
                    {
                        return activeTrigger.Brightness;
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

        private void TriggerEffectChangedCallbacks()
        {
            foreach (var callback in _effectChangedCallbacks)
            {
                callback();
            }
        }
    }
}
