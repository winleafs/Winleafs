using Winleafs.Models.Enums;

namespace Winleafs.Models.Scheduling.Triggers
{
    /// <summary>
    /// Trigger interface, all trigger should implement these methods
    /// </summary>
    public interface ITrigger
    {
        /// <summary>
        /// Get the trigger type
        /// </summary>
        TriggerType GetTriggerType();

        /// <summary>
        /// Used by the view to determine the description
        /// </summary>
        string GetDescription();

        /// <summary>
        /// Get the effect name of the trigger if any
        /// </summary>
        string GetEffectName();

        /// <summary>
        /// Get the brightness of the trigger
        /// </summary>
        int GetBrightness();

        /// <summary>
        /// Get the priority of the trigger
        /// </summary>
        int GetPriority();
    }
}
