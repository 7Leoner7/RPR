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
            MoveCamera = false;

            Camera = new Camera();
            Camera.OnUpdatePosition += Camera_OnUpdatePosition;
            Camera.OnUpdateProjection += Camera_OnUpdateProjection;
            Camera.OnUpdateRateSize += Camera_OnUpdateRateSize;
            //Camera.UpdateRateSize(1);
            //Camera.UpdateProjections(View.ActualWidth, View.ActualHeight);
            Camera.PositionRelativeCanvas = new Point(View.ActualWidth / 2, View.ActualHeight / 2);
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
                    MoveCamera= false;
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
            throw new NotImplementedException();
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
            var vec = new System.Windows.Vector(rand.NextDouble(), rand.NextDouble());
            var speed = 0;
            var FrameSpeed = 1000 / 144;
            vec.X *= speed;
            vec.Y *= speed;
            var pos = new Thickness()
            {
                Bottom = 0, //rand.Next(Height, (int)View.ActualHeight - Height),
                Left = rand.Next(Width, (int)View.ActualWidth - Width),
                Right = 0, //rand.Next(Width, (int)View.ActualWidth - Width),
                Top = rand.Next(Height, (int)View.ActualHeight - Height)
            };

            var ellipse = new Ellipse()
            {
                Width = Width,
                Height = Height,
                Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                Margin = pos
            };
            ellipse.MouseDown += Ellipse_MouseDown;
            Update(ellipse);

            IsInitialized = true;

            for (; IsInitialized;)
            {
                if ((pos.Top + vec.Y + Height > View.ActualHeight) || (pos.Top + vec.Y <= 0))
                    vec.Y *= -1;

                if ((pos.Left + vec.X + Width > View.ActualWidth) || (pos.Left + vec.X <= 0))
                    vec.X *= -1;

                //if ((pos.Top > View.ActualHeight) || (pos.Top < 0) || (pos.Left > View.ActualWidth) || (pos.Left < 0))
                //    pos = new Thickness()
                //    {
                //        Bottom = 0,
                //        Left = rand.Next(Width, (int)View.ActualWidth - Width),
                //        Right = 0,
                //        Top = rand.Next(Height, (int)View.ActualHeight - Height)
                //    };

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
            DeInit();
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
