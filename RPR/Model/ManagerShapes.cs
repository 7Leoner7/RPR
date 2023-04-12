using RPR.Shapes;
using System.Collections.Generic;

namespace RPR.Model
{
    public class ManagerShapes
    {
        static protected List<SmartShape>? SmartShapes { get; set; }

        public List<SmartShape>? Shapes 
        {
            get
            {
                return SmartShapes;
            }
            set
            {
                SmartShapes = value;
            }
        }

        static public bool ProcessIsStoped { get; set; }

        public void Add(SmartShape shape)
        {
            SmartShapes?.Add(shape);
        }

        public void Remove(SmartShape shape) =>
            SmartShapes?.Remove(shape);

        static public ArgsSmartShapes? GetArgsSmartShape(SmartShape shape)//Нужна оптимизация!!!
        {
            if (shape.IsFollowInnerRules == true) return null;

            var args = new ArgsSmartShapes() { Called_Shape = null, Sender = shape, Bound_Collision = false, Inner_Collision = false, WorldArgs = World.GetWorldArgs() };

            for (int j = 0; j < SmartShapes?.Count; j++)
            {
                if (shape.Equals(SmartShapes[j])) continue;

                if (shape.IsCollisionBounds(SmartShapes[j]))
                {
                    args.Called_Shape = SmartShapes[j];
                    args.Bound_Collision = true;
                }
                if (shape.FindCollissionBetweenShapes(SmartShapes[j]))
                {
                    args.Inner_Collision = true;
                }
            }

            return args;
        }

        public void StartRulles()
        {
            ProcessIsStoped = false;
            foreach (var item in SmartShapes)
                item.StartFollowRulles();
        }

        public ManagerShapes()
        {
            SmartShapes = new List<SmartShape>();
            ProcessIsStoped = true;
        }
    }
}
