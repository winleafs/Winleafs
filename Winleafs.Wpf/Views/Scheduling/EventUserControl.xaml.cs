using System.Collections.Generic;
using System.Linq;
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
                TimeType = TimeType.ProcessEvent,
                ProcessName = processName,
                Priority = EventTriggers.Count == 0 ? 1 : EventTriggers.Max(eventTrigger => eventTrigger.Priority) + 1
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

                var spotifyEventWindow = new AddSpotifyEventWindow(this, playlist);
                spotifyEventWindow.ShowDialog();
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
                TimeType = TimeType.SpotifyEvent,
                PlaylistName = playlistName,
                PlaylistId = playlistId,
                Priority = EventTriggers.Count == 0 ? 1 : EventTriggers.Max(eventTrigger => eventTrigger.Priority) + 1
            });

            BuildTriggerList();

            return true;
        }

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            EventTriggers.Sort(); //Do not use OrderBy().ToList() since the schedule then loses its binding to the EventTriggers object

            var maxPriority = EventTriggers.Any() ? EventTriggers.Last().Priority : 0;

            foreach (var trigger in EventTriggers)
            {
                TriggerList.Children.Add(
                    new EventTriggerUserControl(
                        this,
                        trigger,
                        trigger.Priority <= 1,
                        trigger.Priority == maxPriority));
            }
        }

        public void DeleteTrigger(TriggerBase trigger)
        {
            EventTriggers.Remove(trigger);

            EventTriggers.Sort(); //Do not use OrderBy().ToList() since the schedule then loses its binding to the EventTriggers object

            //Restore the gap in priorities when a trigger is deleted
            for (var i = 0; i < EventTriggers.Count; i++)
            {
                EventTriggers[i].Priority = i + 1;
            }

            BuildTriggerList();
        }

        /// <summary>
        /// Increases the priority of the event trigger with the given
        /// <paramref name="priorityOfItem"/>.
        /// </summary>
        /// <remarks>
        /// Increasing means positioning it higher in the priority,
        /// hence decreasing the priority value.
        /// </remarks>
        public void PriorityUp(int priorityOfItem)
        {
            EventTriggers.Sort(); //Do not use OrderBy().ToList() since the schedule then loses its binding to the EventTriggers object

            //This works on the assumption that priority is always a positive integer and there are no gaps in the priorities in triggers
            EventTriggers[priorityOfItem - 1].Priority--; //Current item should go up in priority
            EventTriggers[priorityOfItem - 2].Priority++; //Item before current item should go down in priority

            BuildTriggerList();
        }

        /// <summary>
        /// Decreases the priority of the event trigger with the given
        /// <paramref name="priorityOfItem"/>.
        /// </summary>
        /// <remarks>
        /// Decreasing means positioning it lower in the priority,
        /// hence increasing the priority value.
        /// </remarks>
        public void PriorityDown(int priorityOfItem)
        {
            EventTriggers.Sort(); //Do not use OrderBy().ToList() since the schedule then loses its binding to the EventTriggers object

            //This works on the assumption that priority is always a positive integer and there are no gaps in the priorities in triggers
            EventTriggers[priorityOfItem].Priority--; //Item after current item should go up in priority
            EventTriggers[priorityOfItem - 1].Priority++; //Current item should go down in priority

            BuildTriggerList();
        }
    }
}
