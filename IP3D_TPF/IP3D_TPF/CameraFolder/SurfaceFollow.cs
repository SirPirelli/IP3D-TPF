
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace IP3D_TPF.CameraFolder
{
    class SurfaceFollow : ICamera
    {

        CameraManager cameraManager;

        #region FIELDS
        private float yaw;
        private float pitch;

        private Vector3 position;

        #endregion

        #region PROPERTIES

        public float ScaleRadiansPerPixel { get; set; }
        public float Velocity { get; set; }
        public float OffsetY { get; set; }

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 Target { get; set; }
        public Matrix ViewMatrix { get; set; }

        public Matrix ProjectionMatrix { get; set; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public float FieldOfViewDegrees { get; set; }

        #endregion

        #region CONSTRUCTORS

        public SurfaceFollow(CameraManager cameraManager, Vector3 position, Vector3 target, float radiansPerPixel, float velocity, Viewport viewport, float fovDegAngle, float nearPlane, float farPlane)
        {
            this.cameraManager = cameraManager;
            this.Position = position;
            this.Target = target;
            this.ScaleRadiansPerPixel = radiansPerPixel;
            this.Velocity = velocity;

            OffsetY = 5f;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(fovDegAngle), viewport.AspectRatio, nearPlane, farPlane);
            ViewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

            var yawPitchRoll = MathHelpersCls.ExtractYawPitchRoll(ViewMatrix);
            yaw = yawPitchRoll.X;
            pitch = yawPitchRoll.Y;

            Game1.inputs.SetMousePosition(cameraManager.Viewport.Bounds.Center.ToVector2());
        }
        public SurfaceFollow(CameraManager cameraManager, float nearPlaneDistance, float farPlaneDistance)
        {
            this.cameraManager = cameraManager;
            this.Position = new Vector3(50,50, 50);
            this.Target = new Vector3(300, 30, 300);
            this.ScaleRadiansPerPixel = MathHelper.Pi / 1000f;
            this.Velocity = 30f;
            this.NearPlaneDistance = nearPlaneDistance;
            this.FarPlaneDistance = farPlaneDistance;
            this.FieldOfViewDegrees = 45;

            OffsetY = 5f;

            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfViewDegrees), cameraManager.Viewport.AspectRatio, NearPlaneDistance, FarPlaneDistance);
            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            var yawPitchRoll = MathHelpersCls.ExtractYawPitchRoll(ViewMatrix);
            yaw = yawPitchRoll.X;
            pitch = yawPitchRoll.Y;

            Game1.inputs.SetMousePosition(cameraManager.Viewport.Bounds.Center.ToVector2());

        }

        #endregion

        public void Update(GameTime gameTime)
        {
            #region MOUSE STATE

            var delta = Game1.inputs.GetMouseDeltaPosition(cameraManager.Viewport.Bounds.Center.ToVector2());

            yaw -= delta.X * ScaleRadiansPerPixel;
            pitch -= delta.Y * ScaleRadiansPerPixel;
            pitch = MathHelper.Clamp(pitch, -1f, 1f);

            Matrix rotation = Matrix.CreateFromYawPitchRoll(yaw, pitch, 0);
            Vector3 direction = Vector3.Transform(-Vector3.UnitZ, rotation);
            #endregion

            #region KEYBOARD STATE
            
            Vector3 right = Vector3.Cross(direction, Vector3.Up);

            if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad8))
            {
                Position += direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad5))
            {
                Position -= direction * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad4))
            {
                Position -= right * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.NumPad6))
            {
                Position += right * Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds
;
            }
            #endregion

            position.X = MathHelper.Clamp(Position.X, 0, cameraManager.Terrain.TerrainBounds.X - cameraManager.Terrain.PlaneLength - 1);
            position.Z = MathHelper.Clamp(Position.Z, 0, cameraManager.Terrain.TerrainBounds.Y - cameraManager.Terrain.PlaneLength - 1);
            position.Y = cameraManager.Terrain.CalculateHeightOfTerrain(Position) + OffsetY;

            Target = Position + direction;

            ViewMatrix = Matrix.CreateLookAt(Position, Target, Vector3.Up);

            Game1.inputs.SetMousePosition(cameraManager.Viewport.Bounds.Center.ToVector2());
        }
    }
}
