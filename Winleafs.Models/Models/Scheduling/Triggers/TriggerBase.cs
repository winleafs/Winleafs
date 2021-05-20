using System;
using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Base class for all triggers
    /// </summary>
    public abstract class TriggerBase : ITrigger, IComparable<TriggerBase>
    {
        public string EffectName { get; set; }

        public int Brightness { get; set; }

        public TriggerType EventTriggerType { get; set; }

        public int Priority { get; set; }

        public int GetBrightness()
        {
            return Brightness;
        }

        public abstract string GetDescription();

        public string GetEffectName()
        {
            return EffectName;
        }

        public TriggerType GetTriggerType()
        {
            return EventTriggerType;
        }

        public int GetPriority()
        {
            return Priority;
        }

        /// <summary>
        /// Implement the comparable interface
        /// to be able to use <see cref="System.Collections.Generic.List{T}.Sort()"/>.
        /// </summary>
        public int CompareTo(TriggerBase other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}
