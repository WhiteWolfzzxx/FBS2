﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace TetrisRemasteredXNA
{
    public class MainMenuClass
    {
        private Texture2D title;
        private SpriteFont bigFont;
        private SpriteFont smallFont;
        private int menuOption = 1;
        private float menuChangeTimer;
        private float minMenuChangeTimer = 0.1f;
        private int redIntensity = 0;
        private int greenIntensity = 50;
        private int blueIntensity = 100;
        private bool redIncrease = true;
        private bool greenIncrease = true;
        private bool blueIncrease = true;

        public MainMenuClass(SpriteFont small, SpriteFont big)
        {
            bigFont = big;
            smallFont = small;
        }

        public void Update(GameTime gameTime)
        {
            colorChanger();
            menuChangeTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Navigate the menu
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                if (menuChangeTimer > minMenuChangeTimer)
                {
                    menuOption++;
                    menuChangeTimer = 0.0f;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if (menuChangeTimer > minMenuChangeTimer)
                {
                    menuOption--;
                    menuChangeTimer = 0.0f;
                }
            }

            //Resets the menu options
            if (menuOption > 6)
            {
                menuOption = 1;
            }
            if (menuOption < 1)
            {
                menuOption = 6;
            }
        }

        public void LoadContent(ContentManager Content)
        {
            title = Content.Load<Texture2D>(@"Textures\TetroTitle");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(title, new Vector2(50, 50), new Color(redIntensity, greenIntensity, blueIntensity));

            if (menuOption == 1)
            {
                spriteBatch.DrawString(smallFont, "Test", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "Play", new Vector2(290, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "High Scores", new Vector2(540, 325), Color.LightGray);
            }
            if (menuOption == 2)
            {
                spriteBatch.DrawString(smallFont, "Play", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "High Scores", new Vector2(200, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "Controls", new Vector2(540, 325), Color.LightGray);
            }
            if (menuOption == 3)
            {
                spriteBatch.DrawString(smallFont, "High Scores", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "Controls", new Vector2(240, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "Exit", new Vector2(540, 325), Color.LightGray);
            }
            if (menuOption == 4)
            {
                spriteBatch.DrawString(smallFont, "Controls", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "Exit", new Vector2(290, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "Load Game", new Vector2(540, 325), Color.LightGray);
            }
            if (menuOption == 5)
            {
                spriteBatch.DrawString(smallFont, "Exit", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "Load Game", new Vector2(200, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "Test", new Vector2(540, 325), Color.LightGray);
            }
            if (menuOption == 6)
            {
                spriteBatch.DrawString(smallFont, "Load Game", new Vector2(75, 325), Color.LightGray);
                spriteBatch.DrawString(bigFont, "Test", new Vector2(290, 300), Color.LightGray);
                spriteBatch.DrawString(smallFont, "Play", new Vector2(540, 325), Color.LightGray);
            }
        }

        public int detectGameState()
        {
            //playing state
            switch (menuOption)
            {
                case 1:
                    //Playing
                    return 1;
                    break;

                case 2:
                    //High Scores
                    return 2;
                    break;

                case 3:
                    //Controls
                    return 3;
                    break;

                case 4:
                    //Exit
                    return 4;
                    break;

                case 5:
                    //Load Game
                    return 5;
                    break;

                case 6:
                    //Windowed/Fullscreen
                    return 6;
                    break;

                default:
                    //Nothing is happening
                    return 0;
                    break;
            }
        }

        private void colorChanger()
        {
            if (redIncrease)
            {
                redIntensity++;
                if (redIntensity >= 240)
                {
                    redIncrease = false;
                }
            }
            else
            {
                redIntensity--;
                if (redIntensity <= 20)
                {
                    redIncrease = true;
                }
            }

            //blue
            if (blueIncrease)
            {
                blueIntensity += 2;
                if (blueIntensity >= 240)
                {
                    blueIncrease = false;
                }
            }
            else
            {
                blueIntensity -= 2;
                if (blueIntensity <= 20)
                {
                    blueIncrease = true;
                }
            }

            //green
            if (greenIncrease)
            {
                greenIntensity += 3;
                if (greenIntensity >= 240)
                {
                    greenIncrease = false;
                }
            }
            else
            {
                greenIntensity -= 3;
                if (greenIntensity <= 20)
                {
                    greenIncrease = true;
                }
            }
        }
    }
}

