using Nanoleaf_Models.Models.Scheduling.Triggers;
using System;
using System.Collections.Generic;

namespace Nanoleaf_Models.Models.Scheduling
{
    public class Schedule
    {
        /// <summary>
        /// List of programs, length is always 7, in order of days from monday until sunday, starting at monday
        /// </summary>
        public List<Program> Programs { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Only 1 schedule can be active at any time
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Indicates whether the lights should be turned off when shutting down the application or Windows
        /// </summary>
        public bool TurnOffAtApplicationShutdown { get; set; }

        /// <summary>
        /// A schedule has a list of time-independent event triggers
        /// </summary>
        public List<IEventTrigger> EventTriggers { get; set; }

        public Schedule(bool addPrograms = false)
        {
            Programs = new List<Program>();

            if (addPrograms)
            {
                //Add a program for each day of the week
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
                Programs.Add(new Program());
            }

            EventTriggers = new List<IEventTrigger>();
        }

        private bool ScheduleHasTriggers()
        {
            foreach (var program in Programs)
            {
                if (program.Triggers.Count > 0)
                {
                    return true;
                }
            }

            return false;
        }

        private int GetTodaysProgramIndex()
        {
            var dayOfWeek = DateTime.Now.DayOfWeek; //Sunday = 0

            return dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        }

        public TimeTrigger GetActiveTrigger()
        {
            if (ScheduleHasTriggers())
            {
                var now = DateTime.Now;

                var todaysIndex = GetTodaysProgramIndex();

                var currentTrigger = GetCurrentTimeTriggerForProgram(now, now, Programs[todaysIndex]); //First check if any trigger of today should be active

                if (currentTrigger == null) //We need to look in previous days for the trigger that should be active
                {
                    var dayIndex = todaysIndex == 0 ? 6 : todaysIndex - 1;
                    var dateOfProgram = now.AddDays(-1);

                    while (currentTrigger == null)
                    {
                        currentTrigger = GetCurrentTimeTriggerForProgram(now, dateOfProgram, Programs[dayIndex]);

                        dayIndex = dayIndex == 0 ? 6 : dayIndex - 1;
                        dateOfProgram = dateOfProgram.AddDays(-1);
                    }
                }

                return currentTrigger;
            }
            else
            {
                return null;
            }
        }

        private TimeTrigger GetCurrentTimeTriggerForProgram(DateTime now, DateTime dateOfProgram, Program program)
        {
            TimeTrigger currentTrigger = null;

            //This assumes Triggers are sorted in ascending time order
            for (var i = 0; i < program.Triggers.Count; i++)
            {
                if (now.Ticks > program.Triggers[i].GetActualDateTime(dateOfProgram).Ticks)
                {
                    currentTrigger = program.Triggers[i];
                }
            }

            return currentTrigger;
        }
    }
}
