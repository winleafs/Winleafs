using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;

namespace Winleafs.Wpf.Api.Effects
{
    public interface ICustomEffect
    {
        public static readonly Color DefaultColor = Color.DarkGray;

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

        /// <summary>
        /// Returns a list of colors used by this effect
        /// </summary>
        List<Color> GetColors();

        /// <summary>
        /// Return the name of this effect
        /// </summary>
        string GetName();
    }

    /// <summary>
    /// Custom equality comparer for <see cref="ICustomEffect"/>
    /// </summary>
    public class CustomEffectEqualityComparer : IEqualityComparer<ICustomEffect>
    {
        public bool Equals([AllowNull] ICustomEffect x, [AllowNull] ICustomEffect y)
        {
            if (x == null && y != null || y == null && x != null)
            {
                return false;
            }

            // If X is null here, both x and y must be null.
            return x == null ||
                (x.GetName() == y.GetName());
        }

        public int GetHashCode([DisallowNull] ICustomEffect obj)
        {
            return obj.GetName().GetHashCode();
        }
    }
}
