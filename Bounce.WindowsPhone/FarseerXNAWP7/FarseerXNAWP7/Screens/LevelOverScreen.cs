
namespace FarseerXNAWP7.Screens
{
    using System;
    using FarseerXNABase.Controls;
    using FarseerXNABase.ScreenSystem;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    /// <summary>
    /// Level Completed or Game Over
    /// </summary>
    public class LevelOverScreen : GameScreen
    {
        #region Properties & Variables

        private ContentManager _content;
        private Rectangle _viewport;

        private Texture2D _woodPlank, _gameoverTitle, _totalScoreBG, _menuNormal, _menuClicked, _retryLevelNormal, _retryLevelClicked, _continueNormal, _continueClicked;
        private PanelControl _pnlMenu;

        private int _distance, _diamonds;
        private bool _isGameOver, _isNewHighscore;
        private string _titleText;

        public event EventHandler ShowMenuScreen, RestartLevel, NextLevel;

        #endregion

        #region Initialization and Load

        public LevelOverScreen(int Distance, int Diamonds, bool IsGameOver, string LevelName)
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
            this.IsPopup = true;

            this._distance = Distance;
            this._diamonds = Diamonds;
            this._isGameOver = IsGameOver;
            if (IsGameOver)
                _titleText = "GAME OVER";
            else
                _titleText = "LEVEL CLEARED";

            Assets.InitializeScores(Assets.Levels);
            _isNewHighscore = Assets.IsNewHighscore(_distance + Assets.DIAMONDPOINTS * _diamonds, LevelName);
            Assets.AddLevelScore(_distance + Assets.DIAMONDPOINTS * _diamonds, LevelName);

        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            UpdateScreen();
            ScreenManager.Camera.ProjectionUpdated += UpdateScreen;

            //Textures
            LoadTextures();

            //Menu Button
            CreateMenuButton();

            base.LoadContent();
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
            _woodPlank = ScreenManager.ContentManager.Load<Texture2D>(Assets.HANGING_WOOD_PLANK);
            _gameoverTitle = ScreenManager.ContentManager.Load<Texture2D>(Assets.GAMEOVER_TITLE);
            _totalScoreBG = ScreenManager.ContentManager.Load<Texture2D>(Assets.TOTALSCORE_BG);
            _menuNormal = ScreenManager.ContentManager.Load<Texture2D>(Assets.MENU_NORMAL);
            _menuClicked = ScreenManager.ContentManager.Load<Texture2D>(Assets.MENU_CLICKED);
            _retryLevelNormal = ScreenManager.ContentManager.Load<Texture2D>(Assets.RETRYLEVEL_NORMAL);
            _retryLevelClicked = ScreenManager.ContentManager.Load<Texture2D>(Assets.RETRYLEVEL_CLICKED);
            _continueNormal = ScreenManager.ContentManager.Load<Texture2D>(Assets.CONITNUE_NORMAL);
            _continueClicked = ScreenManager.ContentManager.Load<Texture2D>(Assets.CONITNUE_CLICKED);
        }

        #endregion

        #region Game Methods

