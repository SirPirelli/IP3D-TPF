using IP3D_TPF;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace WanderBehaviour.Geometry
{
    class Circle
    {

        #region FIELDS
        Vector3 center;
        float radius;
        int numOfDivisions;
        float offsetY;
        BasicEffect effect;

        VertexBuffer vertexBuffer;
        VertexPositionColor[] vertices;
        VertexPositionColor[] positions;

        #endregion

        #region PROPERTIES
        public Vector3 Center { get => center; set => center = value; }
        public float Radius { get => radius; set => radius = value; }

        public Matrix WorldMatrix { get; set; }
        public Matrix ViewMatrix { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        #endregion

        #region CONSTRUCTORS
        public Circle(Vector3 center, float radius, GraphicsDevice graphics)
        {
            this.center = center;
            this.radius = radius;
            this.numOfDivisions = 32;
            this.offsetY = 1.5f;
            this.vertices = new VertexPositionColor[numOfDivisions];

            this.positions = new VertexPositionColor[numOfDivisions];

            this.vertexBuffer = new VertexBuffer(graphics, typeof(VertexPositionColor), numOfDivisions, BufferUsage.None);
            this.WorldMatrix = Matrix.Identity;

            CreateGeometry();
            SetEffect(graphics);
        }
        #endregion

        /// <summary>
        /// Only used for DebugDraw purposes.
        /// Sets the <see cref="VertexPositionColor"/> ready to be sent to <see cref="VertexBuffer"/>
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {

            for(int i = 0; i < numOfDivisions; i++)
            {
                positions[i] = vertices[i];
                positions[i].Position += center + Vector3.Up * 1.5f;
            }

            vertexBuffer.SetData<VertexPositionColor>(positions);
        }

        /// <summary>
        /// Only used for DebugDraw purposes.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        public void Draw(GraphicsDevice graphics, Matrix viewMatrix, Matrix projectionMatrix)
        {
            graphics.SetVertexBuffer(vertexBuffer);

            effect.World = WorldMatrix;
            effect.View = viewMatrix;
            effect.Projection = projectionMatrix;

            foreach(EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawPrimitives(PrimitiveType.LineStrip, 0, numOfDivisions - 1);
            }
        }

        void CreateGeometry()
        {

            for(int i = 0; i < numOfDivisions-1; i++)
            {
                Vector2 pos = MathHelpersCls.GetPositionFromAngle(MathHelpersCls.GetAngleFromCircle(numOfDivisions, i));
                pos.X *= radius;
                pos.Y *= radius;

                vertices[i] = new VertexPositionColor(new Vector3(pos.X, 0, pos.Y), Color.Red);
            }
            vertices[numOfDivisions - 1] = vertices[0];
        }

        void SetEffect(GraphicsDevice graphics)
        {
            effect = new BasicEffect(graphics)
            {
                View = ViewMatrix,
                Projection = ProjectionMatrix,
                VertexColorEnabled = true,
                TextureEnabled = false,
                LightingEnabled = false,
                World = Matrix.Identity
            };
        }

        
    }
}
