using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Scheduling;
using Nanoleaf_wpf.Views.Scheduling;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Nanoleaf_wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private UserSettings _userSettings;

        public static readonly string SCHEDULESETTINGKEY = "SCHEDULES";

        public MainWindow()
        {
            InitializeComponent();

            LoadSettings();
        }

        private void LoadSettings()
        {
            try
            {
                _userSettings = UserSettings.LoadSettings();
            }
            catch (SettingsFileJsonException)
            {
                //TODO: handle
            }

            BuildScheduleList();
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.Show();
        }

        public void AddedSchedule(Schedule schedule)
        {
            _userSettings.AddSchedule(schedule);            

            BuildScheduleList();
        }

        public void UpdatedSchedule()
        {
            _userSettings.SaveSettings();
            BuildScheduleList();
        }

        private void BuildScheduleList()
        {
            ScheduleList.Children.Clear();

            foreach (var schedule in _userSettings.Schedules)
            {
                ScheduleList.Children.Add(new ScheduleItemUserControl(this, schedule));
            }
        }

        public void EditSchedule(Schedule schedule)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Edit, schedule);
            scheduleWindow.Show();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            _userSettings.DeleteSchedule(schedule);

            BuildScheduleList();
        }
    }
}
