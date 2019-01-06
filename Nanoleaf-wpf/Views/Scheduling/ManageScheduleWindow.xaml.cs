using Nanoleaf_wpf.Models.Scheduling;
using System.Windows;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for ManageScheduleWindow.xaml
    /// </summary>
    public partial class ManageScheduleWindow : Window
    {
        public Schedule Schedule { get; set; }

        public ManageScheduleWindow(Schedule schedule = null)
        {
            InitializeComponent();

            if (schedule == null)
            {
                Schedule = new Schedule();
            }

            DataContext = Schedule;
        }
    }
}
