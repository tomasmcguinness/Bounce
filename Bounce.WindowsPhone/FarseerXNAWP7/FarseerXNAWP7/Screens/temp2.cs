using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerXNABase.ScreenSystem;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerXNABase;
using Microsoft.Xna.Framework.Graphics;
using RenderXNA;

namespace FarseerXNAWP7.Screens
{
    public class temp2 : PhysicsGameScreen
    {
        public override void LoadContent()
        {
            this.World = new FarseerPhysics.Dynamics.World(-Vector2.UnitX * 10);

            base.LoadContent();

            Fixture f = FixtureFactory.CreateRectangle(World, ConvertUnits.ToSimUnits(470), ConvertUnits.ToSimUnits(790), 10,
                new RenderMaterial(ScreenManager.ContentManager.Load<Texture2D>("Game/BlackLevelEntity/Obstacle/Obs1"), "Game_WoodBox"));
            f.Body.IsStatic = true;
            f.Body.Position = Camera2D.ConvertScreenToWorld(new Vector2(240, 400));
        }
    }
}
