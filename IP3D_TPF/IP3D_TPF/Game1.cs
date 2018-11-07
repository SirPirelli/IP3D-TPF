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
        TerrainGenerator terrainGen;
        Camera cam;
        Inputs inputs;
        FPSCounter fpsCounter;
        Texture2D sky;
        Texture2D CubeTexture;
        Model tankModel;

        Tank tank;

        float aspectRatio;


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
            //mesh = new MeshLoader();
         
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            fpsCounter = new FPSCounter();
            fpsCounter.LoadContent(Content);

            /* initialize terrain */
            float planeLength = 1f;
            float heightRatio = 0.08f;
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("GridTexture");
            terrainGen = new TerrainGenerator(GraphicsDevice, planeLength, heightRatio, heightMapTex, terrainTex);

            /* load meshes */
            CubeTexture = Content.Load<Texture2D>("Sunteste");
            tankModel = Content.Load<Model>("tank");
            sky = Content.Load<Texture2D>("sky5");

            tank = new Tank(tankModel, new Vector3(50f, 40f, 50f), Vector3.Zero, terrainGen, 0.008f, 1);

            /* initialize camera */
            Vector3 startCamPos = new Vector3(100f, 100f, 100f);
            Vector3 camTarget = new Vector3(300f, 70f, 220f);
            Vector2 viewportCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            float radiansPP = MathHelper.Pi / 1000f;
            float camVelocity = 200f;
            float offsetY = 25f;

            cam = new Camera(startCamPos, camTarget, viewportCenter, radiansPP, camVelocity, terrainGen, offsetY);
            /*-------------------------------------*/

            inputs = new Inputs();
            //tankLoader.LoadContent();
            tank.LoadContent(Content);
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
            cam.Update(gameTime, terrainGen, inputs);
            fpsCounter.Update(gameTime);
            //tankLoader.Update(gameTime, cam, inputs, terrainGen);
            tank.Update(gameTime, inputs, cam);
            
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
            fpsCounter.Draw(spriteBatch);


            base.Draw(gameTime);
        }
    }
}
