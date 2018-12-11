using System;
using Microsoft.Xna.Framework;
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

        //change setters so it doesnt allow to set maxDivision values lower than minDivision and vice versa
        public int MinDivision { get => minDivision; set => minDivision = value; }
        public int MaxDivision { get => maxDivision; set => maxDivision = value; }
        public int DivisionStep { get => divisionStep; set => divisionStep = value; }

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

        
        /// <summary>
        /// Calculates a new direction vector.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <returns>Unit direction vector. </returns>
        public Vector2 GeneratePoint()
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
            if (division + divisionStep > maxDivision && !valueChanged)
            {
                max = maxDivision;
                int diff = (division + divisionStep) - maxDivision;
                min = division - divisionStep - diff;
            }
            else max = division + divisionStep;

            division = random.Next(min, max + 1);
            Vector2 point = MathHelpersCls.GetPositionFromAngle(MathHelpersCls.GetAngleFromCircle(numOfDivisions, division));

            return point;
        }
    }
}
