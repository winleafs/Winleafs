using Nanoleaf_Models.Enums;

namespace Nanoleaf_Models.Models.Scheduling.Triggers
{
    public class TimeTrigger : ITrigger
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TriggerType TriggerType { get; set; }

        public string Effect { get; set; }

        public int Brightness { get; set; }

        public string GetDisplayName()
        {
            if (TriggerType == TriggerType.Time)
            {
                var hours = Hours < 10 ? $"0{Hours}" : Hours.ToString();
                var minutes = Minutes < 10 ? $"0{Minutes}" : Minutes.ToString();
                return $"{hours}:{minutes}";
            }
            else
            {
                return TriggerType.ToString();
            }
        }

        public string GetDescription()
        {
            return Effect;
        }

        public void Trigger()
        {
            //TODO: actually apply effect, perhaps check first which effect is active so we dont apply the same effect twice
        }

        public TriggerType GetTriggerType()
        {
            return TriggerType;
        }
    }
}
