using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for TaskbarIcon.xaml
    /// </summary>
    public partial class TaskbarIcon : UserControl
    {
        private MainWindow _parent;

        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    SelectedDeviceChanged();
                    DevicesDropdown.SelectedItem = _selectedDevice;
                }
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }

        public TaskbarIcon()
        {
            InitializeComponent();
        }

        //Called after the MainWindow is intialized
        public void Initialize(MainWindow mainWindow)
        {
            _parent = mainWindow;

            DeviceNames = _parent.DeviceNames;
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectedDeviceChanged()
        {

            if (_parent != null)
            {
                _parent.SelectedDevice = SelectedDevice; //Also trigger main window device change 
            }                       
        }
    }
}
