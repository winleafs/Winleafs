using Nanoleaf_Models.Enums;
using Nanoleaf_Models.Models;
using Nanoleaf_Models.Models.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Nanoleaf_wpf.Views.Scheduling
{
    /// <summary>
    /// Interaction logic for AddTriggerWindow.xaml
    /// </summary>
    public partial class AddTimeTriggerWindow : Window
    {
        private DayUserControl _parent;

        private TriggerType _triggerType { get; set; }

        public string SelectedEffect { get; set; }

        public TriggerType TriggerType
        {
            get { return _triggerType; }
            set
            {
                _triggerType = value;

                TriggerTypeChanged();
            }
        }

        public IEnumerable<TriggerType> TriggerTypes
        {
            get
            {
                return Enum.GetValues(typeof(TriggerType)).Cast<TriggerType>();
            }
        }

        public List<Effect> Effects { get; set; }

        public AddTimeTriggerWindow(DayUserControl parent)
        {
            _parent = parent;
            Effects = UserSettings.Settings.ActviceDevice.Effects;

            DataContext = this;

            InitializeComponent();

            TriggerType = TriggerType.Time;
        }

        private void TriggerTypeChanged()
        {
            if (_triggerType == TriggerType.Time)
            {
                TimeGrid.Visibility = Visibility.Visible;
            }
            else
            {
                TimeGrid.Visibility = Visibility.Hidden;
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            //TODO: add checks
            _parent.TriggerAdded(_triggerType, Convert.ToInt32(Hours.Text), Convert.ToInt32(Minutes.Text), SelectedEffect);
            Close();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[0-9][0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
