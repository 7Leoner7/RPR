using System.Windows;
using System.Windows.Media;

namespace RPR.Model
{
    public class Camera : IBaseCamera
    {
        public double Zoom { get; set; }
        public double WidthProjection { get; set; }
        public double HeightProjection { get; set; }

        public Matrix Matrix { get; set; }

        public Point Position
        {
            get;
            set;
        }

        public delegate void UpdateCameraHandler(Camera camera, EventArgsCamera e);
        public event UpdateCameraHandler? OnUpdatePosition;
        public event UpdateCameraHandler? OnUpdateProjection;
        public event UpdateCameraHandler? OnUpdateZoom;

        public Camera()
        {

        }

        public void Init()
        {
            Matrix = Matrix.Identity;
            Zoom = 1;
        }

        public void UpdatePosition(Vector vector)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = vector,
                Height = new Vector(HeightProjection, HeightProjection),
                Width = new Vector(WidthProjection, WidthProjection),
                Matrix = Matrix,
                Zoom = new Vector(Zoom, Zoom)
            };
            this.Position += vector;
            OnUpdatePosition?.Invoke(this, CameraEvent);
        }

        public void UpdateProjections(double width, double height)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = new Vector(0, 0),
                Width = new Vector(WidthProjection, width),
                Height = new Vector(HeightProjection, height),
                Matrix = this.Matrix,
                Zoom = new Vector(Zoom, Zoom)
            };
            this.WidthProjection = width;
            this.HeightProjection = height;
            OnUpdateProjection?.Invoke(this, CameraEvent);
        }

        public void UpdateZoom(double zoom)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = new Vector(0, 0),
                Width = new Vector(WidthProjection, WidthProjection),
                Height = new Vector(HeightProjection, HeightProjection),
                Matrix = this.Matrix,
                Zoom = new Vector(Zoom, zoom)
            };

            OnUpdateZoom?.Invoke(this, CameraEvent);
        }

        public void GoToTheBegin()
        {
            UpdatePosition(new Vector(-this.Position.X, -this.Position.Y));
        }

    }

    public class EventArgsCamera
    {
        public Vector Velocity { get; set; }
        public Vector Width { get; set; }
        public Vector Height { get; set; }
        public Matrix Matrix { get; set; }
        public Vector Zoom { get; set; }
    }
}
