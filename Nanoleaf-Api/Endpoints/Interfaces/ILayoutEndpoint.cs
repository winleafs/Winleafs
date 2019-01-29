using System.Threading.Tasks;

using Winleafs.Models.Models.Layouts;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface ILayoutEndpoint
    {
        /// <summary>
        /// Gets the current Nanoleaf layout.
        /// </summary>
        /// <returns>The current Nanoleaf layout.</returns>
        Task<Layout> GetLayout();

        /// <summary>
        /// Causes the panels to flash in unison. This is typically used to help users differentiate between multiple panels.
        /// </summary>
        /// <returns>An awaitable task.</returns>
        Task Identify();
    }
}