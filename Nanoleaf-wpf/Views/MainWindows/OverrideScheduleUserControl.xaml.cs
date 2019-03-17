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

            Effects = new List<Effect>(UserSettings.Settings.ActiveDevice.Effects);
            Effects.InsertRange(0, OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice).GetCustomEffectAsEffects());

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
            if (UserSettings.Settings.ActiveDevice.OperationMode == OperationMode.Manual)
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

                if (await orchestrator.TrySetOperationMode(OperationMode.Schedule, true))
                {
                    MainWindow.UpdateCurrentEffectLabelsAndLayout();
                }
            }
        }

        private async Task Override()
        {
            if (!string.IsNullOrEmpty(SelectedEffect))
            {
                try
                {
                    var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

                    if (await orchestrator.TrySetOperationMode(OperationMode.Manual, true))
                    {
                        orchestrator.Device.OverrideEffect = SelectedEffect;
                        orchestrator.Device.OverrideBrightness = Brightness;

                        await orchestrator.ActivateEffect(SelectedEffect, Brightness);

                        MainWindow.UpdateCurrentEffectLabelsAndLayout();
                    }                    
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
