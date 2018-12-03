/* Created by José Pereira on 06/11/2018 
    jose_miguel_pereira@hotmail.com        */

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
        public Matrix[] BoneTransforms { get;protected set; }
        public Matrix WorldMatrix { get;protected set; }
        public Matrix Translation { get;protected set; }
        public Matrix Rotation { get;protected set; }
        public Matrix Scale { get;protected set; }

        protected TerrainGenerator Terrain { get; set; }    /*como noutros projectos podemos nao utilizar o terrainGenerator,
                                                                é melhor ir buscar a referencia ao terreno fora desta classe */

        protected float yaw, pitch, roll;

        public Vector3 GetPosition { get { return WorldMatrix.Translation; } }

        public abstract void Update(GameTime gameTime);
        public abstract void LoadContent(ContentManager content);
        public abstract void Draw(GraphicsDevice graphics, Matrix world, Matrix view, float aspectRatio);


        /// <summary>
        /// Function that returns the Model's <see cref="WorldMatrix"/> based on class properties:<see cref="Rotation"/>, 
        /// <see cref="WorldMatrix"/> and <see cref="Translation"/>.
        /// 
        /// </summary>
        /// <returns></returns>
        internal protected virtual Matrix GetWorldMatrix()
        {
            return  (Rotation * WorldMatrix * Translation);
        }
    }
}
