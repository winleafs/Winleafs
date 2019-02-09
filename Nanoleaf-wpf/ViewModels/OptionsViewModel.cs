using System.Collections.Generic;

namespace Winleafs.Wpf.ViewModels
{
    public class OptionsViewModel
    {
        public bool StartAtWindowsStartUp { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public int AmbilightRefreshRatePerSecond { get; set; }

        public List<string> MonitorNames { get; set; }

        public string SelectedMonitor { get; set; }
    }
}
