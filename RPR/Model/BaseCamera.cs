using System.Windows;
using System.Windows.Media;

namespace RPR.Model
{
    public interface IBaseCamera
    {
        public double Zoom { get; set; }
        static public double WidthProjection { get; set; }
        static public double HeightProjection { get; set; }

        public Matrix Matrix { get; set; }

        public Point Position { get; set; }

        public void UpdatePosition(Vector vector);

        public void UpdateProjections(double width, double height);

        public void UpdateZoom(double zoom);
    }
}
