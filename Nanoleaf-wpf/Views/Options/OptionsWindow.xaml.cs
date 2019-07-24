using System;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using Microsoft.Win32;
using Winleafs.External;
using Winleafs.Models.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Navigation;
using Winleafs.Wpf.Views.Popup;
using Winleafs.Wpf.ViewModels;
using System.Collections.ObjectModel;

namespace Winleafs.Wpf.Views.Options
{

    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel OptionsViewModel { get; set; }

        private RegistryKey _startupKey;

        private static readonly Dictionary<string, string> _languageDictionary =
            new Dictionary<string, string>() { { "Nederlands", "nl" }, { "English", "en" }, };

        public OptionsWindow()
        {
            InitializeComponent();

            var monitors = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets();

            OptionsViewModel = new OptionsViewModel(this)
            {
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture),
                ScreenMirrorRefreshRatePerSecond = UserSettings.Settings.ScreenMirrorRefreshRatePerSecond,
                ScreenMirrorControlBrightness = UserSettings.Settings.ScreenMirrorControlBrightness,
                MonitorNames = monitors.Select(m => m.FriendlyName).ToList(),
                SelectedMonitor = monitors[UserSettings.Settings.ScreenMirrorMonitorIndex].FriendlyName,
                SelectedLanguage = FullNameForCulture(UserSettings.Settings.UserLocale),
                Languages = _languageDictionary.Keys.ToList(),
                MinimizeToSystemTray = UserSettings.Settings.MinimizeToSystemTray,
                SelectedDevice = UserSettings.Settings.ActiveDevice.Name,
                ScreenMirrorAlgorithm = UserSettings.Settings.ActiveDevice.ScreenMirrorAlgorithm,
                DeviceNames = new ObservableCollection<string>(UserSettings.Settings.Devices.Select(d => d.Name)),
                AlgorithmPerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorAlgorithm)
            };

            _startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            DataContext = OptionsViewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            #region LatLong

            double latitude = 0;
            double longitude = 0;
            try
            {
                if (!string.IsNullOrWhiteSpace(OptionsViewModel.Latitude))
                {
                    latitude = Convert.ToDouble(OptionsViewModel.Latitude, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                PopupCreator.Error(Options.Resources.InvalidLatitude);
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(OptionsViewModel.Longitude))
                {
                    longitude = Convert.ToDouble(OptionsViewModel.Longitude, CultureInfo.InvariantCulture);
                }
            }
            catch
            {
                PopupCreator.Error(Options.Resources.InvalidLongitude);
                return;
            }

            if ((latitude != UserSettings.Settings.Latitude || longitude != UserSettings.Settings.Longitude) && (latitude != 0  && longitude != 0))
            {
                var client = new SunsetSunriseClient();

                try
                {
                    var sunTimes = client.GetSunsetSunriseAsync(latitude, longitude).GetAwaiter().GetResult();

                    UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
                }
                catch
                {
                    PopupCreator.Error(Options.Resources.SunsetSunriseError);
                    return;
                }

                UserSettings.Settings.Latitude = latitude;
                UserSettings.Settings.Longitude = longitude;
            }
            #endregion

            #region StartAtWindowsStartup
            if (UserSettings.Settings.StartAtWindowsStartup != OptionsViewModel.StartAtWindowsStartUp)
            {
                if (OptionsViewModel.StartAtWindowsStartUp)
                {
                    _startupKey.SetValue(UserSettings.APPLICATIONNAME, $"{System.Reflection.Assembly.GetExecutingAssembly().Location} -s");
                }
                else
                {
                    _startupKey.DeleteValue(UserSettings.APPLICATIONNAME, false);
                }

                _startupKey.Close();

                UserSettings.Settings.StartAtWindowsStartup = OptionsViewModel.StartAtWindowsStartUp;
            }
            #endregion

            #region ScreenMirror
            #region General
            UserSettings.Settings.ScreenMirrorRefreshRatePerSecond = OptionsViewModel.ScreenMirrorRefreshRatePerSecond;
            UserSettings.Settings.ScreenMirrorControlBrightness = OptionsViewModel.ScreenMirrorControlBrightness;

            var monitors = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets();
            var selectedMonitor = monitors.FirstOrDefault(m => m.FriendlyName.Equals(OptionsViewModel.SelectedMonitor));

            UserSettings.Settings.ScreenMirrorMonitorIndex = Array.IndexOf(monitors, selectedMonitor);
            #endregion

            #region Device
            foreach (var selectedAlgorithm in OptionsViewModel.AlgorithmPerDevice)
            {
                UserSettings.Settings.Devices.FirstOrDefault(d => d.Name.Equals(selectedAlgorithm.Key)).ScreenMirrorAlgorithm = selectedAlgorithm.Value;
            }
            #endregion
            #endregion

            #region Language

            if (OptionsViewModel.SelectedLanguage != null)
            {
                UserSettings.Settings.UserLocale = _languageDictionary[OptionsViewModel.SelectedLanguage];
            }

            #endregion

            #region MinimizeToSystemTray
            UserSettings.Settings.MinimizeToSystemTray = OptionsViewModel.MinimizeToSystemTray;
            #endregion

            UserSettings.Settings.SaveSettings();
            Close();
        }

        private void GeoIp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var geoIpClient = new GeoIpClient();
                var geoIpData = geoIpClient.GetGeoIpData();
                OptionsViewModel.Latitude = geoIpData.Latitude.ToString("N7", CultureInfo.InvariantCulture);
                OptionsViewModel.Longitude = geoIpData.Longitude.ToString("N7", CultureInfo.InvariantCulture);

                LatitudeTextBox.Text = OptionsViewModel.Latitude;
                LongitudeTextBox.Text = OptionsViewModel.Longitude;

                PopupCreator.Success(string.Format(Options.Resources.LocationDetected, geoIpData.City, geoIpData.Country));
            }
            catch
            {

                PopupCreator.Error(Options.Resources.LatLongReceiveError);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private string FullNameForCulture(string text)
        {
            foreach (var key in _languageDictionary.Keys)
            {
                if (_languageDictionary[key].Equals(text))
                {
                    return key;
                }
            }

            return null;
        }
    }
}
