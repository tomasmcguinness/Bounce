
namespace FarseerXNAWP7.Screens
{

    using System;
    using FarseerXNABase.Controls;
    using FarseerXNABase.ScreenSystem;
    using FarseerXNAWP7.Helpers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

    public class LevelSelectorScreen : GameScreen
    {
        #region Properties & Variables

        private Rectangle _viewport;

        private Texture2D _btnNormal, _btnClicked, _background;
        PanelControl _pnlMenu;
        private int _currentLevelIndex;
        private GameLevelPhysicsScreen _currentLevelScreen;

        private ResumeState _resumeStateData;
        public ResumeState ResumeStateData
        {
            get
            {
                if (_currentLevelScreen == null)
                    return null;
                if (!_currentLevelScreen.IsActive)
                    return null;
                return new ResumeState()
                {
                    BallPosition = _currentLevelScreen.BalllPosition,
                    CameraPosition = _currentLevelScreen.CameraPosition,
                    CurrentLevel = _currentLevelScreen.CurrentLevelIndex,
                    DiamondsCollected = _currentLevelScreen.DiamondsCollected,
                    DistanceTravelled = _currentLevelScreen.DistanceTravelled,
                    BackgroundPosition = _currentLevelScreen.BackgroundPosition
                };
            }
            set
            {
                _resumeStateData = value;
            }
        }
        #endregion

        #region Initialization and Load

        public LevelSelectorScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = GestureType.Flick | GestureType.VerticalDrag | GestureType.DragComplete | GestureType.Tap;
        }
        public LevelSelectorScreen(ResumeState ResumeState)
            : this()
        {
            _resumeStateData = ResumeState;
        }

        public override void LoadContent()
        {
            _btnNormal = ScreenManager.ContentManager.Load<Texture2D>(Assets.WOODPLANK_NORMAL);
            _btnClicked = ScreenManager.ContentManager.Load<Texture2D>(Assets.WOODPLANK_CLICKED);
            _background = ScreenManager.ContentManager.Load<Texture2D>(Assets.MAIN_BACKGROUND);

            UpdateScreen();
            ScreenManager.Camera.ProjectionUpdated += UpdateScreen;

            CreateMenu();

            base.LoadContent();

            //if(_resumeStateData != null)
            //    ResumeLevelFromMemoryData(_resumeStateData);
        }

        private void UpdateScreen()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public override void HandleInput(InputHelper input)
        {
            _pnlMenu.HandleInput(input);
            base.HandleInput(input);
        }
        public override void HandleGamePadInput(InputHelper input)
        {
            base.HandleGamePadInput(input);
        }
        public override void HandleKeyboardInput(InputHelper input)
        {
            base.HandleKeyboardInput(input);
        }

        #endregion

        #region Game Methods

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            
            // Allows popup to be closed by back button
            if (this.IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.ExitScreen();
            }

            //Update Menu states
            _pnlMenu.Update(gameTime);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) return;

            ScreenManager.SpriteBatch.Begin();

            //Draw Background 
            ScreenManager.SpriteBatch.Draw(_background, _viewport, Color.White);

            ScreenManager.SpriteBatch.End();

            //Draw Level Selector list
            Control.BatchDraw(_pnlMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }
        #endregion

        #region Menu

        private void CreateMenu()
        {
            //Initialize MenuPanel
            _pnlMenu = new ScrollingPanelControl();

            //Add MenuItems
            int i = 0;
            foreach (Level item in Assets.Levels)
            {
                Button mi = new Button(ScreenManager.Game)
                            {
                                Width = _viewport.Width - 20,
                                Height = 96,
                                NormalButtonTexture = _btnNormal,
                                ClickedButtonTexture = _btnClicked,
                                DisplayText = item.LevelName + "\n" + item.Description,
                                TextVisible = true,
                                Font = ScreenManager.SpriteFonts.SegoeBold,
                                Tag = i,
                                TextSize = Button.FontSize.Big,
                                TextAlignment = Button.TextAlign.Left,
                                Position = new Vector2(10, i * 100)
                            };

                mi.OnClicked += (Button sender) =>
                    {
                        LoadLevelByIndex((int)sender.Tag);
                    };

                _pnlMenu.AddChild(mi);
                i++;
            }

        }

        private void NextLevelClicked(int CurrentLevelIndex)
        {
            LoadLevelByIndex(CurrentLevelIndex + 1);
        }
        private void LoadLevelByIndex(int LevelIndex)
        {
            this._currentLevelIndex = LevelIndex;
            this._currentLevelScreen = new GameLevelPhysicsScreen(Assets.Levels[LevelIndex], LevelIndex);
            this._currentLevelScreen.OnNextLevel += (int CurrentLevelIndex) =>
                {
                    if (CurrentLevelIndex == Assets.Levels.Count - 1)
                        CurrentLevelIndex = -1;
                    LoadLevelByIndex(CurrentLevelIndex + 1);
                };
            ScreenManager.AddScreen(this._currentLevelScreen, null);
        }
        public void ResumeLevelFromMemoryData()
        {
            this._currentLevelIndex = _resumeStateData.CurrentLevel;
            this._currentLevelScreen = new GameLevelPhysicsScreen(
                Assets.Levels[_resumeStateData.CurrentLevel], _resumeStateData.CurrentLevel,
                _resumeStateData.CameraPosition, _resumeStateData.BallPosition,
                _resumeStateData.DistanceTravelled, _resumeStateData.DiamondsCollected, _resumeStateData.BackgroundPosition);

            this._currentLevelScreen.OnNextLevel += (int CurrentLevelIndex) =>
            {
                LoadLevelByIndex(CurrentLevelIndex + 1);
            };
            ScreenManager.AddScreen(this._currentLevelScreen, null);

            //Show Loading Screen to give user sometime to recover his mind
            ScreenManager.AddScreen(new LoadingScreen(), null);
        }
        #endregion
    }

}
