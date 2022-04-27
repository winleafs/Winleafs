using System.Collections.Generic;
using System.Windows;

using Winleafs.Models.Enums;
using Newtonsoft.Json;

using Winleafs.Wpf.Views.MainWindows;
using System.Linq;
using Winleafs.Models;
using Winleafs.Models.Scheduling;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Scheduling
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

        private List<DayUserControl> _dayUserControls;

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

            _dayUserControls = new List<DayUserControl>();
            _dayUserControls.Add(MondayUserControl);
            _dayUserControls.Add(TuesdayUserControl);
            _dayUserControls.Add(WednesdayUserControl);
            _dayUserControls.Add(ThursdayUserControl);
            _dayUserControls.Add(FridayUserControl);
            _dayUserControls.Add(SaturdayUserControl);
            _dayUserControls.Add(SundayUserControl);

            SetupDayUserControls();

            EventUserControl.EventTriggers = Schedule.EventTriggers;
            EventUserControl.BuildTriggerList();

            //Set the device dropdown values
            DevicesDropdown.ItemsSource = UserSettings.Settings.Devices.Select(device => device.Name);
            DevicesDropdown.SelectedValue = Schedule.AppliedDeviceNames;
        }

        private void SetupDayUserControls()
        {
            foreach (var dayUserControl in _dayUserControls)
            {
                dayUserControl.Program = Schedule.Programs[dayUserControl.IndexOfDay];
                dayUserControl.ParentWindow = this;
                dayUserControl.BuildTriggerList();
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            //Set the devices of the schedule
            Schedule.AppliesToDeviceNames = string.IsNullOrEmpty(DevicesDropdown.SelectedValue) ? new List<string>() : DevicesDropdown.SelectedValue.Split(',').Select(x => x.Trim()).ToList();

            //Check if a name is entered, a device is selected and if there is any event or trigger
            if (!Schedule.AppliesToDeviceNames.Any()
                || string.IsNullOrWhiteSpace(Schedule.Name)
                || (!Schedule.Programs.Any(program => program.Triggers.Any()) && !Schedule.EventTriggers.Any()))
            {
                PopupCreator.Error(Scheduling.Resources.SaveScheduleError);
                return;
            }

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

        public void CopyProgramToDays(Program program, List<int> indexes)
        {
            foreach (var i in indexes)
            {
                var serialized = JsonConvert.SerializeObject(program); //Deep copy the program such that the programs do not interfere with eachother
                var newProgram = JsonConvert.DeserializeObject<Program>(serialized);

                Schedule.Programs[i] = newProgram;
            }

            SetupDayUserControls();
        }
    }
}
