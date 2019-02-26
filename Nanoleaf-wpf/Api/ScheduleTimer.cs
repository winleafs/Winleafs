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

                    var activeTrigger = device.GetActiveTrigger();

                    if (activeTrigger == null)
                    {
                        var client = NanoleafClient.GetClientForDevice(device);
                        //There are no triggers so the lights can be turned off if it is not off already
                        await client.StateEndpoint.SetStateWithStateCheckAsync(false);
                    }
                    else
                    {
                        await EffectActivator.ActivateEffect(device, activeTrigger.Effect, activeTrigger.Brightness);
                    }
                }
            }
        }
    }
}
