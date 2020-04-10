using System.Collections.Generic;
using System.Windows.Controls;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Server;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        public List<TriggerBase> EventTriggers { get; set; }

        public EventUserControl()
        {
            InitializeComponent();
        }

        private void AddProcessEvent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addProcessEventWindow = new AddProcessEventWindow(this);
            addProcessEventWindow.ShowDialog();
        }

        public bool ProcessEventTriggerAdded(string processName, string effectName, int brightness)
        {
            if (string.IsNullOrWhiteSpace(processName))
            {
                PopupCreator.Error(Scheduling.Resources.ProcessNameCanNotBeEmpty);
                return false;
            }

            processName = processName.Trim();

            if (string.IsNullOrEmpty(effectName))
            {
                PopupCreator.Error(Scheduling.Resources.MustChooseEffect);
                return false;
            }

            foreach (var eventTrigger in EventTriggers)
            {
                var processEventTrigger = eventTrigger as ProcessEventTrigger;
                if (processEventTrigger != null && processEventTrigger.ProcessName.ToLower().Equals(processName.ToLower()))
                {
                    PopupCreator.Error(string.Format(Scheduling.Resources.ProcessNameAlreadyExists, processName));
                    return false;
                }
            }

            EventTriggers.Add(new ProcessEventTrigger()
            {
                Brightness = brightness,
                EffectName = effectName,
                EventTriggerType = TriggerType.ProcessEvent,
                ProcessName = processName
            });

            BuildTriggerList();

            return true;
        }

        private void AddSpotifyEvent_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var winleafsServerClient = new WinleafsServerClient();
            
            try
            {
                var playlist = winleafsServerClient.SpotifyEndpoint.GetPlaylists();

                var addSpotiofyEventWindow = new AddSpotifyEventWindow(this, playlist);
                addSpotiofyEventWindow.ShowDialog();
            }
            catch
            {
                PopupCreator.Error(Scheduling.Resources.ConnectToSpotifyOrError);
            }
        }

        public bool SpotifyEventTriggerAdded(string playlistId, string playlistName, string effectName, int brightness)
        {
            if (string.IsNullOrWhiteSpace(playlistId) || string.IsNullOrWhiteSpace(playlistName))
            {
                PopupCreator.Error(Scheduling.Resources.PlaylistCanNotBeEmpty);
                return false;
            }

            playlistName = playlistName.Trim();

            if (string.IsNullOrEmpty(effectName))
            {
                PopupCreator.Error(Scheduling.Resources.MustChooseEffect);
                return false;
            }

            foreach (var eventTrigger in EventTriggers)
            {
                var spotifyEventTrigger = eventTrigger as SpotifyEventTrigger;
                if (spotifyEventTrigger != null && spotifyEventTrigger.PlaylistId.ToLower().Equals(playlistId.ToLower()))
                {
                    PopupCreator.Error(string.Format(Scheduling.Resources.PlaylistAlreadyExists, playlistName));
                    return false;
                }
            }

            EventTriggers.Add(new SpotifyEventTrigger()
            {
                Brightness = brightness,
                EffectName = effectName,
                EventTriggerType = TriggerType.SpotifyEvent,
                PlaylistName = playlistName,
                PlaylistId = playlistId
            });

            BuildTriggerList();

            return true;
        }

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in EventTriggers)
            {
                TriggerList.Children.Add(new EventTriggerUserControl(this, trigger));
            }
        }

        public void DeleteTrigger(TriggerBase trigger)
        {
            EventTriggers.Remove(trigger);

            BuildTriggerList();
        }
    }
}
