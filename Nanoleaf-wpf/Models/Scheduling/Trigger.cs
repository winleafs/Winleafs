namespace Nanoleaf_wpf.Models.Scheduling
{
    public class Trigger
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TriggerType Type { get; set; }

        public string Effect { get; set; }

        public string GetDisplayName()
        {
            if (Type == TriggerType.Time)
            {
                var hours = Hours < 10 ? $"0{Hours}" : Hours.ToString();
                var minutes = Minutes < 10 ? $"0{Minutes}" : Minutes.ToString();
                return $"{hours}:{minutes}";
            }
            else
            {
                return Type.ToString();
            }
        }
    }
}
