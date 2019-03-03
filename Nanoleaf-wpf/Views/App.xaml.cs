using System.Threading.Tasks;
using System.Windows;

using Winleafs.Api;

using Winleafs.Models.Models;

using NLog;

using Winleafs.Wpf.Views.MainWindows;
using Winleafs.Wpf.Views.Setup;
using Winleafs.Api.Helpers;
using Winleafs.Wpf.Api;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using Winleafs.External;

using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Winleafs.Wpf.Views
{
    using System;

    using Winleafs.Wpf.Views.Popup;

    using System.Globalization;
    using System.Threading;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        [DllImport("User32")]
        private static extern int ShowWindow(int hwnd, int nCmdShow);

        private const int SW_SHOW = 5;

        private static bool PerformRegularShutdownOprations = true;

        void App_Startup(object sender, StartupEventArgs e)
        {
            Process process = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Where(p => p.ProcessName == process.ProcessName).Count();

            if (count > 1)
            {
                PerformRegularShutdownOprations = false;
                Current.Shutdown();
                return;
            }

            if (!UserSettings.HasSettings() || UserSettings.Settings.Devices.Count == 0)
            {
                var setupWindow = new SetupWindow(true);
                setupWindow.Show();
            }
            else
            {
                NormalStartup(e);
            }

        }

        public static void NormalStartup(StartupEventArgs e)
        {
            var silent = false;
            if (e != null && e.Args.Length > 0)
            {
                silent = e.Args[0].Equals("-s");
            }

            try
            {
                UserSettings.LoadSettings();
            }
            catch (SettingsFileJsonException ex)
            {
                _logger.Fatal("Corrupt settings file found", ex);
                PopupCreator.CreateErrorPopup(string.Format(AppResources.CorruptSettings, UserSettings.SettingsFolder + "Settings.json"));
                return;
            }

            UserSettings.Settings.ResetOperationModes();

            SunTimesUpdater.UpdateSunTimes();

            ScheduleTimer.InitializeTimer();

            if (!string.IsNullOrEmpty(UserSettings.Settings.UserLocale))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(UserSettings.Settings.UserLocale);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(UserSettings.Settings.UserLocale);
            }

            var mainWindow = new MainWindow();

            if (!silent)
            {
                mainWindow.Show();
            }

            CheckForUpdate();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            try
            {
                if (PerformRegularShutdownOprations)
                {
                    var task = Task.Run(() => TurnOffLights());
                    task.Wait(); //We actually want the code to execute directly instead of waiting
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Exception occurred during application exit");
            }
        }

        /// <summary>
        /// On Windows shutdown
        /// </summary>
        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            try
            {
                if (PerformRegularShutdownOprations)
                {
                    var task = Task.Run(() => TurnOffLights());
                    task.Wait(); //We actually want the code to execute directly instead of waiting
                }
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex, "Exception occurred during windows shutdown");
            }
        }

        private async Task TurnOffLights()
        {
            //Check if any lights need to be turned off
            foreach (var device in UserSettings.Settings.Devices)
            {
                if (device.ActiveSchedule != null && device.ActiveSchedule.TurnOffAtApplicationShutdown)
                {
                    var client = NanoleafClient.GetClientForDevice(device);
                    await client.StateEndpoint.SetStateWithStateCheckAsync(false);
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Fatal(e.Exception, "Unhandled exception occurred");
        }

        public static void ResetAllSettings(MainWindow mainWindow)
        {
            UserSettings.DeleteSettings();
            var setupWindow = new SetupWindow(true);
            setupWindow.Show();

            mainWindow.Close();
        }

        private static void CheckForUpdate()
        {
            var client = new ReleaseClient();
            var release = client.GetLatestVersion().GetAwaiter().GetResult();

            if (release == UserSettings.APPLICATIONVERSION)
            {
                return;
            }

            new NewVersionPopup().Show();
            _logger.Info($"New version available upgrade from {UserSettings.APPLICATIONVERSION} to {release}");

            // Check release with current version.
        }
    }
}
