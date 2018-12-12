using System.Collections.Generic;
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

            if (Game1.inputs.Check(Keys.Space) && pressable == true)
            {
                timer = (float)gameTime.TotalGameTime.TotalSeconds;
                pressable = false;
                bulletList.Add(new Shot(tank, shell));

            }

            if (gameTime.TotalGameTime.TotalSeconds > timer + 1f)
            {
                pressable = true;
            }

            foreach (Shot shot in bulletList)
            {
                shot.UpdateParticle(gameTime);
            }

            for (int i = 0; i < bulletList.Count; i++)
            {
                /* if the bullet is out of bound */
                if (bulletList[i].WorldMatrix.Translation.X < 0 ||
                    bulletList[i].WorldMatrix.Translation.Z < 0 ||
                    bulletList[i].WorldMatrix.Translation.X > tank.Terrain.TerrainBounds.X ||
                    bulletList[i].WorldMatrix.Translation.Z > tank.Terrain.TerrainBounds.Y)
                {
                    if (bulletList[i].WorldMatrix.Translation.Y <= 0)
                    {
                        bulletList.Remove(bulletList[i]);
                        continue;
                    }
                }
                /* if the bullet is inside the terrain bounds */
                else if (bulletList[i].WorldMatrix.Translation.Y <= tank.Terrain.CalculateHeightOfTerrain(bulletList[i].WorldMatrix.Translation))
                {
                    bulletList.Remove(bulletList[i]);
                    continue;
                }

                /* checks for collision */
                if (CollisionHandler.IsColliding(bulletList[i].BoundingSphere, tank2.BoundingSphere) == true)
                {
                    bulletList.Remove(bulletList[i]);
                    tank2.Health -= 10;
                    if (tank2.Health <= 0) tank2.Dead = true;
                }


            }

            if(bulletList.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine(bulletList[0].WorldMatrix.Translation);
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
