using System.Collections.Generic;
using System.Windows.Media;

namespace Winleafs.Wpf.ViewModels
{
    public class EffectComboBoxItemViewModel
    {
        public int Width { get; set; }

        public string EffectName { get; set; }

        public IEnumerable<Color> Colors { get; set; }
    }
}
