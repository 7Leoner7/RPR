using RPR.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Windows.Threading;
using System.Numerics;

namespace RPR.ViewModel
{
    public class GameView : BaseView
    {
        public GameView(ref Canvas view)
        {
            View = view;
        }

        async public override void Init()
        {
            var rand = new Random();
            var Width = 100;
            var Height = 100;
            var vec = new System.Windows.Vector(rand.NextDouble(), rand.NextDouble());
            var speed = 10;
            var FrameSpeed = 1000/144;
            vec.X *= speed;
            vec.Y *= speed;
            var pos = new Thickness()
            {
                Bottom = 0, //rand.Next(Height, (int)View.ActualHeight - Height),
                Left = rand.Next(Width, (int)View.ActualWidth - Width),
                Right = 0, //rand.Next(Width, (int)View.ActualWidth - Width),
                Top = rand.Next(Height, (int)View.ActualHeight - Height)
            };

            IsInitialized = true;

            for(; IsInitialized;)
            {
                if((pos.Top + vec.Y + Height > View.ActualHeight)||(pos.Top + vec.Y <= 0))
                    vec.Y *= -1; 
                pos.Top += vec.Y;
                
                if ((pos.Left + vec.X + Width > View.ActualWidth)||(pos.Left + vec.X <= 0))
                    vec.X *= -1;
                pos.Left += vec.X;

                if((pos.Top>View.ActualHeight) || (pos.Top < 0) || (pos.Left > View.ActualWidth) || (pos.Left < 0))
                    pos = new Thickness()
                    {
                        Bottom = 0, //rand.Next(Height, (int)View.ActualHeight - Height),
                        Left = rand.Next(Width, (int)View.ActualWidth - Width),
                        Right = 0, //rand.Next(Width, (int)View.ActualWidth - Width),
                        Top = rand.Next(Height, (int)View.ActualHeight - Height)
                    };

                if ((View.ActualHeight == 0) || (View.ActualWidth == 0)) continue;
                
                var ellipse = new Ellipse()
                {
                    Width = Width,
                    Height = Height,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                    Margin = pos
                };
                ellipse.MouseDown += Ellipse_MouseDown;
                
                Update(ellipse);
                await Task.Delay(FrameSpeed);
                Delete(0);
            }
        }

        private void Ellipse_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DeInit();
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
