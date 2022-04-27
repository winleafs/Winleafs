﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for TaskbarIcon.xaml
    /// </summary>
    public partial class TaskbarIcon : UserControl, INotifyPropertyChanged
    {
        private MainWindow _parent;

        private string _selectedDevice;

        public event PropertyChangedEventHandler PropertyChanged;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    OnPropertyChanged(nameof(SelectedDevice));
                    SelectedDeviceChanged();
                    DevicesDropdown.SelectedItem = _selectedDevice;
                }
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }

        public DeviceUserControl DeviceUserControl { get; set; }

        public TaskbarIcon()
        {
            InitializeComponent();

            DataContext = this;
        }

        //Called after the MainWindow is intialized
        public void Initialize(MainWindow mainWindow)
        {
            _parent = mainWindow;

            DeviceNames = _parent.DeviceNames;
            SelectedDevice = _parent.SelectedDevice;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectedDeviceChanged()
        {
            if (_selectedDevice != null)
            {
                DeviceUserControlGrid.Children.Clear();

                var deviceUserControl = new DeviceUserControl(UserSettings.Settings.Devices.FirstOrDefault(device => device.Name == SelectedDevice), _parent);

                DeviceUserControl = deviceUserControl;

                DeviceUserControlGrid.Children.Add(deviceUserControl);
            }            
        }

        public void DeviceDeleted(string deviceName)
        {
            DeviceNames.Remove(deviceName);

            //Reset the selected device to make sure the deleted device is not selected
            SelectedDevice = DeviceNames.FirstOrDefault();

            DevicesDropdown.SelectedItem = SelectedDevice;

            OnPropertyChanged(nameof(DeviceNames));
        }

        public void DeviceAdded()
        {
            DeviceNames = _parent.DeviceNames;

            OnPropertyChanged(nameof(DeviceNames));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
