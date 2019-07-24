using System.Collections.Generic;
using System.Collections.ObjectModel;
using Winleafs.Models.Enums;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Options;

namespace Winleafs.Wpf.ViewModels
{
    public class OptionsViewModel
    {
        #region Properties
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
        #endregion

        private OptionsWindow _parent;

        #region Screen mirror dropdown
        public ScreenMirrorAlgorithm ScreenMirrorAlgorithm { get; set; }
        private Dictionary<string, ScreenMirrorAlgorithm> _screenMirrorAlgorithmMapping { get; set; } //Map display values to enum values

        public Dictionary<string, ScreenMirrorAlgorithm> AlgorithmPerDevice { get; set; }

        public string SelectedScreenMirrorAlgorithm
        {
            get { return EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm); }
            set
            {
                ScreenMirrorAlgorithm = _screenMirrorAlgorithmMapping[value];

                AlgorithmPerDevice[_selectedDevice] = ScreenMirrorAlgorithm;

                //_parent.ScreenMirrorAlgorithmChanged(); Will be used later if we visualize the algorithms
            }
        }

        public IEnumerable<string> ScreenMirrorAlgorithms
        {
            get
            {
                return _screenMirrorAlgorithmMapping.Keys;
            }
        }
        #endregion

        #region Selected device dropdown
        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                //_parent.SelectedDeviceChanged(); Will be used later if we visualize the algorithms
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }
        #endregion

        public OptionsViewModel(OptionsWindow parent)
        {
            _parent = parent;

            _screenMirrorAlgorithmMapping = new Dictionary<string, ScreenMirrorAlgorithm>()
            {
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm.Ambilight), ScreenMirrorAlgorithm.Ambilight },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorAlgorithm.ScreenMirror), ScreenMirrorAlgorithm.ScreenMirror }
            };
        }
    }
}
