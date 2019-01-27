using Nanoleaf_Api;
using Nanoleaf_Api.Timers;
using Nanoleaf_Models.Models;
using Nanoleaf_wpf.Views.MainWindows;
using Nanoleaf_wpf.Views.Setup;
using NLog;
using System.Threading.Tasks;
using System.Windows;

namespace Nanoleaf_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        void App_Startup(object sender, StartupEventArgs e)
        {
            if (!UserSettings.HasSettings())
            {
                var setupWindow = new SetupWindow();
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

                //TODO: add message box to show to user

                return;
            }

            UserSettings.Settings.ResetOperationModes();

            SunTimesUpdater.UpdateSunTimes();

            ScheduleTimer.InitializeTimer();

            MainWindow mainWindow = new MainWindow();

            if (!silent)
            {
                mainWindow.Show();
            }
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var task = Task.Run(() => TurnOffLights());
            task.Wait(); //We actually want the code to execute directly instead of waiting
        }

        /// <summary>
        /// On Windows shutdown
        /// </summary>
        private void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            var task = Task.Run(() => TurnOffLights());
            task.Wait(); //We actually want the code to execute directly instead of waiting
        }

        private async Task TurnOffLights()
        {
            //Check if any lights need to be turned off
            foreach (var device in UserSettings.Settings.Devices)
            {
                if (device.ActiveSchedule != null && device.ActiveSchedule.TurnOffAtApplicationShutdown)
                {
                    var client = NanoleafClient.GetClientForDevice(device);
                    await client.StateEndpoint.SetStateWithStateCheck(false);
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            _logger.Fatal("Unhandled exception occurred", e);
        }

        public static void ResetAllSettings(MainWindow mainWindow)
        {
            UserSettings.DeleteSettings();
            var setupWindow = new SetupWindow();
            setupWindow.Show();

            mainWindow.Close();
        }
    }
}
