using System;
using System.Threading.Tasks;
using System.Timers;

using NLog;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api.Effects;

namespace Winleafs.Wpf.Api
{
    public class ScheduleTimer
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static ScheduleTimer Timer { get; set; }

        private Timer _timer;

        public ScheduleTimer()
        {
            // Create a timer with a one minute interval.
            _timer = new Timer(60000);
            // Hook up the Elapsed event for the timer.
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();

            FireTimer();
        }

        public static void InitializeTimer()
        {
            Timer = new ScheduleTimer();
        }

        public void FireTimer()
        {
            OnTimedEvent(this, null);
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Task.Run(() => SetEffectsForDevices());
        }

        private async Task SetEffectsForDevices()
        {
            foreach (var device in UserSettings.Settings.Devices)
            {
                if (device.OperationMode == OperationMode.Schedule)
                {
                    var client = NanoleafClient.GetClientForDevice(device);

                    var activeTrigger = device.GetActiveTrigger();

                    if (activeTrigger == null)
                    {
                        //There are no triggers so the lights can be turned off if it is not off already
                        await client.StateEndpoint.SetStateWithStateCheckAsync(false);
                    }
                    else
                    {
                        try
                        {
                            var customEffects = CustomEffects.GetCustomEffectsForDevice(UserSettings.Settings.ActviceDevice);

                            //DO NOT change the order of disabling effects, then setting brightness and then enabling effects
                            if (customEffects.HasActiveEffects(activeTrigger.Effect))
                            {
                                await customEffects.DeactivateAllEffects();
                            }

                            await client.StateEndpoint.SetBrightnessAsync(activeTrigger.Brightness);

                            if (customEffects.EffectIsCustomEffect(activeTrigger.Effect))
                            {
                                var customEffect = customEffects.GetCustomEffect(activeTrigger.Effect);

                                if (!customEffect.IsActive())
                                {
                                    await customEffect.Activate();
                                }
                            }
                            else
                            {
                                await client.EffectsEndpoint.SetSelectedEffectAsync(activeTrigger.Effect);
                            }

                        }
                        catch (Exception e)
                        {
                            _logger.Error(e, $"Time trigger failed for device {device.Name} with trigger effect {activeTrigger.Effect}");
                        }
                    }
                }
            }
        }
    }
}
