using System.Windows;
using System.Windows.Controls;

using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Views.Scheduling
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
            if (!UserSettings.Settings.SunriseHour.HasValue)
            {
                MessageBox.Show("Please fill in your location before creating a time based trigger");
                return;
            }

            var addTriggerWindow = new AddTimeTriggerWindow(this);
            addTriggerWindow.ShowDialog();
        }

        public void TriggerAdded(TimeTrigger trigger)
        {
            Program.AddTrigger(trigger);

            BuildTriggerList();
        }

        public void DeleteTrigger(TimeTrigger trigger)
        {
            Program.Triggers.Remove(trigger);

            BuildTriggerList();
        }

        public void BuildTriggerList()
        {
            TriggerList.Children.Clear();

            foreach (var trigger in Program.Triggers)
            {
                TriggerList.Children.Add(new TimeTriggerUserControl(this, trigger));
            }
        }
    }
}
