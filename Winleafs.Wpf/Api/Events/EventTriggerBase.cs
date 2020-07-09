namespace Winleafs.Wpf.Api.Events
{
    public class EventTriggerBase
    {
        public EventTriggerBase(int _brightness, string _effectName, int _priority)
        {
            Brightness = _brightness;
            EffectName = _effectName;
            Priority = _priority;
        }

        public int Brightness { get; set; }

        public string EffectName { get; set; }

        public int Priority { get; set; }
    }
}
