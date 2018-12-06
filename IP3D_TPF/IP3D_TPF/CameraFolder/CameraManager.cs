using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace IP3D_TPF.CameraFolder
{
    class CameraManager
    {

        FreeCamera freeCamera;
        FollowTarget followTargetCam;
        SurfaceFollow surfaceFollowCam;
        int previousActiveCameraIndex;

        public TerrainGenerator Terrain { get; set; }
        public Viewport Viewport { get; set; }
        public FreeCamera FreeCamera { get => freeCamera; }
        public FollowTarget FollowTarget { get => followTargetCam; }
        public SurfaceFollow SurfaceFollow { get => surfaceFollowCam; }
        public int ActiveCameraIndex { get; protected set; }

        private List<ModelObject>PlayersList { get; set; }

        /* ESTAS PROPRIEDADES SAO TEMPORARIAS 
         Como ao fazer listas de interfaces, nao temos acesso à instancia do objecto,
         temos que criar uma classe Camera :  ICamera e só depois criamos classes
         SurfaceFollow:Camera, etc, senao nao conseguimos agrupar as camaras numa lista */
        public Matrix ActiveViewMatrix { get; protected set; }
        public Matrix ActiveProjectionMatrix { get; protected set; }


        #region CONSTRUCTORS
        public CameraManager(Viewport viewport, TerrainGenerator terrain, float nearPlaneDistance, float farPlaneDistance, List<ModelObject> playersList)
        {
            this.Viewport = viewport;
            this.Terrain = terrain;
            PlayersList = playersList;

            freeCamera = new FreeCamera(this, nearPlaneDistance, farPlaneDistance);
            followTargetCam = new FollowTarget(this, nearPlaneDistance, farPlaneDistance);
            surfaceFollowCam = new SurfaceFollow(this, nearPlaneDistance, farPlaneDistance);

            ActiveCameraIndex = 0;
            ActiveViewMatrix = freeCamera.ViewMatrix;
            ActiveProjectionMatrix = freeCamera.ProjectionMatrix;

        }
        #endregion

        public void Update(GameTime gametime)
        {
            CheckInputs();

            switch(ActiveCameraIndex)
            {
                case 0:
                    freeCamera.Update(gametime);
                    ActiveViewMatrix = freeCamera.ViewMatrix;
                    ActiveProjectionMatrix = freeCamera.ProjectionMatrix;
                    break;

                case 1:
                    surfaceFollowCam.Update(gametime);
                    ActiveViewMatrix = surfaceFollowCam.ViewMatrix;
                    ActiveProjectionMatrix = surfaceFollowCam.ProjectionMatrix;
                    break;

                case 2:
                    
                    followTargetCam.Update(gametime);
                    ActiveViewMatrix = followTargetCam.ViewMatrix;
                    ActiveProjectionMatrix = followTargetCam.ProjectionMatrix;
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("CAMERA MANAGER SWITCH CASE IS GOING TO DEFAULT");
                    break;

            }

            previousActiveCameraIndex = ActiveCameraIndex;

        }

        private void CheckInputs()
        {
            if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                ActiveCameraIndex = 0;
            }
            else if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.F2))
            {
                ActiveCameraIndex = 1;
            }
            else if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.F3))
            {
                followTargetCam.TargetModel = PlayersList[0];
                ActiveCameraIndex = 2;
            }
            else if (Game1.inputs.Check(Microsoft.Xna.Framework.Input.Keys.F4))
            {
                followTargetCam.TargetModel = PlayersList[1];
                ActiveCameraIndex = 2;
            }
        }
    }
}
