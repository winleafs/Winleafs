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
        private List<Schedule> _schedules;

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
                var settings = UserSettings.LoadSettings();

                Nanoleaf_Models.Models.Effects.Effect.Effects = settings.Effects;
                _schedules = settings.Schedules;
            }
            catch (NoSettingsFileException)
            {
                Nanoleaf_Models.Models.Effects.Effect.Effects = new List<Nanoleaf_Models.Models.Effects.Effect>();

                //TODO: should load from settings, and a user can call a manual function to update the effects and the effects should be loaded when pairing a device
                Nanoleaf_Models.Models.Effects.Effect.Effects.Add(new Nanoleaf_Models.Models.Effects.Effect { Name = "Flames" });
                Nanoleaf_Models.Models.Effects.Effect.Effects.Add(new Nanoleaf_Models.Models.Effects.Effect { Name = "Forest" });
                Nanoleaf_Models.Models.Effects.Effect.Effects.Add(new Nanoleaf_Models.Models.Effects.Effect { Name = "Nemo" });
                Nanoleaf_Models.Models.Effects.Effect.Effects.Add(new Nanoleaf_Models.Models.Effects.Effect { Name = "Snowfall" });
                Nanoleaf_Models.Models.Effects.Effect.Effects.Add(new Nanoleaf_Models.Models.Effects.Effect { Name = "Inner Peace" });
                Nanoleaf_Models.Models.Effects.Effect.Effects = Nanoleaf_Models.Models.Effects.Effect.Effects.OrderBy(eff => eff.Name).ToList();

                _schedules = new List<Schedule>();
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
            _schedules.Add(schedule);
            _schedules = _schedules.OrderBy(s => s.Name).ToList();

            BuildScheduleList();
            SaveSettings();
        }

        public void UpdatedSchedule()
        {
            //TODO: check if it is needed to update the list, expectation is no since c# works with refs
            BuildScheduleList();
            SaveSettings();
        }

        private void SaveSettings()
        {
            UserSettings.SaveSettings(_schedules, Nanoleaf_Models.Models.Effects.Effect.Effects);
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
