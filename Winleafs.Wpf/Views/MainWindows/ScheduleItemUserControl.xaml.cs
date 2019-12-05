using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Winleafs.Models.Models.Scheduling;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for ScheduleItemUserControl.xaml
    /// </summary>
    public partial class ScheduleItemUserControl : UserControl
    {
        private MainWindow _parent;
        public Schedule Schedule { get; set; } //Must stay public since its used in the view

        public ScheduleItemUserControl(MainWindow parent, Schedule schedule)
        {
            Schedule = schedule;
            _parent = parent;

            InitializeComponent();
            DataContext = this;

            if (Schedule.Active)
            {
                Background = (Brush)new BrushConverter().ConvertFromInvariantString("#7F3F6429");
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            _parent.EditSchedule(Schedule);
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show(MainWindows.Resources.DeleteScheduleAreYouSure, MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                _parent.DeleteSchedule(Schedule);
            }
        }

        private void ActiveSchedule_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _parent.ActivateSchedule(Schedule);
        }
    }
}
