using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;

using Hardcodet.Wpf.TaskbarNotification;

using NLog;

using Winleafs.Api;
using Winleafs.Api.Timers;

using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling;

using Winleafs.Wpf.Views.Options;
using Winleafs.Wpf.Views.Scheduling;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskbarIcon _taskbarIcon;

        public MainWindow()
        {
            InitializeComponent();

            _taskbarIcon = (TaskbarIcon)FindResource("NotifyIcon"); //https://www.codeproject.com/Articles/36468/WPF-NotifyIcon-2
            _taskbarIcon.DoubleClickCommand = new TaskbarDoubleClickCommand(this);

            BuildScheduleList();
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.ShowDialog();
        }

        public void AddedSchedule(Schedule schedule)
        {
            UserSettings.Settings.AddSchedule(schedule, true);

            ScheduleTimer.Timer.FireTimer(); //Fire the timer to immediately update the schedule

            BuildScheduleList();
        }

        public void UpdatedSchedule(Schedule originalSchedule, Schedule newSchedule)
        {
            UserSettings.Settings.DeleteSchedule(originalSchedule);
            UserSettings.Settings.AddSchedule(newSchedule, false);

            ScheduleTimer.Timer.FireTimer(); //Fire the timer to immediately update the schedule

            BuildScheduleList();
        }

        private void BuildScheduleList()
        {
            ScheduleList.Children.Clear();

            foreach (var schedule in UserSettings.Settings.ActviceDevice.Schedules)
            {
                ScheduleList.Children.Add(new ScheduleItemUserControl(this, schedule));
            }
        }

        public void EditSchedule(Schedule schedule)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Edit, schedule);
            scheduleWindow.ShowDialog();
        }

        public void DeleteSchedule(Schedule schedule)
        {
            UserSettings.Settings.DeleteSchedule(schedule);

            ScheduleTimer.Timer.FireTimer(); //Fire the timer to immediately update the schedule

            BuildScheduleList();
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        public void ActivateSchedule(Schedule schedule)
        {
            UserSettings.Settings.ActivateSchedule(schedule);

            ScheduleTimer.Timer.FireTimer(); //Fire the timer to immediately update the schedule

            BuildScheduleList();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow();
            optionsWindow.ShowDialog();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            App.ResetAllSettings(this);
        }

        private class TaskbarDoubleClickCommand : ICommand
        {
            private MainWindow _window;

            public TaskbarDoubleClickCommand(MainWindow window)
            {
                _window = window;
            }

            public void Execute(object parameter)
            {
                _window.Show();
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public event EventHandler CanExecuteChanged; //Must be included for the interface
        }

        private async void Reload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var device = UserSettings.Settings.ActviceDevice;
                var nanoleafClient = new NanoleafClient(device.IPAddress, device.Port, device.AuthToken);
                var effects = await nanoleafClient.EffectsEndpoint.GetEffectsListAsync();
                device.Effects.Clear();
                device.LoadEffectsFromNameList(effects);
                // TODO Remove me for a popup
                MessageBox.Show(MainWindows.Resources.ReloadSuccessful);
            }
            catch(Exception exception)
            {
                MessageBox.Show(MainWindows.Resources.ReloadFailed);
                LogManager.GetCurrentClassLogger().Error(exception, "Failed to reload effects list");
            }


        }

        private void Stuck_Click(object sender, RoutedEventArgs e)
        {
            // Unsure if this would be needed but don't want to execute any program.
            // Doing this won't do much and it will be difficult to execute a program like this but it's better than nothing.
            if (!File.Exists(UserSettings.SettingsFolder))
            {
                Process.Start(UserSettings.SettingsFolder);
            }
        }
    }
}
