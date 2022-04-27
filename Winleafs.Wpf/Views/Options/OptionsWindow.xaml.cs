﻿using Microsoft.Win32;
using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Winleafs.External;
using Winleafs.Models;
using Winleafs.Models.Effects;
using Winleafs.Models.Enums;
using Winleafs.Server;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.ViewModels;
using Winleafs.Wpf.Views.MainWindows;
using Winleafs.Wpf.Views.Popup;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;

namespace Winleafs.Wpf.Views.Options
{

    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel OptionsViewModel { get; set; }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private RegistryKey _startupKey;

        private static readonly Dictionary<string, string> _languageDictionary =
            new Dictionary<string, string>() { { "Nederlands", "nl" }, { "English", "en" }, };

        private bool _visualizationOpen;

        private readonly MainWindow _mainWindow;
        private readonly List<string> _monitorNames;
        private readonly WinleafsServerClient _winleafsServerClient;

        public OptionsWindow(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;

            InitializeComponent();

            //Initialize the viewmodel
            _monitorNames = ScreenBoundsHelper.GetMonitorNames();

            OptionsViewModel = new OptionsViewModel(this)
            {
                ScreenMirrorAlgorithmPerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorAlgorithm),
                ScreenMirrorFlipPerDevice = UserSettings.Settings.Devices.ToDictionary(d => d.Name, d => d.ScreenMirrorFlip),
                ScreenMirrorRefreshRatePerSecond = UserSettings.Settings.ScreenMirrorRefreshRatePerSecond,
                SelectedMonitor = _monitorNames.ElementAt(UserSettings.Settings.ScreenMirrorMonitorIndex),
                DeviceNames = new ObservableCollection<string>(UserSettings.Settings.Devices.Select(d => d.Name)),
                MonitorNames = _monitorNames,
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture),
                SelectedLanguage = FullNameForCulture(UserSettings.Settings.UserLocale),
                Languages = _languageDictionary.Keys.ToList(),
                MinimizeToSystemTray = UserSettings.Settings.MinimizeToSystemTray,
                CustomColorEffects = UserSettings.Settings.CustomEffects == null ? new List<UserCustomColorEffect>() : UserSettings.Settings.CustomEffects.ToList(),
                WinleafsServerURL = UserSettings.Settings.WinleafServerURL,
                ProcessResetIntervalText = UserSettings.Settings.ProcessResetIntervalInSeconds.ToString()
            };

            foreach (var customEffects in OptionsViewModel.CustomColorEffects)
            {
                ColorList.Children.Add(new ColorUserControl(this, customEffects.EffectName, customEffects.Color));
            }

            OptionsViewModel.SelectedDevice = UserSettings.Settings.ActiveDevice.Name; //Set this one last since it triggers changes in properties

            //Initialize variables
            _startupKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            _visualizationOpen = false;

            foreach (var device in UserSettings.Settings.Devices)
            {
                DeviceList.Children.Add(new DeviceUserControl(device.Name, this));
            }

            //Draw monitors
            DrawMonitors();

            //Check Spotify connection
            _winleafsServerClient = new WinleafsServerClient();
            InitializeSpotifyButtons();

            //last: set datacontext
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
                var screenMirrorFlip = OptionsViewModel.ScreenMirrorFlipMapping[OptionsViewModel.SelectedScreenMirrorFlip];

                var monitorIndex = Array.IndexOf(_monitorNames.ToArray(), OptionsViewModel.SelectedMonitor);

                var device = UserSettings.Settings.Devices.FirstOrDefault(d => d.Name == OptionsViewModel.SelectedDevice);

                var visualizeWindow = new ScreenMirrorVisualizationWindow(device, monitorIndex, screenMirrorAlgorithm, screenMirrorFlip);
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
                    //Replace .dll with .exe since in .net core 3 the current executing assembly is the dll
                    _startupKey.SetValue(UserSettings.APPLICATIONNAME, $"{System.Reflection.Assembly.GetExecutingAssembly().Location.Replace(".dll", ".exe")} -s");
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
            foreach (var device in UserSettings.Settings.Devices)
            {
                device.ScreenMirrorAlgorithm = OptionsViewModel.ScreenMirrorAlgorithmPerDevice[device.Name];
                device.ScreenMirrorFlip = OptionsViewModel.ScreenMirrorFlipPerDevice[device.Name];
            }

            UserSettings.Settings.ScreenMirrorMonitorIndex = Array.IndexOf(_monitorNames.ToArray(), OptionsViewModel.SelectedMonitor);
            UserSettings.Settings.ScreenMirrorRefreshRatePerSecond = OptionsViewModel.ScreenMirrorRefreshRatePerSecond;

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

            #region Colors

            var deletedColors = UserSettings.Settings.CustomEffects?.Except(OptionsViewModel.CustomColorEffects).ToList();

            UserSettings.Settings.CustomEffects = OptionsViewModel.CustomColorEffects;

            //Remove invalid triggers from the schedules
            if (deletedColors?.Any() == true)
            {
                UserSettings.Settings.DeleteTriggers(deletedColors.Select(color =>
                    UserCustomColorEffect.DisplayName(color.EffectName)));
            }

            #endregion Colors

            #region ProcessResetInterval
            if (OptionsViewModel.ProcessResetIntervalText != UserSettings.Settings.ProcessResetIntervalInSeconds.ToString())
            {
                try
                {
                    var processResetIntervalInSeconds = Convert.ToInt32(OptionsViewModel.ProcessResetIntervalText);

                    if (processResetIntervalInSeconds <= 0)
                    {
                        //Just throw an empty exception here, it is not logged
                        throw new Exception();
                    }

                    UserSettings.Settings.ProcessResetIntervalInSeconds = processResetIntervalInSeconds;
                }
                catch
                {
                    PopupCreator.Error(Options.Resources.ProcessResetIntervalError);
                    return;
                }
            }
            #endregion

            #region Advanced
            if (!string.IsNullOrWhiteSpace(OptionsViewModel.WinleafsServerURL))
            {
                UserSettings.Settings.WinleafServerURL = OptionsViewModel.WinleafsServerURL;
            }
            #endregion

            UserSettings.Settings.SaveSettings();

            //Reload the orchestrator so custom effects are reloaded.
            OrchestratorCollection.ResetOrchestrators();

            //Reload effects such that custom effects are updated in the view
            _mainWindow.ReloadEffectsInView();

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

        private void ConnectToSpotify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _winleafsServerClient.SpotifyEndpoint.Connect();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unknown error when trying to connect to Spotify");
                PopupCreator.Error(Options.Resources.SpotifyUnknownError);
            }
        }

        private void DisconnectFromSpotify_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _winleafsServerClient.SpotifyEndpoint.Disconnect();
                PopupCreator.Success(Options.Resources.DisconnectSuccessful, true);

                DisconnectFromSpotifyButton.Visibility = Visibility.Hidden;
                ConnectToSpotifyButton.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unknown error when trying to connect to Spotify");
                PopupCreator.Error(Options.Resources.SpotifyUnknownError);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            MainWindow.OpenURL(e.Uri.AbsoluteUri);
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

            if (UserSettings.Settings.CustomEffects != null && UserSettings.Settings.CustomEffects.Any(effect => effect.EffectName == name))
            {
                PopupCreator.Error(Options.Resources.NameTaken);
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                PopupCreator.Error(Options.Resources.NoNameProvided);
                return;
            }

            if (color == null)
            {
                PopupCreator.Error(Options.Resources.NoColorSelected);
                return;
            }

            var customEffect = new UserCustomColorEffect()
            {
                Color = Color.FromArgb(color.Value.A, color.Value.R, color.Value.G, color.Value.B),
                EffectName = name
            };

            if (OptionsViewModel.CustomColorEffects == null)
            {
                OptionsViewModel.CustomColorEffects = new List<UserCustomColorEffect>();
            }

            // Add color in settings.
            OptionsViewModel.CustomColorEffects.Add(customEffect);
            
            // Add color to UI.
            ColorList.Children.Add(new ColorUserControl(this, customEffect.EffectName, customEffect.Color));
        }

        public void DeleteColor(string description, UIElement child)
        {
            // Remove color out of the settings.
            var toDeleteEffect = OptionsViewModel.CustomColorEffects.FirstOrDefault(effect => effect.EffectName == description);
            OptionsViewModel.CustomColorEffects.Remove(toDeleteEffect);
            
            // Remove color from the list.
            ColorList.Children.Remove(child);
            
        }

        public void DeleteDevice(string deviceName)
        {
            var messageBoxResult = MessageBox.Show(string.Format(Options.Resources.AreYouSureDeviceDeletion, deviceName), MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                //Start deletion process in main window then close this window
                _mainWindow.DeleteDevice(deviceName);

                Close();
            }
        }

        private void InitializeSpotifyButtons()
        {
            try
            {
                if (_winleafsServerClient.SpotifyEndpoint.IsConnected())
                {
                    ConnectToSpotifyButton.Visibility = Visibility.Hidden;
                }
                else
                {
                    DisconnectFromSpotifyButton.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                //Winleafs server is offline/unreachable, disable both buttons and show the error message
                Logger.Error(ex, "Unknown error when trying to connect to Spotify");
                ConnectToSpotifyButton.Visibility = Visibility.Hidden;
                DisconnectFromSpotifyButton.Visibility = Visibility.Hidden;
                SpotifyUnknownErrorLabel.Visibility = Visibility.Visible;
            }            
        }

        private void DrawMonitors()
        {
            var monitorBounds = new List<System.Drawing.Rectangle>();
            for (var i = 0; i < _monitorNames.Count; i++)
            {
                monitorBounds.Add(ScreenBoundsHelper.GetScreenBounds(i));
            }

            var minX = monitorBounds.Min(bounds => bounds.X);
            var minY = monitorBounds.Min(bounds => bounds.Y);

            //Normalize all X and Y coordinates such that the left most bottom monitor start at 0,0
            for (var i = 0; i < monitorBounds.Count; i++)
            {
                var rectangle = monitorBounds[i];

                var x = rectangle.X;
                if (minX > 0)
                {
                    x = rectangle.X - minX;
                }
                else if (minX < 0)
                {
                    x = rectangle.X + Math.Abs(minX);
                }

                var y = rectangle.Y;
                if (minY > 0)
                {
                    y = rectangle.Y - minY;
                }
                else if (minY < 0)
                {
                    y = rectangle.Y + Math.Abs(minY);
                }

                monitorBounds[i] = new System.Drawing.Rectangle(x, y, rectangle.Width, rectangle.Height);
            }

            var maxMonitorWidth = monitorBounds.Max(bounds => bounds.X + bounds.Width);
            var maxMonitorHeight = monitorBounds.Max(bounds => bounds.Y + bounds.Height);

            var scale = Math.Max(maxMonitorWidth / MonitorCanvas.Width, maxMonitorHeight / MonitorCanvas.Height);

            for (var i = 0; i < monitorBounds.Count; i++)
            {
                //Draw a polygon in the shape of the monitor, scaled down
                var polygon = new Polygon();

                var scaledX = monitorBounds[i].X / scale;
                var scaledY = monitorBounds[i].Y / scale;
                var scaledXWidth = (monitorBounds[i].X + monitorBounds[i].Width) / scale;
                var scaledYHeight = (monitorBounds[i].Y + monitorBounds[i].Height) / scale;
                
                polygon.Points.Add(new System.Windows.Point(scaledX, scaledY)); //Left top
                polygon.Points.Add(new System.Windows.Point(scaledX, scaledYHeight)); //Left bottom
                polygon.Points.Add(new System.Windows.Point(scaledXWidth, scaledYHeight)); //Right bottom
                polygon.Points.Add(new System.Windows.Point(scaledXWidth, scaledY)); //Right top

                polygon.Stroke = Brushes.LightGray;
                polygon.StrokeThickness = 3;

                MonitorCanvas.Children.Add(polygon);

                //Draw the number of the monitor
                var textBlock = new TextBlock();

                textBlock.Text = (i + 1).ToString(); //+ 1 since i starts at 0
                textBlock.Foreground = Brushes.LightGray;
                textBlock.FontSize = 20;

                Canvas.SetLeft(textBlock, ((monitorBounds[i].X + (monitorBounds[i].Width / 2)) / scale) - 5); //-5 and -15 to position the numbers more to the center
                Canvas.SetTop(textBlock, ((monitorBounds[i].Y + (monitorBounds[i].Height / 2)) / scale) - 15);

                MonitorCanvas.Children.Add(textBlock);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //In the case the user has a schedule with Spotify events, lost connection and just reconnected using the Options window,
            //Reinitialize the SpotifyEventTimer. Call this function on all close events, even if the user click cancel or the X button.
            //Initialize also stops the timer if there is no connection, in case the user just disconnected.
            SpotifyEventTimer.Initialize();
        }
    }
}
