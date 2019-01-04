using Nanoleaf_wpf.ViewModels;
using System;
using System.Windows;

namespace Nanoleaf_wpf.Views.Setup
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        private SetupViewModel setupViewModel;

        public SetupWindow()
        {
            InitializeComponent();

            setupViewModel = new SetupViewModel();

            setupViewModel.LoadDummy(); //TODO: replace by real method call
        }

        private void DiscoverDevice_Loaded(object sender, RoutedEventArgs e)
        {
            DiscoverDevice.ParentWindow = this;
            DiscoverDevice.DataContext = setupViewModel;
        }

        public void Pair_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Refresh_Click(object sender, RoutedEventArgs e)
        {

        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void Continue_Click(object sender, RoutedEventArgs e)
        {
            //TODO: add check if a device is selected
            AuthorizeDevice.ParentWindow = this;
            AuthorizeDevice.Visibility = Visibility.Visible;
            DiscoverDevice.Visibility = Visibility.Hidden;
        }

        private void AuthorizeDevice_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
