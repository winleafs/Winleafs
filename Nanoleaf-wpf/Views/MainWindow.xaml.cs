using Nanoleaf_wpf.Views.Setup;
using System.Windows;

namespace Nanoleaf_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var setupWindow = new SetupWindow();
            setupWindow.Show();
            setupWindow.Activate();
        }
    }
}
