using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FarseerXNABase.Components
{
    /// <summary>
    /// Current Game Specific ScoreBoard. But Can be modified to be used in more generic way.
    /// </summary>
    public class ScoreBoard
    {
        public int Score { get; set; }
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public SpriteFont Font { get; set; }
        public float Height { get; set; }
        public float Width { get; set; }
        public int Diamonds { get; set; }
        public int Distance
        {
            get
            {
                return (this.Score - Diamonds * _diamondPoints);
            }
        }

        private int _diamondPoints;
        public ScoreBoard(int DiamondPoints)
        {
            this.Score = 0;
            this.Diamonds = 0;
            this._diamondPoints = DiamondPoints;
        }

        public void Update(GameTime gameTime)
        {
            //We update the score every update.
            this.Score++;
        }
        public void CollectedDiamond()
        {
            //A Diamond collected to increase the score accordingly.
            CollectedDiamond(_diamondPoints);
        }
        private void CollectedDiamond(int Points)
        {
            this.Diamonds++;
            this.Score += Points;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (Position != null && Texture != null && Font != null)
            {
                spriteBatch.Draw(Texture, new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height), Color.White);
                spriteBatch.DrawString(Font, this.Score.ToString(), new Vector2(Position.X + 5, Position.Y + 23), new Color(31, 135, 205), 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            }
        }
    }
}
