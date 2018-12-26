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
using Microsoft.Xna.Framework.Audio;
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
        #region Variables
        public static GraphicsDeviceManager graphics;
        public static Inputs inputs;
        public static GraphicsDevice graphicsDevice;
        public static Random random;

        SpriteBatch spriteBatch;
        FPSCounter fpsCounter;

        SoundEffect battleSong;
        SoundEffectInstance battleSongInstance;
        Viewport viewport;

        bool hasCollided;
        bool showControls;

        float aspectRatio;

        TerrainGenerator terrainGen;
   
        Tank tank;
        Tank tank2;
        List<Tank> playersList;
        Flag flag1;
        PlayerLabel playerLabel;
        Texture2D label, label2;
        Texture2D sky;
        Texture2D cover;
        Texture2D coverControls;
        BoundingSphereCls sphere;
        SpriteFont font;
        CameraManager cameraManager;
        Model shell;
        ShotManager shotManager;

        internal List<Tank> PlayersList { get => playersList; set => playersList = value; }
        #endregion


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
            //Sets graphics preferred Backbuffer Size
            graphics.PreferredBackBufferWidth = 1366;
            graphics.PreferredBackBufferHeight = 768;
            graphics.ApplyChanges();
           
            //Calculates value of aspect ratio based on previous numbers
            aspectRatio = (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //System random initilization
            random = new Random();

            //Loads sound effects and sets properties
            battleSong = Content.Load<SoundEffect>("battlesong");
            battleSongInstance = battleSong.CreateInstance();
            battleSongInstance.IsLooped = true;
            battleSongInstance.Play();

            //Sets Graphic Device and initilializes spritebatch
            graphicsDevice = GraphicsDevice;
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Setup of new Flag Model
            flag1 = new Flag();
            flag1.LoadContent(Content);
          
            //Setup of initial bool values
            hasCollided = false;
            showControls = false;

            /* FPS COUNTER */
            fpsCounter = new FPSCounter();
            fpsCounter.LoadContent(Content);
            /*--------------------*/

            /* initialize terrain */
            float planeLength = 1f;
            float heightRatio = 0.06f;
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("RGB");
            terrainGen = new TerrainGenerator(GraphicsDevice, planeLength, heightRatio, heightMapTex, terrainTex);
            /* ----------------------------------- */

            /* load meshes */
            Texture2D CubeTexture = Content.Load<Texture2D>("Sunteste");
            Model tankModel = Content.Load<Model>("tank");
            shell = Content.Load<Model>("shell");
            sky = Content.Load<Texture2D>("sky5");

            //Setup of initial tank position
            Vector3 tank1Pos = new Vector3(terrainGen.HeightMap.Size.X * 0.8f, 20, terrainGen.HeightMap.Size.Y * 0.15f);
            Vector3 tank2Pos = new Vector3(terrainGen.HeightMap.Size.X * 0.2f, 20, terrainGen.HeightMap.Size.Y * 0.85f);

            //Initializes Tanks
            tank = new Tank(this, tankModel, tank1Pos, tank2Pos, terrainGen, 0.008f, 15f, 1);
            tank2 = new Tank(this, tankModel, tank2Pos, tank1Pos, terrainGen, 0.008f, 15f, 2);

            //Takes care of initial content loader and Ai targets and states
            tank.LoadContent(Content);
            tank2.LoadContent(Content);
            tank2.IsAI = false;
            tank2.SeekFlee.Target = tank;

            //Adds tanks to the list of players
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


            //Setup of label and texture loading
            playerLabel = new PlayerLabel();
            label = Content.Load<Texture2D>("label");
            label2 = Content.Load<Texture2D>("label2");

            //Sets Bounding sphere for tank2 shooting range
            sphere = new BoundingSphereCls(tank.GetPosition + Vector3.UnitY, 2.5f);

            //Font loading
            font = Content.Load<SpriteFont>("Conthrax");

            //Initializes shotManager and loads content
            shotManager = new ShotManager(tank, tank2, shell);
            shotManager.LoadContent(Content);

            //Load of UI textures
            cover = Content.Load<Texture2D>("cover");
            coverControls = Content.Load<Texture2D>("cover_controls");

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

            //Input update
            inputs.Update();

            #region UIBehaviour
            if (inputs.ReleasedKey(Keys.Enter)) showControls = !showControls;

            if (inputs.ReleasedKey(Keys.Y)) tank2.IsAI = !tank2.IsAI;

            if(!showControls)
            {

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
            }

            #endregion

            //classes updates
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

            #region ModelAndTextureDraws
            terrainGen.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix);
            tank.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix, cameraManager.ActiveProjectionMatrix, aspectRatio);
            tank2.Draw(GraphicsDevice, cameraManager.ActiveViewMatrix, cameraManager.ActiveProjectionMatrix, aspectRatio);
            flag1.Draw(Vector3.Zero, GraphicsDevice, cameraManager.ActiveProjectionMatrix, cameraManager.ActiveViewMatrix, aspectRatio, sky);
            shotManager.DrawParticles(GraphicsDevice,cameraManager.ActiveViewMatrix, label, aspectRatio);
           
            playerLabel.DrawLabel(GraphicsDevice, spriteBatch,cameraManager.CameraIndex  , label, label2, cameraManager.ActiveViewMatrix, aspectRatio, tank, tank2);

            //DEBUG PURPOSES
            spriteBatch.Begin();
            spriteBatch.Draw(cover, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            if (showControls) spriteBatch.Draw(coverControls, new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), Color.White);

            spriteBatch.DrawString(font, tank2.Health.ToString() , new Vector2(90f, 67f), Color.White);
            spriteBatch.DrawString(font, tank.Health.ToString(), new Vector2(90f, 100f), Color.White);
    
            spriteBatch.End();
            //--------------
            fpsCounter.Draw(font, spriteBatch, hasCollided, cameraManager.ActiveCameraIndex, tank2);
            #endregion

            base.Draw(gameTime);
        }
    }
}
