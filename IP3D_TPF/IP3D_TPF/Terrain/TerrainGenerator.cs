using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    class TerrainGenerator
    {
        VertexBuffer vertexBuffer;
        int vertexCount;
        IndexBuffer indexBuffer;
        int primitiveCount;

        BasicEffect effect;
        HeightMap heightMap;
        float planeLength;
        Vector2 terrainBounds;
        float heightRatio;

        /* para poder saber as normals em runtime, é necessario aceder a qualquer altura ao array de vertices */ 
        VertexPositionNormalTexture[] vertices;

        #region PROPERTIES

        public HeightMap HeightMap { get { return heightMap; } }
        public float PlaneLength { get { return planeLength; } }
        public Vector2 TerrainBounds { get { return terrainBounds; } }
        public float HeightRatio { get { return heightRatio; } }

        #endregion     

        #region CONSTRUCTOR
        public TerrainGenerator(GraphicsDevice graphics, float a_planeLength, float a_heightRatio, Texture2D texHeightMap, Texture2D texture)
        {
            effect = new BasicEffect(graphics);

            SetEffect(graphics, texture, Matrix.Identity);
            SetLighting();

            #region RASTERIZERSTATE PARAMETERS (NOT NEEDED)
            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;
            //rasterizerState1.FillMode = FillMode.WireFrame;
            graphics.RasterizerState = rasterizerState1;
            #endregion

            heightMap = HeightMap.TextureToHeightMap(texHeightMap);
            planeLength = a_planeLength;
            heightRatio = a_heightRatio;

            terrainBounds = new Vector2(planeLength * (int)heightMap.Size.X, (planeLength * (int)heightMap.Size.Y));

            primitiveCount = 2 * (int)heightMap.Size.X - 2;
            vertexCount = heightMap.Values.Length;

            CreateTerrain(graphics);
        }
        #endregion

        #region OVERLOADED METHODS
        public void LoadContent(ContentManager content)
        {

        }


        /* transformar a textura num array de cores, para sabermos os valores do height map.
         * depois percorrer o array fazendo triangle strips. p = y * w + x */

        public void Draw(GraphicsDevice graphics, Matrix viewMatrix)
        {
            int i, startIndex;

            effect.View = viewMatrix;
            graphics.SetVertexBuffer(vertexBuffer);
            graphics.Indices = indexBuffer;

            // Indica o efeito para desenhar os eixos
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                /* 128*128 vertices = 16384 */
                for (i = 0; i < heightMap.Size.X; i++)
                {
                    startIndex = i * 2 * (int)heightMap.Size.X;

                    pass.Apply();

                    graphics.DrawIndexedPrimitives(PrimitiveType.TriangleStrip,
                                                    0,
                                                    startIndex,
                                                    primitiveCount);
                }

            }



        }

        #endregion

        private void CreateTerrain(GraphicsDevice device)
        {

            SetVertices(device);
            SetIndices(device);

        }

        private void CalculateNormals(VertexPositionNormalTexture[] vertices)
        {
            int index;
            /* 8 é o numero de vertices que rodeiam um ponto */
            int[] surroundingIndexes = new int[8];
            Vector3[] surroundingVector = new Vector3[8];

            /* este for ira percorrer o interior do terreno, nao calcula as normals nos vertices fronteira */
            for (int i = 1; i < heightMap.Size.X - 1; i++)
            {
                for (int j = 1; j < heightMap.Size.Y - 1; j++)
                {
                    index = MathHelpersCls.CalculateIndex(i, j, (int)heightMap.Size.Y);

                    surroundingIndexes[0] = MathHelpersCls.CalculateIndex(i + 1, j - 1, (int)heightMap.Size.Y);
                    surroundingIndexes[1] = MathHelpersCls.CalculateIndex(i + 1, j, (int)heightMap.Size.Y);
                    surroundingIndexes[2] = MathHelpersCls.CalculateIndex(i + 1, j + 1, (int)heightMap.Size.Y);
                    surroundingIndexes[3] = MathHelpersCls.CalculateIndex(i, j + 1, (int)heightMap.Size.Y);
                    surroundingIndexes[4] = MathHelpersCls.CalculateIndex(i - 1, j + 1, (int)heightMap.Size.Y);
                    surroundingIndexes[5] = MathHelpersCls.CalculateIndex(i - 1, j, (int)heightMap.Size.Y);
                    surroundingIndexes[6] = MathHelpersCls.CalculateIndex(i - 1, j - 1, (int)heightMap.Size.Y);
                    surroundingIndexes[7] = MathHelpersCls.CalculateIndex(i, j - 1, (int)heightMap.Size.Y);

                    for (int k = 0; k < 8; k++)
                    {
                        surroundingVector[k] = vertices[surroundingIndexes[k]].Position - vertices[index].Position/*vertices[index].Position - vertices[surroundingIndexes[k]].Position*/;
                        surroundingVector[k].Normalize();
                    }

                    vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);

                }
            }

            /* h < 4 pq existem 4 cantos, calculamos a normal apenas com 3 vertices */
            surroundingVector = new Vector3[3];
            surroundingIndexes = new int[3];
            for (int h = 0; h < 4; h++)
            {
                switch (h)
                {
                    case 0: /* normal do primeiro vertice */
                        index = 0;

                        surroundingIndexes[0] = MathHelpersCls.CalculateIndex(1, 0, (int)heightMap.Size.Y);
                        surroundingIndexes[1] = MathHelpersCls.CalculateIndex(1, 1, (int)heightMap.Size.Y);
                        surroundingIndexes[2] = MathHelpersCls.CalculateIndex(0, 1, (int)heightMap.Size.Y);
                        break;

                    case 1:     /*  x = max e z = 0 */
                        index = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, 0, (int)heightMap.Size.Y);

                        surroundingIndexes[0] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, 1, (int)heightMap.Size.Y);
                        surroundingIndexes[1] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, 1, (int)heightMap.Size.Y);
                        surroundingIndexes[2] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, 0, (int)heightMap.Size.Y);
                        break;

                    case 2: /* x = max e z = max */
                        index = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);

                        surroundingIndexes[0] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);
                        surroundingIndexes[1] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                        surroundingIndexes[2] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                        break;

                    case 3: /* X = 0 e z = max */
                        index = MathHelpersCls.CalculateIndex(0, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);

                        surroundingIndexes[0] = MathHelpersCls.CalculateIndex(0, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                        surroundingIndexes[1] = MathHelpersCls.CalculateIndex(1, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                        surroundingIndexes[2] = MathHelpersCls.CalculateIndex(1, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);
                        break;

                    default:
                        index = -1;
                        System.Diagnostics.Debug.WriteLine("ERROR! LOOP ITERATION OUT OF BOUNDS. Calculating normals of edges of terrain.");
                        break;

                }

                for (int l = 0; l < 3; l++)
                {
                    surroundingVector[l] = vertices[surroundingIndexes[l]].Position - vertices[index].Position;
                    surroundingVector[l].Normalize();

                }
                vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);

            }
            /*-------------------------------------------------------------------------------------*/
            /* Calcular as normals dos vertices fronteira excepto nos cantos */
            surroundingIndexes = new int[5];
            surroundingVector = new Vector3[5];
            for (int h = 0; h < 4; h++)
            {
                switch (h)
                {
                    case 0: /* strip x = 0 */
                        for (int i = 1; i < (int)heightMap.Size.X - 1; i++)
                        {
                            index = MathHelpersCls.CalculateIndex(i, 0, (int)heightMap.Size.Y);

                            surroundingIndexes[0] = MathHelpersCls.CalculateIndex(i + 1, 0, (int)heightMap.Size.Y);
                            surroundingIndexes[1] = MathHelpersCls.CalculateIndex(i + 1, 1, (int)heightMap.Size.Y);
                            surroundingIndexes[2] = MathHelpersCls.CalculateIndex(i, 1, (int)heightMap.Size.Y);
                            surroundingIndexes[3] = MathHelpersCls.CalculateIndex(i - 1, 1, (int)heightMap.Size.Y);
                            surroundingIndexes[4] = MathHelpersCls.CalculateIndex(i - 1, 0, (int)heightMap.Size.Y);

                            for (int j = 0; j < 5; j++)
                            {
                                surroundingVector[j] = vertices[surroundingIndexes[j]].Position - vertices[index].Position;
                                surroundingVector[j].Normalize();
                            }

                            vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);
                        }
                        break;

                    case 1: /* strip x = max */

                        for (int i = 1; i < (int)heightMap.Size.X - 1; i++)
                        {
                            index = MathHelpersCls.CalculateIndex(i, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);

                            surroundingIndexes[0] = MathHelpersCls.CalculateIndex(i - 1, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);
                            surroundingIndexes[1] = MathHelpersCls.CalculateIndex(i - 1, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                            surroundingIndexes[2] = MathHelpersCls.CalculateIndex(i, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                            surroundingIndexes[3] = MathHelpersCls.CalculateIndex(i + 1, (int)heightMap.Size.Y - 2, (int)heightMap.Size.Y);
                            surroundingIndexes[4] = MathHelpersCls.CalculateIndex(i + 1, (int)heightMap.Size.Y - 1, (int)heightMap.Size.Y);

                            for (int j = 0; j < 5; j++)
                            {
                                surroundingVector[j] = vertices[surroundingIndexes[j]].Position - vertices[index].Position;
                                surroundingVector[j].Normalize();
                            }

                            vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);
                        }
                        break;

                    case 2: /* strip y = 0 */

                        for (int i = 1; i < (int)heightMap.Size.Y - 1; i++)
                        {
                            index = MathHelpersCls.CalculateIndex(0, i, (int)heightMap.Size.Y);

                            surroundingIndexes[0] = MathHelpersCls.CalculateIndex(0, i - 1, (int)heightMap.Size.Y);
                            surroundingIndexes[1] = MathHelpersCls.CalculateIndex(1, i - 1, (int)heightMap.Size.Y);
                            surroundingIndexes[2] = MathHelpersCls.CalculateIndex(1, i, (int)heightMap.Size.Y);
                            surroundingIndexes[3] = MathHelpersCls.CalculateIndex(1, i + 1, (int)heightMap.Size.Y);
                            surroundingIndexes[4] = MathHelpersCls.CalculateIndex(0, i + 1, (int)heightMap.Size.Y);

                            for (int j = 0; j < 5; j++)
                            {
                                surroundingVector[j] = vertices[surroundingIndexes[j]].Position - vertices[index].Position;
                                surroundingVector[j].Normalize();
                            }

                            vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);
                        }
                        break;

                    case 3: /* strip y = max */

                        for (int i = 1; i < (int)heightMap.Size.Y - 1; i++)
                        {
                            index = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, i, (int)heightMap.Size.Y);

                            surroundingIndexes[0] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, i + 1, (int)heightMap.Size.Y);
                            surroundingIndexes[1] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, i + 1, (int)heightMap.Size.Y);
                            surroundingIndexes[2] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, i, (int)heightMap.Size.Y);
                            surroundingIndexes[3] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 2, i - 1, (int)heightMap.Size.Y);
                            surroundingIndexes[4] = MathHelpersCls.CalculateIndex((int)heightMap.Size.X - 1, i - 1, (int)heightMap.Size.Y);

                            for (int j = 0; j < 5; j++)
                            {
                                surroundingVector[j] = vertices[surroundingIndexes[j]].Position - vertices[index].Position;
                                surroundingVector[j].Normalize();
                            }

                            vertices[index].Normal = MathHelpersCls.CalculateNormal(surroundingVector);
                        }
                        break;

                    default:
                        System.Diagnostics.Debug.WriteLine("CALCULATING TERRAIN NORMALS: EDGE VERTICES LOOP, ITERATION OUT OF BOUNDS");
                        break;
                }
            }
        }

        private void SetVertices(GraphicsDevice device)
        {
            int index, texX, texY;

            vertices = new VertexPositionNormalTexture[heightMap.Values.Length];

            /* 
                inicializar os vertices (vertices inicializados em colunas)                                        */

            for (int i = 0; i < heightMap.Size.X; i++)
            {
                for (int j = 0; j < heightMap.Size.Y; j++)
                {
                    /* definir o valor da textura */
                    if (i % 2 == 0) texX = 0;
                    else texX = 1;

                    if (j % 2 == 0) texY = 0;
                    else texY = 1;
                    /* ---------------------------*/

                    index = i * (int)heightMap.Size.Y + j;
                    vertices[index] = new VertexPositionNormalTexture(new Vector3(i * planeLength,
                                                                                  heightMap.GetValueFromHeightMap(index) * heightRatio,
                                                                                  j * planeLength),
                                                                    Vector3.Zero,   /* a normal e inicializada a 0, calculamos depois */
                                                                    new Vector2(texX, texY));
                }
            }

            CalculateNormals(vertices);

            /* o vertex buffer e o index buffer é guardado na gpu, para diminuir o trafego entre o cpu e o gpu */
            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertexCount, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);
        }

        private void SetIndices(GraphicsDevice device)
        {
            int index, indexNext;

            /* setting up index buffer */
            /* i < heightMap.size.X-1 pq nao temos mais vertices alem da ultima coluna(eixo x do heightMap) */

            short[] indexBuff = new short[2 * ((short)heightMap.Size.X - 1) * (short)heightMap.Size.Y];
            int k = 0;
            for (int i = 0; i < heightMap.Size.X - 1; i++)
            {
                for (int j = 0; j < heightMap.Size.Y; j++)
                {

                    index = (i * (int)heightMap.Size.Y) + j;
                    indexNext = ((i + 1) * (int)heightMap.Size.Y) + j;

                    indexBuff[k++] = (short)index;
                    indexBuff[k++] = (short)indexNext;

                }
            }

            indexBuffer = new IndexBuffer(device, typeof(short), indexBuff.Length, BufferUsage.None);
            indexBuffer.SetData<short>(indexBuff);

        }




        #region EFFECTS AND LIGHTING
        private void SetLighting()
        {
            effect.LightingEnabled = true;
            effect.DirectionalLight0.DiffuseColor = new Vector3(0.22f, 0.17f, 0.2f);
            effect.DirectionalLight0.Direction = new Vector3(0.25f, -0.7f, 0.25f);
            //effect.DirectionalLight0.SpecularColor = new Vector3(0.1f, 0.1f, 0.1f);
            effect.DirectionalLight1.Enabled = true;
            effect.DirectionalLight1.DiffuseColor = new Vector3(0.19f, 0.12f, 0.08f);
            effect.DirectionalLight1.Direction = new Vector3(-0.7f, -0.9f, 0.5f);
            //effect.DirectionalLight1.SpecularColor = new Vector3(0, 0, 0);
            effect.AmbientLightColor = new Vector3(0.1f, 0.08f, 0.01f);
            effect.FogEnabled = true;
            effect.FogColor = Color.Gray.ToVector3(); // For best results, ake this color whatever your background is.
            effect.FogStart = 0f;
            effect.FogEnd = 1200f;
        }

        private void SetEffect(GraphicsDevice graphics, Texture2D texture, Matrix worldMatrix)
        {
            // Calcula a aspectRatio, a view matrix e a projeção
            float aspectRatio = (float)graphics.Viewport.Width / graphics.Viewport.Height;

            //effect.View = Matrix.CreateLookAt(new Vector3(0f, 200f, 800f), Vector3.Zero, Vector3.Up);

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45f), aspectRatio, 0.1f, 2000.0f);
            effect.World = worldMatrix;
            effect.VertexColorEnabled = false;
            effect.TextureEnabled = true;
            effect.Texture = texture;
        }
        #endregion

        public Vector3 GetNormalAtPosition(Vector3 position)
        {
            Vector3[] positions = new Vector3[4];
            positions[0] = new Vector3(position.X, position.Y, position.Z);
            positions[1] = new Vector3(position.X + PlaneLength, position.Y, position.Z);
            positions[2] = new Vector3(position.X, position.Y, position.Z + planeLength);
            positions[3] = new Vector3(position.X + planeLength, position.Y, position.Z + planeLength);

            Vector3[] normals = new Vector3[4];
            for (int i = 0; i < 4; i++)
            {
                normals[i] = GetNormalAtVertice(positions[i]);
            }

            //tem que ser interpolaçao bilinear e nao apenas uma media
            return new Vector3(MathHelpersCls.BiLerp(new Vector2(position.X, position.Z), position.X, positions[3].X, position.Z, positions[3].Z, normals[0].X, normals[1].X, normals[2].X, normals[3].X),
                               MathHelpersCls.BiLerp(new Vector2(position.X, position.Z), position.X, positions[3].X, position.Z, positions[3].Z, normals[0].Y, normals[1].Y, normals[2].Y, normals[3].Y),
                               MathHelpersCls.BiLerp(new Vector2(position.X, position.Z), position.X, positions[3].X, position.Z, positions[3].Z, normals[0].Z, normals[1].Z, normals[2].Z, normals[3].Z));

            //return MathHelpersCls.Average(normals);
        }

        public Vector3 GetNormalAtVertice(Vector3 position)
        {
            return vertices[heightMap.GetIndexFromPosition(position, PlaneLength)].Normal;
        }

    }
}
