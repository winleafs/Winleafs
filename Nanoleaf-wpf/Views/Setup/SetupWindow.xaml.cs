using Nanoleaf_wpf.Models;
using Nanoleaf_wpf.Network;
using Nanoleaf_wpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Nanoleaf_wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private SetupViewModel setupViewModel;

        public SetupWindow()
        {
            InitializeComponent();

            setupViewModel = new SetupViewModel();

            UpdateDevices();
        }

        private void DiscoverDevice_Loaded(object sender, RoutedEventArgs e)
        {
            DiscoverDevice.ParentWindow = this;
            DiscoverDevice.DataContext = setupViewModel;
        }

        public void Pair_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {
            UpdateDevices();
        }

        private void UpdateDevices()
        {
            Task.Run(ScanDevices); //Run async method in different thread
        }

        private async Task ScanDevices()
        {
            var devices = await SSDPScanner.SearchForDevices();
            var discoveredDevices = new ObservableCollection<Device>();

            foreach (var device in devices)
            {
                discoveredDevices.Add(new Device { Name = $"{device.DeviceType} - {device.FriendlyName}", IpAddress = device.PresentationUrl.AbsoluteUri });
            }

            //Use main thread to update UI
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
                AuthorizeDevice.ParentWindow = this;
                AuthorizeDevice.Visibility = Visibility.Visible;
                DiscoverDevice.Visibility = Visibility.Hidden;
            }
        }

        private void AuthorizeDevice_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
