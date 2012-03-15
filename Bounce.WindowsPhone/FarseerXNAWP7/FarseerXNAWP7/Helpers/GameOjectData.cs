using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RenderXNA;

namespace FarseerXNAWP7.Helpers
{
    public class GameObjectData
    {
        public GameObjectData(GameObjectType type)
        {
            this.Type = type;
        }
        public GameObjectType Type { get; set; }

        public static GameObjectType GetType(object obj)
        {
            RenderMaterial material = (RenderMaterial)obj;
            if (material.UserData != null)
                if (material.UserData.GetType() == typeof(GameObjectData))
                    return ((GameObjectData)material.UserData).Type;

            return GameObjectType.NONE;
        }
    }

    public enum GameObjectType
    {
        BALL,
        SOLIDOBJECT,
        SPIKE,
        BOUNDARY_SOLID,
        BOUNDARY_HOLLO,
        DIAMOND,
        LEVELCOMPLEWALL,
        NONE
    }
}
