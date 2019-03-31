using System.Threading.Tasks;
using Winleafs.Models.Models.ExternalControl;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface IExternalControlEndpoint
    {
        /// <summary>
        /// Gets the needed info to start external control
        /// </summary>
        /// <returns>Awaitable task of type <<see cref="ExternalControlInfo"/>><returns>
        Task<ExternalControlInfo> GetExternalControlInfoAsync();

        /// <summary>
        /// Gets the needed info to start external control
        /// </summary>
        /// <returns><see cref="ExternalControlInfo"/><returns>
        ExternalControlInfo GetExternalControlInfo();

        /// <summary>
        /// Prepares the Nanoleaf device for external control
        /// </summary>
        /// <returns>Awaitable task</returns>
        Task PrepareForExternalControl();

        /// <summary>
        /// Sets the color of an individual panel via UDP. Only works if external control is activated
        /// </summary>
        /// <param name="panelId">ID of the panel</param>
        /// <param name="red">The red RDB value</param>
        /// <param name="green">The green RDB value</param>
        /// <param name="blue">The blue RDB value</param>
        /// <param name="transitionTime">The time to transition to this frame from the previous frame (must be equal or greater than 1), default 1</param>
        /// <returns>Awaitable task</returns>
        Task SetPanelColorAsync(int panelId, int red, int green, int blue, int transitionTime = 1);
    }
}
