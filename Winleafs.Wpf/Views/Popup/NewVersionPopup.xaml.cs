using System.Windows;
using Winleafs.Wpf.Views.MainWindows;

namespace Winleafs.Wpf.Views.Popup
{
    /// <summary>
    /// Interaction logic for NewVersionPopup.xaml
    /// </summary>
    public partial class NewVersionPopup : Window
    {
        private const string RELEASE_URL = "https://github.com/winleafs/Winleafs/releases";
        public NewVersionPopup()
        {
            InitializeComponent();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.OpenURL(RELEASE_URL);
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
