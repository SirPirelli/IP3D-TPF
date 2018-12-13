/* Created by José Pereira and Igor Lima */


/* TO DO: 
 * 
 * - Na class Tank, implementar os restantes inputs para o jogador 2;
 * - Camera System, para gerir a camara apresentada; CHECK
 * - Player Manager (?);
 * 
 * */


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IP3D_TPF.Models;
using BoundingSpheresTest;
using System.Collections.Generic;
using IP3D_TPF.CameraFolder;
using System;

namespace IP3D_TPF
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        FPSCounter fpsCounter;

        public static Inputs inputs;
        public static GraphicsDevice graphicsDevice;
        public static Random random;

        Viewport viewport;
        bool hasCollided;

        float aspectRatio;

        TerrainGenerator terrainGen;
        //Camera cam;
        Tank tank;
        Tank tank2;
        List<Tank> playersList;
        Flag flag1;
        PlayerLabel playerLabel;
        Texture2D label, label2;

        Texture2D sky;

        Vector2 clientResult;

        BoundingSphereCls sphere;
        SpriteFont font;

        CameraManager cameraManager;

        Model shell;
        ShotManager shotManager;
        Texture2D cover;

        internal List<Tank> PlayersList { get => playersList; set => playersList = value; }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
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

            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 576;
            //   graphics.GraphicsProfile = GraphicsProfile.HiDef;
          
            graphics.ApplyChanges();

            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            
            flag1 = new Flag();
            flag1.LoadContent(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            graphicsDevice = GraphicsDevice;
            random = new Random();

            hasCollided = false;

            /* FPS COUNTER */
            fpsCounter = new FPSCounter();
            fpsCounter.LoadContent(Content);
            /*--------------------*/

            /* initialize terrain */
            float planeLength = 1f;
            float heightRatio = 0.018f;
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("RGB");
            terrainGen = new TerrainGenerator(GraphicsDevice, planeLength, heightRatio, heightMapTex, terrainTex);
            /* ----------------------------------- */

            /* load meshes */
            Texture2D CubeTexture = Content.Load<Texture2D>("Sunteste");
            Model tankModel = Content.Load<Model>("tank");
            shell = Content.Load<Model>("shell");
            sky = Content.Load<Texture2D>("sky5");

            Vector3 tank1Pos = new Vector3(terrainGen.HeightMap.Size.X * 0.8f, 20, terrainGen.HeightMap.Size.Y * 0.15f);
            Vector3 tank2Pos = new Vector3(terrainGen.HeightMap.Size.X * 0.2f, 20, terrainGen.HeightMap.Size.Y * 0.85f);

            tank = new Tank(this, tankModel, tank1Pos, tank2Pos, terrainGen, 0.008f, 15f, 1);
            tank2 = new Tank(this, tankModel, tank2Pos, tank1Pos, terrainGen, 0.008f, 15f, 2);
            tank.LoadContent(Content);
            tank2.LoadContent(Content);
            tank2.IsAI = false;
            tank2.SeekFlee.Target = tank;
            playersList = new List<Tank>
            {
                tank,
                tank2
            };

            //initialize inputs class
            inputs = new Inputs();

            /* initialize camera */
            Vector3 startCamPos = new Vector3(50f, 30f, 68f);
            Vector3 camTarget = new Vector3(50f, 10f, 50f);
            Vector2 viewportCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            float nearPlane = 0.1f;
            float farPlane = 2000f;
            viewport = GraphicsDevice.Viewport;
            cameraManager = new CameraManager(viewport, terrainGen, nearPlane, farPlane, playersList);
            cameraManager.ActiveCameraIndex = 2;
            cameraManager.FollowTarget.TargetModel = tank;
            /*-------------------------------------*/

            playerLabel = new PlayerLabel();
            label = Content.Load<Texture2D>("label");
            label2 = Content.Load<Texture2D>("label2");

            sphere = new BoundingSphereCls(tank.GetPosition + Vector3.UnitY, 2.5f);

            font = Content.Load<SpriteFont>("Conthrax");

            shotManager = new ShotManager(tank, tank2, shell);
            cover = Content.Load<Texture2D>("CoverFase3");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            inputs.Update();

            if (inputs.ReleasedKey(Keys.Y)) tank2.IsAI = !tank2.IsAI;

            #region PLAYER LABEL
            // PLAYER LABELS
            //Vector3 TankSpace = new Vector3(tank.WorldMatrix.Translation.X, tank.WorldMatrix.Translation.Y + 120, tank.WorldMatrix.Translation.Z);
            //Vector3 vector = viewport.Project(tank.WorldMatrix.Translation, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f), cam.ViewMatrix, Matrix.CreateTranslation(0, 10, 0));
            //clientResult.X = vector.X;
            //clientResult.Y = vector.Y;
            /* -------------------------------------*/
            #endregion

            foreach (Tank t in playersList)
            {
                if (t.Dead)
                {
                    foreach (Tank p in playersList) p.Reset();
                    break;
                }

                t.Update(gameTime);
            }

            #region COLLISION RESPONSE

            /* COLLISION RESPONSE TANK - TANK */
            hasCollided = CollisionHandler.IsColliding(tank.BoundingSphere, tank2.BoundingSphere);
            if (hasCollided)
            {
                Vector3 tankDir = tank.Velocity;
                if (tankDir != Vector3.Zero) tankDir.Normalize();
                Vector3 tankPos = tank.WorldMatrix.Translation;
                Vector3 newTankPos = tankPos - tank.Velocity;
                tank.SetPosition(newTankPos);


                Vector3 tankDir2 = tank2.Velocity;
                if (tankDir2 != Vector3.Zero) tankDir2.Normalize();
                Vector3 tankPos2 = tank2.WorldMatrix.Translation;
                Vector3 newTankPos2 = tankPos2 - tank2.Velocity;
                tank2.SetPosition(newTankPos2);
            }

            #endregion

            shotManager.UpdateShots(gameTime);

            cameraManager.Update(gameTime);
            fpsCounter.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            #region SPRITE BATCH

            spriteBatch.Begin();
                
            spriteBatch.Draw(sky, Vector2.Zero, Color.White);
          


            spriteBatch.End();

            #endregion

            #region GRAPHICS DEVICE PARAMETERS
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;
            #endregion

            terrainGen.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix);
            tank.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix, cameraManager.ActiveProjectionMatrix, aspectRatio);
            tank2.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix, cameraManager.ActiveProjectionMatrix, aspectRatio);
            flag1.Draw(Vector3.Zero, GraphicsDevice, cameraManager.ActiveProjectionMatrix, cameraManager.ActiveViewMatrix, aspectRatio, sky);
            shotManager.DrawParticles(GraphicsDevice,cameraManager.ActiveViewMatrix, label, aspectRatio);

           

            
            playerLabel.DrawLabel(GraphicsDevice, spriteBatch,cameraManager.CameraIndex  , label, label2, cameraManager.ActiveViewMatrix, aspectRatio, tank, tank2);

            //DEBUG PURPOSES
            spriteBatch.Begin();
            spriteBatch.Draw(cover, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);
         
            spriteBatch.DrawString(font, tank2.Health.ToString() , new Vector2(90f, 67f), Color.White);
  
     
            spriteBatch.End();
            //--------------
            fpsCounter.Draw(font, spriteBatch, hasCollided, cameraManager.ActiveCameraIndex, tank2);
            base.Draw(gameTime);
        }
    }
}
