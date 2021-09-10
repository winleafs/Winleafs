using System.Windows.Controls;
using models = Winleafs.Models.Models.Layouts;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for StepItemUserControl.xaml
    /// </summary>
    public partial class FrameUserControl : UserControl
    {
        public string Description { get; set; }

        private CreateEffectWindow _parent;
        private models.Frame _frame;

        public FrameUserControl(CreateEffectWindow parent, int frameNumber, models.Frame frame)
        {
            _parent = parent;
            _frame = frame;

            Description = $"{Layout.Resources.Frame} {frameNumber}";

            InitializeComponent();
            DataContext = this;
        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _parent.DeleteFrame(_frame);
        }

		private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//_parent.HighlightPanels(_frame.PanelIds);
		}

		private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			//_parent.UnhighlightPanels(_frame.PanelIds);
		}
	}
}
