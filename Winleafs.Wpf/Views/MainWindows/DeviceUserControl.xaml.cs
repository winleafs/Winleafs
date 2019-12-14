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
    using Winleafs.Wpf.Views.Effects;
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for DeviceUserControl.xaml
    /// </summary>
    public partial class DeviceUserControl : UserControl, INotifyPropertyChanged, IEffectComboBoxContainer
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public event PropertyChangedEventHandler PropertyChanged;

        private readonly MainWindow _parent;

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
        private string _selectedEffect;

        public DeviceUserControl(Device device, MainWindow parent)
        {
            InitializeComponent();

            _device = device;
            _orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);
            _parent = parent;

            //Initialize the effect combox box
            EffectComboBox.InitializeEffects(_orchestrator);
            EffectComboBox.Parent = this;

            DataContext = this;

            Update();

            DeviceNameLabel.Content = _device.Name;
        }

        private void StopManual_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() => StopManualAsync());
        }

        private void BrightnessSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            Task.Run(() => SelectedEffectChanged());
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
                    _device.ManualEffect = _selectedEffect;
                    _device.ManualBrightness = Brightness;

                    await _orchestrator.ActivateEffect(_selectedEffect, Brightness);

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
        public void Update()
        { 
            //Update UI on main thread
            Dispatcher.Invoke(new Action(() =>
            {
                var activeEffect = _orchestrator.GetActiveEffectName();

                if (string.IsNullOrEmpty(activeEffect))
                {
                    ActiveEffectLabel.Content = MainWindows.Resources.None;
                }
                else
                {
                    ActiveEffectLabel.Content = $"{activeEffect} ({EnumLocalizer.GetLocalizedEnum(_device.OperationMode)})";
                }

                _selectedEffect = activeEffect;

                var activeBrightness = _orchestrator.GetActiveBrightness();
                Brightness = activeBrightness < 0 ? 0 : activeBrightness;

                //Update the colors in the mainwindow
                _parent.UpdateLayoutColors(_device.Name);

                //TODO: Update the dropdown
            }));            
        }

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            _selectedEffect = selectedEffect;
            Task.Run(() => SelectedEffectChanged());
        }
    }
}
