using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace IP3D_TPF
{
    class Flag: ModelObject
    {
        #region Fields
        Model flag;
        Texture2D america;
        #endregion


        #region LoadContent
        public override void LoadContent(ContentManager content)
        {
            flag = content.Load<Model>("flag");
            america = content.Load<Texture2D>("AmericanFlagUV");
        }
        #endregion



        public void Draw(Vector3 position, GraphicsDevice graphics, Matrix projection, Matrix view, float aspectRatio,Texture2D texture)
        {
            foreach (ModelMesh mesh in flag.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //Main effect parameters
                    effect.World =Matrix.CreateScale(1.1f)* Matrix.CreateTranslation(position);
                    effect.View = view;
                    effect.Projection = projection;
                    effect.TextureEnabled = true;
                    effect.Texture = america;

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
                    effect.AmbientLightColor = new Vector3(0.9f, 0.9f, 0.9f);
                }

                //Draws each mesh of model
                mesh.Draw();
            }
        }

        #region Overrided MethodfromHierarchy
        public override void Update(GameTime gameTime)
        {
            throw new Exception();
        }

        public override void Draw(GraphicsDevice graphics, Matrix world, Matrix view, float aspectRatio)
        {
            throw new NotImplementedException();
        }
    }
        #endregion
}
