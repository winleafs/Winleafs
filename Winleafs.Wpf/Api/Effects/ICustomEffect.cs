using System.Threading.Tasks;

namespace Winleafs.Wpf.Api.Effects
{
    public interface ICustomEffect
    {
        /// <summary>
        /// Activates the custom effect.
        /// </summary>
        /// <returns>an awaitable task.</returns>
        Task Activate();

        /// <summary>
        /// Deactivates the custom effect.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task Deactivate();

        /// <summary>
        /// Returns if the effect is a continuous effect.
        /// </summary>
        /// <returns>A boolean to indicate if this is a continuous effect.</returns>
        bool IsContinuous();

        /// <summary>
        /// Checks if the effect is currently active.
        /// </summary>
        /// <returns>A boolean to indicate if the effect is active.</returns>
        bool IsActive();
    }
}
