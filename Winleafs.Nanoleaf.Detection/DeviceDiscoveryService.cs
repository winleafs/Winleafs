using Tmds.MDns;
using Winleafs.Models;

namespace Winleafs.Nanoleaf.Detection
{
    public class DeviceDiscoveryService
    {
        private const string ServiceType = "_nanoleafapi._tcp";

        public event EventHandler<DeviceDiscoveredEventArgs> DeviceDiscovered;

        private readonly ServiceBrowser _serviceBrowser;

        public DeviceDiscoveryService()
        {
            _serviceBrowser = new ServiceBrowser();
            _serviceBrowser.ServiceAdded += OnServiceAdded;
        }

        public void Start()
        {
            _serviceBrowser.StartBrowse(ServiceType);
        }

        public void Stop()
        {
            _serviceBrowser.StopBrowse();
        }

        private void OnServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            // Check if device is already within settings.
            if (UserSettings.HasSettings() &&
                UserSettings.Settings.Devices.Any(d => d.IPAddress.Equals(e.Announcement.Addresses.First().ToString())))

            {
                return;
            }

            //_logger.Info($"Discovered following device: {e.Announcement.Hostname}, IPs: {string.Join(",", e.Announcement.Addresses.Select(ip => ip.ToString()))}, Port: {e.Announcement.Port}");

            DeviceDiscovered?.Invoke(this, new DeviceDiscoveredEventArgs(new Device
            {
                Name = e.Announcement.Hostname,
                IPAddress = e.Announcement.Addresses.First().ToString(),
                Port = e.Announcement.Port
            }));
        }
    }
}
