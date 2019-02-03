using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Winleafs.Api;
using Winleafs.Api.Timers;

using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;

using NLog;
using Winleafs.Wpf.Effects;

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
            Effects.Insert(0, new Effect { Name = Models.Models.Effects.Effect.AMBILIGHTEFFECTNAME });
            Effects.Insert(0, new Effect { Name = Models.Models.Effects.Effect.OFFEFFECTNAME });

            DataContext = this;

            Brightness = 70; //TODO: get the value from the device itself
        }

        private void Override_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Override());
        }

        private void StopOverride_Click(object sender, RoutedEventArgs e)
        {
            if (UserSettings.Settings.ActviceDevice.OperationMode == OperationMode.Manual)
            {
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
                    var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActviceDevice);

                    if (SelectedEffect == Models.Models.Effects.Effect.OFFEFFECTNAME)
                    {
                        await client.StateEndpoint.SetStateWithStateCheckAsync(false);
                    }
                    else if (SelectedEffect == Models.Models.Effects.Effect.AMBILIGHTEFFECTNAME)
                    {
                        AmbilightEffect.GetEffectForDevice(UserSettings.Settings.ActviceDevice).Start();
                    }
                    else
                    {
                        await client.StateEndpoint.SetStateWithStateCheckAsync(true); //Turn on device if it is not on
                        await client.EffectsEndpoint.SetSelectedEffectAsync(SelectedEffect);
                        await client.StateEndpoint.SetBrightnessAsync(_brightness);
                    }

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
