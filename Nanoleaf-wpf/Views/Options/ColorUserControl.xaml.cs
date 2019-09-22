using System.Windows.Controls;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Winleafs.Wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for ColorUserControl.xaml
    /// </summary>
    public partial class ColorUserControl : UserControl
    {
        public string Description { get; set; }

        private OptionsWindow _parent;

        public ColorUserControl(OptionsWindow parent, string description, Color color)
        {
            _parent = parent;

            Description = description;

            InitializeComponent();

            // Fill the color indicator with the color of the effect.
            ColorIndicator.Fill = new SolidColorBrush(System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B));
            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteColor(Description, this);
        }
    }
}
