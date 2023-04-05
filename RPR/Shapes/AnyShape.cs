using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace RPR.Shapes
{
    public class AnyShape : Shape
    {
        protected override Geometry DefiningGeometry => MyGeometry;

        public Geometry MyGeometry { get; protected set; }

        public Shape? Base { get; protected set; }

        public AnyShape(Shape base_shape)
        {
            Base = base_shape;
            GenerateMyGeometry(Base);
        }

        public AnyShape()
        {

        }

        private void GenerateMyGeometry(Shape base_shape)
        {
            MyGeometry = base_shape.RenderedGeometry;
        }
    }
}
