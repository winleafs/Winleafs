using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Winleafs.Models.Enums;
using Winleafs.Models.ExternalControl;

namespace Winleafs.Nanoleaf.Endpoints.Interfaces
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
        /// Sets the color of one or more panels via UDP. Only works if external control is activated
        /// </summary>
        /// <param name="panelIds">List of panelIds/param>
        /// <param name="colors">List of colors, corresponding to the <paramref name="panelIds"/></param>
        void SetPanelsColors(DeviceType deviceType, List<int> panelIds, List<Color> colors);
    }
}
