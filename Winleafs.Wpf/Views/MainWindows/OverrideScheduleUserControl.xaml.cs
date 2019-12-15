using NLog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Enums;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.MainWindows
{
    using System.ComponentModel;
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for OverrideScheduleUserControl.xaml
    /// </summary>
    public partial class OverrideScheduleUserControl : UserControl, INotifyPropertyChanged
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Effects { get; set; }

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

        public MainWindow MainWindow { get; set; }

        public OverrideScheduleUserControl()
        {
            InitializeComponent();

            Effects = new ObservableCollection<string>();
            LoadEffects();
            DataContext = this;

            Brightness = 70; //TODO: get the value from the device itself
        }

        public void LoadEffects()
        {
            // Effects collection is not rebuild.
            Effects.Clear();

            var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

            if (orchestrator == null)
            {
                return;
            }

            var effects = orchestrator.GetCustomEffectAsEffects();
            effects.AddRange(UserSettings.Settings.ActiveDevice.Effects);

            foreach (var effect in effects)
            {
                Effects.Add(effect.Name);
            }
        }

        private void Override_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => Override());
        }

        private void StopOverride_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => StopOverrideAsync());
        }

        private async Task StopOverrideAsync()
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
            try
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

                if (string.IsNullOrEmpty(SelectedEffect))
                {
                    SetSelectedEffect(orchestrator.GetActiveEffectName());
                } 

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
                ToastHelper.ShowNotification(MainWindows.Resources.OverrideError, ToastLogLevel.Information);
            }
        }

        public void SetOverride(string effectName, int brightness)
        {
            SetSelectedEffect(effectName);
            Brightness = brightness;

            Task.Run(() => Override());
        }

        public void StopOverride()
        {
            Task.Run(() => StopOverrideAsync());
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void SetSelectedEffect(string effectName)
        {
            SelectedEffect = effectName;
            OnPropertyChanged(nameof(SelectedEffect));
        }
    }
}
