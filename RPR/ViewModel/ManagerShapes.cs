using Newtonsoft.Json;
using RPR.Model;
using RPR.Shapes;
using System.IO;

namespace RPR.ViewModel
{
    public class ManagerShapes
    {
       public World World { get; protected set; }

        public void Add(SmartShape shape) =>
            World.SmartShapes.Add(shape);

        public void Remove(SmartShape shape) =>
            World.SmartShapes.Remove(shape);

        public void ShapesFollowTheRules()
        {
            for (int i = 0; i < World.SmartShapes.Count; i++)
            {
                if (World.SmartShapes[i].IsFollowInnerRules == true) continue;

                var args = new ArgsSmartShapes() { Called_Shape = null, Sender = World.SmartShapes[i], Bound_Collision = false, Inner_Collision = false };

                for (int j = i + 1; j < World.SmartShapes.Count; j++)
                {
                    if (World.SmartShapes[i].IsCollisionBounds(World.SmartShapes[j].GetShape()))
                    {
                        args.Called_Shape = World.SmartShapes[j];
                        args.Bound_Collision = true;
                    }
                    if (World.SmartShapes[i].FindCollissionBetweenShapes(World.SmartShapes[j]))
                    {
                        args.Inner_Collision= true;
                    }
                }

                foreach (var act in World.SmartShapes[i].Rules)
                    act.GetInstruction().Invoke(args);
            }
        }

        public void SetWorldName(string WorldName)
        {
            World.MetaData.Name = WorldName;
        }

        /// <summary>
        /// Find file in base directory: 
        ///     */Worlds
        /// </summary>
        /// <returns></returns>
        public bool SerializeFileIsExist(string World_Name)
        {
            try
            {
                FileInfo info = new FileInfo(Directory.GetCurrentDirectory() + "/Worlds/" + World_Name + ".json");
                return info.Exists;
            }
            catch
            {
                return false;
            }
        }

        public void Serialize()
        {
            var serialize = new JsonSerializer();
            var dir = Directory.GetCurrentDirectory() + "/Worlds";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(dir + "/" + World.MetaData.Name + ".json"))
            {
                serialize.Serialize(sw, World);
            }
        }

        public void Serialize(string dir, string file_name)
        {
            var serialize = new JsonSerializer();
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            World.MetaData.Name = file_name;
            using (StreamWriter sw = new StreamWriter(dir + "/" + World.MetaData.Name + ".json"))
            {
                serialize.Serialize(sw, World);
            }
        }

        public void Deserialize(string Name_World)
        {
            var serialize = new JsonSerializer();
            var dir = Directory.GetCurrentDirectory() + "/Worlds";
            if (!Directory.Exists(dir)) return;
            using (StreamReader sr = new StreamReader(dir + "/" + Name_World + ".json"))
            {
                World = serialize.Deserialize<World>(new JsonTextReader(sr));
                World.DeserializeShapes();
            }
        }

        public void Deserialize(string Name_World, string dir)
        {
            var serialize = new JsonSerializer();
            if (!Directory.Exists(dir)) return;
            using (StreamReader sr = new StreamReader(dir + "/" + Name_World + ".json"))
            {
                World = serialize.Deserialize<World>(new JsonTextReader(sr));
                World.DeserializeShapes();
            }
        }

        public ManagerShapes()
        {
            World = new World();
        }

        public ManagerShapes(string? WorldName)
        {
            World = new World();
            SetWorldName(WorldName);

            if (SerializeFileIsExist(WorldName))
            {
                Deserialize(WorldName);
            }
                
        }
    }
}
