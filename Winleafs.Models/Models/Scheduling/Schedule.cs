using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Models.Models.Scheduling
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
        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.All)] //Used such that the correct class is known during serialization
        public List<TriggerBase> EventTriggers { get; set; }

        /// <summary>
        /// A list of <see cref="Device.Name"/>s to which devices this
        /// schedule applies.
        /// </summary>
        public List<string> AppliesToDeviceNames { get; set; }

        /// <summary>
        /// Used to display the device names as a string
        /// </summary>
        public string AppliedDeviceNames => string.Join(", ", AppliesToDeviceNames);

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

            EventTriggers = new List<TriggerBase>();
            AppliesToDeviceNames = new List<string>();
        }

        private bool ScheduleHasTriggers()
        {
            return Programs.Any(program => program.Triggers.Any());
        }

        private int GetTodaysProgramIndex()
        {
            var dayOfWeek = DateTime.Now.DayOfWeek; //Sunday = 0

            return dayOfWeek == DayOfWeek.Sunday ? 6 : (int)dayOfWeek - 1;
        }

        public TimeTrigger GetActiveTimeTrigger()
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

        public Tuple<DayOfWeek, TimeTrigger> GetNextTimeTrigger()
        {
            if (Programs.Sum(program => program.Triggers.Count) >= 2) //The schedule must have at least 2 triggers to have a next trigger
            {
                var now = DateTime.Now;

                var todaysIndex = GetTodaysProgramIndex();

                var nextTrigger = GetNextTimeTriggerForProgram(now, now, Programs[todaysIndex]); //First check if any trigger of today is next

                if (nextTrigger == null) //We need to look in the upcoming days for the trigger that is next
                {
                    var dayIndex = todaysIndex;
                    var dateOfProgram = now;

                    while (nextTrigger == null)
                    {
                        dayIndex = dayIndex == 6 ? 0 : dayIndex + 1;
                        dateOfProgram = dateOfProgram.AddDays(1);

                        nextTrigger = GetNextTimeTriggerForProgram(now, dateOfProgram, Programs[dayIndex]);
                    }

                    todaysIndex = dayIndex;
                }

                var dayOfWeek = todaysIndex == 6 ? DayOfWeek.Sunday : (DayOfWeek)(todaysIndex + 1); //Convert our index back to the DayOfWeek enum
                return new Tuple<DayOfWeek, TimeTrigger>(dayOfWeek, nextTrigger);
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

        private TimeTrigger GetNextTimeTriggerForProgram(DateTime now, DateTime dateOfProgram, Program program)
        {
            TimeTrigger nextTrigger = null;

            //This assumes Triggers are sorted in ascending time order
            for (var i = program.Triggers.Count - 1; i >= 0; i--)
            {
                if (now.Ticks < program.Triggers[i].GetActualDateTime(dateOfProgram).Ticks)
                {
                    nextTrigger = program.Triggers[i];
                }
            }

            return nextTrigger;
        }

        /// <summary>
        /// Deletes any trigger in the schedule which effect name is
        /// contained within the given <paramref name="effectNames"/>.
        /// </summary>
        public void DeleteTriggers(IEnumerable<string> effectNames)
        {
            foreach (var program in Programs)
            {
                //Make a copy such that we do not modify the collection while looping
                var triggersCopy = program.Triggers.ToList();

                foreach (var trigger in program.Triggers)
                {
                    if (effectNames.Contains(trigger.EffectName))
                    {
                        triggersCopy.Remove(triggersCopy.FirstOrDefault(triggerCopy => triggerCopy.EffectName == trigger.EffectName));
                    }
                }

                program.Triggers = triggersCopy;
            }

            var eventTriggersCopy = EventTriggers.ToList();

            foreach (var trigger in EventTriggers)
            {
                if (effectNames.Contains(trigger.EffectName))
                {
                    eventTriggersCopy.Remove(eventTriggersCopy.FirstOrDefault(triggerCopy => triggerCopy.EffectName == trigger.EffectName));
                }
            }

            EventTriggers = eventTriggersCopy;
        }

        public bool HasSpotifyTriggers()
        {
            return EventTriggers.Any(eventTrigger => eventTrigger is SpotifyEventTrigger);
        }
    }
}
