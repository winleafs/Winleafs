using System.Windows;
using System.Windows.Controls;

namespace Winleafs.Wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for DeviceUserControl.xaml
    /// </summary>
    public partial class DeviceUserControl : UserControl
    {
        private OptionsWindow _parent;

        public string DeviceName { get; set; }

        public DeviceUserControl(string deviceName, OptionsWindow parent)
        {
            InitializeComponent();

            DeviceName = deviceName;
            _parent = parent;

            DataContext = this;
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            _parent.DeleteDevice(DeviceName);
        }
    }
}
