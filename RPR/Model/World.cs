using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RPR.Model
{
    public interface IWorld : IRuleable
    {
        public ManagerShapes ManagerShapes { get; }

        public WorldMetaData MetaData { get; set; }

        public Camera Camera { get; set; }
        /// <summary>
        /// In millyseconds
        /// </summary>
        static public int Time { get; }
    }

    public class WorldMetaData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime CreateTime { get; set; }
        public JObject? Data { get; set; }
    }

    public class WorldArgs
    {
        public int CurrentTime { get; init; }
        public JObject? Data { get; init; }
    }

    public class World : IWorld
    {
        public Camera Camera { 
            get; 
            set; 
        }

        public ManagerShapes ManagerShapes { get; set; }

        public WorldMetaData MetaData { get; set; }

        public List<Rule> Rules { get; set; }

        static public int Time { get; set; }

        static public bool TimeStoped { get; set; }

        public bool IsFollowInnerRules { get; set; }

        public bool IsFollowOuterRules { get; set; }

        public World()
        {
            Time = 0;
            TimeStoped = true;
        }

        /// <summary>
        /// Plus one millysecond
        /// </summary>
        protected void TimeStep() =>
            Time += 1;

        async public void TimeLoop()
        {
            while (!TimeStoped)
            {
                TimeStep();
                await Task.Delay(1);
            }
            Time = 0;
        }

        public static WorldArgs GetWorldArgs()=>
            new WorldArgs() { CurrentTime = Time, Data= new JObject() };

        public void InitWorld() 
        {
            Rules = new List<Rule>();
            MetaData = new WorldMetaData() { Name = Guid.NewGuid().ToString() };
            MetaData.CreateTime = DateTime.Now;
            ManagerShapes = new ManagerShapes();
            Camera = new Camera();
        }

        public void SetWorldName(string WorldName)
        {
            MetaData.Name = WorldName;
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
            using (StreamWriter sw = new StreamWriter(dir + "/" + MetaData.Name + ".json"))
            {
                serialize.Serialize(sw, this);
            }
        }

        public void Serialize(string dir, string file_name)
        {
            var serialize = new JsonSerializer();
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            MetaData.Name = file_name;
            using (StreamWriter sw = new StreamWriter(dir + "/" + MetaData.Name + ".json"))
            {
                serialize.Serialize(sw, this);
            }
        }

        static public World? Deserialize(string Name_World)
        {
            var serialize = new JsonSerializer();
            var dir = Directory.GetCurrentDirectory() + "/Worlds";
            if (!Directory.Exists(dir)) return null;
            try
            {
                using (StreamReader sr = new StreamReader(dir + "/" + Name_World + ".json"))
                {
                    var world = serialize.Deserialize<World>(new JsonTextReader(sr));
                    return world;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        static public World? Deserialize(string Name_World, string dir)
        {
            var serialize = new JsonSerializer();
            if (!Directory.Exists(dir)) return null;
            try
            {
                using (StreamReader sr = new StreamReader(dir + "/" + Name_World + ".json"))
                {
                    var world = serialize.Deserialize<World>(new JsonTextReader(sr));
                    return world;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
