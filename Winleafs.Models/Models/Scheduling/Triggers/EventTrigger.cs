using System;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    public abstract class EventTrigger : TriggerBase, IComparable<EventTrigger>
    {
        public int Priority { get; set; }

        /// <summary>
        /// Implement the comparable interface
        /// to be able to use <see cref="List.Sort"/>.
        /// </summary>
        public int CompareTo(EventTrigger other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
