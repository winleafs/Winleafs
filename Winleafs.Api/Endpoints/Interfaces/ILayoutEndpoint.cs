using System.Threading.Tasks;
using Winleafs.Models.Layouts;

namespace Winleafs.Nanoleaf.Endpoints.Interfaces
{
    public interface ILayoutEndpoint
    {
        /// <summary>
        /// Gets the current Nanoleaf layout.
        /// </summary>
        /// <returns>The current Nanoleaf layout.</returns>
        Task<Layout> GetLayoutAsync();

	    /// <inheritdoc cref="GetLayoutAsync"/>
		Layout GetLayout();

        /// <summary>
        /// Gets the current global orientation in degrees.
        /// </summary>
        /// <returns>The current global orientation</returns>
        Task<GlobalOrientation> GetGlobalOrientationAsync();

        /// <inheritdoc cref="GetGlobalOrientationAsync"/>
        GlobalOrientation GetGlobalOrientation();
    }
}