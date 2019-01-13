using Hardcodet.Wpf.TaskbarNotification;
using Nanoleaf_Api.Timers;
using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Scheduling;
using Nanoleaf_wpf.Views.Scheduling;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Nanoleaf_wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TaskbarIcon _taskbarIcon;

        public static readonly string SCHEDULESETTINGKEY = "SCHEDULES";

        public MainWindow()
        {
            InitializeComponent();

            _taskbarIcon = (TaskbarIcon)FindResource("NotifyIcon");
            _taskbarIcon.DoubleClickCommand = new TaskbarDoubleClickCommand(this);

            BuildScheduleList();
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.Show();
        }

        public void AddedSchedule(Schedule schedule)
        {
            UserSettings.GetSettings().AddSchedule(schedule);

            //Update the TimeTriggerTimer to follow the correct schedule
            TimeTriggerTimer.Timer.SetTodaysProgram();

            BuildScheduleList();
        }

        public void UpdatedSchedule()
        {
            UserSettings.GetSettings().SaveSettings();
            BuildScheduleList();
        }

        private void BuildScheduleList()
        {
            ScheduleList.Children.Clear();

            foreach (var schedule in UserSettings.GetSettings().Schedules)
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
            UserSettings.GetSettings().DeleteSchedule(schedule);

            //Update the TimeTriggerTimer to follow the correct schedule
            TimeTriggerTimer.Timer.SetTodaysProgram();

            BuildScheduleList();
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            _taskbarIcon.Visibility = Visibility.Visible;
            e.Cancel = true;
            Hide();
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
    }
}
