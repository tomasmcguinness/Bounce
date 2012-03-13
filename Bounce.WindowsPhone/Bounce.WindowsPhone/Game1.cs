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
using FarseerPhysics.DebugViews;
using FarseerPhysics;

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

        DebugViewXNA physicsDebug;

        private Matrix view;
        private Vector2 cameraPosition;
        private Vector2 screenCenter;

        private const float MeterInPixels = 91f;

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
            view = Matrix.Identity;
            cameraPosition = Vector2.Zero;
            screenCenter = new Vector2(graphics.GraphicsDevice.Viewport.Width / 2f, graphics.GraphicsDevice.Viewport.Height / 2f);
            world = new World(new Vector2(0, 0));

            physicsDebug = new DebugViewXNA(world);
            physicsDebug.LoadContent(this.GraphicsDevice, this.Content);
            physicsDebug.AppendFlags(DebugViewFlags.Shape);
            physicsDebug.AppendFlags(DebugViewFlags.PolygonPoints);

            Vector2 bodyPosition = new Vector2(110 / MeterInPixels, 10 / MeterInPixels);
            myBody = BodyFactory.CreateRectangle(world, 1.0f / MeterInPixels, 1.0f / MeterInPixels, 1f, bodyPosition);
            myBody.BodyType = BodyType.Dynamic;
            myBody.Mass = 1.0f;
            myBody.Restitution = 0.3f;
            myBody.Friction = 0.5f;
            //myBody.LinearVelocity = new Vector2(1.0f, 0);

            circleShape = new CircleShape(1.0f, 1.0f);
            fixture = myBody.CreateFixture(circleShape);

            Layer layer = map.GetLayer("HitGround");
            TileArray groundTiles = layer.Tiles;

            for (int x = 0; x < 800; x++)
            {
                for (int y = 0; y < 48; y++)
                {
                    Tile tile = groundTiles[x, y];

                    if (tile != null)
                    {
                        // step 2. create a new box2d object to the box2d world
                        Body bd = BodyFactory.CreateRectangle(world, 16.0f, 16.0f, 1.0f);
                        bd.BodyType = BodyType.Static;
                        bd.Restitution = 1.0f;
                        bd.Mass = 1.0f;
                        bd.Position = new Vector2(x * 16, y * 16);

                        var edgeShape = new CircleShape(16.0f, 1.0f);
                        fixture = bd.CreateFixture(edgeShape);
                    }
                }
            }
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

            // TODO: Add your update logic here
            map.Update(gameTime.ElapsedGameTime.Milliseconds);

            camera.X = (int)(myBody.Position.X * MeterInPixels);
            camera.Y = (int)(myBody.Position.Y * MeterInPixels);

            cameraPosition.X = (int)(myBody.Position.X * MeterInPixels);
            cameraPosition.Y = (int)(myBody.Position.Y * MeterInPixels);

            view = Matrix.CreateTranslation(new Vector3(cameraPosition - screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(screenCenter, 0f));

            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

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

            Vector2 circlePos = myBody.Position * MeterInPixels;

            spriteBatch.Begin();

            //Layer layer = map.GetLayer("HitGround");
            //TileArray groundTiles = layer.Tiles;

            //for (int y = 0; y < 48; y++)
            //{
            //    for (int x = 0; x < 800; x++)
            //    {
            //        Tile tile = groundTiles[x, y];

            //        if (tile != null)
            //        {
            //            spriteBatch.Draw(mSpriteTexture, new Vector2(x * 16, y * 16), null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //        }
            //    }
            //}

            spriteBatch.Draw(mSpriteTexture, circlePos, null, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            spriteBatch.End();

            Matrix proj = Matrix.CreateOrthographicOffCenter(0f, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 0f, 1f);
            physicsDebug.RenderDebugData(ref proj, ref view);

            base.Draw(gameTime);
        }
    }
}
