using NLog;
using System;
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
        private string _selectedEffect;

        public DeviceUserControl(Device device, MainWindow parent)
        {
            InitializeComponent();

            _device = device;
            RegisterWithOrchestrator();

            _parent = parent;

            DeviceNameLabel.Content = _device.Name;

            var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);

            //Set the correct icon
            switch (orchestrator.PanelLayout.DeviceType)
            {
                case DeviceType.Canvas:
                    SquareIcon.Visibility = Visibility.Visible;
                    break;
                case DeviceType.Aurora:
                    TriangleIcon.Visibility = Visibility.Visible;
                    break;
                default:
                    throw new NotImplementedException($"No icon implemented for device type {orchestrator.PanelLayout.DeviceType}");
            }

            //Initialize the effect combox box
            EffectComboBox.InitializeEffects(orchestrator);
            EffectComboBox.ParentUserControl = this;

            DataContext = this;

            Update();
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
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);
                await orchestrator.TrySetOperationMode(OperationMode.Schedule, true, true);

                if (!orchestrator.HasActiveEffect())
                {
                    //Special case: when the user has no active schedule, the schedule will not set an effect and therefore
                    //not trigger an update to the effects, hence we need to call update here
                    Update();
                }
            }
        }

        private async Task SelectedEffectChanged()
        {
            try
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);
                if (await orchestrator.TrySetOperationMode(OperationMode.Manual, true, true))
                {
                    _logger.Info($"User manually enabling effect {_selectedEffect} with brightness {Brightness} for device {_device.IPAddress}");

                    _device.ManualEffect = _selectedEffect;
                    _device.ManualBrightness = Brightness;

                    await orchestrator.ActivateEffect(_selectedEffect, Brightness);

                    UserSettings.Settings.SaveSettings();
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
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);
                var activeEffect = orchestrator.GetActiveEffectName();

                if (string.IsNullOrEmpty(activeEffect))
                {
                    ActiveEffectLabel.Content = MainWindows.Resources.None;
                }
                else
                {
                    ActiveEffectLabel.Content = $"{activeEffect} ({EnumLocalizer.GetLocalizedEnum(_device.OperationMode)})";
                }

                _selectedEffect = activeEffect;

                var activeBrightness = orchestrator.GetActiveBrightness();
                Brightness = activeBrightness < 0 ? 0 : activeBrightness;
                OnPropertyChanged(nameof(Brightness)); //Call property changed to correctly set slider position

                //Update the colors in the mainwindow
                _parent.UpdateLayoutColors(_device.Name);

                //Update the dropdown
                EffectComboBox.UpdateSelection(_selectedEffect);
            }));            
        }

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            _selectedEffect = selectedEffect;
            Task.Run(() => SelectedEffectChanged());
        }

        public void ReloadEffects()
        {
            //Reset the orchestrator since it can happen that the orchestrators have been reset before calling this function
            RegisterWithOrchestrator();

            var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);

            EffectComboBox.InitializeEffects(orchestrator);
            
            EffectComboBox.UpdateSelection(_selectedEffect);
        }

        public void RegisterWithOrchestrator()
        {
            var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(_device);

            orchestrator.AddEffectChangedCallback(Update);
        }
    }
}
