using Winleafs.Models.Enums;

namespace Winleafs.Models.Models.Scheduling.Triggers
{
    /// <summary>
    /// Trigger interface, all trigger should implement these methods
    /// </summary>
    public interface ITrigger
    {
        TriggerType GetTriggerType();

        /// <summary>
        /// Used by the view to determine the display name
        /// </summary>
        string GetDisplayName();

        /// <summary>
        /// Used by the view to determine the description
        /// </summary>
        string GetDescription();
    }
}
