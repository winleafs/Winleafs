using Nanoleaf_wpf.Models;
using Nanoleaf_wpf.Models.Scheduling;
using Nanoleaf_wpf.Views.MainWindows;
using System.Collections.Generic;
using System.Windows;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for ManageScheduleWindow.xaml
    /// </summary>
    public partial class ManageScheduleWindow : Window
    {
        private MainWindow _parent;
        private WorkMode _workMode;

        public Schedule Schedule { get; set; }

        public ManageScheduleWindow(MainWindow parent, WorkMode workMode, Schedule schedule = null)
        {
            _parent = parent;
            _workMode = workMode;

            if (schedule == null)
            {
                Schedule = new Schedule();
            }

            DataContext = Schedule;

            InitializeComponent();

            var dayUserControls = new List<DayUserControl>();
            dayUserControls.Add(MondayUserControl);
            dayUserControls.Add(TuesdayUserControl);
            dayUserControls.Add(TuesdayUserControl);
            dayUserControls.Add(ThursdayUserControl);
            dayUserControls.Add(FridayUserControl);
            dayUserControls.Add(MondayUserControl);
            dayUserControls.Add(SundayUserControl);

            foreach (var dayUserControl in dayUserControls)
            {
                dayUserControl.Program = Schedule.Programs[dayUserControl.IndexOfDay];
                dayUserControl.BuildTriggerList();
            }

            EventUserControl.EventTriggers = Schedule.EventTriggers;
            EventUserControl.BuildTriggerList();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_workMode == WorkMode.Add)
            {
                _parent.AddedSchedule(Schedule);
            }
            else
            {
                _parent.UpdatedSchedule();
            }

            Close();
        }
    }
}
