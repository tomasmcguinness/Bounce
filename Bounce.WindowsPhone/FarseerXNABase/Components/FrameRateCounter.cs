using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerXNABase.ScreenSystem;

namespace FarseerXNABase.Components
{
    /// <summary>
    /// Displays the FPS
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        private TimeSpan _elapsedTime = TimeSpan.Zero;
        private NumberFormatInfo _format;
        private int _frameCounter;
        private int _frameRate;
        private ScreenManager _screenManager;
        private Vector2 _position;

        public FrameRateCounter(ScreenManager screenManager)
            : base(screenManager.Game)
        {
            _screenManager = screenManager;
            _format = new NumberFormatInfo();
            _format.NumberDecimalSeparator = ".";

            _position = new Vector2(30, 25);
        }

        public override void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime;

            //If 1 second passed....
            if (_elapsedTime <= TimeSpan.FromSeconds(1)) return;

            //Reset FrameRate Counter
            _elapsedTime -= TimeSpan.FromSeconds(1);
            _frameRate = _frameCounter;
            _frameCounter = 0;
        }

        public override void Draw(GameTime gameTime)
        {
            _frameCounter++;

            string fps = string.Format(_format, "{0} fps", _frameRate);

            _screenManager.SpriteBatch.Begin();
            _screenManager.SpriteBatch.DrawString(_screenManager.SpriteFonts.FrameRateCounterFont, fps,
                                                  _position, Color.Red, 0, Vector2.Zero, 1.0f,
                                                  SpriteEffects.None, 1);
            _screenManager.SpriteBatch.End();
        }
    }
}
