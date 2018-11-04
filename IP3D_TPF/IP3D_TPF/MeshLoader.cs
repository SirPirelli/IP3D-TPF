using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF
{
    class MeshLoader
    {
        ModelBone turretBone;
        ModelBone cannonBone;
        Matrix cannonTransform;
        Matrix turretTransform;
        Matrix[] boneTransforms;
        Model tankModel;
        Matrix worldMatrix;
        Matrix translation;
        Matrix rotation;
        float amount;
        float amountCanon;

        VertexPositionNormalTexture[] debugNormal = new VertexPositionNormalTexture[2];




        public MeshLoader(Model tankmodel, Vector3 startPosition)
        {
            this.tankModel = tankmodel;
            worldMatrix = Matrix.Identity;
            worldMatrix.Translation = startPosition;
           
        }


        public void Update(GameTime gameTime, Camera cam, Inputs inputs, TerrainGenerator terrainGen)
        {
            KeyboardState kb = inputs.KeyboardState;
            //Inicialização das matrizes de translação e de rotação
            translation = Matrix.Identity;
            rotation = Matrix.Identity;


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
                amountCanon += 0.04f;
            }

            if (kb.IsKeyDown(Keys.Y))
            {
                amountCanon -= 0.04f;
            }


            if (kb.IsKeyDown(Keys.A))
            {
                //Rotation equivale a uma rotação sobre o eixo Y 
                rotation = Matrix.CreateRotationY(MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds);
            }

            if (kb.IsKeyDown(Keys.D))
            {
                //Rotation equivale a uma rotação sobre o eixo Y ,desta vez de valor negativo
                rotation = Matrix.CreateRotationY(-MathHelper.PiOver2 * (float)gameTime.ElapsedGameTime.TotalSeconds);

            }

            if (kb.IsKeyDown(Keys.W))
            {
                //Translation equivale a uma translação, sendo o seu vector de translação o nosso vector mundo, mais especificamente o vector que define a componente de traseira do mundo
                translation = Matrix.CreateTranslation(worldMatrix.Backward * (float)gameTime.ElapsedGameTime.TotalSeconds * 20f);
            }

            if (kb.IsKeyDown(Keys.S))
            {
                //Translation equivale a uma translação, sendo o seu vector de translação o nosso vector mundo, mais especificamente o vector que define a frente do mundo
                translation = Matrix.CreateTranslation(worldMatrix.Forward * (float)gameTime.ElapsedGameTime.TotalSeconds * 20f);

            }

            amount = MathHelper.Clamp(amount, -1.5f, 1.5f);
            amountCanon = MathHelper.Clamp(amountCanon, -1f, -0.3f);

            //No final de cada frame equalizamos a nossa matriz à função getWorldMatrixPosition(), que nos multiplica rotation pela Worldmatrix+translation, nesta ordem especifica
            worldMatrix = GetWorldMatrixPosition();
            float height = cam.CalculateHeightOfTerrain(worldMatrix.Translation);
            Vector3 trans = worldMatrix.Translation;
            trans.Y = height;
            worldMatrix.Translation = trans;


            /* DEBUG NORMALS DO TERRENO NA POSIÇAO DO TANQUE */
            Vector3 posDebugNormal = new Vector3(tankModel.Root.Transform.Translation.X, tankModel.Root.Transform.Translation.Y + 5f, tankModel.Root.Transform.Translation.Z);
            Vector3 posDebufNormal1 = posDebugNormal + terrainGen.GetNormalAtPosition(posDebugNormal) * 100f;
            debugNormal[0] = new VertexPositionNormalTexture(posDebugNormal, Vector3.Zero, new Vector2(0, 0));
            debugNormal[1] = new VertexPositionNormalTexture(posDebufNormal1, Vector3.Zero, new Vector2(1,1));

        }



        /*Função que nos retorna a matrix que corresponde ao nosso "Gizmo" actual "*/
        private Matrix GetWorldMatrixPosition()
        {
            return rotation * worldMatrix * translation;
        }


    

    public void LoadContent()
        {
            turretBone = tankModel.Bones["turret_geo"]; 
            cannonBone = tankModel.Bones["canon_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;
            tankModel.Root.Transform = Matrix.CreateScale(0.08f);

            boneTransforms = new Matrix[tankModel.Bones.Count];
        }

        public void DrawModel( GraphicsDevice graphics, Matrix world, Matrix view,Texture2D texture,Texture2D textureTurret)
        {



            tankModel.Root.Transform = Matrix.CreateScale(0.08f) * worldMatrix;
            tankModel.Bones["turret_geo"].Transform = -turretTransform + Matrix.CreateRotationY(amount) + Matrix.CreateTranslation(new Vector3(0f, 450f, -80));
            tankModel.Bones["canon_geo"].Transform = cannonTransform + Matrix.CreateRotationX(amountCanon) + Matrix.CreateTranslation(new Vector3(0, 200f, 140));

            cannonTransform = Matrix.CreateScale(0.08f) * Matrix.CreateTranslation(new Vector3(0,0.5f,0));

            tankModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

           

            float aspectRatio = (float)graphics.Viewport.Width / graphics.Viewport.Height;
            foreach (ModelMesh mesh in tankModel.Meshes)
            {

                foreach (BasicEffect effect in mesh.Effects)
                {

                  
                    foreach(EffectPass pass in effect.CurrentTechnique.Passes)
                    {

                        effect.World =  boneTransforms[mesh.ParentBone.Index];
                        effect.View = view;
                        effect.TextureEnabled = true;


                        if (mesh.ParentBone.Index >=1 && mesh.ParentBone.Index <= 4)
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

                    /* DEBUG NORMALS NA POSIÇAO DO TANK */



                }
                graphics.DrawUserPrimitives<VertexPositionNormalTexture>(PrimitiveType.LineList, debugNormal, 0, 1);

            }
        }


    }
}
