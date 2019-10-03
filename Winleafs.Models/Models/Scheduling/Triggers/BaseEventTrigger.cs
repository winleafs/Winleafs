using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Base class for all even triggers
    /// </summary>
    public abstract class BaseEventTrigger : ITrigger
    {
        public string EffectName { get; set; }
        public int Brightness { get; set; }

        public TriggerType EventTriggerType { get; set; }

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
    }
}
