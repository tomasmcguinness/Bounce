namespace FarseerXNAWP7.Screens
{
    using System;
    using System.Collections.Generic;
    using FarseerXNABase.Components;
    using FarseerXNABase.Controls;
    using FarseerXNABase.ScreenSystem;
    using FarseerXNAWP7.Helpers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

    //First Screen in game, which shows Level selector, 
    public class StartScreen : GameScreen
    {
        #region Properties & Variables

        private ContentManager _content;
        private Rectangle _viewport;

        //Textures
        Texture2D _background, _logo, _startGameNormal, _startGameClicked,
            _instructionsNormal, _instructionsClicked, _highScoreNormal, _highScoreClicked;

        private ParallaxBackground _clouds;
        private Vector2 _cloudPosition = Vector2.Zero;
        private PanelControl pnlMenu;
        private LevelSelectorScreen _levelSelectorScreen;

        //Tombstone
        private ResumeState _resumeState;
        public ResumeState ResumeState
        {
            get
            {
                if (_levelSelectorScreen == null)
                    return null;
                return _levelSelectorScreen.ResumeStateData;
            }
            set
            {
                _resumeState = value;
            }
        }

        //Event to Notify Exit Screen to Parent.
        public event EventHandler ExitGame;

        //Const
        private const float _CLOUDSPEED = 0.4f;
        #endregion

        #region Initialization and Load

        public StartScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = GestureType.Tap;
        }
        public StartScreen(ResumeState ResumeState)
            : this()
        {
            _resumeState = ResumeState;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            UpdateScreen();
            ScreenManager.Camera.ProjectionUpdated += UpdateScreen;

            //Load Texture, fonts etc here.
            LoadTextures();

            //Menu
            CreateMenu();

            base.LoadContent();

            //if (_resumeState != null)
            //{
            //    _levelSelectorScreen = new LevelSelectorScreen(_resumeState);
            //    ScreenManager.AddScreen(_levelSelectorScreen, null);
            //}
        }

        public override void UnloadContent()
        {
            _content.Unload();
        }

        private void UpdateScreen()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        private void LoadTextures()
        {
            _background = _content.Load<Texture2D>(Assets.MAIN_BACKGROUND);
            _logo = _content.Load<Texture2D>(Assets.GAME_LOGO);
            _startGameNormal = _content.Load<Texture2D>(Assets.STARTGAME_NORMAL);
            _startGameClicked = _content.Load<Texture2D>(Assets.STARTGAME_CLICKED);
            _instructionsNormal = _content.Load<Texture2D>(Assets.INSTRUCTIONS_NORMAL);
            _instructionsClicked = _content.Load<Texture2D>(Assets.INSTRUCTIONS_CLICKED);
            _highScoreNormal = _content.Load<Texture2D>(Assets.HIGHSCORE_NORMAL);
            _highScoreClicked = _content.Load<Texture2D>(Assets.HIGHSCORE_CLICKED);

            //Moving Clouds
            List<Texture2D> lst = new List<Texture2D>();
            lst.Add(ScreenManager.ContentManager.Load<Texture2D>(Assets.BG_CLOUDS));
            _clouds = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width
            };
        }


        #endregion

        #region Game Methods

        public override void HandleInput(InputHelper input)
        {
            pnlMenu.HandleInput(input);
            base.HandleInput(input);
        }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        /// <param name="gameTime">Current Game time object.</param>
        /// <param name="otherScreenHasFocus">Is any other screen has focus</param>
        /// <param name="coveredByOtherScreen">If the screen is covered by another, it should transition off.</param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            //Handle Back button
            if (this.IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                if (ExitGame != null)
                    ExitGame(null, null);

            //Move background clouds
            _clouds.Move(new Vector2(_CLOUDSPEED, 0));

            //Update Menu states
            pnlMenu.Update(gameTime);

            //base Update
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();

            //1. Draw Background
            ScreenManager.SpriteBatch.Draw(_background, _viewport, Color.White);

            //2. Draw Cloouds
            _clouds.Draw(ScreenManager.SpriteBatch);

            //3. Draw Logo
            ScreenManager.SpriteBatch.Draw(_logo, new Rectangle((int)(_viewport.Width / 2 - 167), 320, 334, 125), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);

            ScreenManager.SpriteBatch.End();

            //4. Draw Menu
            Control.BatchDraw(pnlMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
            //pnlMenu.Draw(gameTime, ScreenManager.SpriteBatch);
        }
        #endregion

        #region Menu

        private void CreateMenu()
        {
            //Initialize MenuPanel
            pnlMenu = new PanelControl();
            pnlMenu.Position = new Vector2(_viewport.Width / 2 - 125, 100);

            //Add MenuItems
            //1. Start Game
            Button miStartGame = new Button(ScreenManager.Game)
            {
                Width = 250,
                Height = 60,
                NormalButtonTexture = _startGameNormal,
                ClickedButtonTexture = _startGameClicked,
                //Position = new Vector2(_viewport.Width / 2 - _startGameNormal.Width / 2, _logo.Height + 25)
            };

            //2. Instructions
            Button miInstructions = new Button(ScreenManager.Game)
            {
                Width = 250,
                Height = 60,
                NormalButtonTexture = _instructionsNormal,
                ClickedButtonTexture = _instructionsClicked,
                Position = new Vector2(0, 62)
            };

            //3. Highscore
            Button miHighScore = new Button(ScreenManager.Game)
            {
                Width = 250,
                Height = 60,
                NormalButtonTexture = _highScoreNormal,
                ClickedButtonTexture = _highScoreClicked,
                Position = new Vector2(0, 124)
            };

            //Event Handlers
            miStartGame.OnClicked += new Button.ClickHandler(miStartGame_OnClicked);
            miInstructions.OnClicked += new Button.ClickHandler(miInstructions_OnClicked);
            miHighScore.OnClicked += new Button.ClickHandler(miHighScore_OnClicked);

            //Add MenuItems to Menupanel
            pnlMenu.AddChild(miStartGame);
            pnlMenu.AddChild(miInstructions);
            pnlMenu.AddChild(miHighScore);
        }

        void miStartGame_OnClicked(Button sender)
        {
            if (_levelSelectorScreen == null)
                _levelSelectorScreen = new LevelSelectorScreen();

            ScreenManager.AddScreen(_levelSelectorScreen, null);
        }
        void miInstructions_OnClicked(Button sender)
        {
            ScreenManager.AddScreen(new InstructionsScreen(), null);
        }
        void miHighScore_OnClicked(Button sender)
        {
            ScreenManager.AddScreen(new HighScoreScreen(), null);
        }

        #endregion

        #region Methods
        public void LoadLevelScreen()
        {
            _levelSelectorScreen = new LevelSelectorScreen(_resumeState);
            ScreenManager.AddScreen(_levelSelectorScreen, null);
            _levelSelectorScreen.ResumeLevelFromMemoryData();
        }
        #endregion
    }

}
