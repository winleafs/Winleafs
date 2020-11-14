using System.Windows;
using Winleafs.Models.Enums;
using Winleafs.Models.Exceptions;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Views.Effects;
using Winleafs.Wpf.Views.Popup;

namespace Winleafs.Wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for AddTriggerWindow.xaml
    /// </summary>
    public partial class AddTimeTriggerWindow : Window, IEffectComboBoxContainer
    {
        private DayUserControl _parent;
        private int _brightness { get; set; }

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public AddTimeTriggerWindow(DayUserControl parent)
        {
            _parent = parent;

            DataContext = this;

            InitializeComponent();

            EffectComboBox.InitializeEffects();
            EffectComboBox.ParentUserControl = this;
            EffectComboBox.UpdateSelection(EffectComboBox.Effects[0].EffectName); //Select first effect by default
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (EffectComboBox.SelectedEffect == null)
            {
                PopupCreator.Error(Scheduling.Resources.MustChooseEffect);
                return;
            }

            if (!TimeComponent.TimePicker.SelectedTime.HasValue)
            {
                PopupCreator.Error(Scheduling.Resources.InvalidTimeValue);
                return;
            }

            bool addSucceeded;
            var timeType = TimeComponent.TimeType;

            if (timeType == TimeType.Sunrise || timeType == TimeType.Sunset)
            {
                addSucceeded = _parent.TriggerAdded(new ScheduleTrigger
                {
                    TimeComponent = new TimeComponent
                    {
                        TimeType = timeType,
                        BeforeAfter = TimeComponent.BeforeAfter,
                        ExtraHours = TimeComponent.TimePicker.SelectedTime.HasValue ? TimeComponent.TimePicker.SelectedTime.Value.Hour : 0,
                        ExtraMinutes = TimeComponent.TimePicker.SelectedTime.HasValue ? TimeComponent.TimePicker.SelectedTime.Value.Minute : 0,
                        Hours = GetHoursForTimeType(timeType),
                        Minutes = GetMinutesForTimeType(timeType)
                    },
                    Brightness = _brightness,
                    EffectName = EffectComboBox.SelectedEffect.EffectName,
                });
            }
            else
            {
                addSucceeded = _parent.TriggerAdded(new ScheduleTrigger
                {
                    TimeComponent = new TimeComponent
                    {
                        TimeType = timeType,
                        BeforeAfter = BeforeAfter.None,
                        ExtraHours = 0,
                        ExtraMinutes = 0,
                        Hours = TimeComponent.TimePicker.SelectedTime.Value.Hour,
                        Minutes = TimeComponent.TimePicker.SelectedTime.Value.Minute
                    },
                    Brightness = _brightness,
                    EffectName = EffectComboBox.SelectedEffect.EffectName,
                });
            }

            if (!addSucceeded)
            {
                PopupCreator.Error(Scheduling.Resources.TriggerOverlaps);
                return;
            }

            Close();
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

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            //We do not need to do anything when the selection changed
        }
    }
}
