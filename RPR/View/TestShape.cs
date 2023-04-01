using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPR
{
    public class TestShape : Shape
    {
        protected override Geometry DefiningGeometry
        {
            get { return GenerateMyWeirdGeometry(); }
        }

        private Geometry GenerateMyWeirdGeometry()
        {
            StreamGeometry geom = new StreamGeometry();
            using (StreamGeometryContext gc = geom.Open())
            {
                // isFilled = false, isClosed = true
                gc.BeginFigure(new Point(50.0, 50.0), false, true);
                gc.ArcTo(new Point(75.0, 75.0), new Size(10.0, 20.0), 0.0, false, SweepDirection.Clockwise, true, true);
                gc.ArcTo(new Point(100.0, 100.0), new Size(10.0, 20.0), 0.0, false, SweepDirection.Clockwise, true, true);
                
            }
          
            return geom;
        }
    }
}
