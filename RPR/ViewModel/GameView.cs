using RPR.Model;
using RPR.Shapes;
using RPR.View;
using System;
using System.Collections.Generic;
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
        public Coords Coords { get; protected set; }
        public int FrameSpeed { get; set; }
        public double WheelDelay { get; set; }
        public ManagerShapes ManagerShapes { get; init; }

        public GameView(ref Canvas view, string? WorldName = null)
        {
            View = view;
            View.MouseMove += View_MouseMove;
            View.SizeChanged += View_SizeChanged;
            View.MouseWheel += View_MouseWheel;
            MoveCamera = false;

            FrameSpeed = 1000 / 144;
            WheelDelay = 2.0;

            Camera = new Camera();
            Camera.OnUpdatePosition += Camera_OnUpdatePosition;
            Camera.OnUpdateProjection += Camera_OnUpdateProjection;
            Camera.OnUpdateRateSize += Camera_OnUpdateRateSize;
            Camera.UpdateRateSize(1);
            Camera.UpdateProjections(View.ActualWidth / Camera.Rate_Size, View.ActualHeight / Camera.Rate_Size);

            ManagerShapes= new ManagerShapes(WorldName);
        }

        private void View_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            Camera.UpdateRateSize(Camera.Rate_Size / (e.Delta < 0 ? WheelDelay : 1 / WheelDelay));
        }

        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rate = Math.Sqrt(e.NewSize.Width + e.NewSize.Height) / Math.Sqrt(e.PreviousSize.Width + e.PreviousSize.Height);
            Camera.UpdateRateSize(Camera.Rate_Size * rate);
            Camera.UpdateProjections(View.ActualWidth, View.ActualHeight);
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
                    Vector vector = (Vector)(new_p - old_p);
                    vector.X *= -1;

                    Camera.UpdatePosition(vector);
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

        protected bool IsInException(FrameworkElement element)
        {
            if (!(element is Shape)) return true;
            if (element is TestShape) return true;
            if (!element.Tag.Equals("View_Shape")) return true;
            return false;
        }

        private void Camera_OnUpdateRateSize(Camera camera, EventArgsCamera e)
        {
            foreach (FrameworkElement child in this.View.Children)
            {
                if (IsInException(child)) continue;
                child.Width = child.Width * e.RateSize.Y / e.RateSize.X > 0 ? child.Width * e.RateSize.Y / e.RateSize.X : child.Width;
                child.Height = child.Height * e.RateSize.Y / e.RateSize.X > 0 ? child.Height * e.RateSize.Y / e.RateSize.X : child.Height;
            }
            Coords?.Update(e);
        }

        private void Camera_OnUpdateProjection(Camera camera, EventArgsCamera e)
        {
            Coords?.Init();
            foreach (FrameworkElement child in this.View.Children)
            {
                if (IsInException(child)) continue;
                child.Margin = new Thickness(child.Margin.Left * (e.Width.Y / e.Width.X), child.Margin.Top * (e.Height.Y / e.Height.X), child.Margin.Right, child.Margin.Bottom);
            }
        }

        private void Camera_OnUpdatePosition(Camera camera, EventArgsCamera e)
        {
            foreach (FrameworkElement child in this.View.Children)
            {
                if (IsInException(child)) continue;
                child.Margin = new Thickness()
                {
                    Bottom = 0,
                    Left = child.Margin.Left - e.Velocity.X,
                    Right = 0,
                    Top = child.Margin.Top + e.Velocity.Y
                };
            }
            Coords?.Update(e);
        }

        private void InitCoords()
        {
            Line X = new Line();
            Line Y = new Line();
            var view = View;
            var camera = Camera;
            Coords = new Coords(ref X, ref Y, ref view, ref camera);
            Coords.Init();

            Update(Coords.X);
            Update(Coords.Y);
        }

        async public override void Init()
        {
            InitCoords();

            //InitElements
            Update();
            
            IsInitialized = true;
            //InitLoop
            for (; IsInitialized;)
            {
                ManagerShapes.ShapesFollowTheRules();
                await Task.Delay(FrameSpeed);
            }
        }

        public override void DeInit()
        {
            this.IsInitialized = false;
        }

        public override void Update(FrameworkElement element)
        {
            if (View.Children.Contains(element))
            {
                View.Children.Remove(element);
                View.Children.Add(element);
            }
            else View.Children.Add(element);
        }

        public override void UpdateAll(List<FrameworkElement> elements)
        {
            foreach (FrameworkElement element in elements)
                Update(element);
        }

        /// <summary>
        /// Update from ManagerShapes
        /// </summary>
        protected void Update()
        {
            foreach (var elem in ManagerShapes.World.SmartShapes)
                Update(elem.GetShape());
        }

        public override void Delete(FrameworkElement element)
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

        public Point GetPointMouseInWordl(Point point_rel_canvas)
        {
            var vector = point_rel_canvas - Camera.PositionRelativeCanvas;
            vector.Y *= -1;
            var result = Camera.Position + vector;
            return result;
        }

        public void Save() =>
            ManagerShapes.Serialize();
    }
}