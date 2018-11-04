using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    class Camera
    {
        /* Yaw é a rotação no eixo horizontal
         * Pitch é a rotação no eixo vertical
           Roll é a rotação no seu proprio eixo (pensar num aviao a dar rodar) (no caso da camara nao sera utilizado para movimento) */

        #region Fields
        private Vector3 position;
        private Vector3 target;
        private Matrix viewMatrix;
        private float yaw;
        private float pitch;
        private float velocity;

        private TerrainGenerator terrain;
        private Vector2 viewportCenter;
        private float ScaleRadiansPerPixel;
        private float offsetY;

        #endregion

        #region Properties
        public Vector3 Position { get { return position; } set { position = value; } }
        public Vector3 Target { get { return target; } set { target = value; } }
        public Matrix ViewMatrix { get { return viewMatrix; } set { viewMatrix = value; } }
        public float Yaw { get { return yaw; } set { yaw = value; } }
        public float Pitch { get { return pitch; } set { pitch = value; } }
        public float Velocity { get { return velocity; } set { velocity = value; } }
        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a_position"></param>
        /// <param name="a_target"></param>
        /// <param name="a_viewportCenter"></param>
        /// <param name="a_radiansPP"></param>
        /// <param name="a_velocity"></param>
        /// <param name="a_terrain"></param>
        /// <param name="a_offsetY"></param>
        public Camera(Vector3 a_position, Vector3 a_target, Vector2 a_viewportCenter, float a_radiansPP, float a_velocity, TerrainGenerator a_terrain, float a_offsetY)
        {
            position = a_position;
            target = a_target;
            viewportCenter = a_viewportCenter;
            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            ScaleRadiansPerPixel = a_radiansPP;
            velocity = a_velocity;
            terrain = a_terrain;
            offsetY = a_offsetY;

            /* http://community.monogame.net/t/solved-reverse-createfromyawpitchroll-or-how-to-get-the-vector-that-would-produce-the-matrix-given-only-the-matrix/9054/4 */
            Vector3 yawPitchRoll = MathHelpersCls.ExtractYawPitchRoll(viewMatrix);
            yaw = yawPitchRoll.X;
            pitch = yawPitchRoll.Y;
            Mouse.SetPosition((int)viewportCenter.X, (int)viewportCenter.Y);


        }
        #endregion

        public void Update(GameTime gameTime, TerrainGenerator terrain, Inputs inputs)
        {

            Vector3 direction;
            KeyboardState keyboardState = inputs.KeyboardState;
            MouseState mouseState = inputs.MouseState;

            //System.Diagnostics.Debug.WriteLine("MousePos: " + mouseState.Position);
            //System.Diagnostics.Debug.WriteLine("ViewCenter: " + viewportCenter);
            //System.Diagnostics.Debug.WriteLine("Position: " + position);

            #region MOUSE STATE
            int deltaX = mouseState.X - (int)viewportCenter.X;
            int deltaY = mouseState.Y - (int)viewportCenter.Y;

            yaw -= deltaX * ScaleRadiansPerPixel;
            pitch -= deltaY * ScaleRadiansPerPixel;
            pitch = MathHelper.Clamp(pitch, -1f, 1f);

            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0f);
            direction = Vector3.Transform(-Vector3.UnitZ, rotation);
            #endregion

            #region KEYBOARD STATE
            Vector3 right = Vector3.Cross(direction, Vector3.Up);

            if (keyboardState.IsKeyDown(Keys.NumPad8))
            {
                position += direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad5))
            {
                position -= direction * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if(keyboardState.IsKeyDown(Keys.NumPad4))
            {
                position -= right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if(keyboardState.IsKeyDown(Keys.NumPad6))
            {
                position += right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            #endregion

            target = position + direction;

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            Mouse.SetPosition((int)viewportCenter.X, (int)viewportCenter.Y);

            position.X = MathHelper.Clamp(position.X, 0, terrain.TerrainBounds.X - terrain.PlaneLength - 1);
            position.Z = MathHelper.Clamp(position.Z, 0, terrain.TerrainBounds.Y - terrain.PlaneLength - 1);
            //position.Y = CalculateHeightOfTerrain(position) + offsetY;

        }


        /// <summary>
        /// Returns the height of the terrain at a given position.
        /// </summary>
        /// <param name="position">The position where we want to know the height</param>
        /// <returns>float height</returns>
        public float CalculateHeightOfTerrain(Vector3 position)
        {
            HeightMap heightMap = terrain.HeightMap;
            float planeLength = terrain.PlaneLength;
            float heightRatio = terrain.HeightRatio;
            float yA, yB, yC, yD;

            float x = position.X;                                               float z = position.Z;
            float x1 = (position.X - (position.X % planeLength));            float x2 = x1 + planeLength;
            float z1 = (position.Z - (position.Z % planeLength));            float z2 = z1 + planeLength;


            /* Para nao termos de fazer a multiplicaçao do heightRatio todos os frames, podemos introduzir no heightMap logo os valores finais,
             * depois construimos outra vez o terreno. */
            yA = heightMap.GetValueFromHeightMap(heightMap.GetIndexFromPosition(new Vector3(x1, 0, z1), planeLength)) * heightRatio;
            yB = heightMap.GetValueFromHeightMap(heightMap.GetIndexFromPosition(new Vector3(x2, 0, z1), planeLength)) * heightRatio;
            yC = heightMap.GetValueFromHeightMap(heightMap.GetIndexFromPosition(new Vector3(x1, 0, z2), planeLength)) * heightRatio;
            yD = heightMap.GetValueFromHeightMap(heightMap.GetIndexFromPosition(new Vector3(x2, 0, z2), planeLength)) * heightRatio;

            return MathHelpersCls.BiLerp(new Vector2(x, z), x1, x2, z1, z2, yA, yB, yC, yD);
        }           
    }
}
