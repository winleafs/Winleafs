using Nanoleaf_wpf.Models.Scheduling;
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
        }

        //This method will be called by the parent window to fill this window with the events the user defined
        public void FillEvents(Program program)
        {
            Program = program;
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var addTriggerWindow = new AddTriggerWindow(this);
            addTriggerWindow.Show();
        }

        public void TriggerAdded(TriggerType triggerType, int hour, int minute, string effect)
        {

        }
    }
}
