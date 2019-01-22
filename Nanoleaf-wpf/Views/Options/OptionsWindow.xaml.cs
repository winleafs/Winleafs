using Nanoleaf_Models.Models;
using Nanoleaf_wpf.ViewModels;
using System.Windows;

namespace Nanoleaf_wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        public OptionsViewModel OptionsViewModel { get; set; }

        public OptionsWindow()
        {
            InitializeComponent();

            OptionsViewModel = new OptionsViewModel
            {
                StartAtWindowsStartUp = UserSettings.Settings.StartAtWindowsStartup,
                Latitude = UserSettings.Settings.Latitude?.ToString("N7"),
                Longitude = UserSettings.Settings.Longitude?.ToString("N7")
            };

            DataContext = OptionsViewModel;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
