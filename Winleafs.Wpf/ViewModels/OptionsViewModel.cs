﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Winleafs.Models.Effects;
using Winleafs.Models.Enums;
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

        public string WinleafsServerURL { get; set; }

        public string ProcessResetIntervalText { get; set; }
        #endregion

        #region Screen mirror
        public Dictionary<string, ScreenMirrorAlgorithm> ScreenMirrorAlgorithmPerDevice { get; set; }

        public Dictionary<string, ScreenMirrorFlip> ScreenMirrorFlipPerDevice { get; set; }

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

                ScreenMirrorAlgorithmPerDevice[_selectedDevice] = _selectedScreenMirrorAlgorithm;

                _parent.ScreenMirrorAlgorithmChanged(_selectedScreenMirrorAlgorithm);

                OnPropertyChanged(nameof(SelectedScreenMirrorAlgorithm));
            }
        }

        public IEnumerable<string> ScreenMirrorAlgorithms => ScreenMirrorAlgorithmMapping.Keys;
        #endregion

        #region Screen mirror flip dropdown
        //Map display values to enum values
        public Dictionary<string, ScreenMirrorFlip> ScreenMirrorFlipMapping { get; set; }

        private ScreenMirrorFlip _selectedScreenMirrorFlip;
        public string SelectedScreenMirrorFlip
        {
            get { return EnumLocalizer.GetLocalizedEnum(_selectedScreenMirrorFlip); }
            set
            {
                _selectedScreenMirrorFlip = ScreenMirrorFlipMapping[value];

                ScreenMirrorFlipPerDevice[_selectedDevice] = _selectedScreenMirrorFlip;

                OnPropertyChanged(nameof(SelectedScreenMirrorFlip));
            }
        }

        public IEnumerable<string> ScreenMirrorFlips => ScreenMirrorFlipMapping.Keys;
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

                var screenMirrorAlgorithm = ScreenMirrorAlgorithmPerDevice[_selectedDevice];
                SelectedScreenMirrorAlgorithm = ScreenMirrorAlgorithmMapping.FirstOrDefault(map => map.Value == screenMirrorAlgorithm).Key;

                var screenMirrorFlip = ScreenMirrorFlipPerDevice[_selectedDevice];
                SelectedScreenMirrorFlip = ScreenMirrorFlipMapping.FirstOrDefault(map => map.Value == screenMirrorFlip).Key;
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

            ScreenMirrorFlipMapping = new Dictionary<string, ScreenMirrorFlip>()
            {
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorFlip.None), ScreenMirrorFlip.None },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorFlip.Horizontal), ScreenMirrorFlip.Horizontal },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorFlip.Vertical), ScreenMirrorFlip.Vertical },
                {  EnumLocalizer.GetLocalizedEnum(ScreenMirrorFlip.HorizontalVertical), ScreenMirrorFlip.HorizontalVertical }
            };
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
