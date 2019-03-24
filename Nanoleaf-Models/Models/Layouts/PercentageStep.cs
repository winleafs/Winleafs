using System.Collections.Generic;

namespace Winleafs.Models.Models.Layouts
{
    public class PercentageStep
    {
        public HashSet<int> PanelIds { get; set; }

        public PercentageStep()
        {
            PanelIds = new HashSet<int>();
        }
    }
}
