using System.Windows;
using System.Windows.Controls;

namespace Winleafs.Wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for AuthorizeDeviceControl.xaml
    /// </summary>
    public partial class AuthorizeDeviceControl : UserControl
    {
        public SetupWindow ParentWindow { get; set; }

        public AuthorizeDeviceControl()
        {
            InitializeComponent();
        }

        private void Pair_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.Pair_Click(sender, e);
        }
    }
}
