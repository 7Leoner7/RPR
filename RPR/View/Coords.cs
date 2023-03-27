using RPR.Model;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPR.View
{
    public class Coords
    {
        public Line X { get; protected set; }
        public Line Y { get; protected set; }
        public Canvas BaseView { get; protected set; }
        public Camera Camera { get; protected set; }

        public Coords(ref Line x, ref Line y, ref Canvas b_view, ref Camera camera)
        {
            this.X = x;
            this.Y = y;
            this.BaseView = b_view;
            this.Camera = camera;
        }

        public void Init()
        {
            X.X1 = 0;
            X.Y1 = BaseView.ActualHeight / 2;
            X.X2 = BaseView.ActualWidth;
            X.Y2 = BaseView.ActualHeight / 2;
            X.Stroke = new SolidColorBrush(Colors.White);
            X.StrokeThickness = 2;
            X.Tag = "Coords-";

            Y.X1 = BaseView.ActualWidth / 2;
            Y.Y1 = 0;
            Y.X2 = BaseView.ActualWidth / 2;
            Y.Y2 = BaseView.ActualHeight;
            Y.Stroke = new SolidColorBrush(Colors.White);
            Y.StrokeThickness = 2;
            Y.Tag = "Coords|";
        }

        public void Update(EventArgsCamera e)
        {
            var Top = ((Camera.HeightProjection / 2) - Math.Abs(Camera.Position.Y) >= 10)
                ? X.Margin.Top : X.Margin.Top - e.Velocity.Y;
            X.Margin = new Thickness()
            {
                Bottom = 0,
                Left = X.Margin.Left + e.Velocity.X,
                Right = 0,
                Top = Top,
            };

            var Left = ((Camera.WidthProjection / 2) - Math.Abs(Camera.Position.X) >= 10)
                ? Y.Margin.Left : Y.Margin.Left + e.Velocity.X;
            Y.Margin = new Thickness()
            {
                Bottom = 0,
                Left = Left,
                Right = 0,
                Top = Y.Margin.Top - e.Velocity.Y,
            };
        }
    }
}
