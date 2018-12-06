using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF.AIBehaviour
{
    /// <summary>
    /// This class is not being properly used (architected).
    /// </summary>
    class SeekFlee
    {
        ModelObject parent;
        ModelObject target;
        float maxAcceleration;
        float maxVelocity;

        public ModelObject Parent { get => parent; set => parent = value; }
        public ModelObject Target { get => target; set => target = value; }
        public float MaxAcceleration { get => maxAcceleration; set => maxAcceleration = value; }
        public float MaxVelocity { get => maxVelocity; set => maxVelocity = value; }
        public bool Seek { get; set; }

        public SeekFlee(ModelObject parent, ModelObject target, float maxAcceleration, float maxVelocity)
        {
            this.parent = parent;
            this.target = target;
            this.maxAcceleration = maxAcceleration;
            this.maxVelocity = maxVelocity;
        }

        public SeekFlee()
        {
            this.parent = null;
            this.target = null;
            this.maxAcceleration = 20f;
            this.maxVelocity = 10f;
        }

    }
}
