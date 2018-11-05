
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF.Models
{
    class Tank : ModelObject
    {
        TerrainGenerator terrain;

        ModelBone turretBone;
        ModelBone cannonBone;

        Matrix turretTransform;
        Matrix cannonTransform;

        float amount, amountCannon, yaw, pitch, roll, rotationVelocity, pos,
              moveVelocity;
        float rot = 0f;


        public Tank(Model model, Vector3 startPosition, Vector3 rotation, TerrainGenerator terrain)
        {
            Model = model;
            Translation = Matrix.CreateTranslation(startPosition);
            Rotation = Matrix.CreateFromYawPitchRoll(rotation.X, rotation.Y, rotation.Z);
            WorldMatrix = this.Translation * this.Rotation;
            this.terrain = terrain;
            moveVelocity = 5f;
        }

        public override void LoadContent(ContentManager content)
        {
            turretBone = Model.Bones["turret_geo"];
            cannonBone = Model.Bones["canon_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            Model.Root.Transform = Matrix.CreateScale(0.08f);

            BoneTransforms = new Matrix[Model.Bones.Count];

            yaw = pitch = roll = pos = 0f;
        }

        public override void Update(GameTime gameTime, Inputs inputs, Camera cam)
        {
            KeyboardState kb = inputs.KeyboardState;
            //Inicialização das matrizes de translação e de rotação

           


            rotationVelocity = (float)gameTime.ElapsedGameTime.TotalSeconds * MathHelper.PiOver2;
            pos = 0f;

            //Inicialização do keyboardState
            kb = Keyboard.GetState();

            if (kb.IsKeyDown(Keys.Q))
            {
                amount += 0.04f;
            }

            if (kb.IsKeyDown(Keys.E))
            {
                amount -= 0.04f;
            }


            if (kb.IsKeyDown(Keys.T))
            {
                amountCannon += 0.04f;
            }

            if (kb.IsKeyDown(Keys.Y))
            {
                amountCannon -= 0.04f;
            }


            if (kb.IsKeyDown(Keys.A))
            {
                //Rotation equivale a uma rotação sobre o eixo Y 
                yaw += rotationVelocity;
            }

            if (kb.IsKeyDown(Keys.D))
            {
                //Rotation equivale a uma rotação sobre o eixo Y ,desta vez de valor negativo
                yaw -= rotationVelocity;
            }

            if (kb.IsKeyDown(Keys.W))
            {
                //Translation equivale a uma translação, sendo o seu vector de translação o nosso vector mundo, mais especificamente o vector que define a componente de traseira do mundo
                pos += moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (kb.IsKeyDown(Keys.S))
            {
                //Translation equivale a uma translação, sendo o seu vector de translação o nosso vector mundo, mais especificamente o vector que define a frente do mundo
                pos -= moveVelocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            amount = MathHelper.Clamp(amount, -1.5f, 1.5f);
            amountCannon = MathHelper.Clamp(amountCannon, -1f, -0.3f);


            /* RODAR O TANQUE CONFORME O TERRENO */
            var normal = terrain.GetNormalAtPosition(WorldMatrix.Translation); normal.Normalize();
            var forward = Vector3.Cross(Vector3.Transform(Vector3.UnitX, Matrix.CreateRotationY(yaw)), normal); forward.Normalize();
            var right = Vector3.Cross(forward, normal); right.Normalize();

            pitch += 0.05f;
            roll += 0.05f;
            System.Diagnostics.Debug.WriteLine(pitch);
            //No final de cada frame equalizamos a nossa matriz à função getWorldMatrixPosition(), que nos multiplica rotation pela Worldmatrix+translation, nesta ordem especifica
            Rotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw) * Matrix.CreateRotationZ(roll);
            Rotation.Up = normal;
            Rotation.Down = -normal;
            Rotation.Forward = forward;
            Rotation.Backward = -forward;
            Rotation.Right = right;
            Rotation.Left = -right;

            Translation = Matrix.CreateTranslation(Rotation.Forward * pos);

            WorldMatrix = GetWorldMatrixPosition();

            float height = cam.CalculateHeightOfTerrain(WorldMatrix.Translation);
            Vector3 trans = WorldMatrix.Translation;
            trans.Y = height + 0.1f;
            WorldMatrix.Translation = trans;


            WorldMatrix.Up = normal;
            WorldMatrix.Forward = forward;
            WorldMatrix.Right = right;
            WorldMatrix.Down = -normal;
            WorldMatrix.Left = -right;
            WorldMatrix.Backward = -forward;


        }

        /* as texturas a utilizar guardamos no objecto, e nao no game. Depois mudar */
        public override void Draw(GraphicsDevice graphics, Matrix world, Matrix view, Texture2D texture, Texture2D textureTurret, float aspectRatio)
        {
             rot += 0.01f;
            Model.Root.Transform = Matrix.CreateScale(0.004f) * /*Matrix.CreateRotationX(rot) **/ WorldMatrix;
            Model.Bones["turret_geo"].Transform = -turretTransform + Matrix.CreateRotationY(amount) + Matrix.CreateTranslation(new Vector3(0f, 450f, -80));
            Model.Bones["canon_geo"].Transform = cannonTransform + Matrix.CreateRotationX(amountCannon) + Matrix.CreateTranslation(new Vector3(0, 200f, 140));

            cannonTransform = Matrix.CreateScale(0.08f) * Matrix.CreateTranslation(new Vector3(0, 0.5f, 0));

            Model.CopyAbsoluteBoneTransformsTo(BoneTransforms);

            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                    {

                        effect.World = BoneTransforms[mesh.ParentBone.Index];
                        effect.View = view;
                        effect.TextureEnabled = true;


                        if (mesh.ParentBone.Index >= 1 && mesh.ParentBone.Index <= 4)
                        {
                            effect.Texture = texture;
                            pass.Apply();
                        }

                        if (mesh.ParentBone.Index == 0)
                        {
                            effect.Texture = textureTurret;
                            pass.Apply();
                        }


                        effect.LightingEnabled = true;

                        effect.DirectionalLight1.Enabled = true;
                        effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f);
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
        }
    }
}
