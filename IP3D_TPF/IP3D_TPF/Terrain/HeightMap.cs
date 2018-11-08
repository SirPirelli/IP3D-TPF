using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IP3D_TPF
{
    class HeightMap
    {        /* classe de height map, apenas guarda um array com os valores de altura e o tamanho da textura */

        private Vector2 size;
        private int[] values;

        public Vector2 Size { get { return size; } }
        public int[] Values { get { return values; } }

        public HeightMap(Vector2 a_size, int[] a_values)
        {
            size = a_size;
            values = a_values;
        }

        /* metodo publico estatico que retira os dados de uma textura e constroi um height map */
        public static HeightMap TextureToHeightMap(Texture2D texture)
        {

            Color[] colors = new Color[texture.Height * texture.Width];
            texture.GetData<Color>(colors);

            Vector2 size = new Vector2(texture.Width, texture.Height);

            int[] r = new int[(int)size.X * (int)size.Y];

            for (int i = 0; i < r.Length; i++)
            {
                r[i] = colors[i].R;
            }

            return new HeightMap(size, r);
        }

        /* checks if the index is valid */
        public bool IndexExist(int index)
        {
            if (index < 0 || index >= values.Length) return false;

            return true;
        }

        /* se o index estiver fora dos limites da matriz, lança uma excepçao */
        public int GetValueFromHeightMap(int index)
        {
            try
            {
                return values[index];

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("index out of range: " + index);
                return -9999;
            }
        }

        /* para descobrir o index do vertice atraves da posição no espaço */
        public int CalculateIndexFromPosition(Vector3 position, float planeLength)
        {
            int indexX = (int)((position.X - (position.X % planeLength)) / planeLength);
            int indexY = (int)((position.Z - (position.Z % planeLength)) / planeLength);

            //System.Diagnostics.Debug.WriteLine(indexX + " , " + indexY);

            return indexX * (int)size.Y + indexY;
        }

    }
}
