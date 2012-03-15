using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace RenderXNA
{
    public static class Materials
    {
        private static Dictionary<string, Texture2D> _materials = new Dictionary<string, Texture2D>();
        public static void AddMaterial(string name, Texture2D texture)
        {
            if (!_materials.ContainsKey(name.ToLower()))
                _materials.Add(name.ToLower(), texture);
        }
        public static Texture2D GetMaterial(string name)
        {
            return _materials[name.ToLower()];
        }
    }
}
