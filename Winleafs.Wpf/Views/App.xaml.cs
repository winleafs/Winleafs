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
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Winleafs.External;
using Winleafs.Models.Exceptions;

using Application = System.Windows.Application;

namespace Winleafs.Wpf.Views
{
    using System;

    using Winleafs.Wpf.Views.Popup;

    using System.Globalization;
    using System.Threading;
    using System.Security;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static bool PerformRegularShutdownOprations = true;

        [STAThread]
        public static void Main()
        {
            var application = new App();
            application.InitializeComponent();
            application.Run();
        }

        void App_Startup(object sender, StartupEventArgs e)
        {
            var process = Process.GetCurrentProcess();
            var otherProcess = Process.GetProcesses().FirstOrDefault(p => p.ProcessName == process.ProcessName && p.Id != process.Id);

            if (otherProcess != null)
            {
                PerformRegularShutdownOprations = false;

                ShowWindow(otherProcess);

                Current.Shutdown();
                return;
            }

            if (!UserSettings.HasSettings() || UserSettings.Settings.Devices.Count == 0)
            {
                var setupWindow = new SetupWindow();
                setupWindow.Show();
            }
            else
            {
                var primaryColor = (Color)ColorConverter.ConvertFromString("#3FAE29");
                var secondaryColor = (Color)ColorConverter.ConvertFromString("#2D2F30");
                var baseTheme = Theme.Dark;
                var theme = Theme.Create(baseTheme, primaryColor, secondaryColor);
                Application.Current.Resources.SetTheme(theme);
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
                PopupCreator.Error(string.Format(AppResources.CorruptSettings, UserSettings.SettingsFolder + "Settings.json"));
                return;
            }

            if (!string.IsNullOrEmpty(UserSettings.Settings.UserLocale))
            {
                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(UserSettings.Settings.UserLocale);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(UserSettings.Settings.UserLocale);
            }

            UserSettings.Settings.ResetOperationModes();

            SunTimesUpdater.UpdateSunTimes();

            OrchestratorCollection.Initialize();

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
            var setupWindow = new SetupWindow();
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

        #region Show window of other process
        //Source: https://stackoverflow.com/questions/11399528/show-wpf-window-from-another-application-in-c-sharp

        public static readonly int WM_COPYDATA = 0x004A;
        public static readonly string OPENWINDOWMESSAGE = "Show";

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct MessageStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Message;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;
            public IntPtr lpData;
        }

        [SuppressUnmanagedCodeSecurity]
        internal class NativeMethods
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        }

        private static void ShowWindow(Process process)
        {
            var targetWindowPtr = process.MainWindowHandle;

            if (targetWindowPtr == IntPtr.Zero)
            {
                targetWindowPtr = NativeMethods.FindWindow(null, MainWindows.Resources.Winleafs); //Assumes that the window name is unique
            }

            if (targetWindowPtr == IntPtr.Zero)
            {
                return;
            }

            MessageStruct messageStruct;
            messageStruct.Message = OPENWINDOWMESSAGE;
            int structSize = Marshal.SizeOf(messageStruct);
            IntPtr structIntPtr = Marshal.AllocHGlobal(structSize);

            try
            {
                Marshal.StructureToPtr(messageStruct, structIntPtr, true);

                COPYDATASTRUCT cds = new COPYDATASTRUCT();
                cds.cbData = structSize;
                cds.lpData = structIntPtr;
                NativeMethods.SendMessage(targetWindowPtr, WM_COPYDATA, new IntPtr(), ref cds);
            }
            finally
            {
                Marshal.FreeHGlobal(structIntPtr);
            }
        }
        #endregion
    }
}
