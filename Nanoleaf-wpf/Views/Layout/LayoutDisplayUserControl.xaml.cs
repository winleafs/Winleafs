using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Api;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;
using Winleafs.Wpf.Helpers.Voronoi;

namespace Winleafs.Wpf.Views.Layout
{
    /// <summary>
    /// Interaction logic for LayoutDisplay.xaml
    /// </summary>
    public partial class LayoutDisplayUserControl : UserControl
    {
        private static readonly SolidColorBrush _lockedBorderColor = Brushes.Red;
        private static readonly SolidColorBrush _highLightColor = Brushes.OrangeRed;
        private static readonly SolidColorBrush _selectedBorderColor = Brushes.LightSteelBlue;
        private static readonly SolidColorBrush _borderColor = (SolidColorBrush)Application.Current.FindResource("NanoleafBlack");

        public HashSet<int> SelectedPanelIds { get; set; }

        private int _height;
        private int _width;

        private static readonly Random _random = new Random();

        private List<DrawablePanel> _triangles;
        private bool _panelsClickable;
        private HashSet<int> _lockedPanelIds;
        private Dictionary<int, Brush> _highlightOriginalColors; //This dictionary saves the original colors of the triangles when highlighting

        //Timer to update the colors periodically to update with schedule
        private Timer _colorTimer;

        public LayoutDisplayUserControl()
        {
            InitializeComponent();

            CanvasArea.MouseDown += CanvasClicked;
            SelectedPanelIds = new HashSet<int>();
            _lockedPanelIds = new HashSet<int>();
            _highlightOriginalColors = new Dictionary<int, Brush>();

            _panelsClickable = false;

            _colorTimer = new Timer(30000); //Update the colors every 30 seconds
            _colorTimer.Elapsed += OnTimedEvent;
            _colorTimer.AutoReset = true;
            _colorTimer.Enabled = true;
            _colorTimer.Start();    
        }

        public void DisableColorTimer()
        {
            _colorTimer.Stop();
        }

        public void SetWithAndHeight(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void EnableClick()
        {
            _panelsClickable = true;
        }

        public void DrawLayout()
        {
            CanvasArea.Children.Clear();

            var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);
            _triangles = orchestrator.PanelLayout.GetScaledTriangles(_width, _height);

            /*if (_triangles == null || !_triangles.Any())
            {
                return;
            }

            foreach (var triangle in _triangles)
            {
                triangle.Polygon.MouseDown += TriangleClicked;
            }

            //Draw the triangles
            foreach (var triangle in _triangles)
            {
                CanvasArea.Children.Add(triangle.Polygon);
            }

            UpdateColors();*/

            var edges = VoronoiHelper.ConstructScreenshotAreas(_triangles, _width, _height);

            foreach (var edge in edges)
            {
                var line = new Line()
                {
                    X1 = edge.Start.X,
                    Y1 = edge.Start.Y,
                    X2 = edge.End.X,
                    Y2 = edge.End.Y,
                    Stroke = Brushes.LightSteelBlue,
                    StrokeThickness = 2
                };
                CanvasArea.Children.Add(line);
            }
        }

        private void Redraw_Click(object sender, RoutedEventArgs e)
        {
            DrawLayout();
        }

        public void UpdateColors()
        {
            if (UserSettings.Settings.ActiveDevice == null || _triangles == null)
            {
                return;
            }

            var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActiveDevice);

            //Get colors of current effect
            var effect = client.EffectsEndpoint.GetEffectDetails(OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice).GetActiveEffectName());

            Dispatcher.Invoke(new Action(() =>
            {
                if (effect == null)
                {
                    foreach (var triangle in _triangles)
                    {
                        triangle.Polygon.Fill = Brushes.LightSlateGray;
                    }
                }
                else
                {
                    var colors = effect.Palette.Select(hsb =>
                        new SolidColorBrush(
                            HsbToRgbConverter.ConvertToMediaColor(hsb.Hue, hsb.Saturation, hsb.Brightness)
                            )).ToList();

                    foreach (var triangle in _triangles)
                    {
                        triangle.Polygon.Fill = colors[_random.Next(colors.Count)];
                    }
                }
            }));
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UpdateColors();
        }

        private void TriangleClicked(object sender, MouseButtonEventArgs e)
        {
            if (_triangles == null)
            {
                return;
            }

            if (!_panelsClickable)
            {
                return;
            }

            var triangle = (Polygon)sender;
            var selectedPanel = _triangles.FirstOrDefault(t => t.Polygon == triangle);

            if (selectedPanel == null)
            {
                return;
            }

            var selectedPanelId = selectedPanel.PanelId;

            if (_lockedPanelIds.Contains(selectedPanelId))
            {
                return;
            }

            triangle.Stroke = _selectedBorderColor;
            triangle.StrokeThickness = 2;

            SelectedPanelIds.Add(selectedPanelId);
        }

        private void CanvasClicked(object sender, MouseButtonEventArgs e)
        {
            //if the user clicks anywhere other then on the panels, reset the current selection
            if (e.OriginalSource == CanvasArea)
            {
                if (SelectedPanelIds.Count > 0)
                {
                    ClearSelectedPanels();
                }
            }
        }

        public void ClearSelectedPanels()
        {
            SelectedPanelIds.Clear();

            foreach (var triangle in _triangles)
            {
                if (!_lockedPanelIds.Contains(triangle.PanelId))
                {
                    triangle.Polygon.Stroke = _borderColor;
                    triangle.Polygon.StrokeThickness = 2;
                }
            }
        }

        public void LockPanels(HashSet<int> panelIds)
        {
            foreach (var panelId in panelIds)
            {
                var polygon = _triangles.FirstOrDefault(t => t.PanelId == panelId).Polygon;

                polygon.Stroke = _lockedBorderColor;
                polygon.StrokeThickness = 2;

                _lockedPanelIds.Add(panelId);
            }
        }

        public void UnlockPanels(HashSet<int> panelIds)
        {
            foreach (var panelId in panelIds)
            {
                var polygon = _triangles.FirstOrDefault(t => t.PanelId == panelId).Polygon;

                polygon.Stroke = _borderColor;
                polygon.StrokeThickness = 2;

                _lockedPanelIds.Remove(panelId);
            }
        }

        public void HighlightPanels(HashSet<int> panelIds)
        {
            foreach (var panelId in panelIds)
            {
                var polygon = _triangles.FirstOrDefault(t => t.PanelId == panelId).Polygon;

                _highlightOriginalColors.Add(panelId, polygon.Fill);

                polygon.Fill = _highLightColor;
            }
        }

        public void UnhighlightPanels(HashSet<int> panelIds)
        {
            foreach (var panelId in panelIds)
            {
                var polygon = _triangles.FirstOrDefault(t => t.PanelId == panelId).Polygon;

                polygon.Fill = _highlightOriginalColors[panelId];

                _highlightOriginalColors.Remove(panelId);
            }
        }
    }
}
