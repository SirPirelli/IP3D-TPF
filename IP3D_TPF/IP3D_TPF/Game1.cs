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
using BoundingSpheresTest;

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

        PlayerLabel playerLabel;
        Texture2D label, label2;

        Texture2D sky;

        Vector2 clientResult;

        BoundingSphereCls sphere;

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
            float heightRatio = 0.0006f;
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("Diffuse2");
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
            float camVelocity = 30f;
            float offsetY = 3f;
            float nearPlane = 0.1f;
            float farPlane = 2000f;
            float fovAngleDeg = 45f;
            cam = new Camera(startCamPos, camTarget, viewportCenter, radiansPP, camVelocity, terrainGen, offsetY,
                            fovAngleDeg, nearPlane, farPlane, aspectRatio);
            /*-------------------------------------*/

            inputs = new Inputs();
            tank.LoadContent(Content);
            tank2.LoadContent(Content);

            playerLabel = new PlayerLabel();
            label = Content.Load<Texture2D>("label");
            label2 = Content.Load<Texture2D>("label2");


            sphere = new BoundingSphereCls(tank.GetPosition + Vector3.UnitY, 2.5f);

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
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.F1)) cameraTypeIndex = 1;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.F2)) cameraTypeIndex = 2;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.F3)) cameraTypeIndex = 3;
            if (inputs.CurrentKeyboardState.IsKeyDown(Keys.F4)) cameraTypeIndex = 4;
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

            Vector3 TankSpace = new Vector3(tank.WorldMatrix.Translation.X, tank.WorldMatrix.Translation.Y + 120, tank.WorldMatrix.Translation.Z);
            Vector3 vector = viewport.Project(tank.WorldMatrix.Translation, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f), cam.ViewMatrix, Matrix.CreateTranslation(0, 10, 0));
            clientResult.X = vector.X;
            clientResult.Y = vector.Y;

            fpsCounter.Update(gameTime);
            tank.Update(gameTime, cam);
            tank2.Update(gameTime, cam);

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
                Vector3 newTankPos2 = tankPos2 + (tankDir2 * -tank2.Velocity * 1.1f);
                tank2.SetPosition(newTankPos2);
            }

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
            tank.Draw(GraphicsDevice, cam.ViewMatrix, cam.ProjectionMatrix, aspectRatio);
            tank2.Draw(GraphicsDevice, cam.ViewMatrix, cam.ProjectionMatrix, aspectRatio);
            fpsCounter.Draw(spriteBatch, hasCollided, cameraTypeIndex);

            playerLabel.DrawLabel(GraphicsDevice, spriteBatch, cameraTypeIndex, label, label2, cam, aspectRatio, tank, tank2);


            base.Draw(gameTime);
        }
    }
}
