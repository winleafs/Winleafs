using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Winleafs.Wpf.ViewModels;
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
        public List<SpotifyPlaylistViewModel> Playlists { get; set; }
        public SpotifyPlaylistViewModel SelectedPlaylist { get; set; }

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public AddSpotifyEventWindow(EventUserControl parent, Dictionary<string, string> playlists)
        {
            _parent = parent;

            Playlists = playlists.Select(playlist => new SpotifyPlaylistViewModel { PlaylistId = playlist.Key, PlaylistName = playlist.Value }).ToList();

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
            if (_parent.SpotifyEventTriggerAdded(SelectedPlaylist.PlaylistId, SelectedPlaylist.PlaylistName, EffectComboBox.SelectedEffect?.EffectName, _brightness, null, null))
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
