using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models.Scheduling;
using Nanoleaf_wpf.Views.MainWindows;
using Newtonsoft.Json;
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

        private Schedule _originalSchedule;

        public ManageScheduleWindow(MainWindow parent, WorkMode workMode, Schedule schedule = null)
        {
            _parent = parent;
            _workMode = workMode;

            if (_workMode == WorkMode.Add)
            {
                Schedule = new Schedule(true);
            }
            else
            {
                _originalSchedule = schedule;

                var serialized = JsonConvert.SerializeObject(schedule); //Deep copy the schedule when editing
                Schedule = JsonConvert.DeserializeObject<Schedule>(serialized);
            }

            DataContext = Schedule;

            InitializeComponent();

            var dayUserControls = new List<DayUserControl>();
            dayUserControls.Add(MondayUserControl);
            dayUserControls.Add(TuesdayUserControl);
            dayUserControls.Add(WednesdayUserControl);
            dayUserControls.Add(ThursdayUserControl);
            dayUserControls.Add(FridayUserControl);
            dayUserControls.Add(SaturdayUserControl);
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
                _parent.UpdatedSchedule(_originalSchedule, Schedule);
            }

            Close();
        }
    }
}
