using System;
using System.Collections.Generic;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using RenderXNA;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerXNABase.Components
{
    /// <summary>
    /// Creates Border on screen edges for Physics Screen. Mostly used in testing purpose.
    /// </summary>
    public class Border
    {
        private Body _anchor;
        private World _world;
        private Texture2D _texture;
        private string _textureName;
        private Color _color;

        public Border(World world, float width, float height, float borderWidth, Texture2D texture, string textureName, Color color)
        {
            _world = world;
            _texture = texture;
            _textureName = textureName;
            _color = color;

            CreateBorder(width, height, borderWidth);
        }

        private void CreateBorder(float width, float height, float borderWidth)
        {
            width = Math.Abs(width);
            height = Math.Abs(height);

            _anchor = new Body(_world);
            List<Vertices> borders = new List<Vertices>(4);

            //Bottom
            borders.Add(PolygonTools.CreateRectangle(width, borderWidth, new Vector2(0f, height - borderWidth), 0));

            //Left
            borders.Add(PolygonTools.CreateRectangle(borderWidth, height, new Vector2(-width + borderWidth, 0), 0));

            //Top
            borders.Add(PolygonTools.CreateRectangle(width, borderWidth, new Vector2(0, -height + borderWidth), 0));

            //Right
            borders.Add(PolygonTools.CreateRectangle(borderWidth, height, new Vector2(width - borderWidth, 0), 0));

            RenderMaterial material = new RenderMaterial(_texture, _textureName)
            {
                Color = _color,
                Scale = 8f
            };
            List<Fixture> fixtures = FixtureFactory.CreateCompoundPolygon(borders, 1, _anchor, material);

            foreach (Fixture t in fixtures)
            {
                t.CollisionFilter.CollisionCategories = Category.All;
                t.CollisionFilter.CollidesWith = Category.All;
            }
        }

        public void ResetBorder(float width, float height, float borderWidth)
        {
            _world.RemoveBody(_anchor);
            _world.ProcessChanges();

            CreateBorder(width, height, borderWidth);
        }
    }
}
