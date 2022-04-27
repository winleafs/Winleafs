using System.Threading.Tasks;

namespace Winleafs.Nanoleaf.Endpoints.Interfaces
{
    public interface IIdentifyEndpoint
    {
        /// <summary>
        /// Causes the panels to flash in unison. This is typically used to help
        /// users differentiate between multiple panels.
        /// </summary>
        /// <returns>Awaitable task.</returns>
        Task Identify();
    }
}
