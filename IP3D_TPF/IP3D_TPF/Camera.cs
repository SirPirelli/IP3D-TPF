using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


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
        private Matrix projectionMatrix;
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
        public Matrix ProjectionMatrix { get => projectionMatrix; set => projectionMatrix = value; }
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
        public Camera(Vector3 a_position, Vector3 a_target, Vector2 a_viewportCenter, float a_radiansPP, float a_velocity, TerrainGenerator a_terrain, float a_offsetY,
                        float fovAngleDeg, float nearPlane, float farPlane, float aspectRatio)
        {
            position = a_position;
            target = a_target;
            viewportCenter = a_viewportCenter;
            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovAngleDeg), aspectRatio, nearPlane, farPlane);
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

        // criar uma interface para camaras, metodo update (pelo menos) tem que ser virtual
        //visto q cada camara precisa de argumentos diferentes

        public void UpdateFreeCamera(GameTime gameTime, TerrainGenerator terrain)
        {
            Vector3 direction;
            KeyboardState keyboardState = Game1.inputs.CurrentKeyboardState;
            MouseState mouseState = Game1.inputs.CurrentMouseState;

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
            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                position -= right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad6))
            {
                position += right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            #endregion

            target = position + direction;

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            Mouse.SetPosition((int)viewportCenter.X, (int)viewportCenter.Y);

            float heightMin = terrain.HeightMap.GetValueFromHeightMap(terrain.HeightMap.CalculateIndexFromPosition(position,
                                                                                terrain.PlaneLength)) * terrain.HeightRatio;

            position.X = MathHelper.Clamp(position.X, 0, terrain.TerrainBounds.X - terrain.PlaneLength - 1);
            position.Z = MathHelper.Clamp(position.Z, 0, terrain.TerrainBounds.Y - terrain.PlaneLength - 1);
            position.Y = MathHelper.Clamp(position.Y, heightMin + 2f, 3000f);

        }

        public void UpdateFollow(GameTime gameTime, Vector3 tankPosition, Vector3 cameraRotationalTarget)
        {
            //position = tankPosition + (cameraRotationalTarget * 20) + (Vector3.UnitY * 10);

            Vector3 Offset = new Vector3(0, -50, 0);
            cameraRotationalTarget.Normalize();
            Vector3 BackwardsCorrected = (cameraRotationalTarget * 20) + Offset;
            BackwardsCorrected.Y = 30; tankPosition.Y += 5;

            position = tankPosition + (BackwardsCorrected);
            // se a camara estiver fora do terreno, programa crasha pq nao encontra o indice do terreno. criar condiçao caso esteja fora do terreno
            position.Y = MathHelper.Clamp(position.Y, terrain.CalculateHeightOfTerrain(position)+2f, 20f);

            viewMatrix = Matrix.CreateLookAt(position, tankPosition, Vector3.Up);
        }

        public void UpdateSurfaceFollow(GameTime gameTime, TerrainGenerator terrain, Inputs inputs)
        {
            Vector3 direction;
            KeyboardState keyboardState = inputs.CurrentKeyboardState;
            MouseState mouseState = inputs.CurrentMouseState;

            //System.Diagnostics.Debug.WriteLine("MousePos: " + mouseState.Position);
            //System.Diagnostics.Debug.WriteLine("ViewCenter: " + viewportCenter);
            //System.Diagnostics.Debug.WriteLine("Position: " + position);

            #region MOUSE STATE
            int deltaX = mouseState.X - (int)viewportCenter.X;
            int deltaY = mouseState.Y - (int)viewportCenter.Y;

            yaw -= deltaX * ScaleRadiansPerPixel;
            pitch -= deltaY * ScaleRadiansPerPixel;
            Pitch = MathHelper.Clamp(pitch, -1f, 1f);

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
            if (keyboardState.IsKeyDown(Keys.NumPad4))
            {
                position -= right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (keyboardState.IsKeyDown(Keys.NumPad6))
            {
                position += right * velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            #endregion

            target = position + direction;
            //Debug Logic of target
            //System.Diagnostics.Debug.WriteLine(target);

            viewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
            Mouse.SetPosition((int)viewportCenter.X, (int)viewportCenter.Y);

            //Fixação dos valores de boundary do terreno
            position.X = MathHelper.Clamp(position.X, 0, terrain.TerrainBounds.X - terrain.PlaneLength - 1);
            position.Z = MathHelper.Clamp(position.Z, 0, terrain.TerrainBounds.Y - terrain.PlaneLength - 1);
            position.Y = terrain.CalculateHeightOfTerrain(position) + offsetY;

        }


                 
    }
}
