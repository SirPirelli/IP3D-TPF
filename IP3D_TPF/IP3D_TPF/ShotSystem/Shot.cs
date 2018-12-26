using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace IP3D_TPF
{
    class Shot
    {
        Tank Tank;

        #region Fields
        Model shell;
        public Matrix WorldMatrix;
        Matrix tankW;
        Vector3 fowardHorizontal;
        Vector3 Direction;
        Vector3 ScalarDirection;
        Vector3 right;
        Vector3 forwardCorrected;
     
        public BoundingSpheresTest.BoundingSphereCls bulletCollider;
        #endregion

        #region Properties
        public Tank Parent { get => Tank; }
        #endregion


        #region Constructor
        public Shot(Tank Tank, Model shell)
        {
            //Creates new Bounding sphere for the shell
            bulletCollider = new BoundingSpheresTest.BoundingSphereCls(WorldMatrix.Translation, 0.35f);


            this.Tank = Tank;
            this.shell = shell;

            //Transforms horizontal Direction of tank, creating a rotation from the turret yaw
            fowardHorizontal = Vector3.Transform(Tank.WorldMatrix.Forward, Matrix.CreateRotationY(Tank.TurretRot));

            // Given the fowardHorizontal vector and the right, the dot product will be the right vector
            right = Vector3.Cross(fowardHorizontal, Tank.Rotation.Up);

            //Knowing the right vector and the normal(UP) of the tank, we know the correct forward vector
            forwardCorrected = Vector3.Cross(right, Tank.Rotation.Up);

            //Pitches Y component of the direction in which the bullet will travel, multiplied by a modular scalar
            forwardCorrected.Y += -Tank.CannonPitch * 0.01f;

            ////Forward Matrix of the tank
            Direction = forwardCorrected;
            Direction.Normalize();

            //Gets world Matrix of the tank
            tankW = Tank.GetWorldMatrix();

        }
        #endregion


        public void UpdateParticle(GameTime gameTime)
        {
            //Every frame the Y component gets decrememented by 1.2f, simulating a kind of gravity
            Direction.Y -= 1.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Scales velocity
            ScalarDirection += Direction * 1.2f;

            //Accumulation of all matrix as the world matrix
            WorldMatrix =  Matrix.CreateScale(10)*Matrix.CreateTranslation(Tank.Model.Bones["canon_geo"].ModelTransform.Translation.X, 300, Tank.Model.Bones["canon_geo"].ModelTransform.Translation.Z) * tankW * Matrix.CreateTranslation(ScalarDirection);

            //Sets Center of collider as the center of the shot model in a world relative position
            bulletCollider.Center = this.WorldMatrix.Translation;

        }


        //Regular Draw model method
        public void DrawParticle(GraphicsDevice device, Matrix view, Matrix projection, Texture2D texture)
        {
            bulletCollider.Draw(device, view, projection);

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
    }
}
