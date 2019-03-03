using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;

using NLog;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api;

namespace Winleafs.Wpf.Views.MainWindows
{
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for OverrideScheduleUserControl.xaml
    /// </summary>
    public partial class OverrideScheduleUserControl : UserControl
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public List<Effect> Effects { get; set; }

        public string SelectedEffect { get; set; }

        private int _brightness;

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public MainWindow MainWindow;

        public OverrideScheduleUserControl()
        {
            InitializeComponent();

            Effects = new List<Effect>(UserSettings.Settings.ActviceDevice.Effects);
            Effects.InsertRange(0, CustomEffects.GetCustomEffectAsEffects(UserSettings.Settings.ActviceDevice));

            DataContext = this;

            Brightness = 70; //TODO: get the value from the device itself
        }

        private void Override_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Override());
        }

        private void StopOverride_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => StopOverride());
        }

        private async Task StopOverride()
        {
            if (UserSettings.Settings.ActviceDevice.OperationMode == OperationMode.Manual)
            {
                UserSettings.Settings.ActviceDevice.OperationMode = OperationMode.Schedule;

                var customEffects = CustomEffects.GetCustomEffectsForDevice(UserSettings.Settings.ActviceDevice);

                if (customEffects.HasActiveEffects())
                {
                    await customEffects.DeactivateAllEffects();
                }

                ScheduleTimer.Timer.FireTimer();

                MainWindow.UpdateCurrentEffectLabels();
            }
        }

        private async Task Override()
        {
            if (!string.IsNullOrEmpty(SelectedEffect))
            {
                try
                {
                    var device = UserSettings.Settings.ActviceDevice;
                    device.OverrideEffect = SelectedEffect;
                    device.OverrideBrightness = Brightness;

                    await EffectActivator.ActivateEffect(device, SelectedEffect, Brightness);

                    UserSettings.Settings.ActviceDevice.OperationMode = OperationMode.Manual;

                    MainWindow.UpdateCurrentEffectLabels();
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error during overriding schedule");
                    PopupCreator.CreateErrorPopup(MainWindows.Resources.OverrideError);
                }
            }
        }
    }
}
