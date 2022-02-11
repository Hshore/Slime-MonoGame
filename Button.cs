using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace monoSlime2
{
    public class Button : Component     
    {
        public enum Location 
        {
            TopLeft,
            TopRight,
            TopMiddle,
            BottomMiddle,
            BottomLeft,
            BottomRight,
        }

        public Location location { get; set; }
        
        public Component_Type type = Component_Type.Button;
        #region Fields
        private MouseState _currentmouse;
        private SpriteFont _font;
        private bool _isHovering;
        private MouseState _previousMouse;
        private Texture2D _texture;
        #endregion

        #region Properties
        public event EventHandler Click;
        public bool Clicked { get; private set; }

        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 PositionScale { get; set; }
        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public string Text { get; set; }
        #endregion

        #region Methods
        public Button(Texture2D texture, SpriteFont font)
        {
            _texture = texture;
            _font = font;
            PenColour = Color.Black;
        }

        

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            var color = Color.White;

            if (_isHovering)
            {
                color = Color.Gray;
            }

            sprite.Draw(_texture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X/2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y/2);

                sprite.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {

            _previousMouse = _currentmouse;
            _currentmouse = Mouse.GetState();
            Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
            var mouseRect = new Rectangle(_currentmouse.X, _currentmouse.Y, 1, 1);

            _isHovering = false;
            if (mouseRect.Intersects(Rectangle))
            {
                _isHovering = true;

                if (_currentmouse.LeftButton == ButtonState.Released && _previousMouse.LeftButton == ButtonState.Pressed)
                {
                    Click?.Invoke(this, new EventArgs());
                }
            }
            
            
        }

        #endregion
    }
}
