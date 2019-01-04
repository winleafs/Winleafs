using System.Windows;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for DiscoverDevice.xaml
    /// </summary>
    public partial class DiscoverDeviceControl : UserControl
    {
        public SetupWindow ParentWindow { get; set; }

        public DiscoverDeviceControl()
        {
            InitializeComponent();
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Refresh_Click(sender, e);
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Cancel_Click(sender, e);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Continue_Click(sender, e);
        }
    }
}
