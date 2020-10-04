using System.Windows.Controls;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class EventTriggerUserControl : UserControl
    {
        private EventTrigger _trigger;
        private EventUserControl _parent;

        public string TriggerType { get; set; }
        public string Description { get; set; }
        public string EffectName { get; set; }
        public string Brightness { get; set; }
        public int Priority { get; set; }

        public EventTriggerUserControl(EventUserControl parent, EventTrigger trigger, bool highestPriority, bool lowestPriority)
        {
            _trigger = trigger;
            _parent = parent;

            InitializeComponent();

            TriggerType = ""; //TODO: replace by getting the values from the type of the trigger since we no longer have the enum EnumLocalizer.GetLocalizedEnum(trigger.GetTriggerType());
            Description = trigger.Description;
            EffectName = trigger.EffectName;
            Brightness = trigger.Brightness.ToString();
            Priority = trigger.Priority;

            if (highestPriority)
            {
                PriorityUpButton.Visibility = System.Windows.Visibility.Hidden;
            }
            
            if (lowestPriority)
            {
                PriorityDownButton.Visibility = System.Windows.Visibility.Hidden;
            }

            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteTrigger(_trigger);
        }

        private void PriorityUp_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.PriorityUp(Priority);
        }

        private void PriorityDown_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.PriorityDown(Priority);
        }
    }
}
