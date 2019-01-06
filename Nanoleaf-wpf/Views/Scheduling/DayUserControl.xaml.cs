using Nanoleaf_wpf.Models.Scheduling;
using System.Linq;
using System.Windows.Controls;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class DayUserControl : UserControl
    {
        public string NameOfDay { get; set; }
        public Program Program { get; set; }

        public DayUserControl()
        {
            InitializeComponent();
            DataContext = this; //With this we can use the variables in the view
            Program = new Program(); //Placeholder
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addTriggerWindow = new AddTriggerWindow(this);
            addTriggerWindow.Show();
        }

        public void TriggerAdded(TriggerType triggerType, int hours, int minutes, string effect)
        {
            Program.Triggers.Add(new Trigger
            {
                Effect = effect,
                Minutes = minutes,
                Hours = hours,
                Type = triggerType
            });

            Program.Triggers = Program.Triggers.OrderBy(t => t.Hours).ThenBy(t => t.Minutes).ToList();

            BuildProgramList();
        }

        private void BuildProgramList()
        {
            EventList.Children.Clear();
            TimeList.Children.Clear();

            foreach (var trigger in Program.Triggers)
            {
                if (trigger.Type == TriggerType.Time)
                {
                    TimeList.Children.Add(new TriggerUserControl(trigger));
                }
                else
                {
                    EventList.Children.Add(new TriggerUserControl(trigger));
                }
            }
        }
    }
}
