﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Tmds.MDns;

using Winleafs.Wpf.ViewModels;
using NLog;
using Winleafs.Models;
using Winleafs.Nanoleaf;
using Winleafs.Orchestration;

namespace Winleafs.Wpf.Views.Setup
{
    using Winleafs.Models.Enums;
    using Winleafs.Wpf.Views.MainWindows;
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private SetupViewModel setupViewModel;
        private List<Device> discoveredDevices;
        private NanoleafClient nanoleafClient;
        private Device selectedDevice;
        private MainWindow _parent;

        public SetupWindow(MainWindow parent = null)
        {
            _parent = parent;

            InitializeComponent();

            setupViewModel = new SetupViewModel();
            discoveredDevices = new List<Device>();

            ServiceBrowser serviceBrowser = new ServiceBrowser();
            serviceBrowser.ServiceAdded += onServiceAdded;
            serviceBrowser.StartBrowse("_nanoleafapi._tcp");
        }

        public void Finish_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(setupViewModel.Name))
            {
                PopupCreator.Error(Setup.Resources.NameCannotBeEmpty);
                return;
            }
            else if (UserSettings.HasSettings() && UserSettings.Settings.Devices.Any(d => d.Name.ToLower().Equals(setupViewModel.Name)))
            {
                PopupCreator.Error(Setup.Resources.NameAlreadyExists);
                return;
            }

            selectedDevice.Name = setupViewModel.Name;

            UserSettings.Settings.AddDevice(selectedDevice);

            _logger.Info($"Successfully added device {selectedDevice.Name}");

            if (_parent != null)
            {
                OrchestratorCollection.AddOrchestratorForDevice(selectedDevice);
                _parent.DeviceAdded(selectedDevice);
                _parent.SelectedDevice = selectedDevice.Name;
            }
            else
            {
                UserSettings.Settings.SetActiveDevice(selectedDevice.Name); //The first added device must always be active
                App.NormalStartup(null);
            }

            Close();
        }

        public void Pair_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Pair());
        }

        private async Task Pair()
        {
            try
            {
                var authToken = await nanoleafClient.AuthorizationEndpoint.GetAuthTokenAsync();

                await nanoleafClient.IdentifyEndpoint.Identify();

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
                PopupCreator.Error(Setup.Resources.UnknownError);
            }
        }

        private void onServiceAdded(object sender, ServiceAnnouncementEventArgs e)
        {
            if (!UserSettings.HasSettings() || !UserSettings.Settings.Devices.Any(d => d.IPAddress.Equals(e.Announcement.Addresses.First().ToString())))
            { //Only add devices that not have been added before
                _logger.Info($"Discovered following device: {e.Announcement.Hostname}, IPs: {string.Join(",", e.Announcement.Addresses.Select(ip => ip.ToString()))}, Port: {e.Announcement.Port}");

                discoveredDevices.Add(new Device
                {
                    Name = e.Announcement.Hostname,
                    IPAddress = e.Announcement.Addresses.First().ToString(),
                    Port = e.Announcement.Port
                });
            }

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
            if (!UserSettings.HasSettings() || UserSettings.Settings.Devices.Count == 0)
            { //If the user has no settings or devices, cancel click is a shutdown
                Application.Current.Shutdown();
            }
            else
            {
                Close();
            }
        }

        public void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (DiscoverDevice.Devices.SelectedItem != null)
            {
                AuthorizeDevice.Visibility = Visibility.Visible;
                DiscoverDevice.Visibility = Visibility.Hidden;

                selectedDevice = (Device)DiscoverDevice.Devices.SelectedItem;
				_logger.Info($"Selected following device: {selectedDevice.IPAddress}:{selectedDevice.Port}");

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
