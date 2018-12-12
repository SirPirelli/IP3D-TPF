using BoundingSpheresTest;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IP3D_TPF
{
    class Shot
    {

        #region FIELDS
        ModelObject parentTank;
        Model shell;
        public Matrix WorldMatrix;
        Matrix rotationMatrix;
        Vector3 fowardHorizontal;
        Vector3 Direction;
        Vector3 ScalarDirection;
        Vector3 forward;
        Vector3 right;
        Vector3 forwardCorrected;
        BoundingSphereCls bulletCollider;
        #endregion

        #region PROPERTIES
        public BoundingSphereCls BoundingSphere { get => bulletCollider; }
        #endregion

        #region CONSTRUCTORS
        public Shot(Tank Tank, Model shell)
        {
            bulletCollider = new BoundingSphereCls(WorldMatrix.Translation, 1);
            this.parentTank = Tank;
            this.shell = shell;

            fowardHorizontal = Vector3.Transform(Vector3.UnitZ, Matrix.CreateRotationY(Tank.yaw + Tank.turretRot));
            right = Vector3.Cross(fowardHorizontal, Tank.Rotation.Up);
            forwardCorrected = Vector3.Cross(right, Tank.Rotation.Up);         

            ////Forward Matrix of the tank
            Direction = forwardCorrected;

            //////Transformation of the Direction Matrix
            //Direction = Vector3.Transform(Direction, rotationMatrix);
            Direction.Normalize();
        }
        #endregion

        #region MAIN METHODS
        public void UpdateParticle(GameTime gameTime)
        {
            Direction.Y= -5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Makes the model move in the direction of the transformed vector3
            ScalarDirection += Direction;

            //Accumulation of all matrix as the world matrix
            WorldMatrix = Matrix.CreateTranslation(Direction.X, 8, Direction.Z)* Matrix.CreateScale(10) * parentTank.GetWorldMatrix() * Matrix.CreateTranslation(ScalarDirection)* Matrix.CreateTranslation(Direction);
           
            //System.Diagnostics.Debug.WriteLine(Direction);
            bulletCollider.Center = this.WorldMatrix.Translation;

        }


        //Regular Draw model method
        public void DrawParticle(Matrix view, Matrix projection, Texture2D texture)
        {
            foreach (ModelMesh mesh in shell.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Main effect parameters
                    effect.World = WorldMatrix;
                    effect.View = view;
                    effect.Projection = projection;
                    effect.TextureEnabled = true;
                    effect.Texture = texture;

                    //Enables first directional light
                    effect.DirectionalLight1.Enabled = true;
                    effect.LightingEnabled = true; // turn on the lighting subsystem.
                    effect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.2f, 0); // a red light
                    effect.DirectionalLight0.Direction = new Vector3(1, -0.3f, 0);  // coming along the x-axis

                    //Enables second directional light
                    effect.DirectionalLight0.Enabled = true;
                    effect.DirectionalLight1.DiffuseColor = new Vector3(0.2f, 0.05f, 0.0f); // a red light
                    effect.DirectionalLight1.Direction = new Vector3(-1, 0, 0);  // coming along the x-axis

                    //Enables and sets ambient light of particle meshes
                    effect.AmbientLightColor = new Vector3(0.58f, 0.58f, 0.58f);
                }

                //Draws each mesh of model
                mesh.Draw();
            }
        }
        #endregion

    }
}
