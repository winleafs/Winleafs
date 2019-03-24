using System.Windows.Controls;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for StepItemUserControl.xaml
    /// </summary>
    public partial class StepItemUserControl : UserControl
    {
        public string Name { get; set; }

        private PercentageProfileWindow _parent;

        public StepItemUserControl(PercentageProfileWindow parent, int stepNumber)
        {
            _parent = parent;

            Name = $"{Layout.Resources.Step} {stepNumber.ToString()}";

            InitializeComponent();
            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            
        }
    }
}
