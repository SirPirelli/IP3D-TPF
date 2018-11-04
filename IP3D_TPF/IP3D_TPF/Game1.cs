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
        Model tank;
        MeshLoader tankLoader;
        Texture2D tankTexture;
        Texture2D tankTextureTurret;

        Tank tanque;


        VertexPositionColor[] debugNormal = new VertexPositionColor[2];

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

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight =480;
            graphics.ApplyChanges();
            // TODO: Add your initialization logic here

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

            /* load meshes */
            tankTexture = Content.Load<Texture2D>("engine_diff_tex");
            tankTextureTurret = Content.Load<Texture2D>("turret_alt_diff_tex");
            CubeTexture = Content.Load<Texture2D>("Sunteste");
           // teapotModel = Content.Load<Model>("Sun");
            tank = Content.Load<Model>("tank");
            sky = Content.Load<Texture2D>("sky5");
            //tankLoader = new MeshLoader(tank, new Vector3(300f, 40f, 200f));
            tanque = new Tank(tank, new Vector3(300f, 40f, 200f), Vector3.Zero);

            float planeLength = 10f;
            float heightRatio = 0.4f;

            /* initialize terrain */
            Texture2D heightMapTex = Content.Load<Texture2D>("lh3d1");
            Texture2D terrainTex = Content.Load<Texture2D>("GridTexture");
            terrainGen = new TerrainGenerator(GraphicsDevice, planeLength, heightRatio, heightMapTex, terrainTex);

            /* initialize camera */
            Vector3 startCamPos = new Vector3(100f, 100f, 100f);
            Vector3 camTarget = new Vector3(300f, 70f, 220f);
            Vector2 viewportCenter = new Vector2(GraphicsDevice.Viewport.Width / 2, GraphicsDevice.Viewport.Height / 2);
            float radiansPP = MathHelper.Pi / 750f;
            float camVelocity = 200f;
            float offsetY = 25f;

            cam = new Camera(startCamPos, camTarget, viewportCenter, radiansPP, camVelocity, terrainGen, offsetY);
            /*-------------------------------------*/

            inputs = new Inputs();
            //tankLoader.LoadContent();
            tanque.LoadContent(Content);
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
            inputs.Update(gameTime);
            cam.Update(gameTime, terrainGen, inputs);
            fpsCounter.Update(gameTime);
            //tankLoader.Update(gameTime, cam, inputs, terrainGen);
            tanque.Update(gameTime, inputs, cam);
            
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
            //  mesh.DrawModel( GraphicsDevice,teapotModel,Matrix.Identity* Matrix.CreateTranslation(2,23,2), cam.ViewMatrix, CubeTexture);
            //tankLoader.DrawModel(GraphicsDevice, Matrix.Identity, cam.ViewMatrix, tankTexture,tankTextureTurret);
            tanque.Draw(GraphicsDevice, Matrix.Identity, cam.ViewMatrix, tankTexture, tankTextureTurret);
            fpsCounter.Draw(spriteBatch);


            base.Draw(gameTime);
        }
    }
}
