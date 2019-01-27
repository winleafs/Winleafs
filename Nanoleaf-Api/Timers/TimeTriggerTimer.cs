using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Effects;
using NLog;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace Nanoleaf_Api.Timers
{
    public class TimeTriggerTimer
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static TimeTriggerTimer Timer { get; set; }

        private Timer _timer;

        public TimeTriggerTimer()
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
            Timer = new TimeTriggerTimer();
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
                var todaysProgram = device.GetTodaysProgram();

                if (todaysProgram != null)
                {
                    var client = NanoleafClient.GetClientForDevice(device);

                    if (todaysProgram.Triggers.Count == 0)
                    {
                        //Today has no trigger so it can be turned off if it is not off already
                        await client.StateEndpoint.SetStateWithStateCheck(false);
                    }
                    else
                    {
                        var now = DateTime.Now;

                        var hour = now.Hour;
                        var minute = now.Minute;

                        var activeTrigger = todaysProgram.Triggers[0];

                        for (var i = 1; i < todaysProgram.Triggers.Count; i++)
                        {
                            if (todaysProgram.Triggers[i].ActualHours > hour || (todaysProgram.Triggers[i].ActualHours == hour && todaysProgram.Triggers[i].ActualMinutes > minute))
                            {
                                break;
                            }

                            activeTrigger = todaysProgram.Triggers[i];
                        }

                        try
                        {
                            if (activeTrigger.Effect.Equals(Effect.OFFEFFECTNAME))
                            {
                                await client.StateEndpoint.SetStateWithStateCheck(false);
                            }
                            else
                            {
                                await client.StateEndpoint.SetStateWithStateCheck(true); //Turn on device if it is not on
                                await client.EffectsEndpoint.SetSelectedEffectAsync(activeTrigger.Effect);
                                await client.StateEndpoint.SetBrightness(activeTrigger.Brightness);
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
