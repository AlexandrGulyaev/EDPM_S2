using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class InOut
    {
        private static double[,] BitmapToDouble(Bitmap input)
        {
            int width = input.Width;
            int height = input.Height;

            double[,] array2d = new double[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color cl = input.GetPixel(x, y);

                    // A 2d double data is always Grayscale.
                    // So, three elements are averaged.
                    double color = cl.R + cl.G + cl.B;

                    array2d[x, y] = color / 255.0;
                }
            }

            return array2d;
        }

        public static double[,] ImageToDouble(Image image)
        {
            return BitmapToDouble((Bitmap)image);
        }

        public static Image DoubleToImage(double[,] data)
        {
            Bitmap bitmap = new Bitmap(data.GetUpperBound(0), data.GetUpperBound(1));
            for (int i = 0; i < data.GetUpperBound(0); i++)
            {
                for (int j = 0; j < data.GetUpperBound(1); j++)
                {
                    bitmap.SetPixel(i, j, Color.FromArgb((int)data[i, j], (int)data[i, j], (int)data[i, j]));
                }
            }
            Image image = bitmap;
            return image;
        }

        public static void SaveImage(Bitmap image, string name, string extension)
        {
            image.Save(name + "." + extension);
        }
    }
}
