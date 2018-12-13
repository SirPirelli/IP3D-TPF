using System;
using IP3D_TPF.Utilities;
using Microsoft.Xna.Framework;

namespace IP3D_TPF
{
    class MathHelpersCls
    {
        /// <summary>
        /// Interpolation functions between two variables on a 2D grid.
        /// </summary>
        /// <param name="pos">Point to be considered.</param>
        /// <param name="x1">Far left value.</param>
        /// <param name="x2">Far right value.</param>
        /// <param name="y1">Far up value.</param>
        /// <param name="y2">Far down value.</param>
        /// <param name="weight11">x1y1 weight value.</param>
        /// <param name="weight21">x2y2 weight value.</param>
        /// <param name="weight12">x1y2 weight value.</param>
        /// <param name="weight22">x2y2 weight value.</param>
        /// <remarks> https://www.ajdesigner.com/phpinterpolation/bilinear_interpolation_equation.php#ajscroll </remarks>
        /// <returns>Floating-point precision value.</returns>
        public static float BiLerp(Vector2 pos, float x1, float x2, float y1, float y2, float weight11, float weight21, float weight12, float weight22)
        {

            float p1 = (((x2 - pos.X) * (y2 - pos.Y)) / ((x2 - x1) * (y2 - y1))) * weight11;
            float p2 = (((pos.X - x1) * (y2 - pos.Y)) / ((x2 - x1) * (y2 - y1))) * weight21;
            float p3 = (((x2 - pos.X) * (pos.Y - y1)) / ((x2 - x1) * (y2 - y1))) * weight12;
            float p4 = (((pos.X - x1) * (pos.Y - y1)) / ((x2 - x1) * (y2 - y1))) * weight22;

            return p1 + p2 + p3 + p4;

        }

