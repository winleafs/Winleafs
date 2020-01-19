using System.Collections.Generic;
using System.Windows.Media;
using Winleafs.Models.Enums;

namespace Winleafs.Wpf.ViewModels
{
    public class EffectComboBoxItemViewModel
    {
        public int Width { get; set; }

        public string EffectName { get; set; }

        public IEnumerable<Color> Colors { get; set; }

        public EffectType EffectType { get; set; }
    }
}
