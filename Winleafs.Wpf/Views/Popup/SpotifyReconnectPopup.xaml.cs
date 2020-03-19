using NLog;
using System;
using System.Windows;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Popup
{
    /// <summary>
    /// Interaction logic for SpotifyReconnectPopup.xaml
    /// </summary>
    public partial class SpotifyReconnectPopup : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public SpotifyReconnectPopup()
        {
            InitializeComponent();
        }

        private void Reconnect_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;

            try
            {
                Spotify.Connect(ReconnectSuccess);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Unknown error when trying to connect to Spotify");
                PopupCreator.Error(PopupResource.SpotifyUnknownError);
            }

            this.Close();
        }

        private void ReconnectSuccess()
        {
            //Run code on main thread since we update the UI
            Dispatcher.Invoke(new Action(() =>
            {
                if (Spotify.WebAPIIsConnected())
                {
                    PopupCreator.Success(PopupResource.SpotifySuccessfullyConnected, true);
                }
                else
                {
                    PopupCreator.Error(PopupResource.SpotifyConnectionFailed);
                }
            }));
        }

        private void Disconnect_Click(object sender, RoutedEventArgs e)
        {
            Spotify.Disconnect();
            this.Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            this.Close();
        }
    }
}
