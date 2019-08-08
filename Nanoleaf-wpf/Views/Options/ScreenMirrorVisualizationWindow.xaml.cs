using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Winleafs.Models.Enums;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.Options
{
    /// <summary>
    /// Interaction logic for ScreenMirrorVisualizationWindow.xaml
    /// </summary>
    public partial class ScreenMirrorVisualizationWindow : Window
    {
        private static readonly double _defaultDpi = 96; //A dpi of 96 means you have your windows settings to 100%, 144 = 150%, etc

        public ScreenMirrorVisualizationWindow(Device device, int monitorIndex, ScreenMirrorAlgorithm screenMirrorAlgorithm)
        {
            var screenBounds = MonitorHelper.GetScreenBounds(monitorIndex);

            //Since some users might have scaling enabled, we need to shrink the actual window to fit the monitor according to the DPI scaling setting
            uint dpiX, dpiY;
            MonitorHelper.GetDpi(screenBounds.X, screenBounds.Y, DpiType.Effective, out dpiX, out dpiY);
            var scaleX = Convert.ToDouble(dpiX) / _defaultDpi;
            var scaleY = Convert.ToDouble(dpiY) / _defaultDpi;

            var scaleType = screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit ? ScaleType.Fit : ScaleType.Stretch;

            //var panels = OrchestratorCollection.GetOrchestratorForDevice(device).PanelLayout.GetScaledTriangles(Convert.ToInt32(screenBounds.Width / scaleX), Convert.ToInt32(screenBounds.Height / scaleY), scaleType);
            var panels = OrchestratorCollection.GetOrchestratorForDevice(device).PanelLayout.GetScaledTriangles(screenBounds.Width, screenBounds.Height, scaleType);

            Width = screenBounds.Width;
            Height = screenBounds.Height;

            Left = screenBounds.X;
            Top = screenBounds.Y;

            InitializeComponent();

            DrawPanels(panels);
        }

        private void DrawPanels(List<DrawablePanel> panels)
        {
            CanvasArea.Children.Clear();

            if (panels == null || !panels.Any())
            {
                return;
            }

            foreach (var panel in panels)
            {
                panel.Polygon.Fill = Brushes.LightSlateGray;
                CanvasArea.Children.Add(panel.Polygon);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}
