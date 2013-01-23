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
        CustomModel catchableObject;
        Camera camera;
        ObjectAnimation anim;
        int score = 0;
  
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
            models.Add(new CustomModel(Content.Load<Model>("body"),Vector3.Zero,new Vector3(0,0,0),new Vector3(50.0f),GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("righthand"), new Vector3(-400,200,0), new Vector3(0, 0, 0), new Vector3(50.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("lefthand"), new Vector3(400, 200, 0), new Vector3(0, 0, 0), new Vector3(50.0f), GraphicsDevice));
            catchableObject = new CustomModel(Content.Load<Model>("Lightbulb"), new Vector3(300,-150,0), Vector3.Zero, new Vector3(.5f), GraphicsDevice);
            camera = new TargetCamera(new Vector3(0, 0, 1200),Vector3.Zero, GraphicsDevice);
            //camera = new ChaseCamera(new Vector3(0, 400, 1500),new Vector3(0, 200, 0),new Vector3(0, 0, 0), GraphicsDevice);
            anim = new ObjectAnimation(new Vector3(0, -150, 0),new Vector3(0, 150, 0),Vector3.Zero,
                                       new Vector3(0, -MathHelper.TwoPi, 0),TimeSpan.FromSeconds(10), true);
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || score == 4)
                this.Exit();

            // TODO: Add your update logic here
            camera.Update();
            updateModel(gameTime);
            updateCamera(gameTime);
            base.Update(gameTime);
            anim.Update(gameTime.ElapsedGameTime);
            //models[1].Position = anim.Position;
            //models[1].Rotation = anim.Rotation;
        }

        void updateModel(GameTime gameTime)
        {
            //KeyboardState keyState = Keyboard.GetState();
            //Vector3 rotChange = new Vector3(0, 0, 0);
            //// Determine on which axes the ship should be rotated on, if any
            //if (keyState.IsKeyDown(Keys.W))
            //     rotChange += new Vector3(1, 0, 0);
            //if (keyState.IsKeyDown(Keys.S))
            //    rotChange += new Vector3(-1, 0, 0);
            //if (keyState.IsKeyDown(Keys.A))
            //    rotChange += new Vector3(0, 1, 0);
            //if (keyState.IsKeyDown(Keys.D))
            //    rotChange += new Vector3(0, -1, 0);
            //models[0].Rotation += rotChange * .025f;
            //// If space isn't down, the ship shouldn't move
            //if (!keyState.IsKeyDown(Keys.Space))
            //    return;
            //// Determine what direction to move in
            //Matrix rotation = Matrix.CreateFromYawPitchRoll(models[0].Rotation.Y, models[0].Rotation.X,models[0].Rotation.Z);
            //// Move in the direction dictated by our rotation matrix
            //models[0].Position += Vector3.Transform(Vector3.Forward,rotation) * (float)gameTime.ElapsedGameTime.TotalMilliseconds *4;
            if (collisionDetect())
            {
                score++;
                catchableObject.Position = generateRandomVector();
            }
            KeyboardState keyState = Keyboard.GetState();
            Vector3 posChange = new Vector3(0, 0, 0);
            if (keyState.IsKeyDown(Keys.D))
                posChange += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                posChange += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.W))
                posChange += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.S))
                posChange += new Vector3(0, -1, 0);
            models[2].Position += posChange * 10f;
            if (keyState.IsKeyUp(Keys.Space))
                return;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(models[2].Rotation.Y, models[2].Rotation.X, models[2].Rotation.Z);
            models[2].Position += Vector3.Transform(Vector3.Forward, rotation) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
        }

        void updateCamera(GameTime gameTime)
        {
            // Move the camera to the new model's position and orientation
            //((ChaseCamera)camera).Move(models[0].Position,models[0].Rotation);
            // Update the camera
            camera.Update();
        }

        public bool collisionDetect()
        {
            if (models[2].BoundingSphere.Intersects(catchableObject.BoundingSphere))
                return true;
            return false;
        }

        public Vector3 generateRandomVector()
        {
            Vector3 v3 = Vector3.Zero;
            Random rnd = new Random();
            v3.X = rnd.Next(200, 500);
            v3.Y = rnd.Next(250);
            if (rnd.Next(100) % 2 == 0)
                v3.X*=-1;
            if (rnd.Next(100) % 2 == 0)
                v3.Y *= -1;
            return v3;     
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
            if(!collisionDetect())
                catchableObject.Draw(camera.View, camera.Projection);

            base.Draw(gameTime);
        }
    }
}
