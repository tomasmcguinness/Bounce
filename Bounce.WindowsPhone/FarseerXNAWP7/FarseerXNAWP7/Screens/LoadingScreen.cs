
namespace FarseerXNAWP7.Screens
{
    using System;
    using FarseerXNABase.ScreenSystem;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;

    public class LoadingScreen : GameScreen
    {
        Texture2D _background;
        Rectangle _viewPort;

        #region Initialization and Load

        public LoadingScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            this.IsPopup = true;
        }

        public override void LoadContent()
        {
            _background = ScreenManager.ContentManager.Load<Texture2D>(Assets.BLANK);
            _viewPort = new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height);
            base.LoadContent();
        }

        #endregion

        #region Game Methods
        TimeSpan ts = new TimeSpan(0, 0, 5);
        TimeSpan startGameTime;
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (startGameTime == null)
                startGameTime = gameTime.TotalGameTime;

            if (startGameTime.Add(ts) < gameTime.TotalGameTime)
                this.ExitScreen();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            if (!this.IsActive) return;

            ScreenManager.SpriteBatch.Begin();

            //Draw semiopaque background
            //ScreenManager.SpriteBatch.Draw(_background, _viewPort, new Color(0, 0, 0, 50 * (5 - gameTime.TotalGameTime.Subtract(startGameTime).Seconds)));
            ScreenManager.SpriteBatch.Draw(_background, _viewPort, new Color(0, 0, 0, 100));

            //Black outline 1
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.SpriteFonts.GameSpriteFont,
                "BE READY IN " + (5 - gameTime.TotalGameTime.Subtract(startGameTime).Seconds).ToString(),
                new Vector2(_viewPort.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").X / 2 - 2,
                            _viewPort.Height / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").Y / 2 - 2),
                Color.Black);

            //Black outline 2
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.SpriteFonts.GameSpriteFont,
                "BE READY IN " + (5 - gameTime.TotalGameTime.Subtract(startGameTime).Seconds).ToString(),
                new Vector2(_viewPort.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").X / 2 + 2,
                            _viewPort.Height / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").Y / 2 + 2),
                Color.Black);

            //Brown
            ScreenManager.SpriteBatch.DrawString(
                ScreenManager.SpriteFonts.GameSpriteFont,
                "BE READY IN " + (5 - gameTime.TotalGameTime.Subtract(startGameTime).Seconds).ToString(),
                new Vector2(_viewPort.Width / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").X / 2,
                            _viewPort.Height / 2 - ScreenManager.SpriteFonts.GameSpriteFont.MeasureString("BE READY IN 0").Y / 2),
                new Color(201, 156, 120));

            ScreenManager.SpriteBatch.End();
        }
        #endregion
    }

}
