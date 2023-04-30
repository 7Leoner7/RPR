using RPR.Model;
using RPR.View;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace RPR.ViewModel
{
    public class GameView : BaseView
    {
        protected bool MoveCamera { get; set; }
        public Coords? Coords { get; protected set; }
        public int FrameSpeed { get; set; }
        public double WheelDelay { get; set; }
        static public World World { get; set; }

        public GameView(ref Canvas view, string? WorldName = null)
        {
            View = view;
            View.MouseMove += View_MouseMove;
            View.SizeChanged += View_SizeChanged;
            View.MouseWheel += View_MouseWheel;
            MoveCamera = false;

            FrameSpeed = 1000 / 1000;
            WheelDelay = 2.0;
            if (WorldName != null)
            { 
                World = World.Deserialize(WorldName) ?? World;
               
                if (World == null)
                {
                    World = new World();
                    World.InitWorld();
                    World.SetWorldName(WorldName);
                }
            }

            World.Camera.OnUpdatePosition += Camera_OnUpdatePosition;
            World.Camera.OnUpdateProjection += Camera_OnUpdateProjection;

            World.Camera.UpdateProjections(View.ActualWidth, View.ActualHeight);
        }

        private void View_MouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            // Camera.UpdateRateSize(Camera.Rate_SizeX / (e.Delta < 0 ? WheelDelay : 1 / WheelDelay), Camera.Rate_SizeY / (e.Delta < 0 ? WheelDelay : 1 / WheelDelay));
        }

        private void View_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rate = Math.Sqrt(e.NewSize.Width + e.NewSize.Height) / Math.Sqrt(e.PreviousSize.Width + e.PreviousSize.Height);
            World.Camera.UpdateProjections(View.ActualWidth, View.ActualHeight);
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

                    World.Camera.UpdatePosition(vector);
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
            try
            {
                if (!(element is Shape)) return true;
                if (element is TestShape) return true;
                if (!element.Tag.Equals("View_Shape")) return true;
                return false;
            }
            catch (Exception ex)
            {
                return true;
            }

        }

        private void Camera_OnUpdateProjection(Camera camera, EventArgsCamera e)
        {
            Coords?.Init();
            foreach (FrameworkElement child in GameView.View.Children)
            {
                if (IsInException(child)) continue;
                child.Margin = new Thickness(child.Margin.Left * (e.Width.Y / e.Width.X), child.Margin.Top * (e.Height.Y / e.Height.X), child.Margin.Right, child.Margin.Bottom);
            }
        }

        private void Camera_OnUpdatePosition(Camera camera, EventArgsCamera e)
        {
            foreach (FrameworkElement child in View.Children)
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
            var camera = World.Camera;
            Coords = new Coords(ref X, ref Y, ref view, ref camera);
            Coords.Init();

            Update(Coords.X);
            Update(Coords.Y);
        }

        public override async void Init()
        {
            InitCoords();

            //InitElements
            Update();
            World.Camera.UpdatePosition(new Vector()); //обновление позиции

            IsInitialized = true;
            //InitLoop
            World.ManagerShapes.StartRulles();
            //while (IsInitialized)
            //{
            //    Update();
            //    await Task.Delay(100);
            //}
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
        public void Update()
        {
            foreach (var elem in World.ManagerShapes.Shapes)
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
            var vector = point_rel_canvas - new Point(View.ActualWidth / 2, View.ActualHeight / 2);
            vector.Y *= -1;
            var result = World.Camera.Position + vector;
            return result;
        }

        public void Save() =>
            World.Serialize();
    }
}