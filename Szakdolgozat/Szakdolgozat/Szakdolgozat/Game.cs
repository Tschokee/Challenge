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
using Microsoft.Kinect;

namespace Szakdolgozat
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        //KinectSensor ks = KinectSensor.KinectSensors[0];
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<CustomModel> models = new List<CustomModel>();

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1200;
            graphics.PreferredBackBufferHeight = 600;
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
            // TODO: use this.Content to load your game content here
            // Create a new SpriteBatch, which can be used to draw textures.
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    Vector3 position = new Vector3(-600 + j * 600, -400 + i * 400, 0);
                    models.Add(new CustomModel(Content.Load<Model>("female elf-fbx"), 
                    position,
                    new Vector3(0, MathHelper.ToRadians(90) * (i * 3 + j), 0),
                    new Vector3(1.25f),
                    GraphicsDevice));
                }
        }

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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            Matrix view = Matrix.CreateLookAt(new Vector3(0, 300, 2000),new Vector3(0, 0, 0),Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45),
                                                                    GraphicsDevice.Viewport.AspectRatio,
                                                                    0.1f,
                                                                    10000.0f);
            // Calculate the starting world matrix
            Matrix baseWorld = Matrix.CreateScale(0.4f) *
            Matrix.CreateRotationY(MathHelper.ToRadians(180));

            foreach (CustomModel model in models)
                model.Draw(view, projection);

            base.Draw(gameTime);
        }
    }
}
