using Nanoleaf_Api.Timers;
using Nanoleaf_Models.Models;
using Nanoleaf_wpf.Views.MainWindows;
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
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            try
            {
                UserSettings.LoadSettings();
            }
            catch (SettingsFileJsonException)
            {
                //TODO: handle
            }

            TimeTriggerTimer.InitializeTimer();

            //TODO: start timer to repeatedly try to get connection with lights            
        }
    }
}
