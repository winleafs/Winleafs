using Nanoleaf_Api.Timers;
using Nanoleaf_Models.Models;
using Nanoleaf_wpf.Views.MainWindows;
using Nanoleaf_wpf.Views.Setup;
using System.Windows;

namespace Nanoleaf_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
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

            MainWindow mainWindow = new MainWindow();

            if (!silent)
            {
                mainWindow.Show();
            }

            try
            {
                UserSettings.LoadSettings();
            }
            catch (SettingsFileJsonException)
            {
                //TODO: handle
            }

            TimeTriggerTimer.InitializeTimer();
        }
    }
}
