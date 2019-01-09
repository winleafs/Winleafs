using Nanoleaf_Models.Models.Scheduling.Triggers;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class TriggerUserControl : UserControl
    {
        public string TriggerName { get; set; }
        public string Description { get; set; }

        public TriggerUserControl(ITrigger trigger)
        {
            InitializeComponent();

            TriggerName = trigger.GetDisplayName();
            Description = trigger.GetDescription();

            DataContext = this;
        }
    }
}
