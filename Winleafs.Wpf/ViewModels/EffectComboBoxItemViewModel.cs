using System.Collections.Generic;
using System.Windows.Media;
using Winleafs.Models.Enums;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.ViewModels
{
    public class EffectComboBoxItemViewModel
    {
        public int Width { get; set; }

        public string EffectName { get; set; }

        public IEnumerable<Color> Colors { get; set; }

        public EffectType EffectType { get; set; }

        public string EffectTypeDisplay => EnumLocalizer.GetLocalizedEnum(EffectType);
    }
}
