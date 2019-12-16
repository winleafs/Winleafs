using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Winleafs.Models.Enums;
using Winleafs.Models.Exceptions;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Scheduling.Triggers;
using Winleafs.Wpf.Helpers;
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
        private TriggerType _triggerType { get; set; }
        private int _brightness { get; set; }
        private Dictionary<string, TriggerType> _triggerTypeMapping { get; set; } //Map display values to enum values

        public int Brightness
        {
            get { return _brightness; }
            set
            {
                _brightness = value;
                BrightnessLabel.Content = value.ToString();
            }
        }

        public string SelectedTriggerType
        {
            get { return EnumLocalizer.GetLocalizedEnum(_triggerType); }
            set
            {
                _triggerType = _triggerTypeMapping[value];

                TriggerTypeChanged();
            }
        }

        public IEnumerable<string> TriggerTypes
        {
            get
            {
                return _triggerTypeMapping.Keys;
            }
        }

        public AddTimeTriggerWindow(DayUserControl parent)
        {
            _parent = parent;

            _triggerTypeMapping = new Dictionary<string, TriggerType>()
            {
                {  EnumLocalizer.GetLocalizedEnum(TriggerType.Time), TriggerType.Time },
                {  EnumLocalizer.GetLocalizedEnum(TriggerType.Sunrise), TriggerType.Sunrise },
                {  EnumLocalizer.GetLocalizedEnum(TriggerType.Sunset), TriggerType.Sunset }
            };

            DataContext = this;

            InitializeComponent();

            SelectedTriggerType = EnumLocalizer.GetLocalizedEnum(TriggerType.Time);

            EffectComboBox.InitializeEffects();
            EffectComboBox.ParentUserControl = this;
        }

        private void TriggerTypeChanged()
        {
            if (_triggerType == TriggerType.Time)
            {
                BeforeRadioButton.Visibility = Visibility.Hidden;
                AfterRadioButton.Visibility = Visibility.Hidden;
                TimeLabel.Content = Scheduling.Resources.Time;
            }
            else
            {
                BeforeRadioButton.Visibility = Visibility.Visible;
                AfterRadioButton.Visibility = Visibility.Visible;
                TimeLabel.Content = Scheduling.Resources.ExtraTime;
            }
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

            int hours;
            int minutes;
            try
            {
                hours = Convert.ToInt32(Hours.Text, CultureInfo.InvariantCulture);

                if (hours < 0 || hours > 23)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                PopupCreator.Error(Scheduling.Resources.InvalidHoursValue);
                return;
            }

            try
            {
                minutes = Convert.ToInt32(Minutes.Text, CultureInfo.InvariantCulture);

                if (minutes < 0 || minutes > 59)
                {
                    throw new ArgumentOutOfRangeException();
                }
            }
            catch
            {
                PopupCreator.Error(Scheduling.Resources.InvalidMinutesValue);
                return;
            }

            bool addSucceeded;

            if (_triggerType == TriggerType.Sunrise || _triggerType == TriggerType.Sunset)
            {
                var beforeAfter = GetBeforeAfter();

                addSucceeded = _parent.TriggerAdded(new TimeTrigger
                {
                    EventTriggerType = _triggerType,
                    BeforeAfter = beforeAfter,
                    Brightness = _brightness,
                    EffectName = EffectComboBox.SelectedEffect.EffectName,
                    ExtraHours = hours,
                    ExtraMinutes = minutes,
                    Hours = GetHoursForTriggerType(_triggerType),
                    Minutes = GetMinutesForTriggerType(_triggerType)
                });
            }
            else
            {
                addSucceeded = _parent.TriggerAdded(new TimeTrigger
                {
                    EventTriggerType = _triggerType,
                    BeforeAfter = BeforeAfter.None,
                    Brightness = _brightness,
                    EffectName = EffectComboBox.SelectedEffect.EffectName,
                    ExtraHours = 0,
                    ExtraMinutes = 0,
                    Hours = hours,
                    Minutes = minutes
                });
            }

            if (!addSucceeded)
            {
                PopupCreator.Error(Scheduling.Resources.TriggerOverlaps);
                return;
            }

            Close();
        }

        private static int GetMinutesForTriggerType(TriggerType type)
        {
            if (type == TriggerType.Sunrise && UserSettings.Settings.SunriseMinute.HasValue)
            {
                return UserSettings.Settings.SunriseMinute.Value;
            }

            if (UserSettings.Settings.SunsetMinute.HasValue)
            {
                return UserSettings.Settings.SunsetMinute.Value;
            }

            throw new InvalidTriggerTimeException();
        }

        private static int GetHoursForTriggerType(TriggerType type)
        {
            if (type == TriggerType.Sunrise && UserSettings.Settings.SunriseHour.HasValue)
            {
                return UserSettings.Settings.SunriseHour.Value;
            }

            if (UserSettings.Settings.SunsetHour.HasValue)
            {
                return UserSettings.Settings.SunsetHour.Value;
            }

            throw new InvalidTriggerTimeException();
        }

        private BeforeAfter GetBeforeAfter()
        {
            if (BeforeRadioButton.IsChecked != null && BeforeRadioButton.IsChecked.Value)
            {
                return BeforeAfter.After;
            }

            return AfterRadioButton.IsChecked != null
                   && AfterRadioButton.IsChecked.Value
                       ? BeforeAfter.After
                       : BeforeAfter.None;
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9][0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            //We do not need to do anything when the selection changed
        }
    }
}
