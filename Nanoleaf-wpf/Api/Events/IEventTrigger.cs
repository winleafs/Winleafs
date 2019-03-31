using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Api.Events
{
    /// <summary>
    /// Interface for event triggers API side, this is not a model class
    /// </summary>
    public interface IEventTrigger
    {
        /// <summary>
        /// Stop the event trigger
        /// </summary>
        void StopEvent();

        /// <summary>
        /// Check if an event is active
        /// </summary>
        bool IsActive();

        /// <summary>
        /// Get the model class belonging to this event trigger
        /// </summary>
        /// <returns></returns>
        ITrigger GetTrigger();
    }
}
