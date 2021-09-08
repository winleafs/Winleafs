using System.Windows.Controls;
using Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for StepItemUserControl.xaml
    /// </summary>
    public partial class FrameUserControl : UserControl
    {
        public string Description { get; set; }

        private CreateEffectWindow _parent;
        private PercentageStep _percentageStep;

        public FrameUserControl(CreateEffectWindow parent, int stepNumber, PercentageStep percentageStep)
        {
            _parent = parent;
            _percentageStep = percentageStep;

            Description = $"{Layout.Resources.Frame} {stepNumber}";

            InitializeComponent();
            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteStep(_percentageStep);
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _parent.HighlightPanels(_percentageStep.PanelIds);
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _parent.UnhighlightPanels(_percentageStep.PanelIds);
        }
    }
}
