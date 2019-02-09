using System;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Win32;

using Winleafs.Api.Endpoints;

using Winleafs.Models.Models;

using Winleafs.Wpf.ViewModels;

namespace Winleafs.Wpf.Views.Options
{
    using System.Diagnostics;
    using System.Windows.Navigation;
    using Winleafs.Api;

    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel OptionsViewModel { get; set; }

        private RegistryKey _startupKey;

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
                SelectedMonitor = monitors[UserSettings.Settings.AmbilightMonitorIndex].DeviceName
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
                latitude = Convert.ToDouble(OptionsViewModel.Latitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                System.Windows.MessageBox.Show("Please enter a valid value for latitude");
                return;
            }

            try
            {
                longitude = Convert.ToDouble(OptionsViewModel.Longitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                System.Windows.MessageBox.Show("Please enter a valid value for longitude");
                return;
            }

            if (latitude != UserSettings.Settings.Latitude || longitude != UserSettings.Settings.Longitude)
            {
                var endpoint = new SunsetEndpoint();

                try
                {
                    var sunTimes = endpoint.GetSunsetSunriseAsync(latitude, longitude).GetAwaiter().GetResult();

                    UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
                }
                catch
                {
                    System.Windows.MessageBox.Show("Something went wrong when updating the sunrise and sunset times");
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

           UserSettings.Settings.SaveSettings();
            Close();
        }

        private void GeoIp_Click(object sender, RoutedEventArgs e)
        {
            var nanoleafClient = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActviceDevice);
            var geoIpData = nanoleafClient.GeoIpEndpoint.GetGeoIpData();
            OptionsViewModel.Latitude = geoIpData.Latitude.ToString("N7", CultureInfo.InvariantCulture);
            OptionsViewModel.Longitude = geoIpData.Longitude.ToString("N7", CultureInfo.InvariantCulture);

            DataContext = OptionsViewModel;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
