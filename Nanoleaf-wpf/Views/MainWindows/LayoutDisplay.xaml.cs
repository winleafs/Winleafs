using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Winleafs.Wpf.Views.MainWindows
{
    /// <summary>
    /// Interaction logic for LayoutDisplay.xaml
    /// </summary>
    public partial class LayoutDisplay : UserControl
    {
        private static readonly SolidColorBrush _lineColor = Brushes.LightSteelBlue;
        private static readonly int _triangleSize = 50;
        private static readonly double _triangleHeight = Math.Sqrt(3) / 2 * _triangleSize;

        public LayoutDisplay()
        {
            InitializeComponent();

            DrawTriangle(50, 50, 180);
            DrawTriangle(100, 50, 0);
        }

        /// <summary>
        /// Draws an equilateral triangle from the given center point and rotation
        /// </summary>
        private void DrawTriangle(double x, double y, double rotation)
        {
            //First assume that we draw the triangle facing up:
            //     C
            //    /\
            //   /  \
            //  /____\
            // A      B

            //Point A is 1/3 of the height downwards, then half of the side length to the left
            double Ax = x - (_triangleSize / 2);
            double Ay = y + (_triangleHeight / 3);

            //Point B is 1/3 of the height downwards, then half of the side length to the right
            double Bx = x + (_triangleSize / 2);
            double By = Ay;

            //Point C is on x and 2/3 of the height upwards
            double Cx = x;
            double Cy = y - (2 * (_triangleHeight / 3));

            var triangle = new Polygon();
            triangle.Points.Add(new Point(Ax, Ay));
            triangle.Points.Add(new Point(Bx, By));
            triangle.Points.Add(new Point(Cx, Cy));

            //Now we can rotate the triangle
            var rotateTransform = new RotateTransform(rotation);
            rotateTransform.CenterX = x;
            rotateTransform.CenterY = y;
            triangle.RenderTransform = rotateTransform;

            triangle.Stroke = _lineColor;
            triangle.HorizontalAlignment = HorizontalAlignment.Left;
            triangle.VerticalAlignment = VerticalAlignment.Top;
            triangle.StrokeThickness = 2;

            CanvasArea.Children.Add(triangle);
        }
    }
}
