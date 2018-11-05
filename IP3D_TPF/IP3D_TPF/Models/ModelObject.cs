using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace IP3D_TPF
{
    /// <summary>
    /// <see langword="abstract"/> <see langword="class"/> <see cref="ModelObject"/> contains properties and methods
    /// to give functionality to a <see cref="Microsoft.Xna.Framework.Graphics.Model"/>
    /// </summary>
    abstract class ModelObject
    {
        public Model Model { get; internal set; }
        /// <summary>
        /// Matrix array to store all the Bone Transforms of the <see cref="Microsoft.Xna.Framework.Graphics.Model"/>
        /// </summary>
        public Matrix[] BoneTransforms;
        public Matrix WorldMatrix;
        public Matrix Translation;
        public Matrix Rotation;

        public abstract void Update(GameTime gameTime, Inputs inputs, Camera cam);
        public abstract void LoadContent(ContentManager content);
        public abstract void Draw(GraphicsDevice graphics, Matrix world, Matrix view, Texture2D texture, Texture2D textureTurret, float aspectRatio);

        /// <summary>
        /// Function that returns the Model's <see cref="WorldMatrix"/> based on class properties:<see cref="Rotation"/>, 
        /// <see cref="WorldMatrix"/> and <see cref="Translation"/>.
        /// 
        /// </summary>
        /// <returns></returns>
        internal virtual Matrix GetWorldMatrixPosition()
        {
            return Rotation * WorldMatrix * Translation;
        }
    }
}
