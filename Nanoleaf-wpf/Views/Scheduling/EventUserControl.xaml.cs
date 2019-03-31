using System.Collections.Generic;
using System.Windows.Controls;
using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        public List<BaseEventTrigger> EventTriggers { get; set; }

        public EventUserControl()
        {
            InitializeComponent();
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addProcessEventWindow = new AddProcessEventWindow(this);
            addProcessEventWindow.ShowDialog();
        }

        public bool ProcessEventTriggerAdded(string processName, string effectName, int brightness)
        {
            processName = processName.Trim();

            if (string.IsNullOrEmpty(processName))
            {
                PopupCreator.Error(Scheduling.Resources.ProcessNameCanNotBeEmpty);
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

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in EventTriggers)
            {
                TriggerList.Children.Add(new EventTriggerUserControl(this, trigger));
            }
        }

        public void DeleteTrigger(BaseEventTrigger trigger)
        {
            EventTriggers.Remove(trigger);

            BuildTriggerList();
        }
    }
}
