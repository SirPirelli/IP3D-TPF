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

        #region FIELDS
        private int player;

        readonly Game1 game;

        ModelBone   turretBone, cannonBone;

        Texture2D   tankTexture, tankTurretTexture;

        Matrix      turretTransform, cannonTransform;

        internal float       turretRot, cannonPitch,
                    rotationVelocity, forwardMoveRatio,
                    moveVelocity, rotationVelocityFactor;

        float       wanderRotationalVelFactor;

        //AI vars
        bool        isAI;
        double timeSinceLastAIChange;
        int timeToChangeAIState;
        AIStates aIStates;

        /// <summary>
        /// AI wander movement class
        /// </summary>
        WanderMovement wanderMovement;

        /// <summary>
        /// 
        /// </summary>
        SeekFlee seekFleeMovement;

        /// <summary>
        /// Bounding Sphere used for collision handling
        /// </summary>
        BoundingSphereCls boundingSphere;

        /// <summary>
        /// Offset of bounding sphere
        /// </summary>
        Vector3 sphereOffset;

        Vector3 startPosition, startRotation;
        ParticleSystem particleSystem;
        #endregion

        #region PROPERTIES
        public int Player { get => player; protected set { player = value; } }
        public Vector3 CameraRotationalTarget { get { return -Rotation.Forward; } }
        public BoundingSphereCls BoundingSphere { get => boundingSphere; }
        public Vector3 Velocity { get => Rotation.Forward * forwardMoveRatio; }
        public Vector3 Direction { get => Rotation.Forward; }
        public SeekFlee SeekFlee { get => seekFleeMovement; }
        public bool IsAI { get => isAI; set => isAI = value; }
        public AIStates AIState { get => aIStates; }

        public float Health { get; set; }
        public bool Dead { get; set; }
        #endregion

        #region CONSTRUCTORS

        public Tank(Game1 game, Model model, Vector3 startPosition, Vector3 rotation, TerrainGenerator terrain, float scale, float moveVelocity, int playerNum)
        {
            this.game = game;
            Model = model;
            this.startPosition = startPosition;
            this.startRotation = rotation;
            Translation = Matrix.CreateTranslation(startPosition);
            Rotation = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            WorldMatrix = this.Rotation * this.Translation;
            this.Scale = Matrix.CreateScale(scale);
            this.moveVelocity = moveVelocity;
            this.rotationVelocityFactor = 30f;
            this.wanderRotationalVelFactor = rotationVelocityFactor / 15f;

            var m = Matrix.CreateLookAt(startPosition, rotation, Vector3.Up);
            var ypr = MathHelpersCls.ExtractYawPitchRoll(m);

            base.yaw = ypr.X;
            base.pitch = base.roll = 0f;
            
            // bounding sphere initialization
            sphereOffset = new Vector3(0, 0.5f, 0);
            boundingSphere = new BoundingSphereCls(GetPosition + sphereOffset, 2.8f);

            //player construtor
            if (playerNum > 2 || playerNum < 1) throw new ArgumentOutOfRangeException("player");
            else Player = playerNum;
            /*--------------------------*/
            //terrain reference
            base.Terrain = terrain;

            this.timeSinceLastAIChange = 0d;
            this.timeToChangeAIState = 3;
            this.Health = 100;
            
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
            var particle = content.Load<Model>("Mud");

            //store bone transforms
            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            BoneTransforms = new Matrix[Model.Bones.Count];

            forwardMoveRatio = 0f;

            wanderMovement = new WanderMovement();
            seekFleeMovement = new SeekFlee(this, null, moveVelocity * 1.5f, moveVelocity);
            isAI = false;
            aIStates = AIStates.WANDER;

            particleSystem = new ParticleSystem(this, particle);

        }

        public override void Update(GameTime gameTime)
        {

            //update fields
            rotationVelocity = (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2 * rotationVelocityFactor;
            forwardMoveRatio = 0f;
            /* ----------------*/

            if(isAI)
            {
                //checks if is time to change AI state
                DefineAIState(gameTime);

                //Update AI state
                UpdateAIState(gameTime);
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
            particleSystem.Update(gameTime, this.WorldMatrix);

        }

        public override void Draw(GraphicsDevice graphics, Matrix view, Matrix projection, float aspectRatio)
        {
            particleSystem.DrawParticles(view, projection, tankTexture);

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

                        effect.VertexColorEnabled = false;
                        effect.TextureEnabled = true;
                        mesh.Draw();

                    }                  
                }
            }

            #region DEBUG BOUNDINGSPHERE (commented)

            //boundingSphere.Draw(graphics, view, projection);

            #endregion

        }

        #endregion

        public void Reset()
        {
            Translation = Matrix.CreateTranslation(startPosition);
            Rotation = Matrix.CreateFromYawPitchRoll(startRotation.X, startRotation.Y, startRotation.Z);
            WorldMatrix = this.Rotation * this.Translation;
            Dead = false;
            Health = 100;
        }

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

        /// <summary>
        /// Clamps position given the <see cref="TerrainGenerator"/> bounds and height of Terrain.
        /// Does not have in account the scale matrix.
        /// </summary>
        /// <returns></returns>
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

        #region AI METHODS

        private void DefineAIState(GameTime gameTime)
        {
            timeSinceLastAIChange += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeSinceLastAIChange > timeToChangeAIState)
            {
                int state = Game1.random.Next(0, 3);
                int t;

                switch (aIStates)
                {
                    case AIStates.SEEK:

                        if (state == 0) //flee
                        {
                            t = Game1.random.Next(1, 5);
                            aIStates = AIStates.FLEE;
                        }
                        else    //wander
                        {
                            t = Game1.random.Next(4, 9);
                            aIStates = AIStates.WANDER;
                        }

                        break;

                    case AIStates.WANDER:

                        if (state == 0) //flee
                        {
                            t = Game1.random.Next(1, 5);
                            aIStates = AIStates.FLEE;
                        }
                        else    //seek
                        {
                            t = Game1.random.Next(3, 8);
                            aIStates = AIStates.SEEK;
                        }
                        break;

                    case AIStates.FLEE:
                        if (state == 0) //wander
                        {
                            t = Game1.random.Next(3, 8);
                            aIStates = AIStates.WANDER;

                        }
                        else    //seek
                        {
                            t = Game1.random.Next(1, 5);
                            aIStates = AIStates.SEEK;
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("CANT RECOGNIZE AISTATE VALUE IN TANK CLASS: DEFINEAISTATE");
                        t = timeToChangeAIState;
                        break;
                }

                timeSinceLastAIChange = 0d;
                timeToChangeAIState = t;
            }
        }

        private void UpdateAIState(GameTime gameTime)
        {
            switch (aIStates)
            {
                case AIStates.SEEK:

                    //vou descobrir a distancia entre os vetores do objecto e a posição do target.
                    var distLeft = Vector3.Distance(GetPosition + Rotation.Left, seekFleeMovement.Target.GetPosition);
                    var distRight = Vector3.Distance(GetPosition + Rotation.Right, seekFleeMovement.Target.GetPosition);
                    var distForw = Vector3.Distance(GetPosition + Rotation.Forward, seekFleeMovement.Target.GetPosition);
                    var distBack = Vector3.Distance(GetPosition + Rotation.Backward, seekFleeMovement.Target.GetPosition);

                    //only moves if the distance to target is bigger than a value
                    if (distBack > 6 && distForw > 6 && distLeft > 6 && distRight > 6)
                    {
                        forwardMoveRatio += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else forwardMoveRatio = 0f;

                    //if the distance value between the distRight and distLeft are less than a value, it doesnt move the yaw.
                    if (Math.Abs(distLeft - distRight) >= 0.3f)
                    {
                        if (distForw <= distBack)            //se o target esta a frente do objecto
                        {
                            if (distLeft <= distRight)       //se esta à frente e à esquerda
                            {
                                yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else                            //se esta a direita
                            {
                                yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                        else                                // se o target esta atras do objecto
                        {
                            if (distLeft <= distRight)       //se esta atras e à esquerda
                            {
                                yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else                            //se esta atras e à direita

                            {
                                yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                    }

                    break;

                case AIStates.FLEE:

                    //vou descobrir a distancia entre os vetores do objecto e a posição do target.
                    distLeft = Vector3.Distance(GetPosition + Rotation.Left, seekFleeMovement.Target.GetPosition);
                    distRight = Vector3.Distance(GetPosition + Rotation.Right, seekFleeMovement.Target.GetPosition);
                    distForw = Vector3.Distance(GetPosition + Rotation.Forward, seekFleeMovement.Target.GetPosition);
                    distBack = Vector3.Distance(GetPosition + Rotation.Backward, seekFleeMovement.Target.GetPosition);

                    //only moves if the distance to target is bigger than a value
                    if (distBack < 50 && distForw < 50 && distLeft < 50 && distRight < 50)
                    {
                        forwardMoveRatio += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    }
                    else aIStates = AIStates.WANDER;

                    //if the distance value between the distRight and distLeft are less than a value, it doesnt move the yaw.
                    if (Math.Abs(distLeft - distRight) >= 0.3f)
                    {
                        if (distForw <= distBack)            //se o target esta a frente do objecto
                        {
                            if (distLeft <= distRight)       //se esta à frente e à esquerda
                            {
                                yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else                            //se esta a direita
                            {
                                yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                        else                                // se o target esta atras do objecto
                        {
                            if (distLeft <= distRight)       //se esta atras e à esquerda
                            {
                                yaw -= rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                            else                            //se esta atras e à direita

                            {
                                yaw += rotationVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                            }
                        }
                    }
                    break;

                case AIStates.WANDER:

                    var dir = wanderMovement.GeneratePoint();

                    yaw += dir.X * wanderRotationalVelFactor * (float)gameTime.ElapsedGameTime.TotalSeconds;
                    forwardMoveRatio += Math.Abs(dir.Y) * moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    break;
            }
        }

        #endregion

    }
}
