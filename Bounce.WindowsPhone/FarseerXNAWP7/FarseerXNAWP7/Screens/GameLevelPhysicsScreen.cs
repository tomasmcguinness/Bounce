
namespace FarseerXNAWP7.Screens
{
    using System.Collections.Generic;
    using FarseerPhysics.Collision.Shapes;
    using FarseerPhysics.Common;
    using FarseerPhysics.Dynamics;
    using FarseerPhysics.Dynamics.Contacts;
    using FarseerPhysics.Factories;
    using FarseerXNABase;
    using FarseerXNABase.Components;
    using FarseerXNABase.ScreenSystem;
    using FarseerXNAWP7.Helpers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using RenderXNA;
    using FarseerXNABase.Controls;

    public class GameLevelPhysicsScreen : PhysicsGameScreen
    {
        #region Properties & Variables

        private ParallaxBackground _background, _clouds;
        private ScoreBoard _scoreBoard;
        private static Dictionary<string, Texture2D> _textures;

        private Body _ballBody, _spikeStripBody, _fontWall;
        private Entity _ballEntity;
        private Vector2 _jumpForce = Vector2.Zero;
        private Level CurrentLevel;
        private int _xVelocity;
        private const float _jumpImpulse = -26000;
        private float _explotionAnimationFrame = 0;
        private bool _ballExploded = false, _LevelCompleted = false;

        public delegate void NextLevel(int CurrentLevelIndex);
        public NextLevel OnNextLevel;

        private Vector2 _cameraPosition;
        public Vector2 CameraPosition
        {
            get { return ScreenManager.Camera.Position; }
            private set { _cameraPosition = value; }
        }

        private Vector2 _ballPosition;
        public Vector2 BalllPosition
        {
            get { return _ballBody.Position; }
            private set { _ballPosition = value; }
        }

        private int _currentLevelIndex;
        public int CurrentLevelIndex
        {
            get { return _currentLevelIndex; }
            set { _currentLevelIndex = value; }
        }

        private int _diamondsCollected;
        public int DiamondsCollected
        {
            get { return _scoreBoard.Diamonds; }
            set { _diamondsCollected = value; }
        }

        private int _distanceTravelled;
        public int DistanceTravelled
        {
            get { return _scoreBoard.Distance; }
            set { _distanceTravelled = value; }
        }

        private Vector2 _backgroundPosition;
        public Vector2 BackgroundPosition
        {
            get { return _background.Position; }
            set { _backgroundPosition = value; }
        }
        #endregion

        #region Initialization and Load

        public GameLevelPhysicsScreen(Level Lvl, int LevelIndex)
        {
            this.CurrentLevel = Lvl;
            this._currentLevelIndex = LevelIndex;

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
        }
        public GameLevelPhysicsScreen(Level Lvl, int LevelIndex, Vector2 CameraPos, Vector2 BallPos,
            int DistanceTravelled, int DiamondsCollected, Vector2 BackgroundPosition)
            : this(Lvl, LevelIndex)
        {
            this._cameraPosition = CameraPos;
            this._ballPosition = BallPos;
            this._distanceTravelled = DistanceTravelled;
            this._diamondsCollected = DiamondsCollected;
            this._backgroundPosition = BackgroundPosition;
        }

        public override void LoadContent()
        {
            ResetWorld();

            _textures = new Dictionary<string, Texture2D>();
            GetTexture(Assets.EXPLOSION2);

            //Set Maximum size of camera
            ScreenManager.Camera.MaxPosition = new Vector2(this.CurrentLevel.Width, this.CurrentLevel.Height);

            CreateEntities();

            //Set camera position and ball if we are rising from tomb.
            if (this._cameraPosition != Vector2.Zero && this._ballPosition != Vector2.Zero)
            {
                ScreenManager.Camera.Position = _cameraPosition;
                _ballBody.Position = _ballPosition;
                _scoreBoard.Score = _diamondsCollected * Assets.DIAMONDPOINTS + _distanceTravelled;
                _scoreBoard.Diamonds = _diamondsCollected;

                //So that we don't load the same screen again and again when game over happens
                this._ballPosition = this._cameraPosition = Vector2.Zero;
            }
        }

