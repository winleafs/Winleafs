using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using Hardcodet.Wpf.TaskbarNotification;

using NLog;
using Winleafs.Api;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Enums;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Options;
using Winleafs.Wpf.Views.Scheduling;

namespace Winleafs.Wpf.Views.MainWindows
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Interop;
    using Winleafs.Wpf.Views.Layout;
    using Winleafs.Wpf.Views.Popup;
    using Winleafs.Wpf.Views.Setup;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    SelectedDeviceChanged();
                }
            }
        }

        public ObservableCollection<string> DeviceNames { get; set; }

        private List<DeviceUserControl> _deviceUserControls;
        
        public MainWindow()
        {
            InitializeComponent();

            UpdateDeviceNames();
            SelectedDevice = UserSettings.Settings.ActiveDevice.Name;

            DataContext = this;

            NotifyIcon.DoubleClickCommand = new TaskbarDoubleClickCommand(this);

            BuildScheduleList();

            VersionLabel.Content = $"Winleafs {UserSettings.APPLICATIONVERSION}";

            //Must appear last since this user control uses components of the main window
            TaskbarIcon.Initialize(this);
        }

        /// <summary>
        /// Initializes devices, after the view has loaded.
        /// Put all operations here that execute requests to the panels
        /// </summary>
        public void Initialize()
        {
            //Initialize device user controls
            _deviceUserControls = new List<DeviceUserControl>();

            foreach (var device in UserSettings.Settings.Devices)
            {
                var deviceUserControl = new DeviceUserControl(device);

                _deviceUserControls.Add(deviceUserControl);
                DevicesStackPanel.Children.Add(deviceUserControl);
            }

            LayoutDisplay.InitializeResizeTimer();
            LayoutDisplay.DrawLayout();
        }

        /// <summary>
        /// Update all effect dropdowns in the <see cref="_deviceUserControls"/>
        /// </summary>
        public void ReloadEffects()
        {
            //TODO: implement this
        }

        public void UpdateDeviceNames()
        {
            DeviceNames = new ObservableCollection<string>(UserSettings.Settings.Devices.Select(d => d.Name));
        }

        private void SelectedDeviceChanged()
        {
            UserSettings.Settings.SetActiveDevice(_selectedDevice);

            LayoutDisplay.DrawLayout(true);
        }

        private void AddSchedule_Click(object sender, RoutedEventArgs e)
        {
            var scheduleWindow = new ManageScheduleWindow(this, WorkMode.Add);
            scheduleWindow.ShowDialog();
        }

        public void AddedSchedule(Schedule schedule)
        {
            UserSettings.Settings.AddSchedule(schedule, true);

            OrchestratorCollection.ResetOrchestrators();

            BuildScheduleList();

            UpdateActiveEffectLabelsAndLayout();
        }

        public void UpdatedSchedule(Schedule originalSchedule, Schedule newSchedule)
        {
            UserSettings.Settings.DeleteSchedule(originalSchedule);
            UserSettings.Settings.AddSchedule(newSchedule, false);

            OrchestratorCollection.ResetOrchestrators();

            BuildScheduleList();

            UpdateActiveEffectLabelsAndLayout();
        }

        private void BuildScheduleList()
        {
            SchedulesStackPanel.Children.Clear();

            foreach (var schedule in UserSettings.Settings.Schedules)
            {
                SchedulesStackPanel.Children.Add(new ScheduleItemUserControl(this, schedule));
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

            OrchestratorCollection.ResetOrchestrators();

            BuildScheduleList();

            UpdateActiveEffectLabelsAndLayout();
        }

        public void Window_Closing(object sender, CancelEventArgs e)
        {
            if (UserSettings.Settings.MinimizeToSystemTray)
            {
                Hide();
                e.Cancel = true;
            }
        }

        public void ActivateSchedule(Schedule schedule)
        {
            UserSettings.Settings.ActivateSchedule(schedule);

            OrchestratorCollection.ResetOrchestrators();

            BuildScheduleList();

            UpdateActiveEffectLabelsAndLayout();
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            var optionsWindow = new OptionsWindow(this);
            optionsWindow.ShowDialog();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(string.Format(MainWindows.Resources.AreYouSure, _selectedDevice), MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                App.ResetAllSettings(this);
            }
        }

        private class TaskbarDoubleClickCommand : ICommand
        {
            private readonly MainWindow _window;

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
                var device = UserSettings.Settings.ActiveDevice;
                var nanoleafClient = NanoleafClient.GetClientForDevice(device);
                var effects = await nanoleafClient.EffectsEndpoint.GetEffectsListAsync();

                device.LoadEffectsFromNameList(effects);

                UserSettings.Settings.SaveSettings();

                PopupCreator.Success(MainWindows.Resources.ReloadSuccessful);
            }
            catch (Exception exception)
            {
                PopupCreator.Error(MainWindows.Resources.ReloadFailed);
                LogManager.GetCurrentClassLogger().Error(exception, "Failed to reload effects list");
            }
        }

        private void AddDevice_Click(object sender, RoutedEventArgs e)
        {
            var setupWindow = new SetupWindow(this);
            setupWindow.Show();
        }
        
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        //TODO: where to place button to remove a device?
        private void RemoveDevice_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(string.Format(MainWindows.Resources.DeleteDeviceAreYouSure, _selectedDevice), MainWindows.Resources.DeleteConfirmation, MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                UserSettings.Settings.DeleteActiveDevice();

                if (UserSettings.Settings.Devices.Count > 0)
                {
                    DeviceNames.Remove(_selectedDevice);

                    SelectedDevice = DeviceNames.FirstOrDefault();

                    DevicesDropdown.SelectedItem = SelectedDevice;

                    UpdateActiveEffectLabelsAndLayout();
                }
                else
                {
                    var setupWindow = new SetupWindow();
                    setupWindow.Show();

                    Close();
                }
            }
        }

        public void UpdateActiveEffectLabelsAndLayout()
        {
            LayoutDisplay.DrawLayout();

            foreach (var deviceUserControl in _deviceUserControls)
            {
                deviceUserControl.Update();
            }
        }

        private void PercentageProfile_Click(object sender, RoutedEventArgs e)
        {
            var percentageProfileWindow = new PercentageProfileWindow();
            percentageProfileWindow.Show();
        }

        public void UpdateContextMenuMostUsedEffects()
        {
            TaskbarIcon.BuildMostUsedEfectList();
        }

        #region Open window from other process
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
            hwndSource.AddHook(new HwndSourceHook(WndProc));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            System.Windows.Forms.Message m = System.Windows.Forms.Message.Create(hwnd, msg, wParam, lParam);
            if (m.Msg == App.WM_COPYDATA)
            {
                // Get the COPYDATASTRUCT struct from lParam.
                var cds = (App.COPYDATASTRUCT)m.GetLParam(typeof(App.COPYDATASTRUCT));

                // If the size matches
                if (cds.cbData == Marshal.SizeOf(typeof(App.MessageStruct)))
                {
                    // Marshal the data from the unmanaged memory block to a managed struct.
                    var messageStruct = (App.MessageStruct)Marshal.PtrToStructure(cds.lpData, typeof(App.MessageStruct));

                    // Display the MyStruct data members.
                    if (messageStruct.Message == App.OPENWINDOWMESSAGE)
                    {
                        Show();
                    }
                }
            }
            return IntPtr.Zero;
        }
        #endregion
    }
}
