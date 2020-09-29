using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    public class TimeTrigger : TriggerBase
    {
        public override string GetDescription()
        {
            if (ExtraHours > 0 || ExtraMinutes > 0)
            {
                var extraHours = ExtraHours < 10 ? $"0{ExtraHours}" : ExtraHours.ToString();
                var extraMinutes = ExtraMinutes < 10 ? $"0{ExtraMinutes}" : ExtraMinutes.ToString();
                return $"({(BeforeAfter == BeforeAfter.Before ? "-" : "+")} {extraHours}:{extraMinutes}) {GetActualDateTime().ToString("HH:mm")}";
            }
            else
            {
                return $"{GetActualDateTime().ToString("HH:mm")}";
            }
        }
    }
}
