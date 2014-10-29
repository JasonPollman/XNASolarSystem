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

namespace SolarSystem
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {

        // An array of solar bodies, which will be our sun and planets
        Planet[] solarBodies = new Planet[10];
           
        // The keyboard state on the last update
        KeyboardState kLast;
        MouseState mLast;

        // To manipulate the speed via Keys.Up/Keys.Down
        int speedMultiplier = 4;
        const int MAX_SPEED_MULTIPLIER = 12;
        const int MIN_SPEED_MULTIPLIER = 0;

        // To manipulate the body sprite size via Keys.OemPlus/Keys.OemMinus
        float imgScale = .5f;
        const float MIN_SCALE_SIZE = .1f;
        const float MAX_SCALE_SIZE = 1.0f;

        // To manipulate the distance between planets via Keys.Left/Keys.Right
        int distanceMultiplier = 2500;
        const int MIN_DISTANCE_MULTIPLIER = 2000;
        const int MAX_DISTANCE_MULTIPLIER = 5000;

        // The background texture
        Texture2D background;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Matrix SpriteScale;
        Vector2 distTotal = new Vector2(0,0);

        SpriteFont font;

        // To calculate the number of earth years has passed...
        double earthYears = -.5;
        bool yPos = false;
        bool yNeg = false;
        bool gotYPos = false;
        bool gotYNeg = false;
        int gotHover = -1;

        // To change time direction
        bool timeForward = true;

        // The theta angle for each solar body
        double[] theta = new double[10] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set the game width and height to the current screen resolution, and default to full screen;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth  = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.IsFullScreen = true;

            // Allow resizing
            Window.AllowUserResizing = true;

            // Show the mouse
            IsMouseVisible = true;

        } // End constructor

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            kLast = Keyboard.GetState();

        } // End Initialize

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load a font to display text on the screen
            font = Content.Load<SpriteFont>("font/ArialBold");
            
            // Load the background texture
            background = Content.Load<Texture2D>(@"img/stars");

            float screenscaleX = (float) graphics.GraphicsDevice.Viewport.Width  / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float screenscaleY = (float) graphics.GraphicsDevice.Viewport.Height / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            // So that we can scale the sprites when the user changes the window size...
            SpriteScale = Matrix.CreateScale(screenscaleX, screenscaleY, 1);

            // A list of names for each planet so we can create the planets using a loop.
            String[] names = new String[10] { @"sun", @"mercury", @"venus", @"earth", @"mars", @"jupiter", @"saturn", @"uranus", @"neptune", @"pluto" };

            // Oribital distances for each planet: X, Y;
            Vector2[] distanceToPreviousPlanet = new Vector2[10] {
                new Vector2(0, 0),
                new Vector2(2, 1),
                new Vector2(4, 2),
                new Vector2(6, 3),
                new Vector2(8, 4),
                new Vector2(10, 5),
                new Vector2(12, 6),
                new Vector2(14, 7),
                new Vector2(16, 8),
                new Vector2(18, 9)
            
            };

            // The revolution speeds, based on true figures.
            float[] revolutionSpeed = new float[10] { 0f, .01607f, .01174f, .01f, .00802f, .00434f, .00323f, .00228f, .00182f, .00159f };

            for (int i = 0; i < solarBodies.Length; i++)
            {
                distTotal += distanceToPreviousPlanet[i];
                // Load the planet's image
                Texture2D file = Content.Load<Texture2D>(@"img/" + names[i]);
                // Create the Planet Object
                solarBodies[i] = new Planet(i, names[i], file, new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - file.Width * imgScale / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - file.Height * imgScale / 2), revolutionSpeed[i], distanceToPreviousPlanet[i]);
            }

        } // End LoadContent()

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // Enter/Exit full screen mode
            KeyboardState k = Keyboard.GetState();
            if (k.IsKeyDown(Keys.Escape) && !kLast.IsKeyDown(Keys.Escape))
            {
                graphics.IsFullScreen = false;
                graphics.PreferredBackBufferHeight = (int)((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height * .75f);
                graphics.PreferredBackBufferWidth  = (int)((float)GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width * .75f);
                graphics.ApplyChanges();
            }
            else if(k.IsKeyDown(Keys.F12) && !kLast.IsKeyDown(Keys.F12))
            {
                graphics.IsFullScreen = true;
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth  = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.ApplyChanges();
            }
            
            // Increment / Decrement the revolution speed
            if (k.IsKeyDown(Keys.Up) && !kLast.IsKeyDown(Keys.Up))
                speedMultiplier += (speedMultiplier < MAX_SPEED_MULTIPLIER) ? 1 : 0;
            else if (k.IsKeyDown(Keys.Down) && !kLast.IsKeyDown(Keys.Down))
                speedMultiplier -= (speedMultiplier > MIN_SPEED_MULTIPLIER) ? 1 : 0;

            if (k.IsKeyDown(Keys.Right) && !kLast.IsKeyDown(Keys.Right))
                distanceMultiplier += (distanceMultiplier < MAX_DISTANCE_MULTIPLIER) ? 100 : 0;
            else if (k.IsKeyDown(Keys.Left) && !kLast.IsKeyDown(Keys.Left))
                distanceMultiplier -= (distanceMultiplier > MIN_DISTANCE_MULTIPLIER) ? 100 : 0;


            if (k.IsKeyDown(Keys.T) && !kLast.IsKeyDown(Keys.T))
                timeForward = !timeForward;

            // Increment / Decrement the planet size
            if (k.IsKeyDown(Keys.OemPlus) && !kLast.IsKeyDown(Keys.OemPlus)) {
                imgScale += (imgScale < MAX_SCALE_SIZE) ? .1f : 0;
                foreach(Planet e in solarBodies) e.setPosition(new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - e.Img.Width * imgScale / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - e.Img.Height * imgScale / 2));
            }
            else if (k.IsKeyDown(Keys.OemMinus) && !kLast.IsKeyDown(Keys.OemMinus))
            {
                imgScale -= (imgScale > MIN_SCALE_SIZE + .01) ? .1f : 0;
                foreach(Planet e in solarBodies) e.setPosition(new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - e.Img.Width * imgScale / 2, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - e.Img.Height * imgScale / 2));
            }


            kLast = k;
            
            // Update the planet's position based on the planet size, speed, and distance in relation
            // to its neighboring planets.
            int i = 0;
            foreach (Planet e in solarBodies)
            {
                if (e.Name == @"sun")
                    continue;

                float reverse = (timeForward) ? 1 : -1;

                theta[i] = (theta[i] > 360) ? 0 : (theta[i] + e.Speed * speedMultiplier);
                e.setPosition(new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width / 2 - e.Img.Width * imgScale / 2 + (int)(distanceMultiplier * (float)(e.Distance.X + (float)(MAX_DISTANCE_MULTIPLIER / distanceMultiplier)) * ((reverse == 1) ? Math.Cos(theta[i]) : Math.Sin(theta[i])) * Math.PI / 180),
                                       GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - e.Img.Height * imgScale / 2 + (int)(distanceMultiplier * (float)(e.Distance.Y + (float)(MAX_DISTANCE_MULTIPLIER / distanceMultiplier)) * ((reverse == 1) ? Math.Sin(theta[i]) : Math.Cos(theta[i])) * Math.PI / 180)));

                if (e.Name == @"earth")
                {
                    yPos = (e.Position.Y - GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 > 0) ? true : false;
                    yNeg = (e.Position.Y - GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 < 0) ? true : false;

                    if (yPos) gotYPos = true;
                    if (yNeg) gotYNeg = true;

                    if (gotYPos && gotYNeg)
                    {
                        gotYNeg = gotYPos = false;
                        earthYears = (timeForward) ? earthYears + .5 : earthYears - .5;
                    }

                }
                i++;
            }

            // Update the sprite scale, so that the sprites will resize on window resize
            float screenscaleX = (float)graphics.GraphicsDevice.Viewport.Width / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            float screenscaleY = (float)graphics.GraphicsDevice.Viewport.Height / GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            SpriteScale = Matrix.CreateScale(screenscaleX, screenscaleY, 1);

            base.Update(gameTime);

        } // End update

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            MouseState m = Mouse.GetState();
           
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.Default, RasterizerState.CullNone, null, SpriteScale);
            
            // Draw the background
            spriteBatch.Draw(background, new Rectangle(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height), Color.White);
            
            // Draw the screen text / control instructions
            spriteBatch.DrawString(font, "Solar System\nJason Pollman\nITCS-4230-091", new Vector2(20, 20), Color.White);
            spriteBatch.DrawString(font, "Earth Years Passed: " + (int)earthYears + " (" + ((int)earthYears + Convert.ToInt32(DateTime.Now.Year.ToString())).ToString() + ")", new Vector2(20, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 110), new Color(227, 139, 0));
            spriteBatch.DrawString(font, "Speed: " + speedMultiplier.ToString() + "     Time: " + ((timeForward) ? "Forward" : "Backward") + "     Distance: " + distanceMultiplier.ToString() + "     Planet Size: " + imgScale.ToString(), new Vector2(20, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 85), new Color(107, 227, 0));
            spriteBatch.DrawString(font, "Controls:     (ESC/F12) Exit/Enter Full Screen     (+/-) Increase/Decrease Planet Size     (Up/Down) Increase/Decrease Revolution Speed     (Left/Right) Increase/Decrease Planet Distance     (T) Reverse Time", new Vector2(20, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - 60), Color.White);
            
            // Draw the Planets
            foreach (Planet e in solarBodies)
            {
                spriteBatch.Draw(e.Img, e.Position, null, Color.White, 0f, Vector2.Zero, imgScale, SpriteEffects.None, 0f);
                if (new Rectangle(m.X, m.Y, 100, 100).Intersects(new Rectangle((int)e.Position.X, (int)e.Position.Y, (int)(e.Img.Width * imgScale), (int)(e.Img.Height * imgScale))) && gotHover < 0)
                {
                    gotHover = e.Id;
                    spriteBatch.DrawString(font, e.Name.ToUpper(), e.Position + new Vector2(-20, -20), new Color(107, 227, 0));
                }
                else if (m.X == mLast.X && m.Y == mLast.Y && gotHover >= 0)
                {
                    spriteBatch.DrawString(font, solarBodies[gotHover].Name.ToUpper(), solarBodies[gotHover].Position + new Vector2(-20, -20), new Color(107, 227, 0));
                }
                else
                {
                    gotHover = -1;
                }
            }

            spriteBatch.End();
            mLast = m;

            base.Draw(gameTime);

        } // End Draw

    } // End Game1 Class

} // End namespace
