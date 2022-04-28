using Winleafs.Models;

namespace Winleafs.Nanoleaf.Detection
{
    public class DeviceDiscoveredEventArgs : EventArgs
    {
        public Device Device { get; set; }

        public DeviceDiscoveredEventArgs(Device device)
        {
            Device = device;
        }
    }
}
