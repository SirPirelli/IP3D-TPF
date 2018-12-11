using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF
{
    class Particle
    {
        ModelObject Tank;
        Matrix world;
        Vector3 worldPosition;
        Model mud;
        Vector3 Rotation;
        public bool Disabled;
        float timer;
        Random rand;
        float XDetour;
        float YDetour;
        float ZDetour;
        float Velocity;
        float distributionScale;
        Vector3 RightVector;
        float particleSize;
    

        public Particle(ModelObject Tank, Model mud, GameTime gameTime, Random rand)
        {
            Rotation = Tank.Rotation.Backward;
        
            distributionScale = rand.Next(-200, 200);
            this.Tank = Tank;
            this.mud = mud;
            worldPosition = Tank.WorldMatrix.Translation + (new Vector3(Rotation.X * 2.4f, 0.9f, Rotation.Z * 2.4f));

            RightVector = Tank.WorldMatrix.Right * distributionScale;
            Disabled = false;
            timer = (float)gameTime.TotalGameTime.TotalSeconds;
            XDetour = rand.Next(-6, 6);
            ZDetour = rand.Next(-15, 15);
            YDetour = rand.Next(-2, 2);
            XDetour /= 10;
            ZDetour /= 10;
            YDetour /= 10;
            this.Velocity = 0.1f;
            this.particleSize = 0.014f;
        }



        public void UpdateParticle(GameTime gameTime)
        {

            world = Matrix.CreateScale(particleSize) * Matrix.CreateTranslation((worldPosition + RightVector));
            Rotation = Tank.Rotation.Backward;
            Rotation = new Vector3(Rotation.X + XDetour, Rotation.Y + ZDetour, Rotation.Z + ZDetour);

            worldPosition += Rotation * Velocity;
            particleSize += 0.001f;
            Rotation.Z += 3;
            if (gameTime.TotalGameTime.TotalSeconds > timer + 0.1f)
            {
                Disabled = true;
            }
        }


        //Regular Draw model method
        public void DrawParticle(Matrix view, Matrix projection, Texture2D texture)
        {
            foreach (ModelMesh mesh in mud.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Main effect parameters
                    effect.World = world;
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
