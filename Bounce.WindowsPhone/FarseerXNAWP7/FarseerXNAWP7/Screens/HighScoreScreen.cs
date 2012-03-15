
namespace FarseerXNAWP7.Screens
{

    using System;
    using System.Collections.Generic;
    using FarseerXNABase.Components;
    using FarseerXNABase.Controls;
    using FarseerXNABase.HighScore;
    using FarseerXNABase.ScreenSystem;
    using FarseerXNAWP7.Helpers;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using Microsoft.Xna.Framework.Input;
    using Microsoft.Xna.Framework.Input.Touch;

    public class HighScoreScreen : GameScreen
    {
        #region Properties & Variables

        private Rectangle _viewport;
        private Texture2D _background;
        private ScrollingPanelControl pnl;
        private ParallaxBackground _clouds;
        private const float _CLOUDSPEED = 0.4f;

        #endregion

        #region Initialization and Load

        public HighScoreScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.EnabledGestures = GestureType.Flick | GestureType.VerticalDrag | GestureType.DragComplete | GestureType.Tap;
        }

        public override void LoadContent()
        {
            Assets.InitializeScores(Assets.Levels);
            _background = ScreenManager.ContentManager.Load<Texture2D>(Assets.MAIN_BACKGROUND);

            UpdateScreen();
            ScreenManager.Camera.ProjectionUpdated += UpdateScreen;

            //Create Highscore scrollable panle and add text object into it.
            //Color ForegroundColor = new Color(143, 76, 44), HighlighColor = Color.Black;// = new Color(201, 156, 120);
            Color ForegroundColor = new Color(201, 156, 120), HighlighColor = Color.Black;
            pnl = new ScrollingPanelControl();

            int i = 10;
            pnl.AddChild(new TextControl("HIGHSCORE", ScreenManager.SpriteFonts.GameSpriteFont, HighlighColor,
                new Vector2(_viewport.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("HIGHSCORE").X / 2 - 4, i - 4)));
            pnl.AddChild(new TextControl("HIGHSCORE", ScreenManager.SpriteFonts.GameSpriteFont, HighlighColor,
                new Vector2(_viewport.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("HIGHSCORE").X / 2 + 4, i +4)));
            pnl.AddChild(new TextControl("HIGHSCORE", ScreenManager.SpriteFonts.GameSpriteFont, ForegroundColor,
                new Vector2(_viewport.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("HIGHSCORE").X / 2, i)));


            i += 20;
            foreach (Level item in Assets.Levels)
            {
                if (Assets.GetLevelScore(item.LevelName).Count > 0)
                {
                    i += 45;
                    pnl.AddChild(new TextControl(item.LevelName, ScreenManager.SpriteFonts.GameSpriteMediumFont, HighlighColor, new Vector2(10 - 1, i - 1)));
                    pnl.AddChild(new TextControl(item.LevelName, ScreenManager.SpriteFonts.GameSpriteMediumFont, HighlighColor, new Vector2(10 + 1, i + 1)));
                    pnl.AddChild(new TextControl(item.LevelName, ScreenManager.SpriteFonts.GameSpriteMediumFont, ForegroundColor, new Vector2(10, i)));
                    i += 5;
                }

                foreach (HighScoreEntry entry in Assets.GetLevelScore(item.LevelName))
                {
                    i += 29;

                    pnl.AddChild(new TextControl(entry.Name, ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor, new Vector2(50 - 1, i - 1)));
                    pnl.AddChild(new TextControl(entry.Name, ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor, new Vector2(50 + 1, i + 1)));
                    pnl.AddChild(new TextControl(entry.Name, ScreenManager.SpriteFonts.GameSpriteSmallFont, ForegroundColor, new Vector2(50, i)));

                    pnl.AddChild(new TextControl(entry.Score.ToString(), ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor, new Vector2(400 - 1, i - 1)));
                    pnl.AddChild(new TextControl(entry.Score.ToString(), ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor, new Vector2(400 + 1, i + 1)));
                    pnl.AddChild(new TextControl(entry.Score.ToString(), ScreenManager.SpriteFonts.GameSpriteSmallFont, ForegroundColor, new Vector2(400, i)));

                    pnl.AddChild(new TextControl(string.Format("{0:dd MMM yyyy}", entry.Date), ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor,
                        new Vector2(_viewport.Width - ScreenManager.SpriteFonts.GameSpriteSmallFont.MeasureString(string.Format("{0:dd MMM yyyy}", entry.Date)).X - 10 - 1, i - 1)));
                    pnl.AddChild(new TextControl(string.Format("{0:dd MMM yyyy}", entry.Date), ScreenManager.SpriteFonts.GameSpriteSmallFont, HighlighColor,
                        new Vector2(_viewport.Width - ScreenManager.SpriteFonts.GameSpriteSmallFont.MeasureString(string.Format("{0:dd MMM yyyy}", entry.Date)).X - 10 + 1, i + 1)));
                    pnl.AddChild(new TextControl(string.Format("{0:dd MMM yyyy}", entry.Date), ScreenManager.SpriteFonts.GameSpriteSmallFont, ForegroundColor,
                        new Vector2(_viewport.Width - ScreenManager.SpriteFonts.GameSpriteSmallFont.MeasureString(string.Format("{0:dd MMM yyyy}", entry.Date)).X - 10, i)));
                }

            }


            //Moving Clouds
            List<Texture2D> lst = new List<Texture2D>();
            lst.Add(ScreenManager.ContentManager.Load<Texture2D>(Assets.BG_CLOUDS));
            _clouds = new ParallaxBackground(lst, ParallaxDirection.Horizontal)
            {
                Height = ScreenManager.GraphicsDevice.Viewport.Height,
                Width = ScreenManager.GraphicsDevice.Viewport.Width
            };

            base.LoadContent();
        }

        private void UpdateScreen()
        {
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            _viewport = new Rectangle(0, 0, viewport.Width, viewport.Height);
        }

        #endregion

        #region Game Methods

        public override void HandleInput(InputHelper input)
        {
            if (!this.IsActive) return;
            pnl.HandleInput(input);
            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Allows popup to be closed by back button
            if (this.IsActive && GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                this.ExitScreen();
            }

            //Move background clouds
            _clouds.Move(new Vector2(_CLOUDSPEED, 0));

            pnl.Update(gameTime);
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            //Draw Background 
            ScreenManager.SpriteBatch.Draw(_background, _viewport, Color.White);
            //2. Draw Cloouds
            _clouds.Draw(ScreenManager.SpriteBatch);

            ScreenManager.SpriteBatch.End();

            Control.BatchDraw(pnl, ScreenManager.GraphicsDevice, ScreenManager.SpriteBatch, Vector2.Zero, gameTime);
        }
        #endregion
    }

}