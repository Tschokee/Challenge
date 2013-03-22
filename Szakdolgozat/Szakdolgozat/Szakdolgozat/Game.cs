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
        List<CustomModel> catchableObjects = new List<CustomModel>();
        Camera camera;
        ObjectAnimation anim;
        int[] score = new int[4];
        int bombCounter = new Random().Next(5,10);
        Random evilBombCounter = new Random();
        SoundEffect bombEffect, iceCreamEffect, victoryEffect, defeatEffect, extinguisherEffect;
  
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
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            models.Add(new CustomModel(Content.Load<Model>("body"),Vector3.Zero,new Vector3(0,0,0),new Vector3(50.0f),GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("righthand"), new Vector3(-400,200,0), new Vector3(0, 0, 0), new Vector3(30.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("lefthand"), new Vector3(400, 200, 0), new Vector3(0, 0, 0), new Vector3(30.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("foot"), new Vector3(100, 0, 0), new Vector3(0, 0, 0), new Vector3(30.0f, 50.0f, 50.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("foot"), new Vector3(-100, 0, 0), new Vector3(0, 0, 0), new Vector3(30.0f, 50.0f, 50.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("ground"), new Vector3(0, 0, -700), new Vector3(0, 0, 0), new Vector3(150.0f), GraphicsDevice));
            models.Add(new CustomModel(Content.Load<Model>("palm_tree"), new Vector3(-950, 260, -50), new Vector3(80, 0, 0), new Vector3(100.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("ice_cream"), new Vector3(300,-150,0), Vector3.Zero, new Vector3(100.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("ice_cream_2"), new Vector3(-300, -150, 0), Vector3.Zero, new Vector3(25.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("bomb"), new Vector3(200, -150, 0), Vector3.Zero, new Vector3(35.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("bomb"), new Vector3(-200, -150, 0), Vector3.Zero, new Vector3(35.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("f_e"), new Vector3(450, -200, 0), Vector3.Zero, new Vector3(25.0f), GraphicsDevice));
            catchableObjects.Add(new CustomModel(Content.Load<Model>("f_e"), new Vector3(-450, -200, 0), Vector3.Zero, new Vector3(25.0f), GraphicsDevice));
            camera = new TargetCamera(new Vector3(0, 0, 1200),Vector3.Zero, GraphicsDevice);
            anim = new ObjectAnimation(Vector3.Zero, Vector3.Zero, Vector3.Zero,
                                       new Vector3(0, -MathHelper.TwoPi, 0),
                                       TimeSpan.FromSeconds(10), true);
            bombEffect = Content.Load<SoundEffect>("Big Bomb-SoundBible.com-1219802495");
            iceCreamEffect = Content.Load<SoundEffect>("Tiny Button Push-SoundBible.com-513260752");
            victoryEffect = Content.Load<SoundEffect>("Applause-SoundBible.com-151138312");
            defeatEffect = Content.Load<SoundEffect>("Sad_Trombone-Joe_Lamb-665429450");
            extinguisherEffect = Content.Load<SoundEffect>("f_ex");
           
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (score[0] >= 10 && score[1] >= 10))
                this.Exit();

            camera.Update();
            updateModel(gameTime);
            updateCamera(gameTime);
            base.Update(gameTime);
            anim.Update(gameTime.ElapsedGameTime);
            foreach(CustomModel iceCream in catchableObjects)
                iceCream.Rotation = anim.Rotation;
        }

        void updateModel(GameTime gameTime)
        {
            for (int i = 0 ; i < 1 ; i++)
            {
                if (collisionDetect(i))
                {
                    if (!UCanSeeTheBomb(0))
                    {
                        score[i]++;
                        iceCreamEffect.Play();
                    }
                    catchableObjects[i].Position = generateRandomVector(i);
                    while (collisionDetect(i)) 
                       catchableObjects[i].Position = generateRandomVector(i);
                }
                if (UTouchedTheBomb(0))
                {
                    if (UCanSeeTheBomb(0))
                    {
                        bombEffect.Play();
                        score[2]++;
                        bombCounter += evilBombCounter.Next(5, 10);
                    }
                    catchableObjects[2].Position = generateRandomVector(0);
                }
                if (UCanSeeTheBomb(0))
                {
                    if (UTouchedTheExtinguisher(455))
                    {
                        extinguisherEffect.Play();
                        bombCounter += evilBombCounter.Next(5, 10);
                    }
                }
            }
           
            KeyboardState keyState = Keyboard.GetState();
            Vector3 posChange = new Vector3(0, 0, 0);
            Vector3 posChange2 = new Vector3(0, 0, 0);
            if (keyState.IsKeyDown(Keys.D))
                posChange += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                posChange += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.W))
                posChange += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.S))
                posChange += new Vector3(0, -1, 0);
            models[2].Position += posChange * 10f;

            if (keyState.IsKeyDown(Keys.H))
                posChange2 += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.F))
                posChange2 += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.T))
                posChange2 += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.G))
                posChange2 += new Vector3(0, -1, 0);
            models[4].Position += posChange2 * 10f;

            if (keyState.IsKeyUp(Keys.Space))
                return;
            Matrix rotation = Matrix.CreateFromYawPitchRoll(models[2].Rotation.Y, models[2].Rotation.X, models[2].Rotation.Z);
            models[2].Position += Vector3.Transform(Vector3.Forward, rotation) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
           
            Matrix rotation2 = Matrix.CreateFromYawPitchRoll(models[4].Rotation.Y, models[4].Rotation.X, models[4].Rotation.Z);
            models[4].Position += Vector3.Transform(Vector3.Forward, rotation2) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
        }

        void updateCamera(GameTime gameTime)
        {
            // Move the camera to the new model's position and orientation
            //((ChaseCamera)camera).Move(models[0].Position,models[0].Rotation);
            // Update the camera
            camera.Update();
        }

        public bool collisionDetect(int iceCreamNumber)
        {
            if (models[iceCreamNumber+2].BoundingSphere.Intersects(catchableObjects[iceCreamNumber].BoundingSphere))
                return true;
            return false;
        }

        public bool UTouchedTheBomb(int bombNumber)
        {
            if (models[2+bombNumber].BoundingSphere.Intersects(catchableObjects[2+bombNumber].BoundingSphere))
                return true;
            return false;
        }

        public bool UCanSeeTheBomb(int bombScoreId)
        {
            if (score[bombScoreId] == bombCounter)
                return true;
            return false;
        }

        public bool UTouchedTheExtinguisher(int extinguisherNumber)
        {
            if (models[4].BoundingSphere.Intersects(catchableObjects[5].BoundingSphere))
                return true;
            return false;
        }

        public Vector3 generateRandomVector(int direction)
        {
            Vector3 v3 = Vector3.Zero;
            Random rnd = new Random();
            v3.X = rnd.Next(200, 500);
            v3.Y = rnd.Next(250);
            if (direction == 1)
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

            foreach (CustomModel model in models)
                model.Draw(camera.View, camera.Projection);
           /* for (int i = 0; i < 2; i++)
            {
                if (!collisionDetect(i))
                {
                    catchableObjects[i].Draw(camera.View, camera.Projection);
                }
            }*/
            if (!collisionDetect(0) && !UTouchedTheBomb(0))
            {
                if (!UCanSeeTheBomb(0))
                    catchableObjects[0].Draw(camera.View, camera.Projection);
                else
                {
                    catchableObjects[2].Draw(camera.View, camera.Projection);
                    catchableObjects[5].Draw(camera.View, camera.Projection);
                }
            }        
            base.Draw(gameTime);
        }
    }
}
