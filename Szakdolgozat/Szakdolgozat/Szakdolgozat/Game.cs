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
        int[] bombCounter = { new Random().Next(5, 10), new Random().Next(5, 10) };
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (score[0] >= 20 && score[1] >= 20) || (score[2] + score[3] == 3))
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
            for (int i = 0 ; i < 2 ; i++)
            {
                if (collisionDetect(i))
                {
                    if (!UCanSeeTheBomb(i))
                    {
                        score[i]++;
                        iceCreamEffect.Play();
                    }
                    catchableObjects[i].Position = generateRandomVector(i);
                    while (collisionDetect(i)) 
                       catchableObjects[i].Position = generateRandomVector(i);
                }
                if (UTouchedTheBomb(i))
                {
                    if (UCanSeeTheBomb(i))
                    {
                        bombEffect.Play();
                        score[i+2]++;
                        bombCounter[i] += evilBombCounter.Next(5, 10);
                    }
                    catchableObjects[i+2].Position = generateRandomVector(i);
                }
                if (UCanSeeTheBomb(i))
                {
                    if (UTouchedTheExtinguisher(i))
                    {
                        extinguisherEffect.Play();
                        bombCounter[i] += evilBombCounter.Next(5, 10);
                    }
                }
            }
           
            KeyboardState keyState = Keyboard.GetState();
            Vector3 posChange1 = new Vector3(0, 0, 0);
            Vector3 posChange2 = new Vector3(0, 0, 0);
            Vector3 posChange3 = new Vector3(0, 0, 0);
            Vector3 posChange4 = new Vector3(0, 0, 0);
            if (keyState.IsKeyDown(Keys.L))
                posChange1 += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.J))
                posChange1 += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.I))
                posChange1 += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.K))
                posChange1 += new Vector3(0, -1, 0);
            models[1].Position += posChange1 * 10f;

            if (keyState.IsKeyDown(Keys.D))
                posChange2 += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.A))
                posChange2 += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.W))
                posChange2 += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.S))
                posChange2 += new Vector3(0, -1, 0);
            models[2].Position += posChange2 * 10f;

            if (keyState.IsKeyDown(Keys.Right))
                posChange3 += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left))
                posChange3 += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.Up))
                posChange3 += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.Down))
                posChange3 += new Vector3(0, -1, 0);
            models[3].Position += posChange3 * 10f;

            if (keyState.IsKeyDown(Keys.H))
                posChange4 += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.F))
                posChange4 += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.T))
                posChange4 += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.G))
                posChange4 += new Vector3(0, -1, 0);
            models[4].Position += posChange4 * 10f;

            if (keyState.IsKeyUp(Keys.Space))
                return;

            Matrix rotation1 = Matrix.CreateFromYawPitchRoll(models[1].Rotation.Y, models[1].Rotation.X, models[1].Rotation.Z);
            models[1].Position += Vector3.Transform(Vector3.Forward, rotation1) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;

            Matrix rotation2 = Matrix.CreateFromYawPitchRoll(models[2].Rotation.Y, models[2].Rotation.X, models[2].Rotation.Z);
            models[2].Position += Vector3.Transform(Vector3.Forward, rotation2) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;

            Matrix rotation3 = Matrix.CreateFromYawPitchRoll(models[3].Rotation.Y, models[3].Rotation.X, models[3].Rotation.Z);
            models[3].Position += Vector3.Transform(Vector3.Forward, rotation3) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
           
            Matrix rotation4 = Matrix.CreateFromYawPitchRoll(models[4].Rotation.Y, models[4].Rotation.X, models[4].Rotation.Z);
            models[4].Position += Vector3.Transform(Vector3.Forward, rotation4) * (float)gameTime.ElapsedGameTime.TotalMilliseconds * 4;
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
            if (models[2-iceCreamNumber].BoundingSphere.Intersects(catchableObjects[iceCreamNumber].BoundingSphere))
                return true;
            return false;
        }

        public bool UTouchedTheBomb(int bombNumber)
        {
            if (models[2-bombNumber].BoundingSphere.Intersects(catchableObjects[2+bombNumber].BoundingSphere))
                return true;
            return false;
        }

        public bool UCanSeeTheBomb(int bombScoreId)
        {
            if (score[bombScoreId] == bombCounter[bombScoreId])
                return true;
            return false;
        }

        public bool UTouchedTheExtinguisher(int extinguisherNumber)
        {
            if (models[4-extinguisherNumber].BoundingSphere.Intersects(catchableObjects[5-extinguisherNumber].BoundingSphere))
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
            for (int i = 0; i < 2; i++)
            {
                if (!collisionDetect(i) && !UTouchedTheBomb(i))
                {
                    if (!UCanSeeTheBomb(i))
                        catchableObjects[i].Draw(camera.View, camera.Projection);
                    else
                    {
                        catchableObjects[i+2].Draw(camera.View, camera.Projection);
                        catchableObjects[5-i].Draw(camera.View, camera.Projection);
                    }
                }
            }
            base.Draw(gameTime);
        }
    }
}
