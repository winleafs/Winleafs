namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Base class for all triggers
    /// </summary>
    public abstract class TriggerBase
    {
        public string EffectName { get; set; }
        public int Brightness { get; set; }

        public virtual string Description => EffectName;
    }
}
