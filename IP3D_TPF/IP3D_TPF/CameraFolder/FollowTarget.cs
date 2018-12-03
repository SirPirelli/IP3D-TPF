using Microsoft.Xna.Framework;

namespace IP3D_TPF.CameraFolder
{
    class FollowTarget : ICamera
    {
        CameraManager cameraManager;

        private Vector3 position;
        private Vector3 target;
        private Vector3 offset;

        public Vector3 Position { get => position; set => position = value; }
        public Vector3 Target { get => target; set => target = value; }
        public ModelObject TargetModel { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public Vector3 Offset { get => offset; set => offset = value; }
        public float NearPlaneDistance { get; set; }
        public float FarPlaneDistance { get; set; }
        public float FieldOfViewDegrees { get; set; }

        public FollowTarget(CameraManager cameraManager, ModelObject targetModel, float nearPlaneDistance, float farPlaneDistance)
        {

            this.cameraManager = cameraManager;
            this.TargetModel = targetModel;
            this.position = Vector3.Zero;
            this.target = Vector3.Zero;
            this.offset = Vector3.Up * 3;
            this.NearPlaneDistance = nearPlaneDistance;
            this.FarPlaneDistance = farPlaneDistance;
            this.FieldOfViewDegrees = 45;
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfViewDegrees), cameraManager.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);
            this.ViewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

        }
        public FollowTarget(CameraManager cameraManager, float nearPlaneDistance, float farPlaneDistance)
        {
            this.cameraManager = cameraManager;
            this.TargetModel = null;
            this.position = Vector3.Zero;
            this.target = Vector3.Zero;
            this.offset = Vector3.Up * 3;
            this.NearPlaneDistance = nearPlaneDistance;
            this.FarPlaneDistance = farPlaneDistance;
            this.FieldOfViewDegrees = 45;
            this.ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfViewDegrees), cameraManager.Viewport.AspectRatio, nearPlaneDistance, farPlaneDistance);
            this.ViewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);
        }


        public void Update(GameTime gameTime)
        {
            if (TargetModel == null) System.Diagnostics.Debug.WriteLine("FOLLOW TARGET CAM: TARGET MODEL IS NULL");

            Vector3 cameraRotationalTarget = -TargetModel.Rotation.Forward;
            cameraRotationalTarget.Normalize();

            var target = TargetModel.GetPosition + Vector3.Up * 5;

            cameraRotationalTarget = (cameraRotationalTarget * 20f) + offset;
            position = TargetModel.GetPosition + cameraRotationalTarget + Vector3.Up * 20; ;
            position.Y = MathHelper.Clamp(position.Y, cameraManager.Terrain.CalculateHeightOfTerrain(position) + 2f, 20f);

            ViewMatrix = Matrix.CreateLookAt(position, target, Vector3.Up);

        }
    }
}
