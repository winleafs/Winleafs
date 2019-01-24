using Nanoleaf_Api.Endpoints;
using Nanoleaf_Models.Models;
using Nanoleaf_wpf.ViewModels;
using System;
using System.Globalization;
using System.Windows;

namespace Nanoleaf_wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel OptionsViewModel { get; set; }

        public OptionsWindow()
        {
            InitializeComponent();

            OptionsViewModel = new OptionsViewModel
            {
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7", CultureInfo.InvariantCulture),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7", CultureInfo.InvariantCulture)
            };

            DataContext = OptionsViewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            double latitude = 0;
            double longitude = 0;
            try
            {
                latitude = Convert.ToDouble(OptionsViewModel.Latitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for latitude");
                return;
            }

            try
            {
                longitude = Convert.ToDouble(OptionsViewModel.Longitude, CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Please enter a valid value for longitude");
                return;
            }

            if (latitude != UserSettings.Settings.Latitude || longitude != UserSettings.Settings.Longitude)
            {
                var endpoint = new SunsetEndpoint();

                try
                {
                    var sunTimes = endpoint.GetSunsetSunrise(latitude, longitude).GetAwaiter().GetResult();

                    UserSettings.Settings.UpdateSunriseSunset(sunTimes.SunriseHour, sunTimes.SunriseMinute, sunTimes.SunsetHour, sunTimes.SunsetMinute);
                }
                catch
                {
                    MessageBox.Show("Something went wrong when updating the sunrise and sunset times");
                    return;
                }

                UserSettings.Settings.Latitude = latitude;
                UserSettings.Settings.Longitude = longitude;
            }

            if (UserSettings.Settings.StartAtWindowsStartup != OptionsViewModel.StartAtWindowsStartUp)
            {
                //Enable/disable windows startup here

                UserSettings.Settings.StartAtWindowsStartup = OptionsViewModel.StartAtWindowsStartUp;
            }

            UserSettings.Settings.SaveSettings();
            Close();
        }
    }
}
