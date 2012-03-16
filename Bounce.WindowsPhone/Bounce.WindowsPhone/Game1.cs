using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using xTile;
using xTile.Display;
using xTile.Dimensions;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Factories;
using xTile.Tiles;
using xTile.Layers;
using FarseerPhysics;
using RenderXNA;
using FarseerXNABase.ScreenSystem;
using FarseerXNABase;

namespace Bounce.WindowsPhone
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Map map;
        IDisplayDevice mapDisplayDevice;
        xTile.Dimensions.Rectangle camera;

        Vector2 mPosition = new Vector2(0, 0);
        Texture2D mSpriteTexture;

        World world;
        Body myBody;
        CircleShape circleShape;
        Fixture fixture;

        private Matrix view;
        private Vector2 cameraPosition;
        private Vector2 screenCenter;

        private const float MeterInPixels = 91f;

        RenderXNAHelper RenderHelper;
        Camera2D Camera;
        Body BoxBody, FloorBody;
        Texture2D MyTexture;

        List<Body> MapBodies = new List<Body>();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 480;
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromTicks(333333);

            // Extend battery life under lock.
            InactiveSleepTime = TimeSpan.FromSeconds(1);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            mapDisplayDevice = new XnaDisplayDevice(this.Content, this.GraphicsDevice);
            map.LoadTileSheets(mapDisplayDevice);
            camera = new xTile.Dimensions.Rectangle(new Size(820, 480));

            //cameraPosition = new Vector2(0, 0);
            //screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            //view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            //physicsDebug = new DebugViewXNA(world);
            //physicsDebug.LoadContent(this.GraphicsDevice, this.Content);
            //physicsDebug.AppendFlags(DebugViewFlags.Shape);
            //physicsDebug.AppendFlags(DebugViewFlags.PolygonPoints);

            //Vector2 bodyPosition = new Vector2(260, 220);
            //myBody = BodyFactory.CreateCircle(world, 16.0f, 1f, bodyPosition);
            //myBody.BodyType = BodyType.Dynamic;
            //myBody.Mass = 1.0f;
            //myBody.Restitution = 100f;
            //myBody.Friction = 0.5f;
            //myBody.LinearVelocity = new Vector2(0, 0);

            //Layer layer = map.GetLayer("HitGround");
            //TileArray groundTiles = layer.Tiles;

            //for (int x = 0; x < 800; x++)
            //{
            //    for (int y = 0; y < 48; y++)
            //    {
            //        Tile tile = groundTiles[x, y];

            //        if (tile != null)
            //        {
            //            Body bd = BodyFactory.CreateRectangle(world, 16.0f, 16.0f, 1.0f);
            //            bd.BodyType = BodyType.Static;
            //            bd.Restitution = 1.0f;
            //            bd.Mass = 1.0f;
            //            bd.Position = new Vector2((x * 16) + 7, (y * 16) + 7);
            //        }
            //    }
            //}
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            map = Content.Load<Map>("Maps\\Map01");

            mSpriteTexture = Content.Load<Texture2D>("golfball");

            MyTexture = Content.Load<Texture2D>("blank");

            world = new World(new Vector2(0, 15.0f));

            RenderHelper = new RenderXNAHelper(world);
            RenderHelper.AppendFlags(DebugViewFlags.TexturedShape);
            RenderHelper.RemoveFlags(DebugViewFlags.Shape);

            RenderHelper.DefaultShapeColor = Color.White;
            RenderHelper.SleepingShapeColor = Color.LightGray;
            RenderHelper.LoadContent(GraphicsDevice, Content);

            BoxBody = BodyFactory.CreateBody(world);
            FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(50), ConvertUnits.ToSimUnits(50), 10, Vector2.Zero, BoxBody);
            foreach (Fixture fixture in BoxBody.FixtureList)
            {
                fixture.Restitution = 1.0f;
                fixture.Friction = 0.5f;
            }
            BoxBody.BodyType = BodyType.Dynamic;
            BoxBody.Position = ConvertUnits.ToSimUnits(new Vector2(400, 25));

            //Create Floor
            //Fixture floorFixture = FixtureFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(800), ConvertUnits.ToSimUnits(10), 10);
            //floorFixture.Restitution = 0.5f;        //Bounceability
            //floorFixture.Friction = 0.5f;           //Friction
            //FloorBody = floorFixture.Body;          //Get Body from Fixture
            //FloorBody.IsStatic = true;

            //FloorBody.Position = ConvertUnits.ToSimUnits(new Vector2(0, 400));

            Layer layer = map.GetLayer("HitGround");
            TileArray groundTiles = layer.Tiles;

            for (int x = 0; x < 800; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    Tile tile = groundTiles[x, y];

                    if (tile != null)
                    {
                        Fixture smallBox = FixtureFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(16), ConvertUnits.ToSimUnits(16), 10);
                        smallBox.Restitution = 0.5f;        //Bounceability
                        smallBox.Friction = 0.5f;           //Friction
                        var smallBoxBody = smallBox.Body;          //Get Body from Fixture
                        smallBoxBody.IsStatic = true;
                        smallBoxBody.Position = ConvertUnits.ToSimUnits(new Vector2(x * 16, y * 16));

                        MapBodies.Add(smallBoxBody);
                    }
                }
            }

            Camera = new Camera2D(GraphicsDevice);
            Camera.TrackingBody = BoxBody;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //map.Update(gameTime.ElapsedGameTime.Milliseconds);

            //var newPosition = myBody.Position - screenCenter;

            //camera.X = (int)newPosition.X;
            //camera.Y = (int)newPosition.Y;

            //cameraPosition.X = myBody.Position.X / 2f;
            //cameraPosition.Y = myBody.Position.Y / 2f;

            ////view = Matrix.CreateTranslation(new Vector3(screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));
            //view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            // Update the camera

            world.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
            RenderHelper.Update(gameTime);
            Camera.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            map.Draw(mapDisplayDevice, camera);

            spriteBatch.Begin();
            spriteBatch.Draw(MyTexture, new Microsoft.Xna.Framework.Rectangle((int)(ConvertUnits.ToDisplayUnits(BoxBody.Position.X) - 25), (int)(ConvertUnits.ToDisplayUnits(BoxBody.Position.Y) - 25), 50, 50), Color.Green);
            //spriteBatch.Draw(MyTexture, new Microsoft.Xna.Framework.Rectangle((int)ConvertUnits.ToDisplayUnits(FloorBody.Position).X - 240, (int)ConvertUnits.ToDisplayUnits(FloorBody.Position).Y - 5, 480, 10), null, Color.Gray, FloorBody.Rotation, new Vector2(0, 0), SpriteEffects.None, 0);

            foreach (var body in MapBodies)
            {
                spriteBatch.Draw(MyTexture, new Microsoft.Xna.Framework.Rectangle((int)ConvertUnits.ToDisplayUnits(body.Position).X, (int)ConvertUnits.ToDisplayUnits(body.Position).Y, 16, 16), null, Color.Gray, body.Rotation, new Vector2(0, 0), SpriteEffects.None, 0);
            }

            spriteBatch.End();

            RenderHelper.RenderDebugData(ref Camera2D.Projection, ref Camera2D.View);

            base.Draw(gameTime);
        }
    }
}
