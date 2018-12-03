using Microsoft.Xna.Framework;

namespace IP3D_TPF.CameraFolder
{
    class FreeCamera : ICamera
    {

        CameraManager cameraManager;

        #region FIELDS
        float yaw;
        float pitch;
        #endregion


        #region PROPERTIES
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }

        public float ScaleRadiansPerPixel { get; set; }
        public float Velocity { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public float FieldOfViewDegrees { get; set; }

        #endregion

        #region CONSTRUCTORS

        public FreeCamera(CameraManager cameraManager, Vector3 position, Vector3 target, float fovDegAngle, float nearPlane, float farPlane, float radiansPerPixel)
        {
            this.cameraManager = cameraManager;

            this.Position = position;
            this.Target = target;
            this.Velocity = 30f;
            this.ScaleRadiansPerPixel = radiansPerPixel;
            this.FieldOfViewDegrees = fovDegAngle;
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfViewDegrees), cameraManager.Viewport.AspectRatio, nearPlane, farPlane);
            this.ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

        }
        public FreeCamera(CameraManager cameraManager, float nearPlaneDistance, float farPlaneDistance)
        {
            this.cameraManager = cameraManager;

            this.Position = new Vector3(0,20,50);
            this.Target = new Vector3(700, 20, 300);
            this.Velocity = 30f;
            this.ScaleRadiansPerPixel = MathHelper.Pi / 1000f;
            this.NearPlaneDistance = nearPlaneDistance;
            this.FarPlaneDistance = farPlaneDistance;
            this.FieldOfViewDegrees = 45;
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfViewDegrees), cameraManager.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance);
            this.ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            var yawPitchRoll = MathHelpersCls.ExtractYawPitchRoll(ViewMatrix);
            yaw = yawPitchRoll.X;
            pitch = yawPitchRoll.Y;

            cameraManager.Inputs.SetMousePosition(cameraManager.Viewport.Bounds.Center.ToVector2());

        }

        #endregion

        public void Update(GameTime gameTime)
        {
            #region MOUSE STATE

            var delta = cameraManager.Inputs.GetMouseDeltaPosition(cameraManager.Viewport.Bounds.Center.ToVector2());

            yaw -= delta.X * ScaleRadiansPerPixel;
            pitch -= delta.Y * ScaleRadiansPerPixel;
            pitch = MathHelper.Clamp(pitch, -1f, 1f);

            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotation);
            #endregion

            #region KEYBOARD STATE

            Vector3 right = Vector3.Cross(direction, Vector3.Up);

            if(cameraManager.Inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad8))
            {
                Position += direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if(cameraManager.Inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad2))
            {
                Position -= direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }

            if (cameraManager.Inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad4))
            {
                Position -= right * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            else if(cameraManager.Inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad6))
            {
                Position += right * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            }
            #endregion

            Target = Position + direction;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            cameraManager.Inputs.SetMousePosition(cameraManager.Viewport.Bounds.Center.ToVector2());
        }

    }
}
