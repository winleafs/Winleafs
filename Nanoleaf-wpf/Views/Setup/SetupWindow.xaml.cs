using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using Winleafs.Api;

using Winleafs.Models.Models;

using Tmds.MDns;

using Winleafs.Wpf.ViewModels;

namespace Winleafs.Wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private SetupViewModel setupViewModel;
        private List<Device> discoveredDevices;
        private NanoleafClient nanoleafClient;
        private Device selectedDevice;

        public SetupWindow()
        {
            InitializeComponent();

            setupViewModel = new SetupViewModel();
            discoveredDevices = new List<Device>();

            ServiceBrowser serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += onServiceAdded;
            serviceBrowser.StartBrowse("_nanoleafapi._tcp");
        }

        public void Finish_Click(object sender, RoutedEventArgs e)
        {
            selectedDevice.Name = setupViewModel.Name;
            selectedDevice.ActiveInGUI = true;

            UserSettings.Settings.AddDevice(selectedDevice);

            App.NormalStartup(null);

            Close();
        }

        public void Pair_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Pair());
        }

        private async void Pair()
        {
            try
            {
                var authToken = await nanoleafClient.AuthorizationEndpoint.GetAuthToken();
                var effects = await nanoleafClient.EffectsEndpoint.GetEffectsListAsync();

                Dispatcher.Invoke(() =>
                {
                    selectedDevice.AuthToken = authToken;
                    selectedDevice.LoadEffectsFromNameList(effects);

                    AuthorizeDevice.Visibility = Visibility.Hidden;
                    NameDevice.Visibility = Visibility.Visible;
                });
            }
            catch
            {
                MessageBox.Show("Something went wrong when connecting to your lights. Please follow the steps carefully.");
            }
        }

        private void onServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            discoveredDevices.Add(new Device { Name = e.Announcement.Hostname, IPAddress = e.Announcement.Addresses.First().ToString(), Port = e.Announcement.Port });
            BuildDeviceList();
        }

        private void BuildDeviceList()
        {
            Dispatcher.Invoke(() =>
            {
                setupViewModel.Devices.Clear();

                foreach (var device in discoveredDevices)
                {
                    setupViewModel.Devices.Add(device);
                }
            });
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoverDevice.Devices.SelectedItem != null)
            {
                AuthorizeDevice.Visibility = Visibility.Visible;
                DiscoverDevice.Visibility = Visibility.Hidden;

                selectedDevice = (Device)DiscoverDevice.Devices.SelectedItem;

                nanoleafClient = new NanoleafClient(selectedDevice.IPAddress, selectedDevice.Port);
            }
        }

        private void AuthorizeDevice_Loaded(object sender, RoutedEventArgs e)
        {
            AuthorizeDevice.ParentWindow = this;
            AuthorizeDevice.DataContext = setupViewModel;
        }

        private void DiscoverDevice_Loaded(object sender, RoutedEventArgs e)
        {
            DiscoverDevice.ParentWindow = this;
            DiscoverDevice.DataContext = setupViewModel;
        }

        private void NameDevice_Loaded(object sender, RoutedEventArgs e)
        {
            NameDevice.ParentWindow = this;
            NameDevice.DataContext = setupViewModel;
        }
    }
}
