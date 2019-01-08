using Nanoleaf_wpf.Models.Scheduling;
using Nanoleaf_wpf.Models.Scheduling.Triggers;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class EventUserControl : UserControl
    {
        public List<IEventTrigger> Triggers;

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

            BuildProgramList();
        }

        private void BuildProgramList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in Triggers)
            {
                TriggerList.Children.Add(new TriggerUserControl(trigger));
            }
        }
    }
}
