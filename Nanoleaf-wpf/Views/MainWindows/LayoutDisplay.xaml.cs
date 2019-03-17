using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Api;
using Winleafs.Models.Models;
using Winleafs.Wpf.Helpers;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for LayoutDisplay.xaml
    /// </summary>
    public partial class LayoutDisplay : UserControl
    {
        private static readonly SolidColorBrush _lineColor = Brushes.LightSteelBlue;

        //Since we draw in constructor, height and width are not available then, so we use these fixed values
        private static readonly int _height = 400;
        private static readonly int _width = 400;

        private static readonly Random _random = new Random();

        //Values based on testing.
        //For each triangle size, the conversion rate is saved such that the coordinates from Nanoleaf can be properly converted to pixel locations
        //At most, a device is 15 panels wide, 15*25 < 400, so 25 is the lowest value we need
        private static readonly Dictionary<int, double> _sizesWithConversionRate = new Dictionary<int, double>() { { 25, 5.45 }, { 30, 4.6 }, { 40, 3.55 }, { 50, 2.85 }, { 60, 2.38 }, { 70, 2.1 }, { 80, 1.82 }, { 90, 1.62 }, { 100, 1.47 }};

        private List<Polygon> _triangles;
        private RotateTransform _globalRotationTransform;
        private int _triangleSize;
        private double _conversionRate;

        //Timer to update the colors periodically to update with schedule
        private Timer _timer;

        public LayoutDisplay()
        {
            InitializeComponent();

            DrawLayout();

            _timer = new Timer(30000); //Update the colors every 30 seconds
            _timer.Elapsed += OnTimedEvent;
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
        }

        public void DrawLayout()
        {
            CanvasArea.Children.Clear();

            _triangles = new List<Polygon>();

            //Retrieve layout
            var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActiveDevice);
            var layout = client.LayoutEndpoint.GetLayout();
            var globalOrientation = client.LayoutEndpoint.GetGlobalOrientation();

            if (layout == null)
            {
                //Layout could not be retrieved show redraw button and do nothing more
                RedrawButton.Visibility = Visibility.Visible;
                return;
            }
            else
            {
                RedrawButton.Visibility = Visibility.Hidden;
            }

            //Reverse every Y coordinate since Y = 0 means top for the canvas but for Nanoleaf it means bottom
            foreach (var panelPosition in layout.PanelPositions)
            {
                panelPosition.Y = panelPosition.Y * -1;
            }

            //Normalize panel positions such that the coordinates start at 0
            var mostLeftPanelCoordinate = layout.PanelPositions.Min(pp => pp.X);
            var mostTopPanelCoordinate = layout.PanelPositions.Min(pp => pp.Y);

            if (mostLeftPanelCoordinate < 0)
            {
                foreach (var panelPosition in layout.PanelPositions)
                {
                    panelPosition.X += Math.Abs(mostLeftPanelCoordinate);
                }
            }

            if (mostTopPanelCoordinate < 0)
            {
                foreach (var panelPosition in layout.PanelPositions)
                {
                    panelPosition.Y += Math.Abs(mostTopPanelCoordinate);
                }
            }

            //Calculate the maximum triangle size, taking the global orientation into account
            var selectedSizeWithConversionRate = _sizesWithConversionRate.FirstOrDefault();
            var transform = new RotateTransform(globalOrientation.Value, layout.PanelPositions.Max(pp => pp.X) / 2, layout.PanelPositions.Max(pp => pp.X) / 2);
            Point maxPoint = new Point();

            foreach (var panelPosition in layout.PanelPositions)
            {
                var point = transform.Transform(new Point(panelPosition.X, panelPosition.Y));

                maxPoint.X = point.X > maxPoint.X ? point.X : maxPoint.X;
                maxPoint.Y = point.Y > maxPoint.Y ? point.Y : maxPoint.Y;
            }            

            for (var i = 0; i < _sizesWithConversionRate.Count; i++)
            {
                var sizeWithConversionRate = _sizesWithConversionRate.ElementAt(i);

                if (maxPoint.X / sizeWithConversionRate.Value < _height - sizeWithConversionRate.Key
                    && maxPoint.Y / sizeWithConversionRate.Value < _width - sizeWithConversionRate.Key)
                {
                    selectedSizeWithConversionRate = sizeWithConversionRate;
                }
                else
                {
                    break;
                }
            }

            _triangleSize = selectedSizeWithConversionRate.Key;
            _conversionRate = selectedSizeWithConversionRate.Value;

            //Transform the X and Y coordinates such that they can be represented by pixels
            foreach (var panelPosition in layout.PanelPositions)
            {
                panelPosition.TransformedX = panelPosition.X / _conversionRate;
                panelPosition.TransformedY = panelPosition.Y / _conversionRate;
            }

            //Calculate the transform for the global orientation. All panels should rotate over the center of all panels
            _globalRotationTransform = new RotateTransform(globalOrientation.Value, layout.PanelPositions.Max(pp => pp.TransformedX) / 2, layout.PanelPositions.Max(pp => pp.TransformedY) / 2);

            //Draw the panels
            foreach (var panelPosition in layout.PanelPositions)
            {
                CreateTriangle(panelPosition.TransformedX, panelPosition.TransformedY, panelPosition.Orientation);
            }

            //Fix the coordinates if any are placed outside the canvas after placing and rotating
            double minX = 0;
            double minY = 0;

            foreach (var triangle in _triangles)
            {
                foreach (var point in triangle.Points)
                {
                    minX = Math.Min(minX, point.X);
                    minY = Math.Min(minY, point.Y);
                }
            }

            var diffX = minX < 0 ? Math.Abs(minX) : 0;
            var diffY = minY < 0 ? Math.Abs(minY) : 0;

            foreach (var triangle in _triangles)
            {
                for (var i = 0; i < triangle.Points.Count; i++)
                {
                    var x = triangle.Points[i].X + diffX;
                    var y = triangle.Points[i].Y + diffY;

                    triangle.Points[i] = new Point(x, y);
                }
            }

            //Draw the triangles
            foreach (var triangle in _triangles)
            {
                CanvasArea.Children.Add(triangle);
            }

            UpdateColors();
        }

        /// <summary>
        /// Draws an equilateral triangle from the given center point and rotation. Also applies the global rotation
        /// </summary>
        private void CreateTriangle(double x, double y, double rotation)
        {
            //First assume that we draw the triangle facing up:
            //     A
            //    /\
            //   /  \
            //  /____\
            // B      C

            var A = new Point(x, y - ((Math.Sqrt(3) / 3) * _triangleSize));
            var B = new Point(x - (_triangleSize / 2), y + ((Math.Sqrt(3) / 6) * _triangleSize));
            var C = new Point(x + (_triangleSize / 2), y + ((Math.Sqrt(3) / 6) * _triangleSize));

            var rotateTransform = new RotateTransform(rotation, x, y);

            //Apply transformation and add points to polygon
            var triangle = new Polygon();
            triangle.Points.Add(_globalRotationTransform.Transform(rotateTransform.Transform(A)));
            triangle.Points.Add(_globalRotationTransform.Transform(rotateTransform.Transform(B)));
            triangle.Points.Add(_globalRotationTransform.Transform(rotateTransform.Transform(C)));
            
            triangle.Stroke = _lineColor;
            triangle.HorizontalAlignment = HorizontalAlignment.Left;
            triangle.VerticalAlignment = VerticalAlignment.Top;
            triangle.StrokeThickness = 2;

            _triangles.Add(triangle);
        }

        private void Redraw_Click(object sender, RoutedEventArgs e)
        {
            DrawLayout();
        }

        public void UpdateColors()
        {
            if (UserSettings.Settings.ActiveDevice != null)
            {
                var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActiveDevice);

                //Get colors of current effect
                var effect = client.EffectsEndpoint.GetEffectDetails(UserSettings.Settings.ActiveDevice.GetActiveEffect());

                Dispatcher.Invoke(new Action(() =>
                {
                    if (effect == null)
                    {
                        foreach (var triangle in _triangles)
                        {
                            triangle.Fill = null;
                        }
                    }
                    else
                    {
                        var colors = new List<SolidColorBrush>();
                        foreach (var hsb in effect.Palette)
                        {
                            colors.Add(new SolidColorBrush(HSBToRGBConverter.ConvertToMediaColor(hsb.Hue, hsb.Saturation, hsb.Brightness)));
                        }

                        foreach (var triangle in _triangles)
                        {
                            triangle.Fill = colors[_random.Next(colors.Count)];
                        }
                    }
                }));
            }            
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            UpdateColors();
        }
    }
}
