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
    public class Textbox : Component
    {
        public Component_Type type = Component_Type.TextBox;
        #region Fields
        private SpriteFont _font;
        private Texture2D _texture;
        private double _lastGametime;
        private double _currentGametime;

        #endregion

        #region Properties
        public string _name;
        public event EventHandler<Textbox> UpdateText;
        //EventArgs args = new EventArgs();
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


        public Textbox(Texture2D texture, SpriteFont font, string name)
        {
            _texture = texture;
            _font = font;
            PenColour = Color.LightBlue;
            _name = name;
        }
        public override void Draw(GameTime gameTime, SpriteBatch sprite)
        {
            var color = Color.White;
            sprite.Draw(_texture, Rectangle, color);

            if (!string.IsNullOrEmpty(Text))
            {
                var x = (Rectangle.X + (Rectangle.Width / 2)) - (_font.MeasureString(Text).X / 2);
                var y = (Rectangle.Y + (Rectangle.Height / 2)) - (_font.MeasureString(Text).Y / 2);

                sprite.DrawString(_font, Text, new Vector2(x, y), PenColour);
            }
        }

        public override void Update(GameTime gameTime, GameWindow window)
        {
           


            UpdateText?.Invoke(this, this);


            Position = new Vector2(window.ClientBounds.Width * PositionScale.X, window.ClientBounds.Height * PositionScale.Y);



        }


        
        
          
    }
}
