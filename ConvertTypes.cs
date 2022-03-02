using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class ConvertTypes
    {
        public static ushort[,] ByteToUshort2D(byte[] data, int width, int height, int startIndex, bool swapBytes)
        {
            ushort[,] result = new ushort[width, height];
            int x = 0, y = 0;
            for (int i = startIndex; i < data.Length; i++)
            {
                // Перестановка байтов
                if (swapBytes)
                {
                    if (i % 2 != 0)
                    {
                        result[x, y] = (ushort)(data[i] << 8 | data[i - 1]);
                        x++;
                    }
                }
                else
                {
                    result[x, y] = data[i];
                    x++;
                }
                if (x == height)
                {
                    x = 0;
                    y++;
                    if (y == width)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public static ushort[,] Byte2DToUshort2D(byte[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            ushort[,] result = new ushort[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = data[x, y];
                }
            }
            return result;
        }

        public static byte[,] Ushort2DToByte2D(ushort[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            byte[,] result = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = (byte)(data[x, y] > 255 ? 255 : data[x, y]);
                }
            }
            return result;
        }

        public static Bitmap Byte2DToBitmap(byte[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Bitmap result = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result.SetPixel(x, y, Color.FromArgb(data[x, y], data[x, y], data[x, y]));
                }
            }
            return result;
        }

        public static Bitmap Ushort2DToBitmap(ushort[,] data)
        {
            int width = data.GetLength(0);
            int height = data.GetLength(1);
            Bitmap result = new Bitmap(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result.SetPixel(x, y, Color.FromArgb((byte)data[x, y], (byte)data[x, y], (byte)data[x, y]));
                }
            }
            return result;
        }

        public static byte[,] BitmapToByte2D(Bitmap data)
        {
            int width = data.Width;
            int height = data.Height;
            byte[,] result = new byte[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    result[x, y] = data.GetPixel(x, y).R;
                }
            }
            return result;
        }
    }
}
