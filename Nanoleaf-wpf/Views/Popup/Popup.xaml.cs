
using System.Windows;
namespace Winleafs.Wpf.Views.Popup
{
    /// <summary>
    /// Interaction logic for Popup.xaml
    /// </summary>
    public partial class Popup : Window
    {
        public Popup(string content, string title)
        {
            this.InitializeComponent();
            this.Title = title;
            this.PopupContent.Text = content;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
