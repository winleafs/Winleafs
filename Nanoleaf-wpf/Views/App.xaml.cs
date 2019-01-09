using Nanoleaf_Models.Models.Effects;
using Nanoleaf_wpf.Views.MainWindows;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;

namespace Nanoleaf_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();

            //TODO: start timer to repeatedly try to get connection with lights

            //Initialize available effects
            //TODO: replace by API call
            Effect.Effects = new List<Effect>();
            Effect.Effects.Add(new Effect { Name = "Flames" });
            Effect.Effects.Add(new Effect { Name = "Forest" });
            Effect.Effects.Add(new Effect { Name = "Nemo" });
            Effect.Effects.Add(new Effect { Name = "Snowfall" });
            Effect.Effects.Add(new Effect { Name = "Inner Peace" });
            Effect.Effects = Effect.Effects.OrderBy(eff => eff.Name).ToList();
        }
    }
}
