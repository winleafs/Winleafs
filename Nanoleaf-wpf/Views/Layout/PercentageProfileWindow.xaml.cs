using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
    }
}
