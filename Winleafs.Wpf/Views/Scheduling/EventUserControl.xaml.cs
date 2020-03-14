using System.Collections.Generic;
using System.Windows.Controls;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;
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
            if (Spotify.WebAPIIsConnected())
            {
                var addSpotiofyEventWindow = new AddSpotifyEventWindow(this);
                addSpotiofyEventWindow.ShowDialog();
            }
            else
            {
                PopupCreator.Error(Scheduling.Resources.ConnectToSpotify);
            }
        }

        public bool SpotifyEventTriggerAdded(string playlistname, string effectName, int brightness)
        {
            if (string.IsNullOrWhiteSpace(playlistname))
            {
                PopupCreator.Error(Scheduling.Resources.PlaylistNameCanNotBeEmpty);
                return false;
            }

            playlistname = playlistname.Trim();

            if (string.IsNullOrEmpty(effectName))
            {
                PopupCreator.Error(Scheduling.Resources.MustChooseEffect);
                return false;
            }

            foreach (var eventTrigger in EventTriggers)
            {
                var spotifyEventTrigger = eventTrigger as SpotifyEventTrigger;
                if (spotifyEventTrigger != null && spotifyEventTrigger.PlaylistName.ToLower().Equals(playlistname.ToLower()))
                {
                    PopupCreator.Error(string.Format(Scheduling.Resources.PlaylistNameAlreadyExists, playlistname));
                    return false;
                }
            }

            EventTriggers.Add(new SpotifyEventTrigger()
            {
                Brightness = brightness,
                EffectName = effectName,
                EventTriggerType = TriggerType.SpotifyEvent,
                PlaylistName = playlistname
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
