using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Models.Models.Effects;
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Effects
{
    /// <summary>
    /// Interaction logic for EffectComboBoxItem.xaml
    /// </summary>
    public partial class EffectComboBoxItem : UserControl
    {
        private readonly int _width;

        public EffectComboBoxItem(string effectName, int width)
        {
            InitializeComponent();

            EffectLabel.Content = effectName;
            _width = width;
        }

        public EffectComboBoxItem(Device device, Effect effect, int width) : this(effect.Name, width)
        {
            if (effect.Palette == null || !effect.Palette.Any())
            {
                var nanoleafClient = NanoleafClient.GetClientForDevice(device);

                effect = nanoleafClient.EffectsEndpoint.GetEffectDetails(effect.Name);

                //Replace the retrieved effect with the current effect such that we do not have to make this call in the future
                device.UpdateEffect(effect);
            }

            DrawColoredBorder(effect.Palette.Select(palette => HsbToRgbConverter.ConvertToMediaColor(palette.Hue, palette.Saturation, palette.Brightness)));

            EffectLabel.Content = effect.Name;
        }

        public EffectComboBoxItem(ICustomEffect effect, int width) : this(effect.GetName(), width)
        {
            //Convert Drawing.Color to Media.Color
            DrawColoredBorder(effect.GetColors().Select(color => Color.FromArgb(color.A, color.R, color.G, color.B)));

            EffectLabel.Content = effect.GetName();
        }

        public EffectComboBoxItem(Effect effect, int width) : this(effect.Name, width)
        {
            DrawColoredBorder(new List<Color> { Color.FromArgb(ICustomEffect.DefaultColor.A, ICustomEffect.DefaultColor.R, ICustomEffect.DefaultColor.G, ICustomEffect.DefaultColor.B) });

        }

        private void DrawColoredBorder(IEnumerable<Color> colors)
        {
            var borderParts = new int[colors.Count()];

            //Divide the border into equal sized parts
            for (var i = 0; i < colors.Count(); i++)
            {
                borderParts[i] = _width / colors.Count();
            }

            //Divide up the remainder
            for (var i = 0; i < _width % colors.Count(); i++)
            {
                borderParts[i] += 1;
            }

            //Create the borders
            var marginLeft = 0;
            var marginRight = _width;

            for (var i = 0; i < colors.Count(); i++)
            {
                ContentGrid.Children.Add(new Border
                {
                    BorderBrush = new SolidColorBrush(colors.ElementAt(i)),
                    BorderThickness = new Thickness(0, 2, 0, 0),
                    Margin = new Thickness(marginLeft, 0, marginRight - borderParts[i], 0)
                });

                marginLeft += borderParts[i];
                marginRight -= borderParts[i];
            }

            //TODO; when adding these items, try catch and if 1 request fails then automatically break
        }
    }
}
