using System.Windows.Controls;

using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class TimeTriggerUserControl : UserControl
    {
        private TimeTrigger _trigger;
        private DayUserControl _parent;

        public string Description { get; set; }
        public string EffectName { get; set; }

        public TimeTriggerUserControl(DayUserControl parent, TimeTrigger trigger)
        {
            _trigger = trigger;
            _parent = parent;

            InitializeComponent();

            Description = $"{EnumLocalizer.GetLocalizedEnum(trigger.GetTriggerType())}: {trigger.GetDescription()}";
            EffectName = trigger.Effect;

            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteTrigger(_trigger);
        }
    }
}
