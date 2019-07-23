using System.Collections.Generic;

namespace Winleafs.Wpf.ViewModels
{
    public class OptionsViewModel
    {
        public bool StartAtWindowsStartUp { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public int ScreenMirrorRefreshRatePerSecond { get; set; }
        public bool ScreenMirrorControlBrightness { get; set; }

        public List<string> MonitorNames { get; set; }

        public string SelectedMonitor { get; set; }

        public string SelectedLanguage { get; set; }

        public List<string> Languages { get; set; }

        public bool MinimizeToSystemTray { get; set; }
    }
}
