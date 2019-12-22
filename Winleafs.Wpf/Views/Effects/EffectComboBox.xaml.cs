using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
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

        /// <summary>
        /// Initialize effects based on all devices.
        /// Only effects that are shared between all devices will be
        /// part of the dropdown.
        /// </summary>
        public void InitializeEffects()
        {
            var orchestrators = new List<Orchestrator>();
            var customEffects = new List<ICustomEffect>();
            var effects = new List<Effect>();

            foreach (var device in UserSettings.Settings.Devices)
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(device);

                customEffects.AddRange(orchestrator.GetCustomEffects());
                //ToList to make a copy such that we do not get CollectionModifiedException (caused by calling UpdateEffect)
                effects.AddRange(orchestrator.Device.Effects.ToList());
                orchestrators.Add(orchestrator);
            }

            BuildEffects(customEffects.Distinct(new CustomEffectEqualityComparer()), effects.Distinct(), orchestrators);
        }

        /// <summary>
        /// Initialize effects for a sepcific device
        /// </summary>
        public void InitializeEffects(Orchestrator orchestrator)
        {
            //ToList to make a copy such that we do not get CollectionModifiedException (caused by calling UpdateEffect)
            BuildEffects(orchestrator.GetCustomEffects(), orchestrator.Device.Effects.ToList(), new List<Orchestrator> { orchestrator });
        }

        /// <summary>
        /// Builds the effects list from the given effects.
        /// Also updates retrieved effects for all given devices passed
        /// in <paramref name="orchestrators"/>.
        /// </summary>
        private void BuildEffects(IEnumerable<ICustomEffect> customEffects, IEnumerable<Effect> deviceEffects, IEnumerable<Orchestrator> orchestrators)
        {
            var effects = new List<EffectComboBoxItemViewModel>();

            //Take any client. Since the dropdown will only display effects shared accross all devices, it does not matter which lights we use to query the effects
            var nanoleafClient = NanoleafClient.GetClientForDevice(orchestrators.First().Device);

            foreach (var customEffect in customEffects)
            {
                effects.Add(new EffectComboBoxItemViewModel()
                {
                    EffectName = customEffect.GetName(),
                    Width = (int)Width,
                    Colors = customEffect.GetColors().Select(color => Color.FromArgb(color.A, color.R, color.G, color.B))
                });
            }

            var requestFailed = false;
            foreach (var effect in deviceEffects)
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
                        //Do this for each device in the given orchestrators
                        foreach (var orchestrator in orchestrators)
                        {
                            orchestrator.Device.UpdateEffect(effectWithPalette);
                        }                        
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
