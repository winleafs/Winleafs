using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Models;

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

                deviceUserControl.TopBorder.BorderThickness = new Thickness(0); //Hide the white top border
                deviceUserControl.DeviceNameLabel.Visibility = Visibility.Hidden; //Hide the device name

                //Hide the device icons
                deviceUserControl.SquareIcon.Visibility = Visibility.Hidden;
                deviceUserControl.TriangleIcon.Visibility = Visibility.Hidden;

                DeviceUserControl = deviceUserControl;

                DeviceUserControlGrid.Children.Add(deviceUserControl);
            }            
        }

        public void DeviceDeleted(string deviceName)
        {
            var deviceCurrentlySelected = _selectedDevice == deviceName;

            //This changes the _selectedDevice, hence the boolean must be saved before the removal, but the assignment must take place after removal
            DeviceNames.Remove(deviceName);

            //The deleted device was active in the view
            if (deviceCurrentlySelected)
            {
                SelectedDevice = DeviceNames.FirstOrDefault();

                DevicesDropdown.SelectedItem = SelectedDevice;
            }

            OnPropertyChanged(nameof(DeviceNames));
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
