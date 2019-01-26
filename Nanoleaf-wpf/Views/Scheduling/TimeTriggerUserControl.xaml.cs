using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class TimeTriggerUserControl : UserControl
    {
        private TimeTrigger _trigger;
        private DayUserControl _parent;

        public string TriggerName { get; set; }
        public string Description { get; set; }

        public TimeTriggerUserControl(DayUserControl parent, TimeTrigger trigger)
        {
            _trigger = trigger;
            _parent = parent;

            InitializeComponent();

            TriggerName = trigger.GetDisplayName();
            Description = trigger.GetDescription();

            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteTrigger(_trigger);
        }
    }
}
