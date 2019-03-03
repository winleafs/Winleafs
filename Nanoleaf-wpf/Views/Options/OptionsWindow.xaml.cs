using System;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;

using Winleafs.Api.Endpoints;
using Winleafs.External;
using Winleafs.Models.Models;

using Winleafs.Wpf.ViewModels;

namespace Winleafs.Wpf.Views.Options
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Navigation;
    using Winleafs.Api;
    using Winleafs.Wpf.Views.Popup;

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

            var monitors = Screen.AllScreens;

            OptionsViewModel = new OptionsViewModel
            {
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture),
                AmbilightRefreshRatePerSecond = UserSettings.Settings.AmbilightRefreshRatePerSecond,
                MonitorNames = monitors.Select(m => m.DeviceName).ToList(),
                SelectedMonitor = monitors[UserSettings.Settings.AmbilightMonitorIndex].DeviceName,
                SelectedLanguage = FullNameForCulture(UserSettings.Settings.UserLocale),
                Languages = _languageDictionary.Keys.ToList()
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
                PopupCreator.CreateErrorPopup(Options.Resources.InvalidLatitude);
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
                PopupCreator.CreateErrorPopup(Options.Resources.InvalidLongitude);
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
                    PopupCreator.CreateErrorPopup(Options.Resources.SunsetSunriseError);
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

            #region Ambilight
            UserSettings.Settings.AmbilightRefreshRatePerSecond = OptionsViewModel.AmbilightRefreshRatePerSecond;

            var monitors = Screen.AllScreens;
            var selectedMonitor = monitors.FirstOrDefault(m => m.DeviceName.Equals(OptionsViewModel.SelectedMonitor));

            UserSettings.Settings.AmbilightMonitorIndex = Array.IndexOf(monitors, selectedMonitor);
            #endregion

            #region Language

            UserSettings.Settings.UserLocale = _languageDictionary[OptionsViewModel.SelectedLanguage];
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

                PopupCreator.CreateSuccessPopup(string.Format(Options.Resources.LocationDetected, geoIpData.City, geoIpData.Country));
            }
            catch
            {

                PopupCreator.CreateErrorPopup(Options.Resources.LatLongReceiveError);
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
