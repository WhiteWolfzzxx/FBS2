using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace TetrisRemasteredXNA
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        enum GameStates { CreditScreen, MainMenu, Playing, GameOver, HighScoreScreen, PauseGame, Debug, Controls };
        GameStates gameState = GameStates.CreditScreen;

        Texture2D scoreBackground;

        SpriteFont testFont;
        SpriteFont playFont;

        Song playBGM;
        Song menuBGM;
        Song gameOverBGM;

        Vector2[,] lines = new Vector2[10, 20]; 		//block placeing grid
        bool[,] store = new bool[10, 20];				//Block storeing
        Texture2D[,] blocks = new Texture2D[10, 20];	//Block store show
        PlayerClass[] activeBlocks = new PlayerClass[4];	//Blocks that the player can move
        MainMenuClass mainMenuClass;
        ScoreClass scoreClass = new ScoreClass();
        SaveGameClass saveGameClass = new SaveGameClass();
        ControlsClass controlsClass = new ControlsClass();
        BoardClass boardClass;
        BlockHelper blockHelper;
        CreditClass creditClass;
        KeyboardState keyState;
        bool escapeDidSomething = false;
        bool spaceDidSomething = false;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 700;
            graphics.IsFullScreen = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            #region LoadContent
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //TODO: use this.Content to load your game content here 

            testFont = Content.Load<SpriteFont>(@"Fonts\Font1");
            playFont = Content.Load<SpriteFont>(@"Fonts\expFont");

            //BGM
            playBGM = Content.Load<Song>(@"Audio\discoTetrisTitleScreenMainMenu");
            menuBGM = Content.Load<Song>(@"Audio\song2");
            gameOverBGM = Content.Load<Song>(@"Audio\MenuBGM");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(menuBGM);

            //BoardClass
            boardClass = new BoardClass(blocks, store, lines);
            store = boardClass.resetStore();
            lines = boardClass.resetLinesGrid();
            blocks = boardClass.loadBlocksTexture(Content);

            scoreBackground = Content.Load<Texture2D>(@"Textures\ScoreBackground");

            //Load the player controled blocks
            blockHelper = new BlockHelper(activeBlocks, lines, store, playBGM);
            blockHelper.setColors();
            activeBlocks = blockHelper.loadPlayerBlocks(Content);

            //MainMenuClass
            mainMenuClass = new MainMenuClass(testFont, playFont);
            mainMenuClass.LoadContent(Content);

            //CreditClass
            creditClass = new CreditClass(testFont, playFont);
            #endregion
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // For Mobile devices, this logic will close the Game when the Back button is pressed
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            keyState = Keyboard.GetState();

            if (gameState == GameStates.CreditScreen)
            {
                //Update BGM and GameState
                if (creditClass.ChangeScreen(gameTime))
                {
                    gameState = GameStates.MainMenu;
                }
            }

            if (gameState == GameStates.MainMenu)
            {
                mainMenuClass.Update(gameTime);
                if (keyState.IsKeyDown(Keys.Space) && !spaceDidSomething)
                {
                    switch (mainMenuClass.detectGameState())
                    {
                        case 1:
                            store = boardClass.resetStore();
                            for (int i = 0; i < activeBlocks.Length; i++)
                            {
                                activeBlocks[i].setStore(store);
                            }
                            blockHelper.setLevel(1);
                            blockHelper.setScore(0);
                            blockHelper.setTotalClearedLine(0);
                            gameState = GameStates.Playing;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(playBGM);
                            break;

                        case 2:
                            gameState = GameStates.HighScoreScreen;
                            break;

                        case 3:
                            gameState = GameStates.Controls;
                            break;

                        case 4:
                            Exit();
                            break;

                        case 5:
                            store = saveGameClass.loadGameData();
                            for (int i = 0; i < activeBlocks.Length; i++)
                            {
                                activeBlocks[i].setStore(store);
                            }
                            MediaPlayer.Play(playBGM);
                            gameState = GameStates.Playing;
                            break;

                        default:

                            break;
                    }
                    spaceDidSomething = true;
                }
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.Debug))
            {
                blockHelper.setActiveBlocks(activeBlocks);
                blockHelper.BlockHelperUpdate(gameTime);		//The Method that asks questions about all player blocks
                activeBlocks = blockHelper.getActiveBlocks();

                //pause game
                if (keyState.IsKeyDown(Keys.Escape) && !escapeDidSomething)
                {
                    gameState = GameStates.PauseGame;
                    escapeDidSomething = true;
                }

                #region Gameover Detection
                for (int i = 0; i < 10; i++)
                {
                    if (store[i, 0] == true)
                    {
                        scoreClass.recordScore(blockHelper.getScore());
                        gameState = GameStates.GameOver;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(menuBGM);
                        MediaPlayer.IsRepeating = true;
                    }
                }
                #endregion
            }

            if (gameState == GameStates.Playing)
            {
                //debug screen
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    gameState = GameStates.Debug;
                }

                if (Keyboard.GetState().IsKeyDown(Keys.S))
                {
                    saveGameClass.recordGameData(store);
                    MediaPlayer.Play(menuBGM);
                    gameState = GameStates.MainMenu;
                }
            }

            if (gameState == GameStates.Debug)
            {

            }

            if (gameState == GameStates.PauseGame)
            {
                //unpause game
                if (keyState.IsKeyDown(Keys.Escape) && !escapeDidSomething)
                {
                    gameState = GameStates.Playing;
                    escapeDidSomething = true;
                }
            }

            if (gameState == GameStates.GameOver)
            {
                //Main Menu
                if (keyState.IsKeyDown(Keys.Space) && !spaceDidSomething)
                {
                    gameState = GameStates.MainMenu;
                    spaceDidSomething = true;
                }
            }

            if (gameState == GameStates.HighScoreScreen)
            {
                scoreClass.ScoreClassUpdate(gameTime);

                //Main Menu
                if (keyState.IsKeyDown(Keys.Space) && !spaceDidSomething)
                {
                    gameState = GameStates.MainMenu;
                    spaceDidSomething = true;
                }
            }

            if (gameState == GameStates.Controls)
            {
                controlsClass.Update(gameTime);
                if (controlsClass.getFull())
                {
                    graphics.ToggleFullScreen();
                }

                if (keyState.IsKeyDown(Keys.Space) && !spaceDidSomething)
                {
                    gameState = GameStates.MainMenu;
                    spaceDidSomething = true;
                }
            }

            //Single Key press escape
            if (keyState.IsKeyDown(Keys.Escape) && escapeDidSomething)
            {
                escapeDidSomething = true;
            }
            else
            {
                escapeDidSomething = false;
            }

            //Single Key press Space
            if (keyState.IsKeyDown(Keys.Space) && spaceDidSomething)
            {
                spaceDidSomething = true;
            }
            else
            {
                spaceDidSomething = false;
            }

            // TODO: Add your update logic here			
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            //TODO: Add your drawing code here

            spriteBatch.Begin();

            if (gameState == GameStates.CreditScreen)
            {
                creditClass.Draw(spriteBatch);
            }

            if (gameState == GameStates.MainMenu)
            {
                mainMenuClass.Draw(spriteBatch);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.PauseGame) ||
                (gameState == GameStates.Debug))
            {

                spriteBatch.Draw(scoreBackground, new Vector2(300, 0), Color.White);

                //Draws the player controled blocks
                for (int i = 0; i < activeBlocks.Length; i++)
                {
                    activeBlocks[i].Draw(spriteBatch);
                }

                blockHelper.Draw(spriteBatch, blocks);
            }

            if ((gameState == GameStates.Playing) ||
                (gameState == GameStates.PauseGame))
            {
                spriteBatch.DrawString(playFont, "Score: " + blockHelper.getScore().ToString(), new Vector2(325, 300), Color.White);
                spriteBatch.DrawString(playFont, "Lines: " + blockHelper.getTotalClearedLines().ToString(), new Vector2(325, 350), Color.White);
                spriteBatch.DrawString(playFont, "Level: " + blockHelper.getLevel().ToString(), new Vector2(325, 400), Color.White);
            }

            if (gameState == GameStates.Debug)
            {
                spriteBatch.DrawString(testFont, "STORE: " + store[9, 19].ToString(), new Vector2(325, 400), Color.White);
                spriteBatch.DrawString(testFont, "SO: " + saveGameClass.getSo().ToString(), new Vector2(325, 425), Color.White);
                spriteBatch.DrawString(testFont, "Next Pattern: " + activeBlocks[0].getNextPattern().ToString(), new Vector2(325, 450), Color.White);
                spriteBatch.DrawString(testFont, "Pattern: " + activeBlocks[0].getPattern().ToString(), new Vector2(325, 475), Color.White);
                spriteBatch.DrawString(testFont, "Block Speed: " + activeBlocks[0].getMinTimer().ToString(), new Vector2(325, 500), Color.White);
                spriteBatch.DrawString(testFont, "Score: " + blockHelper.getScore().ToString(), new Vector2(325, 525), Color.White);
                spriteBatch.DrawString(testFont, "Lines: " + blockHelper.getTotalClearedLines().ToString(), new Vector2(325, 550), Color.White);
                spriteBatch.DrawString(testFont, "Level: " + blockHelper.getLevel().ToString(), new Vector2(325, 575), Color.White);
            }

            if (gameState == GameStates.PauseGame)
            {
                spriteBatch.DrawString(playFont, "PAUSE", new Vector2(325, 450), Color.White);
            }

            if (gameState == GameStates.GameOver)
            {

            }

            if (gameState == GameStates.Controls)
            {
                controlsClass.Draw(spriteBatch, playFont);
            }

            if (gameState == GameStates.HighScoreScreen)
            {
                scoreClass.Draw(spriteBatch, testFont);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
