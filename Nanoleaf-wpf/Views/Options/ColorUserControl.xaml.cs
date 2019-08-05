using System.Windows.Controls;

namespace Winleafs.Wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for ColorUserControl.xaml
    /// </summary>
    public partial class ColorUserControl : UserControl
    {
        public string Description { get; set; }

        private OptionsWindow _parent;

        public ColorUserControl(OptionsWindow parent, string description)
        {
            _parent = parent;

            Description = description;

            InitializeComponent();
            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //_parent.DeleteColor(Description);
        }
    }
}
