using Nanoleaf_wpf.Models.Scheduling;
using System;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for ScheduleItemUserControl.xaml
    /// </summary>
    public partial class ScheduleItemUserControl : UserControl
    {
        public Schedule Schedule { get; set; }

        public ScheduleItemUserControl(Schedule schedule)
        {
            Schedule = schedule;

            InitializeComponent();
            DataContext = this;
        }

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
