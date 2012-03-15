namespace FarseerXNAWP7.Screens
{
    using System;
    using FarseerXNABase.Controls;
    using FarseerXNABase.ScreenSystem;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Content;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;

    public class InstructionsScreen : GameScreen
    {
        #region Properties & Variables

        private ContentManager _content;
        private Rectangle _viewport;

        Texture2D _background, _howtoplay, _gotitNormal, _gotitClicked;
        PanelControl _pnlMenu;

        #endregion

        #region Initialization and Load

        public InstructionsScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = Microsoft.Xna.Framework.Input.Touch.GestureType.Tap;
            this.IsPopup = true;
        }

        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            UpdateScreen();
            ScreenManager.Camera.ProjectionUpdated += UpdateScreen;

            //Textures
            LoadTextures();

            //Back Button
            CreateBackButton();

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
            _background = _content.Load<Texture2D>(Assets.HANGING_WOOD_PLANK);
            _howtoplay = _content.Load<Texture2D>(Assets.INSTRUCTION_HOWTOPLAY);
            _gotitNormal = _content.Load<Texture2D>(Assets.GOTIT_NORMAL);
            _gotitClicked = _content.Load<Texture2D>(Assets.GOTIT_CLICKED);
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

            //Draw Background
            int BackgroundX = (int)(_viewport.Width / 2 - _background.Width / 2);     //Scaled to 90% to fit on screen, then place it horizontally centred.
            ScreenManager.SpriteBatch.Draw(_background,
                new Rectangle(BackgroundX, _viewport.Y, (int)(_background.Width), (int)(_background.Height))
                , Color.White);

            //Title
            ScreenManager.SpriteBatch.DrawString(ScreenManager.SpriteFonts.SegoeBold, "HOW TO PLAY",
                new Vector2((int)(_viewport.Width / 2 - ScreenManager.SpriteFonts.SegoeBold.MeasureString("HOW TO PLAY").X / 2), 125), Color.Black);   //X = align centre, 125 = distance from top to match with background

            //How to Play
            int HowToPlayX = (int)(_viewport.Width / 2 - _howtoplay.Width / 2);     //Align in centre.
            int HowToPlayY = 125 + (int)ScreenManager.SpriteFonts.SegoeBold.MeasureString("HOW TO PLAY").Y + 15;                               //Distance from top, i.e top to title distance + title height and some margin
            ScreenManager.SpriteBatch.Draw(_howtoplay, new Vector2(HowToPlayX, HowToPlayY), Color.White);

            ScreenManager.SpriteBatch.End();

            //Buttons
            Control.BatchDraw(_pnlMenu, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }
        #endregion

        #region Button

        private void CreateBackButton()
        {
            //Initialize MenuPanel
            int ButtonPosX = _viewport.Width / 2 + (int)(_background.Width / 2) - 150;
            _pnlMenu = new PanelControl();
            _pnlMenu.Position = new Vector2(ButtonPosX, (int)(_background.Height) - 49 - 5);

            //Add MenuItems, Back button
            Button miGotIt = new Button(ScreenManager.Game)
            {
                Width = 150,
                Height = 49,
                NormalButtonTexture = _gotitNormal,
                ClickedButtonTexture = _gotitClicked
            };

            miGotIt.OnClicked += new Button.ClickHandler(miGotIt_OnClicked);

            _pnlMenu.AddChild(miGotIt);
        }

        void miGotIt_OnClicked(Button sender)
        {
            this.ExitScreen();
        }

        #endregion

    }

}
