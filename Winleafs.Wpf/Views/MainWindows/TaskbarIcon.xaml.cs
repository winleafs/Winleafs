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

            BuildMostUsedEfectList(null);
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

            BuildMostUsedEfectList(null);
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void BuildMostUsedEfectList(string selectedEffect)
        {
            MostUsedEffectList.Children.Clear();

            var mostUsedEffects = UserSettings.Settings.ActiveDevice.EffectCounter.Take(5).ToList();

            foreach (var mostUsedEffect in mostUsedEffects)
            {
                MostUsedEffectList.Children.Add(new MostUsedEffectUserControl(this, mostUsedEffect.Key, mostUsedEffect.Key == selectedEffect));
            }
        }

        //TODO: when clicking an effect or changing brightness, set the properties of the override control in the mainwindow
        public void EffectSelected(string effectName)
        {
            BuildMostUsedEfectList(effectName);
        }
    }
}
