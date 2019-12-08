using NLog;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api;

namespace Winleafs.Wpf.Views.MainWindows
{
    using System.ComponentModel;
    using Winleafs.Wpf.Helpers;
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for DeviceUserControl.xaml
    /// </summary>
    public partial class DeviceUserControl : UserControl, INotifyPropertyChanged
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Effects { get; set; }

        private string _selectedEffect;

        public string SelectedEffect
        {
            get { return _selectedEffect;  }
            set
            {
                _selectedEffect = value;
                OnPropertyChanged(nameof(SelectedEffect));
            }
        }

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

        private Device _device;
        private Orchestrator _orchestrator;

        public DeviceUserControl(Device device)
        {
            InitializeComponent();

            _device = device;
            _orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);

            Effects = new ObservableCollection<string>();
            LoadEffects();
            DataContext = this;

            Update();

            DeviceNameLabel.Content = _device.Name;
        }

        public void LoadEffects()
        {
            Effects.Clear();

            var effects = _orchestrator.GetCustomEffectAsEffects();
            effects.AddRange(_device.Effects);

            foreach (var effect in effects)
            {
                Effects.Add(effect.Name);
            }
        }

        private void StopManual_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => StopManualAsync());
        }

        private void BrightnessSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Task.Run(() => SelectedEffectChanged());
        }

        private void EffectsDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (EffectsDropdown.IsDropDownOpen)
            {
                //Only tirgger the update if the dropdown is open, i.e. the user uses the dropdown
                Task.Run(() => SelectedEffectChanged());
            }            
        }

        private async Task StopManualAsync()
        {
            if (_device.OperationMode == OperationMode.Manual)
            {
                if (await _orchestrator.TrySetOperationMode(OperationMode.Schedule, true, true))
                {
                    Update();
                }
            }
        }

        private async Task SelectedEffectChanged()
        {
            try
            {
                if (await _orchestrator.TrySetOperationMode(OperationMode.Manual, true, true))
                {
                    _device.ManualEffect = SelectedEffect;
                    _device.ManualBrightness = Brightness;

                    await _orchestrator.ActivateEffect(SelectedEffect, Brightness);

                    UserSettings.Settings.SaveSettings();

                    Update();
                }
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error during setting manual control");
                PopupCreator.Error(MainWindows.Resources.ManualControlError);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Updates all elements to display the current effect and brightness
        /// </summary>
        private void Update()
        { 
            //Update UI on main thread
            Dispatcher.Invoke(new Action(() =>
            {
                var activeEffect = _orchestrator.GetActiveEffectName();

                if (string.IsNullOrEmpty(activeEffect))
                {
                    ActiveEffectLabel.Content = MainWindows.Resources.Nothing;
                }
                else
                {
                    ActiveEffectLabel.Content = $"{activeEffect} ({EnumLocalizer.GetLocalizedEnum(_device.OperationMode)})";
                }
                
                SelectedEffect = activeEffect;

                var activeBrightness = _orchestrator.GetActiveBrightness();
                Brightness = activeBrightness < 0 ? 0 : activeBrightness;
            }));            
        }
    }
}
