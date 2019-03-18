using System.Collections.Generic;
using System.Windows.Controls;

using Winleafs.Models.Enums;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        public List<IEventTrigger> EventTriggers { get; set; }

        public EventUserControl()
        {
            InitializeComponent();
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        public void TriggerAdded(TriggerType triggerType, int hours, int minutes, string effect)
        {
            //TODO: add trigger

            BuildTriggerList();
        }

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in EventTriggers)
            {
                //TODO: add event trigger user controls
                ////TriggerList.Children.Add(new TimeTriggerUserControl(trigger));
            }
        }
    }
}
