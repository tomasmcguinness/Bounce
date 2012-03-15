using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using RenderXNA;

namespace FarseerXNAWP7.Helpers
{
    public class EntityData
    {
        public EntityType Type { get; set; }

        public static EntityType GetEntityType(object obj)
        {
            RenderMaterial material = (RenderMaterial)obj;
            if (material.UserData != null)
                if (material.UserData.GetType() == typeof(Entity))
                    return ((Entity)material.UserData).Type;

            return EntityType.None;
        }
    }
    public class Entity
    {
        [XmlElement("Type")]
        public EntityType Type { get; set; }

        [XmlElement("X")]
        public int X { get; set; }

        [XmlElement("Y")]
        public int Y { get; set; }

        [XmlElement("Height")]
        public int Height { get; set; }

        [XmlElement("Width")]
        public int Width { get; set; }
    }

    public class Level
    {
        public string LevelName { get; set; }
        public string Description { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public List<string> Backgrounds { get; set; }
        public List<Entity> LevelEntities { get; set; }

        [XmlIgnore]
        public string UniqueID = Guid.NewGuid().ToString().Substring(0, 10);
    }


    public enum EntityType
    {
        None,
        Obstacle1,
        Obstacle2,
        Obstacle3,
        Obstacle4,
        Obstacle5,
        Obstacle6,
        SpikeLeft,
        SpikeRight,
        SpikeUp,
        SpikeDown,
        FloorLeft,
        FloorRight,
        FloorMiddle,
        Ball,
        WoodDiamond
    }
}
