using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for CurrentEffectUserControl.xaml
    /// </summary>
    public partial class CurrentEffectUserControl : UserControl
    {
        private Timer _timer;

        public CurrentEffectUserControl()
        {
            InitializeComponent();

            DataContext = this;

            UpdateLabels();

            _timer = new Timer(5000);
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        public void UpdateLabels()
        {
            var device = UserSettings.Settings.ActviceDevice;

            var effect = device.GetActiveEffect();
            var brightness = device.GetActiveBrightness().ToString();

            Dispatcher.Invoke(new Action(() =>
            {
                CurrentEffect.Content = effect;
                CurrentBrightness.Content = brightness;
            }));
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UpdateLabels();
        }

    }
}
