﻿/* Created by José Pereira on 06/11/2018 
    jose_miguel_pereira@hotmail.com        */

using IP3D_TPF.Utilities;
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

        internal protected TerrainGenerator Terrain { get; set; }    /*como noutros projectos podemos nao utilizar o terrainGenerator,
                                                               é melhor ir buscar a referencia ao terreno fora desta classe */

        internal protected float yaw, pitch, roll;

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

        public Vector3 CalculateDirection(Vector3 target)
        {
            Vector3 direction = (target - GetPosition);
            if (direction != Vector3.Zero) direction.Normalize();
            return direction;
        }

        public Vector3 CalculateAcceleration(Vector3 targetVelocity, Vector3 velocity)
        {
            return targetVelocity - velocity;
        }

        public Vector3 CalculateAcceleration(Vector3 targetVelocity, Vector3 velocity, Vector3 maxAcceleration)
        {
            Vector3 acceleration = CalculateAcceleration(targetVelocity, velocity);
            if (acceleration != Vector3.Zero) acceleration.Normalize();
            acceleration *= maxAcceleration;
            return acceleration;
        }

        /// <summary>
        /// Calculates distance from the four sides (left, right, forward, backward) of the object to the target.
        /// </summary>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public RelativePosition CalculateDistance(Vector3 targetPosition)
        {
            float distLeft = Vector3.Distance(GetPosition + Rotation.Left, targetPosition);
            float distRight = Vector3.Distance(GetPosition + Rotation.Right, targetPosition);
            float distForw = Vector3.Distance(GetPosition + Rotation.Forward, targetPosition);
            float distBack = Vector3.Distance(GetPosition + Rotation.Backward, targetPosition);

            return new RelativePosition(distForw, distBack, distLeft, distRight);
        }

    }
}
