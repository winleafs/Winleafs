using Nanoleaf_wpf.Views.Scheduling;
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

            var schedule = new ManageScheduleWindow();
            schedule.Show();
        }
    }
}
