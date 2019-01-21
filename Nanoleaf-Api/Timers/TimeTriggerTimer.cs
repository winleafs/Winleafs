using Nanoleaf_Models.Models;
using System;
using System.Timers;

namespace Nanoleaf_Api.Timers
{
    public class TimeTriggerTimer
    {
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
            foreach (var device in UserSettings.Settings.Devices)
            {
                var todaysProgram = device.GetTodaysProgram();

                if (todaysProgram != null)
                {
                    if (todaysProgram.Triggers.Count == 0)
                    {
                        //Send off command here
                    }
                    else
                    {
                        var now = DateTime.Now;

                        var hour = now.Hour;
                        var minute = now.Minute;

                        var activeTrigger = todaysProgram.Triggers[0];

                        for (var i = 1; i < todaysProgram.Triggers.Count; i++)
                        {
                            if (todaysProgram.Triggers[i].Hours > hour || (todaysProgram.Triggers[i].Hours == hour && todaysProgram.Triggers[i].Minutes > minute))
                            {
                                break;
                            }

                            activeTrigger = todaysProgram.Triggers[i];
                        }

                        activeTrigger.Trigger(); //Perhaps we need to move the actual triggering code to here because of circular dependencies (and model should not be in charge of controlling the panels anyway)
                    }
                }
            }

        }
    }
}