        public static float Lerp(float t, float v0, float v1)
        {
            return (1 - t) * v0 + t * v1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="x1"></param>
        /// <param name="x2"></param>
        /// <param name="y1"></param>
        /// <param name="y2"></param>
        /// <param name="weight11"></param>3
        /// <param name="weight21"></param>
        /// <param name="weight12"></param>
        /// <param name="weight22"></param>
        /// <returns></returns>
        /// //TENHO QUE FAZER A INTERPOLAÇAO PARA CADA PESO EM SEPARADO, PRIMEIRO X, Y E Z. NAO APENAS COMO VECTOR 3
        public static Vector3 BiLerp(Vector3 pos, float x1, float x2, float y1, float y2, Vector3 weight11, Vector3 weight21, Vector3 weight12, Vector3 weight22)
        {
            Vector3 p1 = (((x2 - pos.X) * (y2 - pos.Y)) / ((x2 - x1) * (y2 - y1))) * weight11;
            Vector3 p2 = (((pos.X - x1) * (y2 - pos.Y)) / ((x2 - x1) * (y2 - y1))) * weight21;
            Vector3 p3 = (((x2 - pos.X) * (pos.Y - y1)) / ((x2 - x1) * (y2 - y1))) * weight12;
            Vector3 p4 = (((pos.X - x1) * (pos.Y - y1)) / ((x2 - x1) * (y2 - y1))) * weight22;

            return p1 + p2 + p3 + p4;
        }


        /// <summary>
        /// Calculates a normal vector.
        /// </summary>
        /// <param name="neighbourVectors">Array of Vector3 sorted clockwise around the point we want to know the normal of.</param>
        /// <returns>Normal Vector</returns>
        public static Vector3 CalculateNormal(Vector3[] neighbourVectors)
        {
            int index, size;
            size = neighbourVectors.Length;
            Vector3[] crossedVectors = new Vector3[size];

            for (index = 0; index < size - 1; index++)
            {
                crossedVectors[index] = Vector3.Cross(neighbourVectors[index], neighbourVectors[index + 1]);
            }
            crossedVectors[index] = Vector3.Cross(neighbourVectors[index], neighbourVectors[0]);

            return -Average(crossedVectors);
        }


        /// <summary>
        /// Returns the average Vector3 of an Vector3 array.
        /// </summary>
        /// <param name="values">Vector3 array.</param>
        /// <returns>Average Vector3</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when parameter is less than one. </exception>
        public static Vector3 Average(Vector3[] values)
        {
            if (values.Length < 1) throw new System.ArgumentOutOfRangeException();

            Vector3 sum = Vector3.Zero;
            for (int i = 0; i < values.Length; i++)
            {
                sum += values[i];
            }

            return sum / values.Length;
        }

        /// <summary>
        /// Returns an integer value, commonsly used as index value.
        /// </summary>
        /// <param name="column">Column number:</param>
        /// <param name="row">Row number:</param>
        /// <param name="numOfRows">Total number of rows.</param>
        /// <example >Given and uni-dimensional array, calculates the index as if it was a bidimensional array.</example>
        /// <returns>Integer value</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when numOfRows is less than one or other args less than 0.</exception>
        public static int CalculateIndex(int column, int row, int numOfRows)
        {
            if (numOfRows < 1 || column < 0 || row < 0 || row > numOfRows) throw new System.ArgumentOutOfRangeException();
            return column * numOfRows + row;
        }

        /// <summary>
        /// Returns the values of yaw, pitch and roll out of a matrix.
        /// </summary>
        /// <param name="matrix">Matrix </param>
        /// <returns>x = yaw, y = pitch, z = roll</returns>
        /// <remarks>http://community.monogame.net/t/solved-reverse-createfromyawpitchroll-or-how-to-get-the-vector-that-would-produce-the-matrix-given-only-the-matrix/9054/4</remarks>
        public static Vector3 ExtractYawPitchRoll(Matrix matrix)
        {
            return new Vector3((float)Math.Atan2(matrix.M13, matrix.M33),
                               (float)Math.Asin(-matrix.M23),
                               (float)Math.Atan2(matrix.M21, matrix.M22));
        }

        /// <summary>
        /// From an angle calculates the unit position on a circle.
        /// </summary>
        /// <param name="angle">Angle in radians.</param>
        /// <returns></returns>
        public static Vector2 GetPositionFromAngle(double angle)
        {
            float x = (float)Math.Cos(angle);
            float z = (float)Math.Sin(angle);

            return new Vector2(x, z);
        }

        /// <summary>
        /// Function that calculates the angle in radians given the numbers of divisions of a circle and the division we want to calculate.
        /// </summary>
        /// <param name="numOfDivisions"></param>
        /// <param name="iteration"></param>
        /// <returns></returns>
        public static double GetAngleFromCircle(int numOfDivisions, float iteration)
        {
            return (2 * Math.PI / numOfDivisions) * iteration;
        }

        /// <summary>
        /// Calculate the yaw from a direction unit vector.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static double CalculateYaw(Vector2 direction)
        {
            var res = Math.Atan2(direction.X, direction.Y);
            return res;
        }

        /// <summary>
        /// Calculate the yaw from a direction unit vector.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static double CalculateYaw(Vector3 direction)
        {
            var res = Math.Atan2(direction.X, direction.Z);
            return res;
        }

        #region RELATIVE POSITION

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="targetPosition"></param>
        /// <returns></returns>
        public static RelativePosition CalculateDistance(Matrix transform, Vector3 targetPosition)
        {
            float distLeft = Vector3.Distance(transform.Translation + transform.Left, targetPosition);
            float distRight = Vector3.Distance(transform.Translation + transform.Right, targetPosition);
            float distForw = Vector3.Distance(transform.Translation + transform.Forward, targetPosition);
            float distBack = Vector3.Distance(transform.Translation + transform.Backward, targetPosition);

            return new RelativePosition(distForw, distBack, distLeft, distRight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rp"></param>
        /// <returns></returns>
        public static RELATIVEPOSITION GetRelativePosition(RelativePosition rp)
        {
            if (rp.forwardDist <= rp.backwardDist)            //se o target esta a frente do objecto
            {
                if (rp.leftDist <= rp.rightDist)       //se esta à frente e à esquerda
                {
                    return RELATIVEPOSITION.FORWARDLEFT;
                }
                else                            //se esta a direita
                {
                    return RELATIVEPOSITION.FORWARDRIGHT;
                }
            }
            else                                // se o target esta atras do objecto
            {
                if (rp.leftDist <= rp.rightDist)       //se esta atras e à esquerda
                {
                    return RELATIVEPOSITION.BACKWARDLEFT;
                }
                else                            //se esta atras e à direita
                {
                    return RELATIVEPOSITION.BACKWARDRIGHT;
                }
            }
        }

        #endregion

    }
}


