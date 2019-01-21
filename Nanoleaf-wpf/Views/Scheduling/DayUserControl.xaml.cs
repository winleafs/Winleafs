using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models.Scheduling;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class DayUserControl : UserControl
    {
        public string NameOfDay { get; set; }
        public int IndexOfDay { get; set; }

        public Program Program { get; set; }

        public DayUserControl()
        {
            InitializeComponent();
            DataContext = this; //With this we can use the variables in the view
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addTriggerWindow = new AddTimeTriggerWindow(this);
            addTriggerWindow.Show();
        }

        public void TriggerAdded(TriggerType triggerType, int hours, int minutes, string effect)
        {
            Program.AddTrigger(triggerType, hours, minutes, effect);

            BuildTriggerList();
        }

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in Program.Triggers)
            {
                TriggerList.Children.Add(new TriggerUserControl(trigger));
            }
        }
    }
}
