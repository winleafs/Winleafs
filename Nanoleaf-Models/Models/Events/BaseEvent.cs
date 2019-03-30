namespace Winleafs.Models.Models.Events
{
    public abstract class BaseEvent
    {
        public string EffectName { get; set; }
        public int Brightness { get; set; }
    }
}
