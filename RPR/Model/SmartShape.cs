using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace RPR.Model
{
    public class SmartShape<TShape> where TShape : Shape
    {
        public TShape Shape { get; protected set; }

        public Point PossitionRelativeView { 
            get
            {
               return new Point(Shape.Margin.Left + Shape.ActualWidth / 2, Shape.Margin.Top + Shape.ActualHeight / 2);
            }
            set
            {
                Shape.Margin = new Thickness()
                {
                    Left = value.X - Shape.ActualWidth / 2,
                    Top = value.Y - Shape.ActualHeight / 2,
                    Bottom = 0,
                    Right = 0,
                };
            }
        }

        public delegate void UpdateSmartShape(SmartShape<TShape> target, EventArgsSmartShapes e);

        public event UpdateSmartShape? OnCollisionShapes;

        protected List<TShape> GetAllShapesInRange(double R) { return new List<TShape>(); } 

        protected void InitShape()
        {
            
        }

        private void Shape_SourceUpdated(object? sender, System.Windows.Data.DataTransferEventArgs e)
        {
            throw new NotImplementedException();
        }

        public SmartShape(TShape base_shape)
        {
            Shape = base_shape;
            InitShape();
        }
    }

    public class EventArgsSmartShapes
    {
        public bool Collision { get; set; }
        public Shape Called_Shape { get; set; }
    }
}
