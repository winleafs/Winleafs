using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using Winleafs.Api;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.ViewModels;

namespace Winleafs.Wpf.Views.Effects
{
    /// <summary>
    /// Interaction logic for EffectComboBox.xaml
    /// </summary>
    public partial class EffectComboBox : UserControl, INotifyPropertyChanged
    {
        private static readonly List<Color> _defaultColors = new List<Color> { Color.FromArgb(ICustomEffect.DefaultColor.A, ICustomEffect.DefaultColor.R, ICustomEffect.DefaultColor.G, ICustomEffect.DefaultColor.B) };

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<EffectComboBoxItemViewModel> Effects { get; set; }

        public EffectComboBoxItemViewModel SelectedEffect { get; set; }

        public IEffectComboBoxContainer ParentUserControl { get; set; }

        public EffectComboBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void InitializeEffects(Orchestrator orchestrator)
        {
            var effects = new List<EffectComboBoxItemViewModel>();

            var nanoleafClient = NanoleafClient.GetClientForDevice(orchestrator.Device);

            foreach (var customEffect in orchestrator.GetCustomEffects())
            {
                effects.Add(new EffectComboBoxItemViewModel()
                {
                    EffectName = customEffect.GetName(),
                    Width = (int)Width,
                    Colors = customEffect.GetColors().Select(color => Color.FromArgb(color.A, color.R, color.G, color.B))
                });
            }

            var requestFailed = false;
            foreach (var effect in orchestrator.Device.Effects.ToList()) //ToList to make a copy
            {
                if (requestFailed)
                {
                    effects.Add(new EffectComboBoxItemViewModel()
                    {
                        EffectName = effect.Name,
                        Width = (int)Width,
                        Colors = _defaultColors
                    });
                }

                try
                {
                    //This executes a request, if 1 fails, let all others revert to blank automatically
                    //We do this because we do not want to execute lots of requests and wait till failure for each request
                    var effectWithPalette = effect;

                    if (effect.Palette == null || !effect.Palette.Any())
                    {
                        effectWithPalette = nanoleafClient.EffectsEndpoint.GetEffectDetails(effect.Name);

                        //Replace the retrieved effect with the current effect such that we do not have to make this call in the future
                        orchestrator.Device.UpdateEffect(effectWithPalette);
                    }

                    effects.Add(new EffectComboBoxItemViewModel()
                    {
                        EffectName = effectWithPalette.Name,
                        Width = (int)Width,
                        Colors = effectWithPalette.Palette.Select(palette => HsbToRgbConverter.ConvertToMediaColor(palette.Hue, palette.Saturation, palette.Brightness))
                    });
                }
                catch
                {
                    requestFailed = true;
                    effects.Add(new EffectComboBoxItemViewModel()
                    {
                        EffectName = effect.Name,
                        Width = (int)Width,
                        Colors = _defaultColors
                    });
                }
            }

            Effects = new ObservableCollection<EffectComboBoxItemViewModel>(effects);
        }

        public void UpdateSelection(string effectName)
        {
            if (effectName != null && (SelectedEffect == null || SelectedEffect.EffectName != effectName))
            {
                SelectedEffect = Effects.FirstOrDefault(effect => effect.EffectName == effectName);
                OnPropertyChanged(nameof(SelectedEffect));
            }
            else if (effectName == null)
            {
                SelectedEffect = null;
                OnPropertyChanged(nameof(SelectedEffect));
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboBox.IsDropDownOpen)
            {
                //Only trigger change when the user chosen an effect (when the dropdown is open)
                ParentUserControl.EffectComboBoxSelectionChanged(SelectedEffect.EffectName);
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
