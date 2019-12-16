using System.Windows.Controls;

using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class EventTriggerUserControl : UserControl
    {
        private TriggerBase _trigger;
        private EventUserControl _parent;

        public string TriggerType { get; set; }
        public string Description { get; set; }
        public string EffectName { get; set; }
        public string Brightness { get; set; }

        public EventTriggerUserControl(EventUserControl parent, TriggerBase trigger)
        {
            _trigger = trigger;
            _parent = parent;

            InitializeComponent();

            TriggerType = EnumLocalizer.GetLocalizedEnum(trigger.GetTriggerType());
            Description = trigger.GetDescription();
            EffectName = trigger.GetEffectName();
            Brightness = trigger.GetBrightness().ToString();

            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteTrigger(_trigger);
        }
    }
}
