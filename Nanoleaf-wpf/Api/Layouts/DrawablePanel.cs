using System.Windows;
using System.Windows.Shapes;

namespace Winleafs.Wpf.Api.Layouts
{
    public class DrawablePanel
    {
        public int PanelId { get; set; }

        public Point MidPoint { get; set; }

        public Polygon Polygon { get; set; }
    }
}
