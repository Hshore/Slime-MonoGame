using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace monoSlime2
{
    class Slider : Component
    {

        #region Fields
        private SpriteFont _font;
        private Texture2D _texture;
        private Texture2D _toggeltexture;
        

        #region Properties
        public string _name;
        public event EventHandler<Slider> UpdateValue;
        //EventArgs args = new EventArgs();
        public Color PenColour { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 TogglePosition { get; set; }
        public Vector2 PositionScale { get; set; }

        public Vector2 MinMaxValues { get; set; }

        public int ToggleValue;
        private MouseState _previousMouse;
        private MouseState _currentmouse;

        public Rectangle Rectangle
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, _texture.Width, _texture.Height);
            }
        }
        public Rectangle ToggleRectangle
        {
            get
            {
                var multiple = (_texture.Width) / MinMaxValues.Y;
                var pX = (ToggleValue * multiple) + Position.X;

                return new Rectangle((int)pX - (_toggeltexture.Width/2), (int)Position.Y + ((_texture.Height/2) - (_toggeltexture.Height/2)),  _toggeltexture.Width, _toggeltexture.Height);
            }
        }
        public string Text { get; set; }
        #endregion

        #endregion

        public Slider(Texture2D texture, Texture2D toggelTexture, SpriteFont font, string name)
        {
            _texture = texture;
            _toggeltexture = toggelTexture;
            _font = font;
            PenColour = Color.LightBlue;
            _name = name;
        }

        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            var color = Color.White;
            sprite.Draw(_texture, Rectangle, color);
            sprite.Draw(_toggeltexture, ToggleRectangle, color);
            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                sprite.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {
            //UpdateValue?.Invoke(this, this);

            _previousMouse = _currentmouse;
            _currentmouse = Mouse.GetState();
            //Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
            var mouseRect = new Rectangle(_currentmouse.X, _currentmouse.Y, 1, 1);

            
            if (mouseRect.Intersects(Rectangle))
            {
                

                if (_currentmouse.LeftButton == ButtonState.Pressed)
                {
                    var multiple = (_texture.Width) / MinMaxValues.Y;
                    int touchPointOnSliderX = _currentmouse.X - Rectangle.X;
                    ToggleValue = (int)(touchPointOnSliderX / multiple);
                    UpdateValue?.Invoke(this, this);
                }
            }
            Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);
        }
    }
}
