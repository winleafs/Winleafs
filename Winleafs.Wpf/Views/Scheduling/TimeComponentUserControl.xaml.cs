using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Winleafs.Models.Enums;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TimeComponentUserControl.xaml
    /// </summary>
    public partial class TimeComponentUserControl : UserControl
    {
        public TimeType TimeType;
        public BeforeAfter BeforeAfter;
        private Dictionary<string, BeforeAfter> _beforeAfterMapping { get; set; } //Map display values to enum values

        public string SelectedBeforeAfter
        {
            get
            {
                if (BeforeAfter == BeforeAfter.None)
                {
                    return string.Empty;
                }

                return EnumLocalizer.GetLocalizedEnum(BeforeAfter);
            }
            set
            {
                BeforeAfter = _beforeAfterMapping[value];
            }
        }

        public IEnumerable<string> BeforeAfterDisplayValues
        {
            get
            {
                return _beforeAfterMapping.Keys;
            }
        }

        public TimeComponentUserControl()
        {
            _beforeAfterMapping = new Dictionary<string, BeforeAfter>()
            {
                { string.Empty, BeforeAfter.None }, //Empty option
                {  EnumLocalizer.GetLocalizedEnum(BeforeAfter.Before), BeforeAfter.Before },
                {  EnumLocalizer.GetLocalizedEnum(BeforeAfter.After), BeforeAfter.After },
            };

            SelectedBeforeAfter = string.Empty;

            DataContext = this;

            InitializeComponent();

            SetTimeType(TimeType.FixedTime);
        }

        private void FixedTime_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTimeType(TimeType.FixedTime);
        }

        private void Sunrise_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTimeType(TimeType.Sunrise);
        }

        private void Sunset_MouseDown(object sender, MouseButtonEventArgs e)
        {
            SetTimeType(TimeType.Sunset);
        }

        private void FixedTimeRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SetTimeType(TimeType.FixedTime);
        }

        private void SunriseRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SetTimeType(TimeType.Sunrise);
        }

        private void SunsetRadioButton_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            SetTimeType(TimeType.Sunset);
        }

        private void SetTimeType(TimeType timeType)
        {
            TimeType = timeType;

            if (TimeType == TimeType.FixedTime)
            {
                ExtraTimeGrid.Visibility = System.Windows.Visibility.Hidden;

                FixedTimeRadioButton.IsChecked = true;
                SunriseRadioButton.IsChecked = false;
                SunsetRadioButton.IsChecked = false;
            }
            else
            {
                ExtraTimeGrid.Visibility = System.Windows.Visibility.Visible;

                if (TimeType == TimeType.Sunrise)
                {
                    SunriseLabel.Visibility = System.Windows.Visibility.Visible;
                    SunsetLabel.Visibility = System.Windows.Visibility.Hidden;

                    FixedTimeRadioButton.IsChecked = false;
                    SunriseRadioButton.IsChecked = true;
                    SunsetRadioButton.IsChecked = false;
                }
                else
                {
                    SunriseLabel.Visibility = System.Windows.Visibility.Hidden;
                    SunsetLabel.Visibility = System.Windows.Visibility.Visible;

                    FixedTimeRadioButton.IsChecked = false;
                    SunriseRadioButton.IsChecked = false;
                    SunsetRadioButton.IsChecked = true;
                }
            }
        }
    }
}
