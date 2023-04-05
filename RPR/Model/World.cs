using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPR.Shapes;
using RPR.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Shapes;

namespace RPR.Model
{
    public interface IWorld : IRuleable
    {
        public List<SmartShape> SmartShapes { get; set; }

        public WorldArgs MetaData { get; set; }
    }

    public class WorldArgs
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// In milliseconds
        /// </summary>
        public DateTime Time { get; set; }
        public JObject? Data { get; set; }
    }

    public class World : IWorld
    {
        public List<SmartShape> SmartShapes { get; set; }

        public WorldArgs MetaData { get; set; }

        public List<Rule> Rules { get; set; }

        public bool IsFollowInnerRules { get; set; }

        public bool IsFollowOuterRules { get; set; }

        public void DeserializeShapes()
        {
            foreach(var item in SmartShapes)
            {
                item.Deserialize();
            }
        }

        public World()
        {
            Rules = new List<Rule>();
            MetaData = new WorldArgs() { Name = Guid.NewGuid().ToString() };
            MetaData.Time = DateTime.Now;
            SmartShapes = new List<SmartShape>();
        }
    }
}
