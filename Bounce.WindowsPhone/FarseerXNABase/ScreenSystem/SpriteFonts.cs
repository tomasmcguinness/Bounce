using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FarseerXNABase.ScreenSystem
{
    /// <summary>
    /// Font helper class.
    /// </summary>
    public class SpriteFonts
    {
        private Dictionary<string, SpriteFont> fonts = new Dictionary<string,SpriteFont>();
        public SpriteFont LargeText
        {
            get
            {
                if (!fonts.ContainsKey("LargeText"))
                    fonts.Add("LargeText", Content.Load<SpriteFont>("Fonts/LargeText"));
                return fonts["LargeText"];
            }
        }
        public SpriteFont FrameRateCounterFont
        {
            get
            {
                if (!fonts.ContainsKey("FrameRateCounterFont"))
                    fonts.Add("FrameRateCounterFont", Content.Load<SpriteFont>("Fonts/frameRateCounterFont"));
                return fonts["FrameRateCounterFont"];
            }
        }
        public SpriteFont SegoeBold
        {
            get
            {
                if (!fonts.ContainsKey("SegoeBold"))
                    fonts.Add("SegoeBold", Content.Load<SpriteFont>("Fonts/SegoeBold"));
                return fonts["SegoeBold"];
            }
        }
        public SpriteFont GameSpriteFont
        {
            get
            {
                if (!fonts.ContainsKey("GameSpriteFont"))
                    fonts.Add("GameSpriteFont", Content.Load<SpriteFont>("Fonts/gamefont"));
                return fonts["GameSpriteFont"];
            }
        }
        public SpriteFont GameSpriteSmallFont
        {
            get
            {
                if (!fonts.ContainsKey("GameSpriteSmallFont"))
                    fonts.Add("GameSpriteSmallFont", Content.Load<SpriteFont>("Fonts/gamefontsmall"));
                return fonts["GameSpriteSmallFont"];
            }
        }
        public SpriteFont GameSpriteMediumFont
        {
            get
            {
                if (!fonts.ContainsKey("GameSpriteMediumFont"))
                    fonts.Add("GameSpriteMediumFont", Content.Load<SpriteFont>("Fonts/gamefontMedium"));
                return fonts["GameSpriteMediumFont"];
            }
        }

        private ContentManager Content;

        public SpriteFonts(ContentManager contentManager)
        {
            Content = contentManager;               
        }
    }
}