        public override void HandleInput(InputHelper input)
        {
            _pnlMenu.HandleInput(input);
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

            // Allows popup to be closed by back button
            if (this.IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.ExitScreen();
                if (ShowMenuScreen != null)
                    ShowMenuScreen(null, null);
            }

            _pnlMenu.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }


        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) return;

            ScreenManager.SpriteBatch.Begin();

            //----Draw Background------
            int BackgroundX = (int)(_viewport.Width / 2 - _woodPlank.Width / 2);     //Scaled to 90% to fit on screen, then place it horizontally centred.
            ScreenManager.SpriteBatch.Draw(_woodPlank,
                new Rectangle(BackgroundX, _viewport.Y, (int)(_woodPlank.Width), (int)(_woodPlank.Height))
                , Color.White);

            //----Title-------

            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.SegoeBold, _titleText,
                new Vector2((int)(_viewport.Width / 2 - ScreenManager.SpriteFonts.SegoeBold.MeasureString(_titleText).X / 2), 125), Color.Black);   //X = align centre, 125 = distance from top to match with background

            //----Scores------
            //Diamonds
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.LargeText, string.Format("{0} DIAMONDS x " + Assets.DIAMONDPOINTS.ToString() + ":", _diamonds),
                new Vector2(220, 180), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.LargeText, (_diamonds * Assets.DIAMONDPOINTS).ToString(),
                new Vector2(510, 180), Color.Black);

            //Total Distance
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.LargeText, "DISTANCE:",
                new Vector2(220, 210), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.LargeText, _distance.ToString(),
                new Vector2(510, 210), Color.Black);

            //Total Score
            ScreenManager.SpriteBatch.Draw(_totalScoreBG, new Rectangle(210, 265, (int)(_totalScoreBG.Width * 1.2), (int)(_totalScoreBG.Height * 1.2)), Color.White);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.SegoeBold, "TOTAL SCORE:",
                new Vector2(220, 250), Color.Black);
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.SegoeBold, (_distance + Assets.DIAMONDPOINTS * _diamonds).ToString(),
                new Vector2(510, 250), Color.Black);

            //Highscore
            if (_isNewHighscore)
                ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.GameSpriteSmallFont, "NEW HIGHSCORE",
                    new Vector2(_viewport.Width / 2 - ScreenManager.SpriteFonts.GameSpriteSmallFont.MeasureString("NEW HIGHSCORE").X / 2, 300), Color.Black);

            ScreenManager.SpriteBatch.End();

            //Menu Button
            Control.BatchDraw(_pnlMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }
        #endregion

        #region Button

        private void CreateMenuButton()
        {
            //Menu Button
            int ButtonPosX = _viewport.Width / 2 - (int)(_woodPlank.Width / 2) - 25;
            _pnlMenu = new PanelControl();
            _pnlMenu.Position = new Vector2(ButtonPosX, (int)(_woodPlank.Height) - 60 - 35);

            //Menu
            Button miMenu = new Button(ScreenManager.Game)
            {
                Width = 150,
                Height = 49,
                NormalButtonTexture = _menuNormal,
                ClickedButtonTexture = _menuClicked,
                Position = new Vector2(310, 0)
            };
            miMenu.OnClicked += new Button.ClickHandler(miMenu_OnClicked);
            _pnlMenu.AddChild(miMenu);


            if (this._isGameOver)
            {
                //Retry
                Button miRetryLevel = new Button(ScreenManager.Game)
                {
                    Width = 150,
                    Height = 49,
                    NormalButtonTexture = _retryLevelNormal,
                    ClickedButtonTexture = _retryLevelClicked
                };

                miRetryLevel.OnClicked += new Button.ClickHandler(miRetryLevel_OnClicked);
                _pnlMenu.AddChild(miRetryLevel);

            }
            else
            {
                //Retry
                Button miContinueToNextLevel = new Button(ScreenManager.Game)
                {
                    Width = 150,
                    Height = 49,
                    NormalButtonTexture = _continueNormal,
                    ClickedButtonTexture = _continueClicked
                };

                miContinueToNextLevel.OnClicked += new Button.ClickHandler(miContinueToNextLevel_OnClicked);
                _pnlMenu.AddChild(miContinueToNextLevel);
            }


        }

        void miContinueToNextLevel_OnClicked(Button sender)
        {
            if (NextLevel != null)
                NextLevel(null, null);
            this.ExitScreen();
        }

        void miRetryLevel_OnClicked(Button sender)
        {
            if (RestartLevel != null)
                RestartLevel(null, null);
            this.ExitScreen();
        }

        void miMenu_OnClicked(Button sender)
        {
            if (ShowMenuScreen != null)
                ShowMenuScreen(null, null);
            this.ExitScreen();
        }

        #endregion
    }

}
