using System.Threading.Tasks;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.ExternalControl;

namespace Winleafs.Api.Endpoints.Interfaces
{
    public interface IExternalControlEndpoint
    {
        /// <summary>
        /// Gets the needed info to start external control
        /// </summary>
        /// <returns>Awaitable task of type <see cref="ExternalControlInfo"/></returns>
        Task<ExternalControlInfo> GetExternalControlInfoAsync(DeviceType deviceType);

        /// <summary>
        /// Prepares the Nanoleaf device for external control
        /// </summary>
        /// <returns>Awaitable task</returns>
        Task PrepareForExternalControl(DeviceType deviceType, string deviceIPAddress);

        /// <summary>
        /// Sets the color of an individual panel via UDP. Only works if external control is activated
        /// </summary>
        /// <param name="panelId">ID of the panel</param>
        /// <param name="red">The red RDB value</param>
        /// <param name="green">The green RDB value</param>
        /// <param name="blue">The blue RDB value</param>
        /// <param name="transitionTime">The time to transition to this frame from the previous frame (must be equal or greater than 1), default 1</param>
        void SetPanelColor(DeviceType deviceType, int panelId, int red, int green, int blue);
    }
}
