using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Winleafs.Api;

using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;

using NLog;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api;

namespace Winleafs.Wpf.Views.MainWindows
{
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
                var customEffects = CustomEffects.GetCustomEffectsForDevice(UserSettings.Settings.ActviceDevice);

                if (customEffects.HasActiveEffects())
                {
                    await customEffects.DeactivateAllEffects();
                }

                UserSettings.Settings.ActviceDevice.OperationMode = OperationMode.Schedule;

                ScheduleTimer.Timer.FireTimer();
            }
        }

        private async Task Override()
        {
            if (!string.IsNullOrEmpty(SelectedEffect))
            {
                try
                {
                    var customEffects = CustomEffects.GetCustomEffectsForDevice(UserSettings.Settings.ActviceDevice);
                    var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActviceDevice);

                    if (customEffects.HasActiveEffects(SelectedEffect))
                    {
                        await customEffects.DeactivateAllEffects();
                    }

                    if (customEffects.EffectIsCustomEffect(SelectedEffect))
                    {
                        var customEffect = customEffects.GetCustomEffect(SelectedEffect);

                        if (!customEffect.IsActive())
                        {
                            await customEffect.Activate();
                        }
                    }
                    else
                    {
                        await client.EffectsEndpoint.SetSelectedEffectAsync(SelectedEffect);
                    }

                    await client.StateEndpoint.SetBrightnessAsync(_brightness);

                    UserSettings.Settings.ActviceDevice.OperationMode = OperationMode.Manual;
                }
                catch (Exception e)
                {
                    _logger.Error(e, "Error during overriding schedule");
                    MessageBox.Show("An unexpected error occurred during overriding.");
                }
            }
        }
    }
}
