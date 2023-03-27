using RPR.Model;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPR.ViewModel
{
    public class GameView : BaseView
    {
        protected bool MoveCamera { get; set; }

        public GameView(ref Canvas view)
        {
            View = view;
            View.MouseMove += View_MouseMove;
            View.SizeChanged += View_SizeChanged;
            View.MouseWheel += View_MouseWheel;
            MoveCamera = false;

            Camera = new Camera();
            Camera.OnUpdatePosition += Camera_OnUpdatePosition;
            Camera.OnUpdateProjection += Camera_OnUpdateProjection;
            Camera.OnUpdateRateSize += Camera_OnUpdateRateSize;
            Camera.UpdateRateSize(1);
            //Camera.UpdateProjections(View.ActualWidth, View.ActualHeight);
            Camera.PositionRelativeCanvas = new Point(View.ActualWidth / 2, View.ActualHeight / 2);
        }

        private void View_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Camera.UpdateRateSize(Camera.Rate_Size / (e.Delta < 0 ? 15.0/10 : 10.0/15));
        }

        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Camera.PositionRelativeCanvas = new Point(View.ActualWidth / 2, View.ActualHeight / 2);
        }

        Point? new_p = null;
        Point? old_p = null;

        private void View_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (MoveCamera || e.LeftButton == System.Windows.Input.MouseButtonState.Pressed)
            {
                if (old_p == null)
                    old_p = e.GetPosition(View);
                else if (new_p == null)
                    new_p = e.GetPosition(View);
                else
                {
                    MoveCamera = true;
                    Camera.UpdatePosition((Vector)(new_p - old_p));
                    old_p = new_p;
                    new_p = null;
                    MoveCamera = false;
                }
            }
            else
            {
                old_p = null;
                new_p = null;
            }
        }

        private void Camera_OnUpdateRateSize(Camera camera, EventArgsCamera e)
        {
            foreach (Shape child in this.View.Children)
            {
                child.Width = child.Width * e.RateSize.Y / e.RateSize.X > 0 ? child.Width * e.RateSize.Y / e.RateSize.X : child.Width;
                child.Height = child.Height * e.RateSize.Y / e.RateSize.X > 0 ? child.Height * e.RateSize.Y / e.RateSize.X : child.Height;
            }
        }

        private void Camera_OnUpdateProjection(Camera camera, EventArgsCamera e)
        {
            throw new NotImplementedException();
        }

        private void Camera_OnUpdatePosition(Camera camera, EventArgsCamera e)
        {
            foreach (Shape child in this.View.Children)
            {
                child.Margin = new Thickness()
                {
                    Bottom = 0,
                    Left = child.Margin.Left - e.Velocity.X,
                    Right = 0,
                    Top = child.Margin.Top - e.Velocity.Y
                };
            }
        }

        async public override void Init()
        {
            var rand = new Random();
            var Width = 100;
            var Height = 100;
            var vec = new Vector(rand.NextDouble(), rand.NextDouble());
            var speed = 10;
            var FrameSpeed = 1000 / 1000;
            vec.X *= speed;
            vec.Y *= speed;

            var ellipse = new Ellipse()
            {
                Width = Width,
                Height = Height,
                Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                Margin = new Thickness()
                {
                    Bottom = 0,
                    Left = rand.Next(Width, (int)View.ActualWidth - Width),
                    Right = 0,
                    Top = rand.Next(Height, (int)View.ActualHeight - Height)
                },
            };
            ellipse.MouseDown += Ellipse_MouseDown;
            Update(ellipse);

            IsInitialized = true;

            for (; IsInitialized;)
            {
                if ((ellipse.Margin.Top + vec.Y + Height > View.ActualHeight) || (ellipse.Margin.Top + vec.Y <= 0))
                    vec.Y *= -1;

                if ((ellipse.Margin.Left + vec.X + Width > View.ActualWidth) || (ellipse.Margin.Left + vec.X <= 0))
                    vec.X *= -1;

                if ((View.ActualHeight <= 0) || (View.ActualWidth <= 0)) continue;

                ellipse.Margin = new Thickness()
                {
                    Right = 0,
                    Bottom = 0,
                    Left = ellipse.Margin.Left + vec.X * speed,
                    Top = ellipse.Margin.Top + vec.Y * speed,
                };
                await Task.Delay(FrameSpeed);
            }
        }

        private void Ellipse_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           //DeInit();
            var shape = (Shape)sender;
            var rand = new Random();
            shape.Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
        }

        public override void DeInit()
        {
            this.IsInitialized = false;
        }

        public override void Update(UIElement element)
        {
            if (View.Children.Contains(element))
            {
                View.Children.Remove(element);
                View.Children.Add(element);
            }
            else View.Children.Add(element);
        }

        public override void Delete(UIElement element)
        {
            if (View.Children.Contains(element))
                View.Children.Remove(element);
        }

        public override void Delete(int elemID)
        {
            if (View.Children.Count > elemID)
                View.Children.RemoveAt(elemID);
        }

        public override void DeleteAll()
        {
            View.Children.Clear();
        }
    }
}
