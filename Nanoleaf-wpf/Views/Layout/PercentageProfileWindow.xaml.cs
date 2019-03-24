using System.Windows;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for PercentageProfileWindow.xaml
    /// </summary>
    public partial class PercentageProfileWindow : Window
    {
        public PercentageProfileWindow()
        {
            InitializeComponent();

            LayoutDisplay.SetWithAndHeight((int)LayoutDisplay.Width, (int)LayoutDisplay.Height);
            LayoutDisplay.DrawLayout();
            LayoutDisplay.EnableClick();
        }

        private void Plus_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
