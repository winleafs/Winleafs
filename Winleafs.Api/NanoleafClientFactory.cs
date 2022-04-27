using System.Collections.Generic;
using Winleafs.Models;

namespace Winleafs.Nanoleaf
{
    public static class NanoleafClientFactory
    {
        private static readonly Dictionary<string, INanoleafClient> _clients = new Dictionary<string, INanoleafClient>();

        /// <summary>
        /// Gets a <see cref="INanoleafClient"/> for the given <paramref name="device"/>.
        /// </summary>
        /// <param name="device">The device wanting the <see cref="INanoleafClient"/> for.</param>
        /// <returns>An instance of a class inheriting <see cref="INanoleafClient"/>.</returns>
        public static INanoleafClient Create(Device device)
        {
            if (!_clients.ContainsKey(device.IPAddress))
            {
                _clients.Add(device.IPAddress, new NanoleafClient(device.IPAddress, device.Port, device.AuthToken));
            }

            return _clients[device.IPAddress];

        }
    }
}
