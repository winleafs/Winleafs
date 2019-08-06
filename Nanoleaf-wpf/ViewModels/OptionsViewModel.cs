using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Winleafs.Models.Enums;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Options;

namespace Winleafs.Wpf.ViewModels
{
    public class OptionsViewModel
    {
        private OptionsWindow _parent;

        #region Properties
        public bool StartAtWindowsStartUp { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public List<string> MonitorNames { get; set; }

        public string SelectedLanguage { get; set; }

        public List<string> Languages { get; set; }

        public bool MinimizeToSystemTray { get; set; }
        #endregion

        #region Screen mirror
        public Dictionary<string, ScreenMirrorAlgorithm> AlgorithmPerDevice { get; set; }
        public Dictionary<string, int> ScreenMirrorRefreshRatePerDevice { get; set; }
        public Dictionary<string, bool> ScreenMirrorControlBrightnessPerDevice { get; set; }
        public Dictionary<string, string> MonitorPerDevice { get; set; }

        private string _selectedMonitor;
        public string SelectedMonitor
        {
            get => _selectedMonitor;
            set
            {
                _selectedMonitor = value;
                MonitorPerDevice[_selectedDevice] = _selectedMonitor;
            }
        }

        private int _screenMirrorRefreshRatePerSecond;
        public int ScreenMirrorRefreshRatePerSecond
        {
            get => _screenMirrorRefreshRatePerSecond;
            set
            {
                _screenMirrorRefreshRatePerSecond = value;
                ScreenMirrorRefreshRatePerDevice[_selectedDevice] = _screenMirrorRefreshRatePerSecond;
            }
        }

        private bool _screenMirrorControlBrightness;
        public bool ScreenMirrorControlBrightness
        {
            get => _screenMirrorControlBrightness;
            set
            {
                _screenMirrorControlBrightness = value;
                ScreenMirrorControlBrightnessPerDevice[_selectedDevice] = _screenMirrorControlBrightness;
            }
        }

        #region Screen mirror algorithm dropdown
        //Map display values to enum values
        public Dictionary<string, ScreenMirrorAlgorithm> ScreenMirrorAlgorithmMapping { get; set; }

        private ScreenMirrorAlgorithm _selectedScreenMirrorAlgorithm;
        public string SelectedScreenMirrorAlgorithm
        {
            get { return EnumLocalizer.GetLocalizedEnum(_selectedScreenMirrorAlgorithm); }
            set
            {
                _selectedScreenMirrorAlgorithm = ScreenMirrorAlgorithmMapping[value];

                AlgorithmPerDevice[_selectedDevice] = _selectedScreenMirrorAlgorithm;

                //_parent.ScreenMirrorAlgorithmChanged(); Will be used later if we visualize the algorithms
            }
        }

        public IEnumerable<string> ScreenMirrorAlgorithms => ScreenMirrorAlgorithmMapping.Keys;
        #endregion
        #endregion

        #region Selected device dropdown
        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                ScreenMirrorControlBrightness = ScreenMirrorControlBrightnessPerDevice[_selectedDevice];
                ScreenMirrorRefreshRatePerSecond = ScreenMirrorRefreshRatePerDevice[_selectedDevice];
                SelectedMonitor = MonitorPerDevice[_selectedDevice];

                var screenMirrorAlgorithm = AlgorithmPerDevice[_selectedDevice];
                SelectedScreenMirrorAlgorithm = ScreenMirrorAlgorithmMapping.FirstOrDefault(map => map.Value == screenMirrorAlgorithm).Key;

                //_parent.SelectedDeviceChanged(); Will be used later if we visualize the algorithms
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }
        #endregion

        public OptionsViewModel(OptionsWindow parent)
        {
            _parent = parent;

            ScreenMirrorAlgorithmMapping = new Dictionary<string, ScreenMirrorAlgorithm>()
            {
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm.Ambilight), ScreenMirrorAlgorithm.Ambilight },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm.ScreenMirrorFit), ScreenMirrorAlgorithm.ScreenMirrorFit },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm.ScreenMirrorStretch), ScreenMirrorAlgorithm.ScreenMirrorStretch }
            };
        }
    }
}
