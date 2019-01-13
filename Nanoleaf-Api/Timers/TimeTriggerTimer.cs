using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Scheduling;
using System;
using System.Linq;
using System.Timers;

namespace Nanoleaf_Api.Timers
{
    public class TimeTriggerTimer
    {
        public static TimeTriggerTimer Timer { get; set; }

        private Timer _timer;

        private Program _todaysProgram;

        public TimeTriggerTimer()
        {
            SetTodaysProgram();

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

        public void SetTodaysProgram()
        {
            var userSettings = UserSettings.GetSettings();

            if (userSettings.Schedules.Any(s => s.Active))
            {
                var dayOfWeek = DateTime.Now.DayOfWeek; //Sunday = 0

                var index = dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;

                _todaysProgram = userSettings.Schedules.FirstOrDefault(s => s.Active).Programs[index];

                OnTimedEvent(_timer, null);
            }
            else //It is possible that a user deletes the active schedule, then _toadysProrgam should be set to null
            {
                _todaysProgram = null;
            }
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            if (_todaysProgram != null)
            {
                if (_todaysProgram.Triggers.Count == 0)
                {
                    //Send off command here
                }
                else
                {
                    var now = DateTime.Now;

                    var hour = now.Hour;
                    var minute = now.Minute;

                    var activeTrigger = _todaysProgram.Triggers[0];

                    for (var i = 1; i < _todaysProgram.Triggers.Count; i++)
                    {
                        if (_todaysProgram.Triggers[i].Hours > hour || (_todaysProgram.Triggers[i].Hours == hour && _todaysProgram.Triggers[i].Minutes > minute))
                        {
                            break;
                        }

                        activeTrigger = _todaysProgram.Triggers[i];
                    }

                    activeTrigger.Trigger(); //Perhaps we need to move this code to here because of circular dependencies
                }
            }
        }
    }
}
