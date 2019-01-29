using System.Windows.Controls;

using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Views.Scheduling
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
