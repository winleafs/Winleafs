using Nanoleaf_wpf.Models.Scheduling;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TriggerUserControl.xaml
    /// </summary>
    public partial class TriggerUserControl : UserControl
    {
        public string TriggerName { get; set; }
        public string EffectName { get; set; }

        public TriggerUserControl(Trigger trigger)
        {
            InitializeComponent();

            TriggerName = trigger.GetDisplayName();
            EffectName = trigger.Effect;

            DataContext = this;
        }
    }
}
