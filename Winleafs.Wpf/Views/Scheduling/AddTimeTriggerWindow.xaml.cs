using System.Windows;
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

            var addSucceeded = _parent.TriggerAdded(new ScheduleTrigger
            {
                TimeComponent = TimeComponent.AsTimeComponent(),
                Brightness = _brightness,
                EffectName = EffectComboBox.SelectedEffect.EffectName,
            });

            if (!addSucceeded)
            {
                PopupCreator.Error(Scheduling.Resources.TriggerOverlaps);
                return;
            }

            Close();
        }

        public void EffectComboBoxSelectionChanged(string selectedEffect)
        {
            //We do not need to do anything when the selection changed
        }
    }
}
