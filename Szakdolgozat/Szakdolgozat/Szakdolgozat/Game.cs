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
        List<CustomModel> models = new List<CustomModel>();
        Camera camera;

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

            models.Add(new CustomModel(Content.Load<Model>("female elf-fbx"),Vector3.Zero,Vector3.Zero,new Vector3(2.0f),GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("Lightbulb"), Vector3.Zero, Vector3.Zero, new Vector3(2.0f), GraphicsDevice));
           // camera = new TargetCamera(new Vector3(300, 300, 1800),Vector3.Zero, GraphicsDevice);
            camera = new ChaseCamera(new Vector3(0, 400, 1500),new Vector3(0, 200, 0),new Vector3(0, 0, 0), GraphicsDevice);
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
            camera.Update();
            updateModel(gameTime);
            updateCamera(gameTime);
            base.Update(gameTime);
        }

        void updateModel(GameTime gameTime)
        {
            KeyboardState keyState = Keyboard.GetState();
            Vector3 rotChange = new Vector3(0, 0, 0);
            // Determine on which axes the ship should be rotated on, if any
            if (keyState.IsKeyDown(Keys.W))
                 rotChange += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.S))
                rotChange += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                rotChange += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.D))
                rotChange += new Vector3(0, -1, 0);
            models[0].Rotation += rotChange * .025f;
            // If space isn't down, the ship shouldn't move
            if (!keyState.IsKeyDown(Keys.Space))
                return;
            // Determine what direction to move in
            Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X,models[0].Rotation.Z);
            // Move in the direction dictated by our rotation matrix
            models[0].Position += Vector3.Transform(Vector3.Forward,rotation) * (float)gameTime.ElapsedGameTime.TotalMilliseconds *4;
        }

        void updateCamera(GameTime gameTime)
        {
            // Move the camera to the new model's position and orientation
            ((ChaseCamera)camera).Move(models[0].Position,models[0].Rotation);
            // Update the camera
            camera.Update();
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here

            foreach (CustomModel model in models)
                model.Draw(camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
