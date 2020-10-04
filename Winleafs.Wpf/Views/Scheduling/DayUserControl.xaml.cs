using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling;
using Winleafs.Models.Models.Scheduling.Triggers;

namespace Winleafs.Wpf.Views.Scheduling
{
    using Winleafs.Wpf.Views.Popup;

    /// <summary>
    /// Interaction logic for DayUserControl.xaml
    /// </summary>
    public partial class DayUserControl : UserControl
    {
        public string NameOfDay { get; set; }
        public int IndexOfDay { get; set; }

        public Program Program { get; set; }
        public ManageScheduleWindow ParentWindow { get; set; }

        public DayUserControl()
        {
            InitializeComponent();
            DataContext = this; //With this we can use the variables in the view
        }

        private void Plus_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (!UserSettings.Settings.SunriseHour.HasValue)
            {
                PopupCreator.Error(Scheduling.Resources.LocationMissing);
                return;
            }

            var addTriggerWindow = new AddTimeTriggerWindow(this);
            addTriggerWindow.ShowDialog();
        }

        /// <summary>
        /// Adds a trigger to the program, returns false when overlaps and adding failed
        /// </summary>
        public bool TriggerAdded(ScheduleTrigger trigger)
        {
            if (!Program.TriggerOverlaps(trigger))
            {
                Program.AddTrigger(trigger);

                BuildTriggerList();

                return true;
            }

            return false;
        }

        public void DeleteTrigger(ScheduleTrigger trigger)
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

        private void AllDays_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 0, 1, 2, 3, 4, 5, 6 });
        }

        private void Weekdays_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 0, 1, 2, 3, 4});
        }

        private void Weekends_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 5, 6 });
        }

        private void Monday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 0 });
        }

        private void Tuesday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 1 });
        }

        private void Wednesday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 2 });
        }

        private void Thursday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 3 });
        }

        private void Friday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 4 });
        }

        private void Saturday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 5 });
        }

        private void Sunday_Click(object sender, RoutedEventArgs e)
        {
            ParentWindow.CopyProgramToDays(Program, new List<int> { 6 });
        }
    }
}
