using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Effects;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Options;

namespace Winleafs.Wpf.ViewModels
{
    public class OptionsViewModel : INotifyPropertyChanged
    {
        private OptionsWindow _parent;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        public bool StartAtWindowsStartUp { get; set; }

        public string Latitude { get; set; }
        public string Longitude { get; set; }

        public List<string> MonitorNames { get; set; }

        public string SelectedLanguage { get; set; }

        public List<string> Languages { get; set; }

        public bool MinimizeToSystemTray { get; set; }

        public List<UserCustomColorEffect> CustomColorEffects { get; set; }

        public string ProcessResetIntervalText { get; set; }
        #endregion

        #region Screen mirror
        public Dictionary<string, ScreenMirrorAlgorithm> AlgorithmPerDevice { get; set; }

        private int _screenMirrorRefreshRatePerSecond;

        public int ScreenMirrorRefreshRatePerSecond
        {
            get
            {
                return _screenMirrorRefreshRatePerSecond;
            }
            set
            {
                _screenMirrorRefreshRatePerSecond = value;
                OnPropertyChanged(nameof(ScreenMirrorRefreshRatePerSecond));
            }
        }
        public string SelectedMonitor { get; set; }

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

                _parent.ScreenMirrorAlgorithmChanged(_selectedScreenMirrorAlgorithm);

                OnPropertyChanged(nameof(SelectedScreenMirrorAlgorithm));
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

                var screenMirrorAlgorithm = AlgorithmPerDevice[_selectedDevice];
                SelectedScreenMirrorAlgorithm = ScreenMirrorAlgorithmMapping.FirstOrDefault(map => map.Value == screenMirrorAlgorithm).Key;
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

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
