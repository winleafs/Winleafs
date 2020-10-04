using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    public class ScheduleTrigger : TriggerBase
    {
        public TimeComponent TimeComponent { get; set; }

        public override string Description
        {
            get
            {
                if (TimeComponent.ExtraHours > 0 || TimeComponent.ExtraMinutes > 0)
                {
                    var extraHours = TimeComponent.ExtraHours < 10 ? $"0{TimeComponent.ExtraHours}" : TimeComponent.ExtraHours.ToString();
                    var extraMinutes = TimeComponent.ExtraMinutes < 10 ? $"0{TimeComponent.ExtraMinutes}" : TimeComponent.ExtraMinutes.ToString();
                    return $"({(TimeComponent.BeforeAfter == BeforeAfter.Before ? "-" : "+")} {extraHours}:{extraMinutes}) {TimeComponent.GetActualDateTime().ToString("HH:mm")}";
                }
                else
                {
                    return $"{TimeComponent.GetActualDateTime().ToString("HH:mm")}";
                }
            }
        }
    }
}
