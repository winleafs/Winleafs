using System.Windows;
using Winleafs.Wpf.Views.Effects;

namespace Winleafs.Wpf.Views.Scheduling
{

    /// <summary>
    /// Interaction logic for AddTriggerWindow.xaml
    /// </summary>
    public partial class AddSpotifyEventWindow : Window, IEffectComboBoxContainer
    {
        private EventUserControl _parent;
        private int _brightness { get; set; }
        public string PlaylistName { get; set; }

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public AddSpotifyEventWindow(EventUserControl parent)
        {
            _parent = parent;

            DataContext = this;

            InitializeComponent();

            EffectComboBox.InitializeEffects();
            EffectComboBox.ParentUserControl = this;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (_parent.SpotifyEventTriggerAdded(PlaylistName, EffectComboBox.SelectedEffect?.EffectName, _brightness))
            {
                Close();
            }        
        }

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            //We do not need to do anything when the selection changed
        }
    }
}
