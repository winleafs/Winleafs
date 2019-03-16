using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Winleafs.Api;
using Winleafs.Models.Models;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for LayoutDisplay.xaml
    /// </summary>
    public partial class LayoutDisplay : UserControl
    {
        private static readonly SolidColorBrush _lineColor = Brushes.LightSteelBlue;

        //Values based on testing. For each triangle size, the conversion rate is saved such that the coordinates from Nanoleaf can be properly converted
        private static readonly Dictionary<int, double> _sizesWithConversionRate = new Dictionary<int, double>() { { 25, 5.45 }, { 30, 4.6 }, { 40, 3.55 }, { 50, 2.85 }, { 60, 2.38 }, { 70, 2.1 }, { 80, 1.82 }, { 90, 1.62 }, { 100, 1.47 }};

        private List<Polygon> _triangles;
        private RotateTransform _globalRotationTransform;
        private int _triangleSize;
        private double _conversionRate;

        private int _height = 400; //Since we draw in constructor, height and width are not available then, so we use these fixed values
        private int _width = 400;

        public LayoutDisplay()
        {
            InitializeComponent();

            _triangles = new List<Polygon>();

            //Retrieve layout
            var client = NanoleafClient.GetClientForDevice(UserSettings.Settings.ActiveDevice);
            var layout = client.LayoutEndpoint.GetLayout();

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

            //Calculate the maximum triangle size
            var selectedSizeWithConversionRate = _sizesWithConversionRate.FirstOrDefault();
            var maxX = layout.PanelPositions.Max(pp => pp.X);
            var maxY = layout.PanelPositions.Max(pp => pp.Y);

            for (var i = 0; i < _sizesWithConversionRate.Count; i++)
            {
                var sizeWithConversionRate = _sizesWithConversionRate.ElementAt(i);

                if (maxX / sizeWithConversionRate.Value < _height - sizeWithConversionRate.Key && maxY / sizeWithConversionRate.Value < _width - sizeWithConversionRate.Key)
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
            var globalOrientation = client.LayoutEndpoint.GetGlobalOrientation();
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

            var finalTriangles = new List<Polygon>();
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
    }
}
