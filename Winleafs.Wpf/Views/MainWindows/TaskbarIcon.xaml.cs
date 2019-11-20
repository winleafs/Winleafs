using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for TaskbarIcon.xaml
    /// </summary>
    public partial class TaskbarIcon : UserControl
    {
        private static readonly int _amountOfEffects = 5;

        private MainWindow _parent;

        private string _selectedDevice;

        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                if (_selectedDevice != value)
                {
                    _selectedDevice = value;
                    SelectedDeviceChanged();
                    DevicesDropdown.SelectedItem = _selectedDevice;
                }
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

        private string _selectedEffect;

        public ObservableCollection<string> DeviceNames { get; set; }

        public TaskbarIcon()
        {
            InitializeComponent();

            DataContext = this;
        }

        //Called after the MainWindow is intialized
        public void Initialize(MainWindow mainWindow)
        {
            _parent = mainWindow;

            DeviceNames = _parent.DeviceNames;
            Brightness = _parent.OverrideScheduleUserControl.Brightness;

            BuildMostUsedEfectList();
        }

        private void Quit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectedDeviceChanged()
        {
            if (_parent != null)
            {
                _parent.SelectedDevice = SelectedDevice; //Also trigger main window device change 
            }

            BuildMostUsedEfectList();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            _selectedEffect = null;
            _parent.OverrideScheduleUserControl.StopOverride();

            BuildMostUsedEfectList();
        }

        public void BuildMostUsedEfectList()
        {
            MostUsedEffectList.Children.Clear();

            if (UserSettings.Settings?.ActiveDevice?.EffectCounter == null)
            {
                return;
            }

            var mostUsedEffects = UserSettings.Settings.ActiveDevice.EffectCounter.Take(_amountOfEffects).ToList();

            foreach (var mostUsedEffect in mostUsedEffects)
            {
                MostUsedEffectList.Children.Add(new MostUsedEffectUserControl(this, mostUsedEffect.Key, mostUsedEffect.Key == _selectedEffect));
            }
        }

        public void EffectSelected(string effectName)
        {
            _selectedEffect = effectName;

            BuildMostUsedEfectList();

            SetOverride();
        }

        private void SetOverride()
        {
            _parent.OverrideScheduleUserControl.SetOverride(_selectedEffect, Brightness);
        }

        private void BrightnessSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            SetOverride();
        }
    }
}
