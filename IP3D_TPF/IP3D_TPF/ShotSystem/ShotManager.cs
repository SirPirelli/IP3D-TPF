using System.Collections.Generic;
using BoundingSpheresTest;
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
        float timer, timer2;
        bool pressed;
        bool pressable;

        BoundingSphereCls shotRadius;

        public ShotManager(Tank tank, Tank tank2,Model shell)
        {
            bulletList = new List<Shot>();
            this.shell = shell;
            this.tank = tank;
            this.tank2 = tank2;
            pressed = false;
            pressable = true;
            timer2 = 5;

            shotRadius = new BoundingSphereCls(tank2.GetPosition, 30);

        }

        public void UpdateShots(GameTime gameTime)
        {
            #region TANK1
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
            #endregion

            #region TANK2 AI

            if(tank2.IsAI)
            {

                if (gameTime.TotalGameTime.TotalSeconds > timer2)
                {

                    shotRadius.Center = tank2.GetPosition;
                    if(shotRadius.Intersects(tank.BoundingSphere))
                    {

                        timer2 = (float)gameTime.TotalGameTime.TotalSeconds + Game1.random.Next(1, 6);
                        bulletList.Add(new Shot(tank2, shell));
                        System.Diagnostics.Debug.WriteLine("FIRE");
                    }             
                }
            }

            #endregion

            foreach (Shot shot in bulletList)
            {
                shot.UpdateParticle(gameTime);
                if (shot.Parent == tank2) System.Diagnostics.Debug.WriteLine(shot.WorldMatrix.Translation);

            }

            for (int i = 0; i < bulletList.Count; i++)
            {
                /* if the bullet is out of bound */
                if (bulletList[i].WorldMatrix.Translation.X < 0 ||
                    bulletList[i].WorldMatrix.Translation.Z < 0 ||
                    bulletList[i].WorldMatrix.Translation.X > tank.Terrain.TerrainBounds.X ||
                    bulletList[i].WorldMatrix.Translation.Z > tank.Terrain.TerrainBounds.Y)
                {
                    if (bulletList[i].WorldMatrix.Translation.Y <= -50)
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
                if(bulletList[i].Parent != tank2)
                {

                    if (CollisionHandler.IsColliding(bulletList[i].bulletCollider, tank2.BoundingSphere) == true)
                    {
                        bulletList.Remove(bulletList[i]);
                        tank2.Health -= 10;
                        if (tank2.Health <= 0) tank2.Dead = true;
                    }

                }
                else
                {
                    if (CollisionHandler.IsColliding(bulletList[i].bulletCollider, tank.BoundingSphere) == true)
                    {
                        bulletList.Remove(bulletList[i]);
                        tank.Health -= 10;
                        if (tank.Health <= 0) tank.Dead = true;
                    }
                }

            }
        }

        public void DrawParticles(GraphicsDevice device,Matrix viewMatrix , Texture2D texture, float aspectRatio)
        {

            #region DEBUG SHOT RADIUS
            //shotRadius.Draw(device, viewMatrix, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f));
            #endregion

            foreach (Shot shot in bulletList)
            {
                shot.DrawParticle(device,viewMatrix, Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 4000.0f), texture);
            }
        }
    }
}
