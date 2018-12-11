using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace IP3D_TPF
{
    class ParticleSystem
    {
        #region FIELDS
        List<Particle> particleList;
        float timer;

        ModelObject Tank;
        Random rand;
        Vector3 position;
        #endregion

        #region PROPERTIES
        public Model Particle { get; set; }
        #endregion

        // Construtor do sistema de Particulas
        //nao precisamos de passar um ModelObject, o particle system apenas deve receber um vector3
        public ParticleSystem(ModelObject Tank, Model mud)
        {
            this.Particle = mud;
            //Initial Constructor data
            //Initializes Particle list
            particleList = new List<Particle>();
            this.Tank = Tank;
            this.rand = new Random();
            position = Tank.Translation.Translation;
        }


        //Draws the particles available in the list of particles
        public void DrawParticles(Matrix viewMatrix, Matrix projectionMatrix, Texture2D texture)
        {
            foreach (Particle particle in particleList)
            {
                //Draws particles, according to viewMatrix,projection and texture of the bill mesh, since the coin does not have any texture applied, only has geometry 
                //and vertex color.

                particle.DrawParticle(viewMatrix, projectionMatrix, texture);
            }
        }


        public void Update(GameTime gameTime, Matrix WorldMatrix)
        {
            if (position != Tank.Translation.Translation)
            {
                //Timer that its defined trough the density given in constructor and after each cycle of time is completed the particles get added to the list
                timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (timer > 0)
                {
                    timer -= 1f / 1200;
                    particleList.Add(new Particle(Tank, Particle, gameTime, rand));
                }
            }

            //Removing particles when reaching certain limit(Limitation on bottom only, since gravity is negative)
            for (int i = 0; i < particleList.Count; i++)
            {
                //Updates the position of each mesh contained on the list of particles
                particleList[i].UpdateParticle(gameTime);

                //Checks if particles are bellow a certain height, if they are, they are removed from list at the correct index of the list
                if (particleList[i].Disabled == true)
                {
                    //Removes particles at the index of the list where the particle reached its limit
                    particleList.RemoveAt(i);
                }
            }
            position = Tank.Translation.Translation;
        }
    }
}



