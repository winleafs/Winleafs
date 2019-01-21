using System.Windows;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for NameDeviceControl.xaml
    /// </summary>
    public partial class NameDeviceControl : UserControl
    {
        public SetupWindow ParentWindow { get; set; }

        public NameDeviceControl()
        {
            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Cancel_Click(sender, e);
        }

        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Finish_Click(sender, e);
        }
    }
}
