using System.Windows;

namespace RPR.Model
{
    abstract public class BaseCamera
    {
        public double Rate_Size { get; protected set; }
        public double WidthProjection { get; protected set; }
        public double HeightProjection { get; protected set; }
        protected Point Absolute_Position;
        public Point WorldPosition { 
            get 
            { 
                return new Point(Absolute_Position.X / Rate_Size, Absolute_Position.Y / Rate_Size);
            } 
        }
        public Point Position { 
            get 
            { 
                return new Point(Absolute_Position.X, Absolute_Position.Y); 
            }
            protected set 
            {
                Absolute_Position = value;        
            } 
        }
        public Point PositionRelativeCanvas { 
            get 
            { 
                return new Point(WidthProjection / 2, HeightProjection / 2); 
            } 
        }

        abstract public void UpdatePosition(Vector vector);

        abstract public void UpdateRateSize(double rate);

        abstract public void UpdateProjections(double width, double height);
    }
}
