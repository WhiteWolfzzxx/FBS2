﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

namespace TetrisRemasteredXNA
{
    public class PlayerClass
    {
        private Random random = new Random();
        private BlockConFigClass blockConFigClass;
        private int rotateState = 0;
        private int locationX;
        private int locationY;
        private int pattern;
        private int nextPattern = 4;
        private int index;
        private int level;
        private Texture2D block;
        private Vector2[,] lines;
        private float moveTimer = 0.0f;
        private float minMoveTimer = 0.1f;
        private float downTimer = 0.0f;
        private float minDownTimer = 0.0f;
        private float rotateTimer = 0.0f;
        private float minRotateTimer = 0.2f;
        private bool stopActiveBlocks; 		//This is used to syic the blocks together
        private bool canRotateBlocks = true;
        private bool cantRotateOtherBlock = false;
        private bool canGoLeft = true;
        private bool otherCantGoLeft = false;
        private bool canGoRight = true;
        private bool otherCantGoRight = false;
        private bool[,] store;

        public void setStopActiveBlocks(bool stop) { stopActiveBlocks = stop; }
        public void setCantRotateOtherBlocks(bool rotate) { cantRotateOtherBlock = rotate; }
        public void setOtherCantGoLeft(bool left) { otherCantGoLeft = left; }
        public void setOtherCantGoRight(bool right) { otherCantGoRight = right; }
        public void setStore(bool[,] st) { store = st; }

        public bool getStopActiveBlocks() { return stopActiveBlocks; }
        public bool getCanRotateBlocks() { return canRotateBlocks; }
        public bool getCanGoLeft() { return canGoLeft; }
        public bool getCanGoRight() { return canGoRight; }
        public int getPattern() { return pattern; }
        public int getNextPattern() { return nextPattern; }

        public PlayerClass(int locationX1, int locationY1, Texture2D block1, Vector2[,] lns, int patt, int inx, bool[,] st)
        {
            locationX = locationX1;
            locationY = locationY1;
            block = block1;
            lines = lns;
            pattern = patt;
            index = inx;
            blockConFigClass = new BlockConFigClass(pattern, index, block1);
            locationX = blockConFigClass.resetPattern("X");
            locationY = blockConFigClass.resetPattern("Y");
            store = st;
        }

        public bool[,] PlayerUpdate(GameTime gameTime)
        {
            //Set timers
            HandleKeyboardInput(Keyboard.GetState(), store);
            moveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            downTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            rotateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            canRotateBlock();
            checkLeft();
            checkRight();

            //Stores location of player controled blocks when collides
            if ((locationY == 19) || (store[locationX, (locationY + 1)] == true) || (stopActiveBlocks == true))
            {
                stopActiveBlocks = true;
                store[locationX, locationY] = true;
            }

            //PlayerBlock will move downward forcefully HAHAHA!!!!
            if (downTimer >= minDownTimer)
            {
                locationY += 1;
                downTimer = 0.0f;
            }

            return store;
        }

        public void setLevel(int lv)
        {
            level = lv;
        }

        //Checks to see if blocks can go right
        private void checkRight()
        {
            if ((locationX == 9) ||
                store[(locationX + 1), locationY])
            {
                canGoRight = false;
            }
            else
            {
                canGoRight = true;
            }
        }

        //Checks to see if blocks can go left
        private void checkLeft()
        {
            if ((locationX == 0) ||
                store[(locationX - 1), locationY])
            {
                canGoLeft = false;
            }
            else
            {
                canGoLeft = true;
            }
        }

        //Checks to detect other blocks or bounderies to rotate
        private void canRotateBlock()
        {
            if ((locationY == 0) ||
                (locationX == 0) ||
                (locationX == 9) ||
                (store[(locationX - 1), locationY] == true) ||
                (store[(locationX + 1), locationY] == true))
            {
                canRotateBlocks = false;
            }
            else
            {
                canRotateBlocks = true;
            }
        }

        //Temp
        public float getMinTimer()
        {
            return minDownTimer;
        }

        private void HandleKeyboardInput(KeyboardState keyState, bool[,] store)
        {
            if ((keyState.IsKeyDown(Keys.Up)) && canRotateBlocks && !cantRotateOtherBlock)
            {
                locationX = blockConFigClass.rotatePattern(locationX, locationY, rotateTimer, minRotateTimer, rotateState, 1);
                locationY = blockConFigClass.rotatePattern(locationX, locationY, rotateTimer, minRotateTimer, rotateState, 2);
                rotateState = blockConFigClass.rotatePattern(locationX, locationY, rotateTimer, minRotateTimer, rotateState, 3);

                if (blockConFigClass.getRotateTimer() == 0.0f)
                {
                    rotateTimer = 0.0f;
                }
            }
            if (keyState.IsKeyDown(Keys.Down))
            {
                minDownTimer = 0.05f;
                if (locationY >= 19)
                {
                    locationY = 19;
                }
            }
            else
            {
                if (level < 6)
                {
                    minDownTimer = 1.0f - ((level - 1) * 0.2f);
                }
                else
                {
                    minDownTimer = 0.1f;
                }
            }
            if (keyState.IsKeyDown(Keys.Left) && canGoLeft && !otherCantGoLeft)
            {
                if (moveTimer >= minMoveTimer)
                {
                    locationX -= 1;
                    moveTimer = 0.0f;
                }
                checkBounds(store);

            }
            if (keyState.IsKeyDown(Keys.Right) && canGoRight && !otherCantGoRight)
            {
                if (moveTimer >= minMoveTimer)
                {
                    locationX += 1;
                    moveTimer = 0.0f;
                }

                checkBounds(store);
            }
        }

        public void resetBlocks()
        {
            pattern = nextPattern;
            nextPattern = random.Next(1, 8);		////////////THIS IS THE PATTERN RANDOMIZER: CHANGE WHEN TESTING!!!!!
            blockConFigClass = new BlockConFigClass(pattern, index, block);
            locationX = blockConFigClass.resetPattern("X");
            locationY = blockConFigClass.resetPattern("Y");
            rotateState = 0;
        }

        private void checkBounds(bool[,] store)
        {
            if (locationX > 9)
            {
                locationX = 9;
            }
            else if (locationX < 0)
            {
                locationX = 0;
            }
            else if (store[locationX, locationY] == true)
            {
                locationX -= 1;
            }
            else if (store[locationX, locationY] == true)
            {
                locationX += 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(block, lines[locationX, locationY], Color.LightGray);
            blockConFigClass.DrawNextPattern(spriteBatch, nextPattern);
        }
    }
}

