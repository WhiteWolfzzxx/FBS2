using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace TetrisRemasteredXNA
{
    public class ControlsClass
    {
        private bool fDidSomething = false;
        private bool toggleFullScreen = false;

        public bool getFull() { return toggleFullScreen; }

        public void ControlClass()
        {
           
        }

        public void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F) && !fDidSomething)
            {
                toggleFullScreen = true;
                fDidSomething = true;
            }
            else
            {
                toggleFullScreen = false;
            }

            //Single Key press Space
            if (Keyboard.GetState().IsKeyDown(Keys.F) && fDidSomething)
            {
                fDidSomething = true;
            }
            else
            {
                fDidSomething = false;
            }
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            spriteBatch.DrawString(font, "Press F to fullscreen", new Vector2(100,100), Color.White);
        }
    }
}
