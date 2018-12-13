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
        Model shell;
        public Matrix WorldMatrix;
        Matrix rotationMatrix;
        Vector3 fowardHorizontal;
        Vector3 Direction;
        Vector3 ScalarDirection;
        Vector3 forward;
        Vector3 right;
        Vector3 forwardCorrected;
        public BoundingSpheresTest.BoundingSphereCls bulletCollider;
        Matrix tankW;
        Vector3 Translate;

        public Tank Parent { get => Tank; }

        public Shot(Tank Tank, Model shell)
        {

            bulletCollider = new BoundingSpheresTest.BoundingSphereCls(WorldMatrix.Translation, 0.35f);
            this.Tank = Tank;
            this.shell = shell;

            fowardHorizontal = Vector3.Transform(Tank.WorldMatrix.Forward, Matrix.CreateRotationY(Tank.TurretRot));
            right = Vector3.Cross(fowardHorizontal, Tank.Rotation.Up);
            forwardCorrected = Vector3.Cross(right, Tank.Rotation.Up);

            forwardCorrected.Y += -Tank.CannonPitch * 0.01f;
            ////Forward Matrix of the tank
            Direction = forwardCorrected;

            //////Transformation of the Direction Matrix
            //Direction = Vector3.Transform(Direction, rotationMatrix);
            Direction.Normalize();
            tankW = Tank.GetWorldMatrix();

        }



        public void UpdateParticle(GameTime gameTime)
        {
            Direction.Y -= 1.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
            ScalarDirection += Direction * 1.2f;

            //Accumulation of all matrix as the world matrix
            WorldMatrix = Matrix.CreateTranslation(0, 35, 0) * Matrix.CreateScale(10) * tankW * Matrix.CreateTranslation(ScalarDirection);

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
