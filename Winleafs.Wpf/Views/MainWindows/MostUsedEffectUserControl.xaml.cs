using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for MostUsedEffectUserControl.xaml
    /// </summary>
    public partial class MostUsedEffectUserControl : UserControl
    {
        private TaskbarIcon _parent;
        private string _effectName;
        private static readonly SolidColorBrush HoverBackgroundColor = Brushes.DarkGray;
        private SolidColorBrush _backgroundColor;

        public MostUsedEffectUserControl(TaskbarIcon parent, string effectName, bool selected)
        {
            InitializeComponent();

            _parent = parent;
            _effectName = effectName;

            NameLabel.Text = _effectName;

            if (selected)
            {
                _backgroundColor = (SolidColorBrush)Application.Current.FindResource("NanoleafGreen");
            }
            else
            {
                _backgroundColor = (SolidColorBrush)Application.Current.FindResource("NanoleafBlack");
            }

            Background = _backgroundColor;
        }

        private void UserControl_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _parent.EffectSelected(_effectName);
        }

        private void UserControl_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Set the background color for hover
            Background = HoverBackgroundColor;
        }

        private void UserControl_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Reset the background color when hovering stops
            Background = _backgroundColor;
        }
    }
}
