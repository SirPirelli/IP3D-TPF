using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WanderBehaviour.Geometry;

namespace IP3D_TPF.AIBehaviour
{
    class WanderMovement
    {
        Circle circle;
        Random random;
        int numOfDivisions, division,
            minDivision, maxDivision,
            divisionStep;

        public Circle Circle { get => circle; set => circle = value; }
        public int DivisionRandomized { get => division; }

        public WanderMovement()
        {
            this.numOfDivisions = 256;
            this.minDivision = 20;
            this.maxDivision = 200;
            this.divisionStep = 20;
            this.division = 60;
            this.random = new Random();
            this.circle = new Circle(Vector3.Zero, 1, Game1.graphicsDevice);
        }

        public Vector2 Update(GameTime gameTime)
        {
            int min, max;
            bool valueChanged = false;

            //set min division
            if (division - divisionStep < minDivision)
            {
                min = minDivision;
                int diff = Math.Abs(division - divisionStep - minDivision);
                max = division + divisionStep + diff;
                valueChanged = true;
            }
            else min = division - divisionStep;

            //set max division
            if (division + divisionStep > maxDivision)
            {
                max = maxDivision;
                int diff = (division + divisionStep) - maxDivision;
                min = division - divisionStep - diff;
            }
            else max = division + divisionStep;

            division = random.Next(min, max);
            Vector2 point = MathHelpersCls.GetPositionFromAngle(MathHelpersCls.GetAngleFromCircle(numOfDivisions, division));

            return point;
        }
    }
}
