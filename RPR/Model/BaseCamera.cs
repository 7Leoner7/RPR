using System.Windows;

namespace RPR.Model
{
    abstract public class BaseCamera
    {
        public double Rate_Size { get; protected set; }
        public double WidthProjection { get; protected set; }
        public double HeightProjection { get; protected set; }
        public Point Position { get; protected set; }
        public Point PositionRelativeCanvas { get; set; }

        abstract public void UpdatePosition(Vector vector);

        abstract public void UpdateRateSize(double rate);

        abstract public void UpdateProjections(double width, double height);
    }
}
