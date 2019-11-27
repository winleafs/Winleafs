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
using Winleafs.Wpf.Api.Effects;
using Winleafs.Wpf.Api.Layouts;
using Winleafs.Wpf.Helpers;

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

        private static readonly Random _random = new Random();

        private List<DrawablePanel> _triangles;
        private bool _panelsClickable;
        private HashSet<int> _lockedPanelIds;
        private Dictionary<int, Brush> _highlightOriginalColors; //This dictionary saves the original colors of the triangles when highlighting

        //Timer to update the colors periodically to update with schedule
        private Timer _colorTimer;

        //Timer to detect when user is done resizing
        private Timer _resizeTimer;

        private PanelLayout _panelLayout;

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

        /// <remarks>
        /// Initialize the timer later such that the resize event is not called on start up
        /// </remarks>
        public void InitializeResizeTimer()
        {
            _resizeTimer = new Timer(250);
            _resizeTimer.Elapsed += ResizeComplete;
            _colorTimer.Enabled = false;
        }

        public void DisableColorTimer()
        {
            _colorTimer.Stop();
        }

        public void EnableClick()
        {
            _panelsClickable = true;
        }

        public void DrawLayout()
        {
            CanvasArea.Children.Clear();

            if (_panelLayout == null)
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);
                _panelLayout = orchestrator.PanelLayout;
            }
            
            _triangles = _panelLayout.GetScaledTriangles((int)ActualWidth, (int)ActualHeight);

            if (_triangles == null || !_triangles.Any())
            {
                return;
            }

            if (_panelsClickable)
            {
                foreach (var triangle in _triangles)
                {
                    triangle.Polygon.MouseDown += TriangleClicked;
                }
            }           

            //Draw the triangles
            foreach (var triangle in _triangles)
            {
                CanvasArea.Children.Add(triangle.Polygon);
            }

            UpdateColors();
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

            //Run code on main thread since we update the UI
            Dispatcher.Invoke(new Action(() =>
            {
                var orchestrator = OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice);

                //Get colors of current effect, we can display colors for nanoleaf effects or custom color effects
                var effectName = orchestrator.GetActiveEffectName();
                var customEffect = orchestrator.GetCustomEffectFromName(effectName);

                List<SolidColorBrush> colors = null;

                if (customEffect != null)
                {
                    if (customEffect is CustomColorEffect customColorEffect)
                    {
                        colors = new List<SolidColorBrush>() { new SolidColorBrush(Color.FromArgb(customColorEffect.Color.A, customColorEffect.Color.R, customColorEffect.Color.G, customColorEffect.Color.B)) };
                    }
                }
                else
                {
                    var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActiveDevice);
                    var effect = client.EffectsEndpoint.GetEffectDetails(OrchestratorCollection.GetOrchestratorForDevice(UserSettings.Settings.ActiveDevice).GetActiveEffectName());

                    if (effect != null)
                    {
                        colors = effect.Palette.Select(hsb =>
                        new SolidColorBrush(
                            HsbToRgbConverter.ConvertToMediaColor(hsb.Hue, hsb.Saturation, hsb.Brightness)
                            )).ToList();
                    }
                }

                if (colors == null)
                {
                    foreach (var triangle in _triangles)
                    {
                        triangle.Polygon.Fill = Brushes.LightSlateGray;
                    }
                }
                else
                {
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

        #region Resizing
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_resizeTimer != null)
            {
                _resizeTimer.Stop();
                _resizeTimer.Start();
            }
        }

        private void ResizeComplete(object sender, ElapsedEventArgs e)
        {
            _resizeTimer.Stop();
            Dispatcher.Invoke(new Action(() =>
            {
                DrawLayout();
            }));
        }
        #endregion
    }
}
