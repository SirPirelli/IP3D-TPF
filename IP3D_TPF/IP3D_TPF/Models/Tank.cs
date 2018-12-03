
using System;
using BoundingSpheresTest;
using IP3D_TPF.AIBehaviour;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF.Models
{
    class Tank : ModelObject, IPlayer
    {
        /* FIELDS */
        private int player;

        ModelBone   turretBone, cannonBone;

        Texture2D   tankTexture, tankTurretTexture;

        Matrix      turretTransform, cannonTransform;

        float       turretRot, cannonPitch,
                    rotationVelocity, forwardMoveRatio,
                    moveVelocity, rotationVelocityFactor;

        bool        isAI;

        /// <summary>
        /// AI movement class
        /// </summary>
        WanderMovement wanderMovement;

        /// <summary>
        /// Bounding Sphere used for collision handling
        /// </summary>
        BoundingSphereCls boundingSphere;

        /// <summary>
        /// Offset of bounding sphere
        /// </summary>
        Vector3 sphereOffset;

        /* ------------------------------------------*/
        public int Player { get => player; protected set { player = value; } }
        public Vector3 CameraRotationalTarget { get { return -Rotation.Forward; } }
        public BoundingSphereCls BoundingSphere { get => boundingSphere; }
        public Vector3 Velocity { get => Rotation.Forward * forwardMoveRatio; }
        public Vector3 Direction { get => Rotation.Forward; }

        /* -----------------------------------------------*/
        #region CONSTRUCTORS

        public Tank(Model model, Vector3 startPosition, Vector3 rotation, TerrainGenerator terrain, float scale, float moveVelocity, int playerNum)
        {
            Model = model;
            Translation = Matrix.CreateTranslation(startPosition);
            Rotation = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            WorldMatrix = this.Translation * this.Rotation;
            this.Scale = Matrix.CreateScale(scale);
            this.moveVelocity = moveVelocity;
            rotationVelocityFactor = 2f;
            
            // bounding sphere initialization
            sphereOffset = new Vector3(0, 0.5f, 0);
            boundingSphere = new BoundingSphereCls(GetPosition + sphereOffset, 2.8f);

            //player construtor
            if (playerNum > 2 || playerNum < 1) throw new ArgumentOutOfRangeException("player");
            else Player = playerNum;
            /*--------------------------*/
            //terrain reference
            base.Terrain = terrain;
        }

        #endregion

        #region MAIN METHODS

        public override void LoadContent(ContentManager content)
        {
            //load tank textures
            tankTexture = content.Load<Texture2D>("engine_diff_tex");
            tankTurretTexture = content.Load<Texture2D>("turret_alt_diff_tex");

            //store model bones
            turretBone = Model.Bones["turret_geo"];
            cannonBone = Model.Bones["canon_geo"];

            //store bone transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            BoneTransforms = new Matrix[Model.Bones.Count];

            base.yaw = base.pitch = base.roll = forwardMoveRatio = 0f;

            wanderMovement = new WanderMovement();
            isAI = true;
        }

        public override void Update(GameTime gameTime)
        {
            //update fields
            rotationVelocity = (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2 * rotationVelocityFactor;
            forwardMoveRatio = 0f;
            /* ----------------*/

            if (Game1.inputs.Check(Keys.N)) isAI = !isAI;

            if(isAI)
            {
                var dir = wanderMovement.Update(gameTime);

                System.Diagnostics.Debug.WriteLine(wanderMovement.DivisionRandomized);

                yaw += dir.X * rotationVelocityFactor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                forwardMoveRatio += Math.Abs(dir.Y) * moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                UpdateInputs(Game1.inputs, gameTime);

            }

            CalculateAndSetRotationVectors();

            Translation = Matrix.CreateTranslation(Rotation.Forward * forwardMoveRatio);

            WorldMatrix = CalculateWorldMatrix();

            /* SEPARATE MESHES TRANSFORMS AND ROTATIONS (TURRET AND CANNON) */
            turretRot = MathHelper.Clamp(turretRot, -1.5f, 1.5f);
            cannonPitch = MathHelper.Clamp(cannonPitch, -1f, -0.3f);

            //update model position and its bones
            Model.Root.Transform = WorldMatrix;
            turretBone.Transform = -turretTransform + Matrix.CreateRotationY(turretRot) + Matrix.CreateTranslation(new Vector3(0f, 450f, -80));
            cannonBone.Transform = cannonTransform + Matrix.CreateRotationX(cannonPitch) + Matrix.CreateTranslation(new Vector3(0, 200f, 140));
            cannonTransform = Matrix.CreateTranslation(new Vector3(5f, 100f, 100f));

            Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);

            //update bounding sphere position
            boundingSphere.Center = GetPosition + sphereOffset;

        }

        /* as texturas a utilizar guardamos no objecto, e nao no game. Depois mudar */
        public override void Draw(GraphicsDevice graphics, Matrix view, Matrix projection, float aspectRatio)
        {           
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {

                        effect.World = BoneTransforms[mesh.ParentBone.Index];
                        effect.View = view;

                        #region DEBUG NORMALS
                        ///* debug normals */
                        //VertexPositionNormalTexture[] debug = new VertexPositionNormalTexture[2];
                        //debug[0] = new VertexPositionNormalTexture(WorldMatrix.Translation, Vector3.Zero, Vector2.Zero);
                        //debug[1] = new VertexPositionNormalTexture(WorldMatrix.Translation + Rotation.Forward * 20f, Vector3.Zero, Vector2.UnitX);
                        //graphics.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineList, debug, 0, 1);
                        ///* ----------------------------------------------------------------------------------------------------------------------------*/
                        #endregion

                        effect.LightingEnabled = true;

                        effect.DirectionalLight1.Enabled = true;
                        effect.Projection = projection;
                        effect.DirectionalLight0.DiffuseColor = new Vector3(0.3f, 0.3f, 0.3f);
                        effect.DirectionalLight0.Direction = new Vector3(0.25f, 1, 0.25f);
                        effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f);
                        effect.DirectionalLight2.Enabled = true;
                        effect.DirectionalLight1.DiffuseColor = new Vector3(0.20f, 0.20f, 0.20f);
                        effect.DirectionalLight1.Direction = new Vector3(0, 0.8f, 1f);
                        effect.DirectionalLight1.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f);
                        effect.AmbientLightColor = new Vector3(0.9f, 0.9f, 0.9f);

                        mesh.Draw();

                    }                  
                }
            }

            #region DEBUG BOUNDINGSPHERE

            boundingSphere.Draw(graphics, view, projection);

            #endregion

        }

        #endregion

        #region POSITION AND VELOCITY HELPER FUNCTIONS

        public void SetMoveVelocity(float moveVel)
        {
            moveVelocity = moveVel;
        }

        public void SetPosition(Vector3 position)
        {
            Matrix newWorld = WorldMatrix;
            newWorld.Translation = position;
            WorldMatrix = newWorld;
        }

        #endregion

        #region ROTATION AND WORLD MATRIX CALC

        /// <summary>
        /// Calculates and updates the Rotation property and the turretRot and cannonPitch fields
        /// </summary>
        private void CalculateAndSetRotationVectors()
        {
            Vector3 normal = base.Terrain.GetNormalAtPosition(WorldMatrix.Translation);                                 normal.Normalize();
            Vector3 forward = Vector3.Cross(Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationY(yaw)), normal);     forward.Normalize();
            Vector3 right = Vector3.Cross(forward, normal);                                                             right.Normalize();

            Matrix rot = Rotation;
            rot.Up = normal;
            rot.Forward = -forward;
            rot.Right = -right;

            Rotation = rot;

        }

        private Matrix CalculateWorldMatrix()
        {
            // calcula se a nova world matrix para sabermos a nova posiçao do tanque
            WorldMatrix = GetWorldMatrix();

            Vector3 trans = WorldMatrix.Translation;
            trans.X = MathHelper.Clamp(trans.X, 0, base.Terrain.TerrainBounds.X - base.Terrain.PlaneLength - (base.Terrain.PlaneLength * 2f));
            trans.Z = MathHelper.Clamp(trans.Z, 0, base.Terrain.TerrainBounds.Y - base.Terrain.PlaneLength - (base.Terrain.PlaneLength * 2f));
            float height = base.Terrain.CalculateHeightOfTerrain(WorldMatrix.Translation);
            trans.Y = height + 0.1f;

            // create new world matrix to append new translation values and vectors according to rotation
            Matrix worldM = WorldMatrix;
            worldM.Translation = trans;
            worldM.Up = Rotation.Up;
            worldM.Forward = -Rotation.Forward;
            worldM.Right = -Rotation.Right;
            worldM = Scale * worldM;

            return worldM;
        }

        #endregion

        #region IPLAYER IMPLEMENTATION

        public void UpdateInputs(Inputs inputs, GameTime gameTime)
        {
            switch(Player)
            {
                case 1:
                    /* Turret Rotation inputs */
                    if (inputs.Check(Keys.Q)) turretRot += 0.04f;
                    if (inputs.Check(Keys.E)) turretRot -= 0.04f;

                    /* Cannon Pitch inputs */
                    if (inputs.Check(Keys.T)) cannonPitch += 0.04f;
                    if (inputs.Check(Keys.Y)) cannonPitch -= 0.04f;


                    /* Tank rotation inputs */
                    if (inputs.Check(Keys.A)) base.yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inputs.Check(Keys.D)) base.yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    /* Tank forward movement input  - Translation equivale a uma translação, 
                     * sendo o seu vector de translação o nosso vector mundo */
                    if (inputs.Check(Keys.W)) forwardMoveRatio += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inputs.Check(Keys.S)) forwardMoveRatio -= moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;

                case 2:
                    if (inputs.Check(Keys.Up)) forwardMoveRatio += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inputs.Check(Keys.Down)) forwardMoveRatio -= moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    /* Tank rotation inputs */
                    if (inputs.Check(Keys.Left)) base.yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (inputs.Check(Keys.Right)) base.yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    break;

                default:
                    throw new Exception("Player number is not assigned to a valid number.");
            }

            
        }

        #endregion

    }
}
