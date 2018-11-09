/* Created by José Pereira and Igor Lima */


/* TO DO: 
 * 
 * - Na class Tank, implementar os restantes inputs para o jogador 2;
 * - Camera System, para gerir a camara apresentada;
 * - Player Manager (?);
 * 
 * */


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IP3D_TPF.Models;

namespace IP3D_TPF
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        FPSCounter fpsCounter;
        public static Inputs inputs;
        int cameraTypeIndex = 1;
        Viewport viewport;
        bool hasCollided;

        float aspectRatio;

        TerrainGenerator terrainGen;
        Camera cam;
        Tank tank;
        Tank tank2;

        Texture2D sky;

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

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight =720;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
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
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hasCollided = false;

            /* FPS COUNTER */
            fpsCounter = new FPSCounter();
            fpsCounter.LoadContent(Content);
            /*--------------------*/

            /* initialize terrain */
            float planeLength = 1f;
            float heightRatio = 0.06f;
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("GridTexture");
            terrainGen = new TerrainGenerator(GraphicsDevice, planeLength, heightRatio, heightMapTex, terrainTex);
            /* ----------------------------------- */

            /* load meshes */
            Texture2D CubeTexture = Content.Load<Texture2D>("Sunteste");
            Model tankModel = Content.Load<Model>("tank");
            sky = Content.Load<Texture2D>("sky5");

            tank = new Tank(tankModel, new Vector3(50f, 40f, 50f), Vector3.Zero, terrainGen, 0.008f, 15f, 1);
            tank2 = new Tank(tankModel, new Vector3(90f, 40f, 50f), Vector3.Zero, terrainGen, 0.008f, 15f, 2);

            /* initialize camera */
            Vector3 startCamPos = new Vector3(50f, 30f, 68f);
            Vector3 camTarget = new Vector3(50f, 10f, 50f);
            Vector2 viewportCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            float radiansPP = MathHelper.Pi / 1000f;
            float camVelocity = 50f;
            float offsetY = 5f;

            cam = new Camera(startCamPos, camTarget, viewportCenter, radiansPP, camVelocity, terrainGen, offsetY);
            /*-------------------------------------*/

            inputs = new Inputs();
            tank.LoadContent(Content);
            tank2.LoadContent(Content);
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

            // TODO: Add your update logic here
            inputs.Update();

            #region CameraSelection
            //Logic for cameraSelection
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.Z)) cameraTypeIndex = 1;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.X)) cameraTypeIndex = 2;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.C)) cameraTypeIndex = 3;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.V)) cameraTypeIndex = 4;
            // Switch Logic for camera update()
            switch (cameraTypeIndex)
            {
                case 1:
                    cam.UpdateFreeCamera(gameTime, terrainGen);
                    break;
                case 2:
                    cam.UpdateFollow(gameTime, tank.WorldMatrix.Translation, tank.CameraRotationalTarget);
                    break;
                case 3:
                    cam.UpdateSurfaceFollow(gameTime, terrainGen, inputs);
                    break;
                case 4:
                    cam.UpdateFollow(gameTime, tank2.WorldMatrix.Translation, tank2.CameraRotationalTarget);
                    break;
                default:

                    break;
            }
            #endregion

            fpsCounter.Update(gameTime);
            tank.Update(gameTime, cam);
            tank2.Update(gameTime, cam);

            if (CollisionHandler.IsCollision(tank.Model, tank.WorldMatrix, tank2.Model, tank2.WorldMatrix) == true) hasCollided = true;
            else hasCollided = false;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Gray);

            spriteBatch.Begin();
            spriteBatch.Draw(sky, Vector2.Zero, Color.White);
            spriteBatch.End();

            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearWrap;

            
            terrainGen.Draw(GraphicsDevice, cam.ViewMatrix);
            tank.Draw(GraphicsDevice, Matrix.Identity, cam.ViewMatrix, aspectRatio);
            tank2.Draw(GraphicsDevice, Matrix.Identity, cam.ViewMatrix, aspectRatio);
            fpsCounter.Draw(spriteBatch, hasCollided, cameraTypeIndex);

            base.Draw(gameTime);
        }
    }
}
