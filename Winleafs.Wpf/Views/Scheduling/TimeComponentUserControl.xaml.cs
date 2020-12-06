using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using Winleafs.Models.Enums;
using Winleafs.Models.Exceptions;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for TimeComponentUserControl.xaml
    /// </summary>
    public partial class TimeComponentUserControl : UserControl
    {
        public TimeType TimeType { get; set; }
        public BeforeAfter BeforeAfter { get; set; }

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

        public TimeComponent AsTimeComponent()
        {
            if (TimeType == TimeType.Sunrise || TimeType == TimeType.Sunset)
            {
                return new TimeComponent
                    {
                        TimeType = TimeType,
                        BeforeAfter = BeforeAfter,
                        ExtraHours = TimePicker.SelectedTime.HasValue ? TimePicker.SelectedTime.Value.Hour : 0,
                        ExtraMinutes = TimePicker.SelectedTime.HasValue ? TimePicker.SelectedTime.Value.Minute : 0,
                        Hours = GetHoursForTimeType(TimeType),
                        Minutes = GetMinutesForTimeType(TimeType)
                    };
            }
            else
            {
                return new TimeComponent
                {
                    TimeType = TimeType,
                    BeforeAfter = BeforeAfter.None,
                    ExtraHours = 0,
                    ExtraMinutes = 0,
                    Hours = TimePicker.SelectedTime.Value.Hour,
                    Minutes = TimePicker.SelectedTime.Value.Minute
                };
            }
        }

        public bool IsTimeSelected()
        {
            return TimePicker.SelectedTime.HasValue;
        }

        private static int GetMinutesForTimeType(TimeType type)
        {
            if (type == TimeType.Sunrise && UserSettings.Settings.SunriseMinute.HasValue)
            {
                return UserSettings.Settings.SunriseMinute.Value;
            }

            if (UserSettings.Settings.SunsetMinute.HasValue)
            {
                return UserSettings.Settings.SunsetMinute.Value;
            }

            throw new InvalidTriggerTimeException();
        }

        private static int GetHoursForTimeType(TimeType type)
        {
            if (type == TimeType.Sunrise && UserSettings.Settings.SunriseHour.HasValue)
            {
                return UserSettings.Settings.SunriseHour.Value;
            }

            if (UserSettings.Settings.SunsetHour.HasValue)
            {
                return UserSettings.Settings.SunsetHour.Value;
            }

            throw new InvalidTriggerTimeException();
        }
    }
}
