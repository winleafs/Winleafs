using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Nanoleaf_Api.Models.Layouts;

namespace Nanoleaf_Api.Endpoints.Interfaces
{
    public interface ILayoutEndpoint
    {
        /// <summary>
        /// Gets the current Nanoleaf layout.
        /// </summary>
        /// <returns>The current Nanoleaf layout.</returns>
        Task<Layout> GetLayout();
    }
}
