using System;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Api.Events
{
    public abstract class EventTriggerBase
    {
        public int Brightness => _eventTrigger.Brightness;

        public string EffectName => _eventTrigger.EffectName;

        public int Priority => _eventTrigger.Priority;

        private EventTrigger _eventTrigger;

        public EventTriggerBase(EventTrigger eventTrigger)
        {
            _eventTrigger = eventTrigger;
        }

        public abstract void Stop();

        /// <summary>
        /// Determines whether the trigger should be active according to
        /// its time component. Returns true when there is no time component
        /// for the trigger.
        /// </summary>
        protected bool TimeIsActive()
        {
            if (_eventTrigger.StartTimeComponent == null || _eventTrigger.EndTimeComponent == null)
            {
                return true;
            }

            var startTime = _eventTrigger.StartTimeComponent.GetActualDateTime();
            var endTime = _eventTrigger.EndTimeComponent.GetActualDateTime();

            //If the end time is earlier than the start time, add 1 day to the end time such that it later than the start time
            //This can happen if the user wants to start at, for example, 23:00 and at at 4:00.
            if (startTime > endTime)
            {
                endTime = endTime.AddDays(1);
            }

            return DateTime.Now >= startTime && DateTime.Now <= endTime;
        }
    }
}
