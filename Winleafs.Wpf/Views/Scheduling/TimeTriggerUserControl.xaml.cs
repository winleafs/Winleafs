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
        private ScheduleTrigger _trigger;
        private DayUserControl _parent;

        public string Description { get; set; }
        public string EffectName { get; set; }
        public string Brightness { get; set; }

        public TimeTriggerUserControl(DayUserControl parent, ScheduleTrigger trigger)
        {
            _trigger = trigger;
            _parent = parent;

            InitializeComponent();

            Description = $"{EnumLocalizer.GetLocalizedEnum(trigger.TimeComponent.TimeType)}: {trigger.Description}";
            EffectName = trigger.EffectName;
            Brightness = trigger.Brightness.ToString();

            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteTrigger(_trigger);
        }
    }
}
