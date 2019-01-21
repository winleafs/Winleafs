using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        public List<IEventTrigger> EventTriggers;

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
                TriggerList.Children.Add(new TriggerUserControl(trigger));
            }
        }
    }
}
