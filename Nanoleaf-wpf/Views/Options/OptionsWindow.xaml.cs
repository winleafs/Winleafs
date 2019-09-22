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
using Winleafs.Wpf.Helpers;
using Winleafs.Models.Enums;
using System.Windows.Media;
using System.Drawing;
using Winleafs.Models.Models.Effects;
using Xceed.Wpf.Toolkit;

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

        private bool _visualizationOpen;

        public OptionsWindow()
        {
            InitializeComponent();

            var monitorNames = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets().Select(m => m.FriendlyName).ToList();

            OptionsViewModel = new OptionsViewModel(this)
            {
                AlgorithmPerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorAlgorithm),
                ScreenMirrorControlBrightnessPerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorControlBrightness),
                ScreenMirrorRefreshRatePerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorRefreshRatePerSecond),
                DeviceNames = new ObservableCollection<string>(UserSettings.Settings.Devices.Select(d => d.Name)),
                MonitorNames = monitorNames,
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture),
                SelectedLanguage = FullNameForCulture(UserSettings.Settings.UserLocale),
                Languages = _languageDictionary.Keys.ToList(),
                MinimizeToSystemTray = UserSettings.Settings.MinimizeToSystemTray
            };

            //Setup MonitorPerDevice with necessairy checks
            var monitorPerDevice = new Dictionary<string, string>();
            foreach (var device in UserSettings.Settings.Devices)
            {
                if (monitorNames.Count <= device.ScreenMirrorMonitorIndex)
                {
                    // It is possible that the user adjusted his/her screen setup and no longer has the monitor the device is set to
                    monitorPerDevice.Add(device.Name, monitorNames.FirstOrDefault());
                }
                else
                {
                    monitorPerDevice.Add(device.Name, monitorNames[device.ScreenMirrorMonitorIndex]);
                }
            }

            OptionsViewModel.MonitorPerDevice = monitorPerDevice;
            OptionsViewModel.SelectedDevice = UserSettings.Settings.ActiveDevice.Name; //Set this one last since it triggers changes in properties

            _startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            _visualizationOpen = false;

            DataContext = OptionsViewModel;
        }

        public void ScreenMirrorAlgorithmChanged(ScreenMirrorAlgorithm selectedScreenMirrorAlgorithm)
        {
            if (selectedScreenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit || selectedScreenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorStretch)
            {
                VisualizeButton.IsEnabled = true;
                VisualizeButton.Foreground = Brushes.White;
            }
            else
            {
                VisualizeButton.IsEnabled = false;
                VisualizeButton.Foreground = Brushes.Gray;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Visualize_Click(object sender, RoutedEventArgs e)
        {
            if (!_visualizationOpen)
            {
                var screenMirrorAlgorithm = OptionsViewModel.ScreenMirrorAlgorithmMapping[OptionsViewModel.SelectedScreenMirrorAlgorithm];

                var monitorNames = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets().Select(m => m.FriendlyName).ToArray();
                var monitorIndex = Array.IndexOf(monitorNames, OptionsViewModel.SelectedMonitor);

                var device = UserSettings.Settings.Devices.FirstOrDefault(d => d.Name == OptionsViewModel.SelectedDevice);

                var visualizeWindow = new ScreenMirrorVisualizationWindow(device, monitorIndex, screenMirrorAlgorithm);
                visualizeWindow.Show();

                var scale = ScreenParameters.GetScreenScaleFactorNonDpiAware(visualizeWindow);

                visualizeWindow.Visualize(scale);
                visualizeWindow.Closed += (sender2, eventArgs) => _visualizationOpen = false; //Set the bool to false when the window is closed

                _visualizationOpen = true;
            }                     
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

            if ((latitude != UserSettings.Settings.Latitude || longitude != UserSettings.Settings.Longitude) && (latitude != 0 && longitude != 0))
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
            var monitorNames = WindowsDisplayAPI.DisplayConfig.PathDisplayTarget.GetDisplayTargets().Select(m => m.FriendlyName).ToArray();

            foreach (var device in UserSettings.Settings.Devices)
            {
                device.ScreenMirrorAlgorithm = OptionsViewModel.AlgorithmPerDevice[device.Name];
                device.ScreenMirrorRefreshRatePerSecond = OptionsViewModel.ScreenMirrorRefreshRatePerDevice[device.Name];
                device.ScreenMirrorControlBrightness = OptionsViewModel.ScreenMirrorControlBrightnessPerDevice[device.Name];
                device.ScreenMirrorMonitorIndex = Array.IndexOf(monitorNames, OptionsViewModel.MonitorPerDevice[device.Name]);
            }
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

        private void AddColor_Click(object sender, RoutedEventArgs e)
        {
            var color = ColorPicker.SelectedColor;
            var name = EffectTextBox.Text;

            if (UserSettings.Settings.CustomEffects.Any(effect => effect.EffectName == name))
            {
                PopupCreator.Error("Name already taken.");
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                PopupCreator.Error("No name provided.");
                return;
            }

            if (color == null)
            {
                PopupCreator.Error("No color selected.");
                return;
            }

            var customEffect = new UserCustomColorEffect()
            {
                Color = Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B),
                EffectName = name
            };

            UserSettings.Settings.CustomEffects.Add(customEffect);
            UserSettings.Settings.SaveSettings();
        }
    }
}
