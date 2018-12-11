/*Custom BoundingSphere class */

using IP3D_TPF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BoundingSpheresTest
{
    class BoundingSphereCls
    {
        Vector3 center;
        float radius;
        int numOfDivisions;
        BasicEffect effect;
        

        public  Vector3  Center      { get => center; set => center = value; }
        public  float    Radius      { get => radius; set => radius = value; }

        public BoundingSphereCls(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
            this.numOfDivisions = 32;

        }

        /// <summary>
        /// Checks if Arg <see cref="BoundingSphereCls"/> intersects <see langword="this"/> object.
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool Intersects(BoundingSphereCls sphere)
        {
            float sqDis = Vector3.DistanceSquared(sphere.center, center);

            if (sqDis > (sphere.Radius + radius) * (sphere.Radius + Radius))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks if the <see cref="BoundingSphereCls"/> is completely contained on <see langword="this"/> object.
        /// </summary>
        /// <param name="sphere"></param>
        /// <returns></returns>
        public bool Contains(BoundingSphereCls sphere)
        {
            float sqDis = Vector3.DistanceSquared(sphere.center, center);

            if (sqDis <= (Radius - sphere.Radius) * (Radius - sphere.Radius))
                return true;
            else
                return false;
        }

        public bool Equals(BoundingSphereCls sphere)
        {
            if (center == sphere.center && radius == sphere.radius)
                return true;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is BoundingSphereCls)
                return this.Equals(obj);
            
                return false;
        }


        /// <summary>
        /// Only used for DebugDraw purposes.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="view"></param>
        /// <param name="projection"></param>
        public void Draw(GraphicsDevice graphics, Matrix view, Matrix projection)
        {
            VertexPositionColor[] vertices = new VertexPositionColor[numOfDivisions];
            VertexPositionColor[] vertices2 = new VertexPositionColor[numOfDivisions];
            VertexPositionColor[] vertices3 = new VertexPositionColor[numOfDivisions];


            effect = new BasicEffect(graphics)
            {
                View = view,
                Projection = projection,
                TextureEnabled = false,
                VertexColorEnabled = true
            };

            //16 é o numero de iteraçoes
            for (int i = 0; i < numOfDivisions-1; i++)
            {
                Vector2 pos = MathHelpersCls.GetPositionFromAngle(MathHelpersCls.GetAngleFromCircle(numOfDivisions, i));
                pos.X *= radius;
                pos.Y *= radius;

                vertices[i] = new VertexPositionColor(new Vector3(center.X, pos.X + center.Y, pos.Y + center.Z), Color.Red);

                vertices2[i] = new VertexPositionColor(new Vector3(pos.X + center.X, center.Y, pos.Y + center.Z), Color.Green);

                vertices3[i] = new VertexPositionColor(new Vector3(pos.X + center.X, pos.Y + center.Y, center.Z), Color.Blue);

            }
            vertices[numOfDivisions-1] = vertices[0]; //repetimos o primeiro vertice para fechar o circulo
            vertices2[numOfDivisions-1] = vertices2[0];
            vertices3[numOfDivisions-1] = vertices3[0];

            //vertices[16 * 2 + 1] = vertices[16];
            //vertices[16 * 3 + 2] = vertices[16 * 2 + 2];



            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices, 0, numOfDivisions-1);
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices2, 0, numOfDivisions-1);
                graphics.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, vertices3, 0, numOfDivisions-1);
            }

        }
    }
}
