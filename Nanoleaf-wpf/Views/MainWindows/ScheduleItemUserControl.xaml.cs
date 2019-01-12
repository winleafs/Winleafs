using Nanoleaf_Models.Models.Scheduling;
using System;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for ScheduleItemUserControl.xaml
    /// </summary>
    public partial class ScheduleItemUserControl : UserControl
    {
        private MainWindow _parent;
        public Schedule Schedule { get; set; }

        public ScheduleItemUserControl(MainWindow parent, Schedule schedule)
        {
            Schedule = schedule;
            _parent = parent;

            InitializeComponent();
            DataContext = this;
        }

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.EditSchedule(Schedule);
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteSchedule(Schedule);
        }
    }
}
