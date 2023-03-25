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

            IsInitialized = true;

            for(; IsInitialized;)
            {
                if ((View.ActualHeight == 0) || (View.ActualWidth == 0)) continue;
                var ellipse = new Ellipse()
                {
                    Width = 100,
                    Height = 100,
                    Fill = new SolidColorBrush(Color.FromRgb((byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255))),
                    Margin = new Thickness() { 
                        Bottom = rand.Next(100, (int)View.ActualHeight - 100), 
                        Left = rand.Next(100, (int)View.ActualWidth - 100), 
                        Right = rand.Next(100, (int)View.ActualWidth - 100), 
                        Top = rand.Next(100, (int)View.ActualHeight - 100) 
                    }
                };
                
                Update(ellipse);
                await Task.Delay(100);
                Delete(0);
            }
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
    }
}
