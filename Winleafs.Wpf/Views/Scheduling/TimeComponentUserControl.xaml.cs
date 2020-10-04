using System.Windows.Controls;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TimeComponentUserControl.xaml
    /// </summary>
    public partial class TimeComponentUserControl : UserControl
    {
        public TimeComponentUserControl()
        {
            InitializeComponent();
        }

        public TimeType GetTimeType()
        {
            if (FixedTimeRadioButton.IsChecked == true)
            {
                return TimeType.FixedTime;
            }
            else if (SunriseRadioButton.IsChecked == true)
            {
                return TimeType.Sunrise;
            }
            else
            {
                return TimeType.Sunset;
            }
        }

        public BeforeAfter GetBeforeAfter()
        {
            //TODO: add check if time is filled in, otherwsie None
            if (BeforeRadioButton.IsChecked == true)
            {
                return BeforeAfter.Before;
            }
            else if (AfterRadioButton.IsChecked == true)
            {
                return BeforeAfter.After;
            }
            else
            {
                return BeforeAfter.None;
            }
        }
    }
}
