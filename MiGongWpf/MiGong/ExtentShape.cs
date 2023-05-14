using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace MiGongWpf.MiGong
{
    internal static class ExtentShape
    {
        internal static Line GetLine(this Line line, Point point1, Point point2)
        {
            line.X1 = point1.X;
            line.Y1 = point1.Y;
            line.X2 = point2.X;
            line.Y2 = point2.Y;
            line.Stroke = new SolidColorBrush(Color.FromArgb(192, 33, 33, 33));
            return line;
        }

        internal static Point GetStartPoint(this Line line)
        {
            return new Point(line.X1, line.Y1);
        }
        internal static Point GetEndPoint(this Line line)
        {
            return new Point(line.X2, line.Y2);
        }

        internal static Point Add(this Point point, Vector vector)
        {
            return Point.Add(point, vector);
        }
    }
}
