using IP3D_TPF.AIBehaviour;
using IP3D_TPF.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    struct RelativePosition
    {
        public float forwardDist;
        public float backwardDist;
        public float leftDist;
        public float rightDist;

        public RelativePosition(float forwardDist, float backwardDist, float leftDist, float rightDist)
        {
            this.forwardDist = forwardDist;
            this.backwardDist = backwardDist;
            this.leftDist = leftDist;
            this.rightDist = rightDist;
        }


    }
}
