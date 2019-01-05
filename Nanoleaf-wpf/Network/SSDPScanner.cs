using Rssdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nanoleaf_wpf.Network
{
    public static class SSDPScanner
    {
        public static async Task<List<SsdpDevice>> SearchForDevices()
        {
            var result = new List<SsdpDevice>();

            using (var deviceLocator = new SsdpDeviceLocator())
            {
                var foundDevices = await deviceLocator.SearchAsync(); 

                foreach (var foundDevice in foundDevices)
                {
                    result.Add(await foundDevice.GetDeviceInfo());
                }
            }

            return result;
        }
    }
}
