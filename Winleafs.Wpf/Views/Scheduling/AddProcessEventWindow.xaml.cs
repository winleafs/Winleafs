using System.Collections.Generic;
using System.Windows;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Scheduling
{

    /// <summary>
    /// Interaction logic for AddTriggerWindow.xaml
    /// </summary>
    public partial class AddProcessEventWindow : Window
    {
        private EventUserControl _parent;
        private int _brightness { get; set; }

        public string SelectedEffect { get; set; }
        public string ProcessName { get; set; }

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }
        
        public List<Effect> Effects { get; set; }

        public AddProcessEventWindow(EventUserControl parent)
        {
            _parent = parent;
            Effects = new List<Effect>(UserSettings.Settings.ActiveDevice.Effects);
            Effects.InsertRange(0, OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice).GetCustomEffectAsEffects());

            DataContext = this;

            InitializeComponent();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_parent.ProcessEventTriggerAdded(ProcessName, SelectedEffect, _brightness))
            {
                Close();
            }            
        }
    }
}
