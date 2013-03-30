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
        GraphicsDeviceManager graphics;
        List<CustomModel> models = new List<CustomModel>();
        List<CustomModel> catchableObjects = new List<CustomModel>();
        Camera camera;
        ObjectAnimation anim;
        int[] score = new int[4];
        int[] bombCounter = { new Random().Next(5, 10), new Random().Next(5, 10) };
        Random evilBombCounter = new Random();
        SoundEffect bombEffect, iceCreamEffect, extinguisherEffect;
        Skeleton[] skeletonData;
        Skeleton skeleton;
        KinectSensor kinect;
  
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
            kinect = KinectSensor.KinectSensors[0];
            kinect.SkeletonStream.Enable();
            kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);
            kinect.Start();
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
            models.Add(new CustomModel(Content.Load<Model>("head"), Vector3.Zero, new Vector3(0, 0, 0), new Vector3(50.0f), GraphicsDevice));
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
            if (Keyboard.GetState().IsKeyDown(Keys.Escape) || (score[0] >= 30 && score[1] >= 30) || (score[2] + score[3] == 3))
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
            if (skeleton != null)
            {
                positionPlsToTheRightPlaceTY(0, JointType.Spine);
                positionPlsToTheRightPlaceTY(1, JointType.HandLeft);
                positionPlsToTheRightPlaceTY(2, JointType.HandRight);
                positionPlsToTheRightPlaceTY(4, JointType.FootLeft,100);
                positionPlsToTheRightPlaceTY(3, JointType.FootRight,100);
                positionPlsToTheRightPlaceTY(5, JointType.Head,550);
            }
        }

        void updateCamera(GameTime gameTime)
        {
            camera.Update();
        }

        public void positionPlsToTheRightPlaceTY(int id, JointType bodyPart)
        {
            models[id].Position = new Vector3(((((0.5f * skeleton.Joints[bodyPart].Position.X) + 0.5f) * (graphics.PreferredBackBufferWidth))) - 600f,
                                                ((((0.5f * skeleton.Joints[bodyPart].Position.Y) + 0.5f) * (graphics.PreferredBackBufferHeight))) - 400f,
                                                0);
        }

        public void positionPlsToTheRightPlaceTY(int id, JointType bodyPart, float shiftY)
        {
            models[id].Position = new Vector3(((((0.5f * skeleton.Joints[bodyPart].Position.X) + 0.5f) * (graphics.PreferredBackBufferWidth))) - 600f,
                                                ((((0.5f * skeleton.Joints[bodyPart].Position.Y) + 0.5f) * (graphics.PreferredBackBufferHeight))) - shiftY,
                                                0);
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

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            // Skeleton Frame
            using (SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }
            if (skeletonData != null)
                foreach (Skeleton skel in skeletonData)
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        skeleton = skel;
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
