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
    class TankLabel
    {
        public Vector2 ScreenLabelCoordinates(GraphicsDevice device, ModelObject tank, float aspectRatio, Camera cam)
        {
            Vector3 clientResult = Vector3.Zero;
            Vector3 TankSpace = new Vector3(tank.WorldMatrix.Translation.X, tank.WorldMatrix.Translation.Y + 120, tank.WorldMatrix.Translation.Z);
            Vector3 vector = device.Viewport.Project(tank.WorldMatrix.Translation, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f), cam.ViewMatrix, Matrix.CreateTranslation(0, 10, 0));
            clientResult.X = vector.X;
            clientResult.Y = vector.Y;
            Vector2 clientResultV2 = new Vector2(clientResult.X - 35, clientResult.Y - 80);
            return clientResultV2;
        }



        public void DrawLabel(GraphicsDevice device, SpriteBatch spriteBatch, int cameraIndex, Texture2D labelTexture1, Texture2D labelTexture2, Camera cam, float aspectRatio, ModelObject tank, ModelObject tank2)
        {
            if (cameraIndex == 2)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(labelTexture1, ScreenLabelCoordinates(device, tank, aspectRatio, cam), Color.White);
                spriteBatch.End();
            }

            else if (cameraIndex == 4)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(labelTexture2, ScreenLabelCoordinates(device, tank2, aspectRatio, cam), Color.White);
                spriteBatch.End();
            }
        }

    }
}
