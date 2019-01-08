namespace Nanoleaf_wpf.Models.Scheduling.Triggers
{
    public class TimeTrigger : ITrigger
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TriggerType TriggerType { get; set; }

        public string Effect { get; set; }

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

        public bool ShouldTrigger()
        {
            throw new System.NotImplementedException();
        }

        public void Trigger()
        {
            throw new System.NotImplementedException();
        }

        public TriggerType GetTriggerType()
        {
            return TriggerType;
        }
    }
}