        private void ResetWorld()
        {
            World = new World(-Vector2.UnitY * 65);
            base.LoadContent();
        }

        private void CreateEntities()
        {
            //Create Moving Background
            CreateParallaxBG();

            //Create ScoreBoard
            CreateScoreBoard();

            //Plot Level Data
            PlottLevelData(this.CurrentLevel);

            //Create Left Spike Strip
            CreateLeftSpikeStrip();

            //Create front wall
            CreateFrontWall();
        }

        private void CreateParallaxBG()
        {
            //Background
            List<Texture2D> lst = new List<Texture2D>();
            foreach (string item in this.CurrentLevel.Backgrounds)
            {
                lst.Add(ScreenManager.ContentManager.Load<Texture2D>(item));
            }

            _background = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width,
                Position = this._backgroundPosition
            };

            //Moving Clouds
            lst = new List<Texture2D>();
            lst.Add(ScreenManager.ContentManager.Load<Texture2D>(Assets.BG_CLOUDS));
            _clouds = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width
            };
        }

        private void CreateScoreBoard()
        {
            _scoreBoard = new ScoreBoard(Assets.DIAMONDPOINTS)
            {
                Height = 56,
                Width = 100,
                Texture = GetTexture(Assets.SCORE_BOARD),
                Font = ScreenManager.SpriteFonts.GameSpriteFont,
                Position = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - 105, 5)
            };
        }

        private void CreateFrontWall()
        {
            _fontWall = BodyFactory.CreateBody(World);
            _fontWall.SleepingAllowed = true;
            FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(1),
                ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height * 2),
                Assets.DENSITY, Vector2.Zero, _fontWall,
                                            new RenderMaterial(GetTexture(Assets.BLANK), Assets.BLANK)
                                            {
                                                Color = Color.White,
                                                UserData = new GameObjectData(GameObjectType.NONE)
                                            });
            _fontWall.IsStatic = true;
            _fontWall.Position = Camera2D.ConvertScreenToWorld(
                new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height));

            foreach (Fixture item in _fontWall.FixtureList)
            {
                item.Friction = 0;
                item.Restitution = 0;
            }
        }

        private void CreateLeftSpikeStrip()
        {
            _spikeStripBody = BodyFactory.CreateBody(World);
            _spikeStripBody.SleepingAllowed = true;
            FixtureFactory.CreateRectangle(Assets.CELLSIZE_FARSEER * 1.2f,
                                            ConvertUnits.ToSimUnits(ScreenManager.GraphicsDevice.Viewport.Height * 2),
                                            Assets.DENSITY, new Vector2(0, 0), _spikeStripBody,
                                            new RenderMaterial(GetTexture(Assets.SPIKE_RIGHT_WOOD), Assets.SPIKE_RIGHT_WOOD, Assets.CELLSIZE_FARSEER * 1.3f)
                                            {
                                                UserData = new GameObjectData(GameObjectType.SPIKE),
                                                CenterOnBody = true
                                            });

            _spikeStripBody.IsStatic = true;
            _spikeStripBody.Position = Camera2D.ConvertScreenToWorld(new Vector2(Assets.CELLSIZE_FARSEER / 2, ScreenManager.GraphicsDevice.Viewport.Height));
        }

        #endregion

        #region Game Methods

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Explosion movement
            if (_ballExploded)
            {
                _explotionAnimationFrame += 0.25f;

                //If Explosion animation over then show GameOverScreen
                if (_explotionAnimationFrame >= 16)
                {
                    _explotionAnimationFrame = 16.1f;
                    _ballExploded = false;

                    LevelOverScreen obj = new LevelOverScreen(_scoreBoard.Distance, _scoreBoard.Diamonds, true, CurrentLevel.LevelName);
                    obj.ShowMenuScreen += (_, __) =>
                        {
                            _ballExploded = false;
                            _explotionAnimationFrame = 0;
                            this.ExitScreen();
                        };
                    obj.RestartLevel += (_, __) =>
                        {
                            _explotionAnimationFrame = 0;
                            _ballExploded = false;
                            ScreenManager.Camera.Position = Vector2.Zero;
                            LoadContent();
                        };
                    ScreenManager.AddScreen(obj, null);
                }
            }
            else
            {
                //Evey update increment the score
                if (this.IsActive)
                {
                    _scoreBoard.Update(gameTime);

                    //Move camera to force player to move
                    ScreenManager.Camera.MoveCamera(Vector2.UnitX / 6);

                    //Move the background
                    _background.Move(ConvertUnits.ToDisplayUnits(Vector2.UnitX / 6));

                    //Move background clouds
                    _clouds.Move(ConvertUnits.ToDisplayUnits(Vector2.UnitX / 4));
                }
            }

            //If game over screen or any other screen is on top of current screen.
            if (this.IsActive)
            {
                //Move Both the front wall and left Spike strip according to camera position
                _spikeStripBody.Position = Camera2D.ConvertScreenToWorld(
                    new Vector2(Assets.CELLSIZE_FARSEER / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2));
                _fontWall.Position = Camera2D.ConvertScreenToWorld(
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height / 2));

                //Rotate the ball to make it look more dynamic.
                _ballBody.AngularVelocity = -2;
            }

            //Base Update
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            //Draw Background
            _background.Draw(ScreenManager.SpriteBatch);
            _clouds.Draw(ScreenManager.SpriteBatch);

            //If Ball exploded then animate explosion
            if (_ballExploded && _explotionAnimationFrame <= 16)
            {
                ScreenManager.SpriteBatch.Draw(GetTexture(Assets.EXPLOSION2),
                        new Rectangle((int)Camera2D.ConvertWorldToScreen(_ballBody.Position).X - 32,
                                      (int)Camera2D.ConvertWorldToScreen(_ballBody.Position).Y - 32, 64, 64),
                        new Rectangle((int)((int)_explotionAnimationFrame % 4) * 128,
                                      (int)((int)_explotionAnimationFrame / 4) * 128, 128, 128), Color.White);
            }
            else
            {
                if (this.IsActive)
                    _scoreBoard.Draw(ScreenManager.SpriteBatch);
            }

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public override void HandleInput(InputHelper input)
        {
            _xVelocity = 0;

            if (input.CurrentMouseState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if (input.CurrentMouseState.X >= 400)
                {
                    _xVelocity = 20;
                }
                else
                {
                    _xVelocity = -20;
                }
            }

            if (input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
            {
                _xVelocity = 20;
            }
            else if (input.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
            {
                _xVelocity = -20;
            }
            _ballBody.LinearVelocity = new Vector2(_xVelocity, _ballBody.LinearVelocity.Y);

            base.HandleInput(input);
        }

        #endregion

        #region Level Data Plotter

        private void PlottLevelData(Level lvl)
        {
            int TotalColumns = lvl.Width / Assets.CELLSIZE;
            int TotalRows = lvl.Height / Assets.CELLSIZE;

            //Create Cell Matrix
            int[,] matrix = new int[TotalRows, TotalColumns];
            for (int i = 0; i < TotalRows; i++)
                for (int j = 0; j < TotalColumns; j++)
                    matrix[i, j] = 0;

            //Create Physics Object from Entity
            _ballEntity = null;
            bool CreateRectangle = false;
            string TextureForRectangle = string.Empty;
            GameObjectData additionData;
            Body body;
            Vertices vertices;

            foreach (Entity item in lvl.LevelEntities)
            {
                CreateRectangle = false;
                TextureForRectangle = string.Empty;
                additionData = new GameObjectData(GameObjectType.NONE);

                body = BodyFactory.CreateBody(World);
                body.SleepingAllowed = true;

                switch (item.Type)
                {
                    case EntityType.Obstacle1:
                    case EntityType.Obstacle2:
                    case EntityType.Obstacle3:
                    case EntityType.Obstacle4:
                    case EntityType.Obstacle5:
                    case EntityType.Obstacle6:
                        TextureForRectangle = Assets.Obstacle(item.Type);
                        CreateRectangle = true;
                        additionData.Type = GameObjectType.NONE;
                        break;
                    case EntityType.SpikeRight:
                        vertices = CreateRightSpike(matrix, body, item);
                        break;
                    case EntityType.SpikeLeft:
                        vertices = CreateLeftSpike(matrix, body, item);
                        break;
                    case EntityType.SpikeUp:
                        vertices = CreateUpSpike(matrix, body, item);
                        break;
                    case EntityType.SpikeDown:
                        vertices = CreateDownSpike(matrix, body, item);
                        break;
                    case EntityType.FloorLeft:
                    case EntityType.FloorMiddle:
                    case EntityType.FloorRight:
                        CreateFloor(body, item);

                        //Fill Matrix Position
                        matrix[item.Y, item.X] = 1;
                        matrix[item.Y + 1, item.X] = 1;
                        break;
                    case EntityType.Ball:
                        _ballEntity = item;

                        //Fill Matrix Position
                        matrix[item.Y, item.X] = 1;
                        matrix[item.Y + 1, item.X] = 1;
                        matrix[item.Y, item.X + 1] = 1;
                        matrix[item.Y + 1, item.X + 1] = 1;
                        break;
                    case EntityType.WoodDiamond:
                        CreateDiamond(body, item);

                        //Fill Matrix Position
                        matrix[item.Y, item.X] = 1;
                        matrix[item.Y + 1, item.X] = 1;
                        matrix[item.Y, item.X + 1] = 1;
                        matrix[item.Y + 1, item.X + 1] = 1;
                        break;
                }


                if (CreateRectangle)
                {
                    if (item.Type == EntityType.Obstacle1 || item.Type == EntityType.Obstacle2 || item.Type == EntityType.Obstacle3 ||
                        item.Type == EntityType.Obstacle4 || item.Type == EntityType.Obstacle5 || item.Type == EntityType.Obstacle6)
                    {
                        FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(Assets.CELLSIZE - 2), 0.1f, Assets.DENSITY, new Vector2(0, 1.7f),
                            body, new RenderMaterial(GetTexture(TextureForRectangle), TextureForRectangle, Assets.CELLSIZE_FARSEER)
                            {
                                Color = Color.White,
                                UserData = new GameObjectData(GameObjectType.SOLIDOBJECT)
                            });
                    }

                    FixtureFactory.CreateRectangle(Assets.CELLSIZE_FARSEER, Assets.CELLSIZE_FARSEER,
                                                        Assets.DENSITY, new Vector2(0, 0), body,
                                                        new RenderMaterial(GetTexture(TextureForRectangle), TextureForRectangle,
                                                                            Assets.CELLSIZE_FARSEER)
                                                        {
                                                            UserData = additionData
                                                        });
                    body.IsStatic = true;
                    body.Position = Camera2D.ConvertScreenToWorld(new Vector2(item.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, item.Y * Assets.CELLSIZE + Assets.CELLSIZE / 2));

                    //Fill Matrix Position
                    matrix[item.Y, item.X] = 1;
                }


                if (body != null)
                {
                    foreach (Fixture fixture in body.FixtureList)
                    {
                        fixture.Restitution = Assets.RESTITUTION;
                        fixture.Friction = Assets.FRICTION;
                    }
                }

            }

            //Create Ball.
            CreateBall();

            //Create Solid Floor where no other object placed.
            CreateSolidFloor(TotalColumns, TotalRows, matrix);

            //Cerate Level Completion wall.
            CreateLevelCompletedWall(TotalColumns, TotalRows);

        }

        private void CreateDiamond(Body body, Entity item)
        {
            //Texture Height 33 and Width 45
            FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(45), ConvertUnits.ToSimUnits(33), Assets.DENSITY,
                                new Vector2(0, 0), body,
                                new RenderMaterial(GetTexture(Assets.WOOD_DIAMOND), Assets.WOOD_DIAMOND, Assets.CELLSIZE_FARSEER * 2)
                                {
                                    Color = Color.White,
                                    UserData = new GameObjectData(GameObjectType.DIAMOND)
                                });
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(new Vector2(item.X * Assets.CELLSIZE + item.Width / 2, item.Y * Assets.CELLSIZE + item.Height / 2));
        }

        private Vertices CreateDownSpike(int[,] matrix, Body body, Entity item)
        {
            Vertices vertices;
            vertices = new Vertices(3);
            vertices.Add(new Vector2(-0.5f, 1.0f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.0f, 0.0f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.5f, 1.0f) * Assets.CELLSIZE_FARSEER);

            body.CreateFixture(new PolygonShape(vertices, Assets.DENSITY),
                new RenderMaterial(GetTexture(Assets.SPIKE_DOWN_WOOD), Assets.SPIKE_DOWN_WOOD, Assets.CELLSIZE_FARSEER * 2)
                {
                    UserData = new GameObjectData(GameObjectType.SPIKE)
                });
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(
                new Vector2(item.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, item.Y * Assets.CELLSIZE + Assets.CELLSIZE));

            //Fill Matrix Position
            matrix[item.Y, item.X] = 1;
            return vertices;
        }

        private Vertices CreateUpSpike(int[,] matrix, Body body, Entity item)
        {
            Vertices vertices;
            vertices = new Vertices(3);
            vertices.Add(new Vector2(-0.5f, 0.0f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.5f, 0.0f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.0f, 1.0f) * Assets.CELLSIZE_FARSEER);

            body.CreateFixture(new PolygonShape(vertices, Assets.DENSITY),
                new RenderMaterial(GetTexture(Assets.SPIKE_UP_WOOD), Assets.SPIKE_UP_WOOD, Assets.CELLSIZE_FARSEER * 2)
                {
                    UserData = new GameObjectData(GameObjectType.SPIKE)
                });
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(
                new Vector2(item.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, item.Y * Assets.CELLSIZE + Assets.CELLSIZE));

            //Fill Matrix Position
            matrix[item.Y, item.X] = 1;
            return vertices;
        }

        private Vertices CreateLeftSpike(int[,] matrix, Body body, Entity item)
        {
            Vertices vertices;
            vertices = new Vertices(3);
            vertices.Add(new Vector2(-0.5f, 0.0f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.5f, -0.5f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.5f, 0.5f) * Assets.CELLSIZE_FARSEER);

            body.CreateFixture(new PolygonShape(vertices, Assets.DENSITY),
                new RenderMaterial(GetTexture(Assets.SPIKE_LEFT_WOOD), Assets.SPIKE_LEFT_WOOD, Assets.CELLSIZE_FARSEER * 2)
                {
                    UserData = new GameObjectData(GameObjectType.SPIKE)
                });
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(
                new Vector2(item.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, item.Y * Assets.CELLSIZE + Assets.CELLSIZE / 2));

            //Fill Matrix Position
            matrix[item.Y, item.X] = 1;
            return vertices;
        }

        private Vertices CreateRightSpike(int[,] matrix, Body body, Entity item)
        {
            Vertices vertices;
            vertices = new Vertices(3);
            vertices.Add(new Vector2(-0.5f, 0.5f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(-0.5f, -0.5f) * Assets.CELLSIZE_FARSEER);
            vertices.Add(new Vector2(0.5f, 0.0f) * Assets.CELLSIZE_FARSEER);

            body.CreateFixture(new PolygonShape(vertices, Assets.DENSITY),
                new RenderMaterial(GetTexture(Assets.SPIKE_RIGHT_WOOD), Assets.SPIKE_RIGHT_WOOD, Assets.CELLSIZE_FARSEER * 2)
                {
                    UserData = new GameObjectData(GameObjectType.SPIKE)
                });
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(
                new Vector2(item.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, item.Y * Assets.CELLSIZE + Assets.CELLSIZE / 2));

            //Fill Matrix Position
            matrix[item.Y, item.X] = 1;
            return vertices;
        }

        private void CreateLevelCompletedWall(int TotalColumns, int TotalRows)
        {
            Body LevelCompletedBody = BodyFactory.CreateBody(World);
            LevelCompletedBody.SleepingAllowed = true;
            FixtureFactory.CreateRectangle(Assets.CELLSIZE_FARSEER, Assets.CELLSIZE_FARSEER * TotalRows,
                                                Assets.DENSITY, new Vector2(0, 0), LevelCompletedBody,
                                                new RenderMaterial(GetTexture(Assets.BLANK), Assets.BLANK, Assets.CELLSIZE_FARSEER)
                                                {
                                                    Color = new Color(0, 0, 0, 0),
                                                    UserData = new GameObjectData(GameObjectType.LEVELCOMPLEWALL)
                                                });
            LevelCompletedBody.IsStatic = true;
            LevelCompletedBody.Position = Camera2D.ConvertScreenToWorld(new Vector2(TotalColumns * Assets.CELLSIZE + Assets.CELLSIZE / 2, TotalRows * Assets.CELLSIZE / 2));
        }

        private void CreateSolidFloor(int TotalColumns, int TotalRows, int[,] matrix)
        {
            for (int j = 0; j < TotalColumns; j++)
            {
                if (matrix[TotalRows - 1, j] == 0)
                {
                    Body floorBody = BodyFactory.CreateBody(World);
                    floorBody.SleepingAllowed = true;
                    FixtureFactory.CreateRectangle(Assets.CELLSIZE_FARSEER, Assets.CELLSIZE_FARSEER,
                                                        Assets.DENSITY, new Vector2(0, 0), floorBody,
                                                        new RenderMaterial(GetTexture(Assets.BLANK), Assets.BLANK, Assets.CELLSIZE_FARSEER)
                                                        {
                                                            Color = new Color(0, 0, 0, 0),
                                                            UserData = new GameObjectData(GameObjectType.SOLIDOBJECT)
                                                        });
                    floorBody.IsStatic = true;
                    floorBody.Position = Camera2D.ConvertScreenToWorld(new Vector2(j * Assets.CELLSIZE + Assets.CELLSIZE / 2, (TotalRows - 1) * Assets.CELLSIZE + Assets.CELLSIZE / 2));

                    foreach (Fixture fixture in floorBody.FixtureList)
                    {
                        fixture.Restitution = Assets.RESTITUTION;
                        fixture.Friction = Assets.FRICTION;
                    }
                }
            }
        }

        private void CreateBall()
        {
            _ballBody = BodyFactory.CreateBody(World);
            _ballBody.SleepingAllowed = true;
            //Create Ball so that it appears over every other thing
            if (_ballEntity != null)
            {
                //Radius 25 Because we have texture of same radius
                Fixture f = FixtureFactory.CreateCircle(ConvertUnits.ToSimUnits(25),
                                                Assets.DENSITY, _ballBody,
                                                new RenderMaterial(GetTexture(Assets.WOODBALL), Assets.WOODBALL,
                                                                    Assets.CELLSIZE_FARSEER * 2)
                                                {
                                                    UserData = new GameObjectData(GameObjectType.BALL)
                                                });
                _ballBody.IsStatic = false;
                _ballBody.BodyType = BodyType.Dynamic;
                _ballBody.Position = Camera2D.ConvertScreenToWorld(new Vector2(_ballEntity.X * Assets.CELLSIZE + Assets.CELLSIZE / 2, _ballEntity.Y * Assets.CELLSIZE + Assets.CELLSIZE / 2));
                f.Restitution = Assets.RESTITUTION;
                f.Friction = Assets.FRICTION;
                f.OnCollision += OnCollision;
                f.BeforeCollision += BeforeCollision;
            }
        }

        private void CreateFloor(Body body, Entity item)
        {
            string Asset = string.Empty;
            if (item.Type == EntityType.FloorLeft) Asset = Assets.FLOORLEFT;
            if (item.Type == EntityType.FloorMiddle) Asset = Assets.FLOORMIDDLE;
            if (item.Type == EntityType.FloorRight) Asset = Assets.FLOORRIGHT;

            FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(item.Width), ConvertUnits.ToSimUnits(item.Height), Assets.DENSITY,
                                            new Vector2(0, 0), body,
                                            new RenderMaterial(GetTexture(Asset), Asset, Assets.CELLSIZE_FARSEER * 2.1f)
                                            {
                                                Color = Color.White,
                                                UserData = new GameObjectData(GameObjectType.NONE)
                                            });
            foreach (Fixture fixture in body.FixtureList)
            {
                fixture.CollisionFilter.CollidesWith = Category.None;
                fixture.CollisionFilter.CollisionCategories = Category.None;
            }
            body.IsStatic = true;
            body.Position = Camera2D.ConvertScreenToWorld(new Vector2(item.X * Assets.CELLSIZE + item.Width / 2, item.Y * Assets.CELLSIZE + item.Height / 2));


            //Create Body just below the Hollo Floor to find when the ball fall into it.
            if (item.Type == EntityType.FloorMiddle)
            {
                Body fakebody = BodyFactory.CreateBody(World);
                fakebody.SleepingAllowed = true;
                FixtureFactory.CreateRectangle(ConvertUnits.ToSimUnits(Assets.CELLSIZE), 0.1f, Assets.DENSITY, new Vector2(0, 0),
                                fakebody, new RenderMaterial(GetTexture(Asset), Asset, Assets.CELLSIZE_FARSEER)
                                {
                                    Color = Color.White,
                                    UserData = new GameObjectData(GameObjectType.BOUNDARY_HOLLO)
                                });
                fakebody.IsStatic = true;
                fakebody.Position = Camera2D.ConvertScreenToWorld(new Vector2(item.X * Assets.CELLSIZE + item.Width,
                                                                                item.Y * Assets.CELLSIZE + item.Height));
            }
        }

        #endregion

        #region Collision

        private bool BeforeCollision(Fixture fixtureA, Fixture fixtureB)
        {
            //Collision With Diamonds
            if (GameObjectData.GetType(fixtureB.UserData) == GameObjectType.DIAMOND)
            {
                //Remove Diamond from screen and add score
                World.RemoveBody(fixtureB.Body);
                _scoreBoard.CollectedDiamond();
            }

            return true;
        }

        private bool OnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            //Collision With bouncable objects
            if (GameObjectData.GetType(fixtureB.UserData) == GameObjectType.BOUNDARY_SOLID ||
                GameObjectData.GetType(fixtureB.UserData) == GameObjectType.SOLIDOBJECT)
            {
                if (_ballBody.LinearVelocity.Y <= 0)
                {
                    _jumpForce.Y = _jumpImpulse;
                    _ballBody.LinearVelocity = Vector2.Zero;
                    _ballBody.ApplyLinearImpulse(_jumpForce, _ballBody.Position);
                }
            }


            //Collision with Spikes
            if (GameObjectData.GetType(fixtureB.UserData) == GameObjectType.SPIKE)
            {
                if (World.BodyList.Contains(fixtureB.Body) && !_ballExploded)
                    World.RemoveBody(fixtureA.Body);
                _ballExploded = true;
            }

            //Collision with the end of hollo floor
            if (GameObjectData.GetType(fixtureB.UserData) == GameObjectType.BOUNDARY_HOLLO)
            {
                if (World.BodyList.Contains(fixtureB.Body))
                    World.RemoveBody(fixtureA.Body);
                _ballExploded = true;
                _explotionAnimationFrame = 16.1f;
            }

            //Collision with Level Completed Wall
            if (GameObjectData.GetType(fixtureB.UserData) == GameObjectType.LEVELCOMPLEWALL)
            {
                if (!_LevelCompleted)
                {
                    LevelOverScreen obj = new LevelOverScreen(_scoreBoard.Distance, _scoreBoard.Diamonds, false, CurrentLevel.LevelName);
                    obj.ShowMenuScreen += (_, __) =>
                    {
                        ScreenManager.RemoveScreen(this);
                    };
                    obj.NextLevel += (_, __) =>
                    {
                        this.ExitScreen();
                        if (OnNextLevel != null)
                            OnNextLevel(this._currentLevelIndex);
                        this._LevelCompleted = true;
                    };
                    ScreenManager.AddScreen(obj, null);
                }
            }

            return true;
        }

        #endregion

        #region Helpers
        private Texture2D GetTexture(string Texture)
        {
            if (!_textures.ContainsKey(Texture))
                _textures.Add(Texture, ScreenManager.ContentManager.Load<Texture2D>(Texture));

            return _textures[Texture];
        }
        #endregion
    }

}