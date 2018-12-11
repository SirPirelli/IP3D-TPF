using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IP3D_TPF.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF
{
    class ShotManager
    {
        List<Shot> bulletList;
        Model shell;
        Tank tank, tank2;
        float timer;
        bool pressed;
        bool pressable;

        public ShotManager(Tank tank, Tank tank2,Model shell)
        {
            bulletList = new List<Shot>();
            this.shell = shell;
            this.tank = tank;
            this.tank2 = tank2;
            pressed = false;
            pressable = true;

        }

        public void UpdateShots(GameTime gameTime)
        {
            float x = tank.Terrain.TerrainBounds.X - tank.Terrain.PlaneLength - 5;
            float z = tank.Terrain.TerrainBounds.Y - tank.Terrain.PlaneLength - 5;

            if (Game1.inputs.CurrentMouseState.LeftButton == ButtonState.Pressed && pressable == true)
            {
                timer = (float)gameTime.TotalGameTime.TotalSeconds;
                pressable = false;
                bulletList.Add(new Shot(tank, shell));

            }

            if (gameTime.TotalGameTime.TotalSeconds > timer + 1f)
            {
                pressable = true;
            }

            System.Diagnostics.Debug.WriteLine(pressable);

            foreach (Shot shot in bulletList)
            {
                shot.UpdateParticle(gameTime);
            }

            for (int i =0; i < bulletList.Count; i++)
            {
                if (bulletList[i].WorldMatrix.Translation.Y <= tank.Terrain.CalculateHeightOfTerrain(bulletList[i].WorldMatrix.Translation))
                {
                    bulletList.Remove(bulletList[i]);
                }
                else if (bulletList[i].WorldMatrix.Translation.X >= x)
                {
                    bulletList.Remove(bulletList[i]);
                }
                else if (bulletList[i].WorldMatrix.Translation.Z >= z)
                {
                    bulletList.Remove(bulletList[i]);
                }
                else if (bulletList[i].WorldMatrix.Translation.X < 1)
                {
                    bulletList.Remove(bulletList[i]);
                }
                else if (bulletList[i].WorldMatrix.Translation.Z < 1)
                {
                    bulletList.Remove(bulletList[i]);
                }

                else {
                    if (CollisionHandler.IsColliding(bulletList[i].BoundingSphere, tank2.BoundingSphere) == true)
                {
                        bulletList.Remove(bulletList[i]);
                        tank2.Health -= 10;
                    }
                }
                if (tank2.Health <= 0) tank2.Dead = true;

            }


        }

        public void DrawParticles(Matrix viewMatrix , Texture2D texture, float aspectRatio)
        {
            foreach (Shot shot in bulletList)
            {
                shot.DrawParticle(viewMatrix, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f), texture);
            }
        }
    }
}
