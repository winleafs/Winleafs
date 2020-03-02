using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Winleafs.Models.Models.Scheduling;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for ScheduleItemUserControl.xaml
    /// </summary>
    public partial class ScheduleItemUserControl : UserControl
    {
        private MainWindow _parent;

        public Schedule Schedule { get; set; } //Must stay public since its used in the view

        public string NextEffectDisplay { get; set; }

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

            var nextEffect = schedule.GetNextTimeTrigger();

            if (nextEffect != null)
            {
                NextEffectDisplay = string.Format(MainWindows.Resources.NextEffect, EnumLocalizer.GetLocalizedEnum(nextEffect.Item1), nextEffect.Item2.GetActualDateTime().ToString("HH:mm"), nextEffect.Item2.EffectName);
            }
            else
            {
                NextEffectDisplay = MainWindows.Resources.NextEffectNone;
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
