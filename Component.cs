using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace monoSlime2
{
    public abstract class Component
    {
        public enum Component_Type
        {
            Button,
            ButtonSmall,
            TextBox
        }
        

        public abstract void Draw(GameTime gameTime, SpriteBatch sprite);
        public abstract void Update(GameTime gameTime, GameWindow window);
    }
}
