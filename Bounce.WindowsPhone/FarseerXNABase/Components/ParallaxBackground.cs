using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FarseerXNABase.Components
{
    /// <summary>
    /// Moving Background for game screens.
    /// </summary>
    public class ParallaxBackground
    {
        #region Properties/Variables

        private float _speedX = 1;
        /// <summary>
        /// Gets or sets the speed X.
        /// </summary>
        /// <value>The speed X.</value>
        /// <remarks></remarks>
        public float SpeedX
        {
            get { return _speedX; }
            set { _speedX = value; }
        }

        private float _speedY = 1;
        /// <summary>
        /// Gets or sets the speed Y.
        /// </summary>
        /// <value>The speed Y.</value>
        /// <remarks></remarks>
        public float SpeedY
        {
            get { return _speedY; }
            set { _speedY = value; }
        }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        /// <value>The height.</value>
        /// <remarks></remarks>
        public int Height { get; set; }
        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        /// <value>The width.</value>
        /// <remarks></remarks>
        public int Width { get; set; }

        /// <summary>
        /// Gets or sets the scrolling textures.
        /// </summary>
        /// <value>The scrolling textures.</value>
        /// <remarks></remarks>
        public List<Texture2D> ScrollingTextures { get; set; }
        private List<TextureDetails> _textures { get; set; }

        private Vector2 _defaultMovementSpeed;
        private ParallaxDirection _direction;

        private Vector2 _currentPosition;
        public Vector2 Position
        {
            get { return _currentPosition; }
            set { _currentPosition = value; }
        }
        #endregion

        #region Initialization and Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallaxBackground"/> class.
        /// </summary>
        /// <param name="ScrollingTextures">The scrolling textures. Order of texture should be maintained.</param>
        /// <param name="Direction">The direction.</param>
        /// <remarks></remarks>
        public ParallaxBackground(List<Texture2D> ScrollingTextures, ParallaxDirection Direction)
        {
            this._direction = Direction;
            this.ScrollingTextures = ScrollingTextures;
            Initialize();
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            //Set Speed according to Direction
            switch (_direction)
            {
                case ParallaxDirection.Horizontal:
                    SpeedX = 1; SpeedY = 0;
                    break;
                case ParallaxDirection.Vertical:
                    SpeedX = 0; SpeedY = 1;
                    break;
                default:
                    break;
            }

            this._currentPosition = Vector2.Zero;
            this._defaultMovementSpeed = new Vector2(SpeedX, SpeedY);

            //Extract data from ScrollingTextures
            int LastWidth = 0, LastHeight = 0;
            _textures = new List<TextureDetails>();

            int i = 1;
            foreach (Texture2D texture in ScrollingTextures)
            {
                _textures.Add(new TextureDetails(texture, texture.Width, texture.Height, LastWidth, LastHeight, i.ToString()));
                LastWidth += texture.Width;
                LastHeight += texture.Height;
                i++;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Moves the specified distance.
        /// </summary>
        /// <param name="Distance">The distance.</param>
        /// <remarks></remarks>
        public void Move(Vector2 Distance)
        {
            //Change the CurrentPosition according to the Distance and the Direction we are moving.
            _currentPosition.X += SpeedX * Distance.X;
            _currentPosition.Y += SpeedY * Distance.Y;

            if (_currentPosition.X >= (_textures[_textures.Count - 1].BeltPositionX + _textures[_textures.Count - 1].Width))
                _currentPosition.X = 0;

            if (_currentPosition.Y >= (_textures[_textures.Count - 1].BeltPositionY + _textures[_textures.Count - 1].Height))
                _currentPosition.Y = 0;
        }
        /// <summary>
        /// Moves by default speed and distance.
        /// </summary>
        /// <remarks></remarks>
        public void Move()
        {
            Move(_defaultMovementSpeed);
        }
        #endregion

        #region Game Methods

public void Draw(SpriteBatch spriteBatch)
{
    //Iterate through all texture
    //If Current position fall into current texture then render it and
    //Increment the current location by width (min(texturewidth and windowWidth)

    float positionX = _currentPosition.X;
    float positionY = _currentPosition.Y;
    for (int i = 0; i < _textures.Count; i++)
    {
        if (this._direction == ParallaxDirection.Horizontal)
            if (positionX >= _textures[i].BeltPositionX && positionX <= (_textures[i].BeltPositionX + _textures[i].Width))
            {
                positionX += Math.Min(_textures[i].Width, this.Width);
                spriteBatch.Draw(_textures[i].Texture, new Vector2(_textures[i].BeltPositionX - _currentPosition.X, 0), Color.White);
            }

        if (this._direction == ParallaxDirection.Vertical)
            if (positionY >= _textures[i].BeltPositionY && positionY <= (_textures[i].BeltPositionY + _textures[i].Height))
            {
                positionY += Math.Min(_textures[i].Height, this.Height);
                spriteBatch.Draw(_textures[i].Texture, new Vector2(0, _textures[i].BeltPositionY - _currentPosition.Y), Color.White);
            }
    }

    //Means last texture reached and we can't reset Position to zero unless untill we completely render last texture,
    //So render first texture just by the last texture
    // |-------------|--------------------------|
    // | Last        | First                    |
    // | Texture     | Texture                  |
    // |-------------|--------------------------|
    if (this._direction == ParallaxDirection.Horizontal)
        if (_currentPosition.X >= (_textures[_textures.Count - 1].BeltPositionX + _textures[_textures.Count - 1].Width - this.Width))
        {
            spriteBatch.Draw(_textures[0].Texture, new Vector2((_textures[_textures.Count - 1].BeltPositionX + _textures[_textures.Count - 1].Width - _currentPosition.X), 0), Color.White);
        }
    if (this._direction == ParallaxDirection.Vertical)
        if (_currentPosition.Y >= (_textures[_textures.Count - 1].BeltPositionY + _textures[_textures.Count - 1].Height - this.Height))
        {
            spriteBatch.Draw(_textures[0].Texture, new Vector2(0, (_textures[_textures.Count - 1].BeltPositionY + _textures[_textures.Count - 1].Height - _currentPosition.Y)), Color.White);
        }
}
        
        #endregion

        #region Texture Details storage DataStructure
        /// <summary>
        /// DataStructure to store information about the scrolling tectures. Used in current class only.
        /// </summary>
        struct TextureDetails
        {
            public string Name;
            public Texture2D Texture;
            public int Width;
            public int Height;
            public int BeltPositionX;
            public int BeltPositionY;

            public TextureDetails(Texture2D texture, int width, int height, int posX, int posY, string Name)
            {
                this.Texture = texture;
                this.Width = width;
                this.Height = height;
                this.BeltPositionX = posX;
                this.BeltPositionY = posY;
                this.Name = Name;
            }
        }

        #endregion
    }

    /// <summary>
    /// Parallax Background moving Direction.
    /// </summary>
    public enum ParallaxDirection
    {
        Horizontal,
        Vertical
    }
}