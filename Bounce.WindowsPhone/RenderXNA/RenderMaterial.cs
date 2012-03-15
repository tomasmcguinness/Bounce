using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace RenderXNA
{
    public class RenderMaterial
    {
        public Color Color { get; set; }
        public float Depth { get; set; }
        public bool CenterOnBody { get; set; }
        public float Scale { get; set; }
        public Texture2D Texture { get; set; }
        public string TextureName { get; set; }
        public object UserData { get; set; }

        public RenderMaterial(Texture2D texture, string name, float scale)
        {
            Color = Color.White;
            Depth = 0f;
            this.Texture = texture;
            Scale = scale;
            CenterOnBody = true;
            TextureName = name;

            Materials.AddMaterial(name.ToLower(), texture);
        }
        public RenderMaterial(Texture2D texture, string name)
            : this(texture, name, 1f)
        { }

        
    }
}
