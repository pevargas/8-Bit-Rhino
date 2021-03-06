/*  File:       Game1.cs                    Feb 2013
 *  Author:     JKB                         University of Colorado Boulder
 *  Students:   8-Bit Rhino                 (Andrew Davis, Spencer Fink, Andrew Oetjen, Patrick Vargas)
 *  Description:
 *      The main program.
 *  Needed to follow this to get XACT to work: http://stackoverflow.com/questions/10307898/xna-4-0-cannot-create-an-audioengine
 */

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
using System.Diagnostics;
using System.Threading;

namespace ATLS_4519_Lab3
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        // Sprite Objects
        clsSprite ball;
        clsSprite opponent;
        clsSprite player;

        // SpriteBatch which will draw (render) the sprite
        SpriteBatch spriteBatch;

        // Create a SoundEffect resource
        SoundEffect walls, start, miss, win, lose, killshot;

        // Audio Objects
        AudioEngine audioEngine;
        SoundBank soundBank;
        WaveBank waveBank;

        WaveBank streamingWaveBank;
        Cue musicCue;

        // Font Objects
        SpriteFont Font1;
        Vector2 FontPos;
        bool isDone;
        int finalScore;
        public string victory; // used to hold the congratulations/blnt message

        // Set window deimensions for ease.
        int winX, winY;

        // Define threshold for oppenent's ai complexity
        int threshold;
        
        // Define enum for ease of programming
        enum Who { Player, Opponent };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // changing the back buffer size changes the window size (in windowed mode)
            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 600;
            threshold = graphics.PreferredBackBufferWidth / 2;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set window size
            winX = graphics.PreferredBackBufferWidth;
            winY = graphics.PreferredBackBufferHeight;

            // Initialize isDone boolean
            isDone = false;
            finalScore = 7;

            // Start first sound bite
            start = Content.Load<SoundEffect>("herewego");
            start.Play();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Set Some Variables
            float padX  = 24; // Paddle Width
            float padY  = 64; // Paddle Height
            float ballD = 32; // Ball Diameter
            Random rnd = new Random();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load the SoundEffect resource
            walls = Content.Load<SoundEffect>("VOLTAGE");
            miss  = Content.Load<SoundEffect>("neat");
            win   = Content.Load<SoundEffect>("score");
            lose  = Content.Load<SoundEffect>("miss");
            killshot = Content.Load<SoundEffect>("killshot");

            // Load files built from XACT project
            audioEngine = new AudioEngine("Content\\Lab3Sounds.xgs");
            waveBank    = new WaveBank(audioEngine, "Content\\Wave Bank.xwb");
            soundBank   = new SoundBank(audioEngine, "Content\\Sound Bank.xsb");

            // Load streaming wave bank
            streamingWaveBank = new WaveBank(audioEngine, "Content\\Music.xwb", 0, 4);
            // The audio engine must be updated before the streaming cue is ready
            audioEngine.Update();
            // Get cue for streaming music
            musicCue = soundBank.GetCue("EricJordan_2012_30sec");
            // Start the background music
            musicCue.Play();

            // Load Font
            Font1 = Content.Load<SpriteFont>("Courier New");

            // Load a 2D texture sprite
            ball     = new clsSprite(Content.Load<Texture2D>("volt-ball-final"), 
                            new Vector2(winX/2, winY/2), new Vector2(ballD, ballD),
                            winX, winY);
            opponent = new clsSprite(Content.Load<Texture2D>("vt_left_paddle"),
                            new Vector2(10, winY/2 - padY), new Vector2(padX, padY),
                            winX, winY);
            player   = new clsSprite(Content.Load<Texture2D>("vt_right_paddle"),
                            new Vector2(winX - (10+padX), winY / 2 - padY), new Vector2(padX, padY),
                            winX, winY);
            // Create a SpriteBatch to render the sprite
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // Set the velocity of the ball sprites will move
            ball.velocity = new Vector2(rnd.Next(3,6), rnd.Next(-5, 6));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            //  Free the previously allocated resources
            ball.texture.Dispose();
            opponent.texture.Dispose();
            player.texture.Dispose();
            spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Get User input
            KeyboardState keyboardState = Keyboard.GetState();

            // Add chance to the mix
            Random rnd = new Random();
            // Move ball and get information
            int wall = ball.Move();
            // Check for kill shot
            int[] kill = new int [] { 0, 0 };

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Check if isDoneed
            if (ball.score[(int)Who.Player] >= finalScore) {
                victory = "That's pretty neat! You won!";
                isDone = true;
            }
            else if (ball.score[(int)Who.Opponent] >= finalScore) {
                victory = "G Dangit! You lost!";
                isDone = true;
            }

            // Move the sprite
            if (wall > 0) { // Check if hit wall
                switch (wall) {
                    case 2: // Player Miss
                        threshold -= 10;
                        ball.position = new Vector2(winX / 2, winY / 2);
                        ball.velocity = new Vector2(-rnd.Next(3, 6), rnd.Next(-5, 6));
                        miss.Play(1f, 1f, 0f);
                        break;
                    case 3: // Opponent Miss
                        threshold += 10;
                        ball.position = new Vector2(winX / 2, winY / 2);
                        ball.velocity = new Vector2(rnd.Next(3, 6), rnd.Next(-5, 6));
                        miss.Play(1f, 1f, 0f);
                        break;
                    default:
                        walls.Play(.50f, .75f, 0f);
                        break;
                }
            }

            if (!isDone) {

                // Change the sprite 2 position using the left thumbstick of the Xbox 360 controller
                // Vector2 LeftThumb = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left;
                // opponent.position += new Vector2(LeftThumb.X, -LeftThumb.Y) * 5;
                
                //  Change the sprite 2 position using the keyboard
                if (keyboardState.IsKeyDown(Keys.Up) && player.position.Y > 0) { player.position += new Vector2(0, -5); }
                if (keyboardState.IsKeyDown(Keys.Down) && player.position.Y < (winY-player.size.Y)) { player.position += new Vector2(0, 5); }

                // Opponent Algorithm
                if (ball.position.X < threshold) {
                    if (opponent.position.Y > ball.position.Y) { opponent.position += new Vector2(0, -5); }
                    if (opponent.position.Y < ball.position.Y) { opponent.position += new Vector2(0, 5); }
                }

                // Collision Detection and Handeling
                if ((kill[(int)Who.Player] = ball.Collides(player)) > 0 ||
                    (kill[(int)Who.Opponent] = ball.Collides(opponent)) > 0) {
                        if (kill[(int)Who.Player] == 2 || kill[(int)Who.Opponent] == 2) { //Kill Shot!
                            ball.velocity = new Vector2(-ball.velocity.X + (rnd.Next(-10, 10)), ball.velocity.Y + (rnd.Next(-10, 10)));
                            killshot.Play(1f, 1f, 0f);
                        }
                        else
                            ball.velocity = new Vector2(-ball.velocity.X + (rnd.Next(-2, 2)), ball.velocity.Y + (rnd.Next(-2, 2)));
                    GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
                    walls.Play(1f, 1f, 0f);
                    }
                else
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                // Update the audio engine
                audioEngine.Update();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // Draw the sprite using Alpha Blend, which uses transparency information if available 
            // In 4.0, this behavior is the default; in XNA 3.1, it is not
            spriteBatch.Begin();

            if (!isDone) {
                // Write Scores
                spriteBatch.DrawString(Font1, "Opponent: " + ball.score[(int)Who.Opponent], new Vector2(5, 10), Color.Green);
                spriteBatch.DrawString(Font1, "Player: " + ball.score[(int)Who.Player], new Vector2(winX - Font1.MeasureString("Player: " + ball.score[0]).X - 5, 10), Color.Green);
                spriteBatch.DrawString(Font1, "Vel: ( " + ball.velocity.X + ", " + ball.velocity.Y + " ) Threshold: " + threshold, new Vector2(5, winY - Font1.LineSpacing), Color.Green);
            
                // Draw physical objects
                ball.Draw(spriteBatch);
                opponent.Draw(spriteBatch);
                player.Draw(spriteBatch);
            }
            else {
                FontPos = new Vector2((winX / 2) - 400, (winY / 2) - 50);
                spriteBatch.DrawString(Font1, victory, FontPos, Color.Green);
                ball.score[0] = ball.score[1] = 0;
            }

            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
