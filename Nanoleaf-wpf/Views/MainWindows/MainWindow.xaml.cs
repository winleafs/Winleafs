using Nanoleaf_wpf.Models;
using Nanoleaf_wpf.Models.Scheduling;
using Nanoleaf_wpf.Views.Scheduling;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;

namespace Nanoleaf_wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Schedule> _schedules;

        public static readonly string SCHEDULESETTINGKEY = "SCHEDULES";

        public MainWindow()
        {
            InitializeComponent();

            LoadSchedules();
        }

        private void LoadSchedules()
        {
            //TODO: move this logic to Service layer after the Model layer is completed
            try
            {
                _schedules = JsonConvert.DeserializeObject<List<Schedule>>(Properties.Settings.Default[SCHEDULESETTINGKEY].ToString());
                BuildScheduleList();
            }
            catch (JsonException e)
            {
                //TODO: handle json exception
            }
            catch (SettingsPropertyNotFoundException)
            {
                //There is no setting yet, create the setting
                _schedules = new List<Schedule>();
                new SettingsProperty(SCHEDULESETTINGKEY);
            }
        }

        private void SaveSchedules()
        {
            var json = JsonConvert.SerializeObject(_schedules);

            Properties.Settings.Default[SCHEDULESETTINGKEY] = json;
            Properties.Settings.Default.Save();
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.Show();
        }

        public void AddedSchedule(Schedule schedule)
        {
            _schedules.Add(schedule);
            _schedules = _schedules.OrderBy(s => s.Name).ToList();

            BuildScheduleList();
            SaveSchedules();
        }

        public void UpdatedSchedule()
        {
            //TODO: check if it is needed to update the list, expectation is no since c# works with refs
            BuildScheduleList();
            SaveSchedules();
        }

        private void BuildScheduleList()
        {
            ScheduleList.Children.Clear();

            foreach (var schedule in _schedules)
            {
                ScheduleList.Children.Add(new ScheduleItemUserControl(schedule));
            }
        }
    }
}
