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

namespace Winleafs.Wpf.Views.Popup
{
    using System.Diagnostics;
    using System.Windows.Navigation;

    /// <summary>
    /// Interaction logic for NewVersionPopup.xaml
    /// </summary>
    public partial class NewVersionPopup : Window
    {
        private const string RELEASE_URL = "https://github.com/StijnOostdam/Winleafs/releases";
        public NewVersionPopup()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(RELEASE_URL);
            e.Handled = true;
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }
    }
}
