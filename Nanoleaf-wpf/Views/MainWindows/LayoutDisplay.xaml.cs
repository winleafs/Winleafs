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
        private static readonly int _triangleSize = 50; //When changing this, also update _coordinateConversion
        private static readonly double _coordinateConversion = 2.78;
        private static readonly double _triangleHeight = Math.Sqrt(3) / 2 * _triangleSize;

        private List<Polygon> _triangles;
        private RotateTransform _globalRotationTransform;

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

            //Normalize panel positions such that the coordinates start at _triangleSize + _triangleSize
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

            //Transform the X and Y coordinates such that they can be represented by pixels
            foreach (var panelPosition in layout.PanelPositions)
            {
                panelPosition.X = (int)(panelPosition.X / _coordinateConversion);
                panelPosition.Y = (int)(panelPosition.Y / _coordinateConversion);
            }

            //Calculate the transform for the global orientation. All panels should rotate over the center of all panels
            var globalOrientation = client.LayoutEndpoint.GetGlobalOrientation();
            _globalRotationTransform = new RotateTransform(globalOrientation.Value, layout.PanelPositions.Max(pp => pp.X) / 2, layout.PanelPositions.Max(pp => pp.Y) / 2);

            //Draw the panels
            foreach (var panelPosition in layout.PanelPositions)
            {
                CreateTriangle(panelPosition.X, panelPosition.Y, panelPosition.Orientation);
            }

            //Fix the coordinates if any are placed outside the canvas after drawing and rotating
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
        /// Draws an equilateral triangle from the given center point and rotation
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
