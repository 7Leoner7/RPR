using System.Windows;

namespace RPR.Model
{
    public class Camera : BaseCamera
    {
        public delegate void UpdateCameraHandler(Camera camera, EventArgsCamera e);
        public event UpdateCameraHandler? OnUpdatePosition;
        public event UpdateCameraHandler? OnUpdateRateSize;
        public event UpdateCameraHandler? OnUpdateProjection;

        public override void UpdatePosition(Vector vector)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = vector,
                RateSize = new Vector(this.Rate_Size, this.Rate_Size),
                Width = new Vector(this.WidthProjection, this.WidthProjection),
                Height = new Vector(this.HeightProjection, this.HeightProjection),
            };
            this.Position += vector;
            OnUpdatePosition?.Invoke(this, CameraEvent);
        }

        public override void UpdateProjections(double width, double height)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = new Vector(0,0),
                RateSize = new Vector(this.Rate_Size, this.Rate_Size),
                Width = new Vector(this.WidthProjection, width),
                Height = new Vector(this.HeightProjection, height),
            };
            this.WidthProjection = width;
            this.HeightProjection = height;
            OnUpdateProjection?.Invoke(this, CameraEvent);
        }

        public override void UpdateRateSize(double rate)
        {
            var CameraEvent = new EventArgsCamera()
            {
                Velocity = new Vector(0,0),
                RateSize = new Vector(this.Rate_Size, rate),
                Width = new Vector(this.WidthProjection, this.WidthProjection),
                Height = new Vector(this.HeightProjection, this.HeightProjection),
            };
            this.Rate_Size = rate;
            OnUpdateRateSize?.Invoke(this, CameraEvent);
        }
    }

    public class EventArgsCamera
    {
        public Vector Velocity { get; set; }
        public Vector RateSize { get; set; }
        public Vector Width { get; set; }
        public Vector Height { get; set; }
    }
}
