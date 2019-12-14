using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using Winleafs.Wpf.Api;

namespace Winleafs.Wpf.Views.Effects
{
    /// <summary>
    /// Interaction logic for EffectComboBox.xaml
    /// </summary>
    public partial class EffectComboBox : UserControl
    {
        public ObservableCollection<EffectComboBoxItem> Effects { get; set; }

        public EffectComboBoxItem SelectedEffect { get; set; }

        public EffectComboBox()
        {
            InitializeComponent();

            DataContext = this;
        }

        public void InitializeEffects(Orchestrator orchestrator)
        {
            var effects = new List<EffectComboBoxItem>();

            foreach (var customEffect in orchestrator.GetCustomEffects())
            {
                effects.Add(new EffectComboBoxItem(customEffect, (int)Width));
            }

            var requestFailed = false;
            foreach (var effect in orchestrator.Device.Effects.ToList()) //ToList to make a copy
            {
                if (requestFailed)
                {
                    effects.Add(new EffectComboBoxItem(effect, (int)Width));
                }

                try
                {
                    //This constructor executes a request, if 1 fails, let all others revert to blank automatically
                    //We do this because we do not want to execute lots of requests and wait till failure for each request
                    effects.Add(new EffectComboBoxItem(orchestrator.Device, effect, (int)Width));
                }
                catch
                {
                    requestFailed = true;
                    effects.Add(new EffectComboBoxItem(effect, (int)Width));
                }
            }

            Effects = new ObservableCollection<EffectComboBoxItem>(effects);
        }
    }
}
