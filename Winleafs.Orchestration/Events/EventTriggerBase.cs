namespace Winleafs.Orchestration.Events
{
    public abstract class EventTriggerBase
    {
        public EventTriggerBase(int brightness, string effectName, int priority)
        {
            Brightness = brightness;
            EffectName = effectName;
            Priority = priority;
        }

        public int Brightness { get; set; }

        public string EffectName { get; set; }

        public int Priority { get; set; }

        public abstract void Stop();
    }
}
