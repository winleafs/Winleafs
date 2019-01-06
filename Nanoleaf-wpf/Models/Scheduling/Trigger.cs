namespace Nanoleaf_wpf.Models.Scheduling
{
    public class Trigger
    {
        public int Hours { get; set; }
        public int Minutes { get; set; }

        public TriggerType Type { get; set; }

        public string Effect { get; set; } //TODO: check what this should be, an identifier of an effect?
    }
}
