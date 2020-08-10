using System;
using System.Collections.Generic;
using System.Drawing;
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
        private readonly Rectangle _screenBounds;
        private readonly Device _device;
        private readonly ScreenMirrorAlgorithm _screenMirrorAlgorithm;
        private readonly ScreenMirrorFlip _screenMirrorFlip;

        public ScreenMirrorVisualizationWindow(Device device, int monitorIndex, ScreenMirrorAlgorithm screenMirrorAlgorithm, ScreenMirrorFlip screenMirrorFlip)
        {
            _screenMirrorAlgorithm = screenMirrorAlgorithm;
            _screenMirrorFlip = screenMirrorFlip;
            _device = device;
            _screenBounds = ScreenBoundsHelper.GetScreenBounds(monitorIndex);

            Width = _screenBounds.Width;
            Height = _screenBounds.Height;

            Left = _screenBounds.X;
            Top = _screenBounds.Y;

            InitializeComponent();
        }

        public void Visualize(double scale)
        {
            var width = Convert.ToInt32(_screenBounds.Width / scale);
            var height = Convert.ToInt32(_screenBounds.Height / scale);
            var scaleType = _screenMirrorAlgorithm == ScreenMirrorAlgorithm.ScreenMirrorFit ? ScaleType.Fit : ScaleType.Stretch;
            var flipType = FlipTypeHelper.ScreenMirrorFlipToFlipType(_screenMirrorFlip);

            var panels = OrchestratorCollection.GetOrchestratorForDevice(_device).PanelLayout.GetScaledPolygons(width, height, scaleType, flipType);

            DrawPanels(panels);

            //Horizontally center the close label
            CloseInfoLabel.Margin = new Thickness(width / 2, 0, 0, 0);
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
                panel.Polygon.Fill = System.Windows.Media.Brushes.LightSlateGray;
                CanvasArea.Children.Add(panel.Polygon);
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            Close();
        }
    }
}
