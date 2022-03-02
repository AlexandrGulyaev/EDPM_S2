using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class Model
    {
        //public static Image shiftImage(Image Image, float coeff)
        //{
        //    int width = Image.Width;
        //    int height = Image.Height;

        //    ImageAttributes imageAttributes = new ImageAttributes();

        //    float[][] colorMatrixElements = {
        //       new float[] { coeff,  0,  0,  0, 0},
        //       new float[] {0, coeff,  0,  0, 0},
        //       new float[] {0,  0, coeff,  0, 0},
        //       new float[] {0,  0,  0, coeff, 0},
        //       new float[] {0f, 0f, 0f, 0, coeff}};

        //    var colorMatrix = new ColorMatrix(colorMatrixElements);
        //    imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

        //    var result = new Bitmap(Image.Width, Image.Height);
        //    using (var gr = Graphics.FromImage(result))
        //        gr.DrawImage(Image,
        //                   new Rectangle(0, 0, Image.Width, Image.Height),
        //                   0, 0,
        //                   Image.Width,
        //                   Image.Height,
        //                   GraphicsUnit.Pixel,
        //                   imageAttributes);

        //    return result;
        //}
        static double[,] MatrixR, MatrixG, MatrixB;

        
        public static void getRGBMatrix(Bitmap img)
        {
            int height = img.Height;
            int width = img.Width;

            MatrixR = new double[height, width];
            MatrixG = new double[height, width];
            MatrixB = new double[height, width];

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    Color c = img.GetPixel(y, x);
                    MatrixR[x, y] = c.R;
                    MatrixG[x, y] = c.G;
                    MatrixB[x, y] = c.B;
                }
            }
        }

        public static double shiftColor(double color, int shift)
        {
            color += shift;
            color = color > 255 ? 255 : color;
            color = color < 0 ? 0 : color;
            return color;
        }

        public static Image matrixToRGB(Bitmap img)
        {
            int height = MatrixR.GetLength(0);
            int width = MatrixG.GetLength(1);

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    img.SetPixel(y, x, Color.FromArgb((int)MatrixR[x, y], (int)MatrixG[x, y], (int)MatrixB[x, y]));
                }
            }
            return img;
        }

        public static Image shiftImage(Image img, int shift)
        {
            int height = img.Height;
            int width = img.Width;
            getRGBMatrix((Bitmap)img);
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    MatrixR[x, y] = shiftColor(MatrixR[x, y], shift);
                    MatrixG[x, y] = shiftColor(MatrixG[x, y], shift);
                    MatrixB[x, y] = shiftColor(MatrixB[x, y], shift);
                }
            }
            return matrixToRGB((Bitmap)img);
        }

        public static double intensifyColor(double color, double coeff)
        {
            color = (int)(color * coeff);
            color = color > 255 ? 255 : color;
            color = color < 0 ? 0 : color;
            return color;
        }

        public static Image intensifyImage(Image img, double coeff)
        {
            int height = img.Height;
            int width = img.Width;
            getRGBMatrix((Bitmap)img);
            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    MatrixR[x, y] = intensifyColor(MatrixR[x, y], coeff);
                    MatrixG[x, y] = intensifyColor(MatrixG[x, y], coeff);
                    MatrixB[x, y] = intensifyColor(MatrixB[x, y], coeff);
                }
            }
            return matrixToRGB((Bitmap)img);
        }

        public static double scalingColor(double color, double min, double max)
        {
            color = (int)((color - min) / (max - min) * 255);
            color = color > 255 ? 255 : color;
            color = color < 0 ? 0 : color;
            return color;
        }

        public static Image scalingImage(Image img)
        {
            int height = img.Height;
            int width = img.Width;
            getRGBMatrix((Bitmap)img);

            double minR, minG, minB, maxR, maxG, maxB;
            minR = Analysis.getMin(MatrixR);
            maxR = Analysis.getMax(MatrixR);
            minG = Analysis.getMin(MatrixG);
            maxG = Analysis.getMax(MatrixG);
            minB = Analysis.getMin(MatrixB);
            maxB = Analysis.getMax(MatrixB);

            for (int x = 0; x < height; x++)
            {
                for (int y = 0; y < width; y++)
                {
                    MatrixR[x, y] = scalingColor(MatrixR[x, y], minR, maxR);
                    MatrixG[x, y] = scalingColor(MatrixG[x, y], minG, maxG);
                    MatrixB[x, y] = scalingColor(MatrixB[x, y], minB, maxB);
                }
            }
            return matrixToRGB((Bitmap)img);
        }

        /// <summary>
        /// Изменить размер изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="coeff">Коэффициент изменения</param>
        /// <param name="nearestNeighbor">Режим изменения размера изображения:
        ///                    true - метод ближайшего соседа;
        ///                    false - метод билинейной интерполяции.</param>
        /// <returns></returns>
        public static ushort[,] ResizeImage(byte[,] image, double coeff, bool nearestNeighbor = true)
        {
            return nearestNeighbor ? NearestNeighbor(image, coeff) : BilinearInterpolation(image, coeff);
        }


        /// <summary>
        /// Метод ближайшего соседа
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="coeff">Коэффициент изменения размера</param>
        /// <returns></returns>
        private static ushort[,] NearestNeighbor(byte[,] image, double coeff)
        {
            int newWidth = (int)((double)image.GetLength(0) * coeff);
            int newHeight = (int)((double)image.GetLength(1) * coeff);

            ushort[,] temp = new ushort[newWidth, newHeight];

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    temp[x, y] = image[(int)(x / coeff), (int)(y / coeff)];
                }
            }

            return temp;
        }

        /// <summary>
        /// Метод 4-смежной билинейной интерполяции
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="coeff">Коэффициент изменения размера</param>
        /// <returns></returns>
        private static ushort[,] BilinearInterpolation(byte[,] image, double coeff)
        {
            ushort[,] result = new ushort[(int)(image.GetLength(0) * coeff), (int)(image.GetLength(1) * coeff)];
            for (int x = 0; x < result.GetLength(0); x++)
            {
                for (int y = 0; y < result.GetLength(1); y++)
                {
                    result[x, y] = FourAdjacent((int)(x / coeff), (int)(y / coeff), image);
                }
            }
            return result;
        }

        /// <summary>
        /// Подсчёт значения 4-смежной билинейной интерполяции
        /// Q11(x1,y1)     Q12(x2,y1)
        ///           X(x,y)
        /// Q21(x1,y2)     Q22(x2,y2)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="image"></param>
        /// <returns></returns>
        private static ushort FourAdjacent(int x, int y, byte[,] image)
        {
            int x1 = x - 1;
            int x2 = x + 1;
            int y1 = y - 1;
            int y2 = y + 1;
            byte Q11, Q12, Q21, Q22;
            // Q11
            try
            {
                Q11 = image[x1, y1];
            }
            catch
            {
                Q11 = 0;
            }
            // Q12
            try
            {
                Q12 = image[x2, y1];
            }
            catch
            {
                Q12 = 0;
            }
            // Q21
            try
            {
                Q21 = image[x1, y2];
            }
            catch
            {
                Q21 = 0;
            }
            // Q22
            try
            {
                Q22 = image[x2, y2];
            }
            catch
            {
                Q22 = 0;
            }

            double s1, s2, s3, s4;

            s1 = Math.Abs(((double)Q11 / (double)((x2 - x1) * (y2 - y1)) * (double)((x2 - x) * (y - y1))));
            s2 = Math.Abs(((double)Q12 / (double)((x2 - x1) * (y2 - y1)) * (double)((x - x1) * (y - y1))));
            s3 = Math.Abs(((double)Q21 / (double)((x2 - x1) * (y2 - y1)) * (double)((x2 - x) * (y2 - y))));
            s4 = Math.Abs(((double)Q22 / (double)((x2 - x1) * (y2 - y1)) * (double)((x - x1) * (y2 - y))));

            return (ushort)(s1 + s2 + s3 + s4);
        }

        /// <summary>
        /// Градационные преобразования изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="mode">Тип градационного преобразования:
        ///     0 - линейное преобразование (негатив)
        ///     1 - логарифмическое преобразование
        ///     2 - гамма-преобразование (степенное)</param>
        /// <param name="c">Коэффициент при логарифмическом или степенном преобразовании или предельное значение (целое) для линейного преобразования</param>
        /// <param name="degree">Степень логарифмического или степенного преобразования</param>
        /// <returns></returns>
        public static ushort[,] GradationTransformation(ushort[,] image, byte mode = 0, double c = 1, double degree = 1)
        {
            switch (mode)
            {
                case 0:
                    int L = c <= 1 ? 256 : (int)c;
                    return LinearTransformation(image, L);
                case 1:
                    return LogarithmicTransformation(image, c, degree);
                default:
                    return GammaTransformation(image, c, degree);
            }
        }

        /// <summary>
        /// Градационное линейное преобразование
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns></returns>
        private static ushort[,] LinearTransformation(ushort[,] image, int L)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = (ushort)(L - image[x, y] - 1);
                }
            }
            return image;
        }

        /// <summary>
        /// Градационное логарифмическое преобразование
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="c">Коэффициент при логарифмическом преобразовании</param>
        /// <param name="degree">Степень логарифмического преобразования</param>
        /// <returns></returns>
        private static ushort[,] LogarithmicTransformation(ushort[,] image, double c, double degree)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = (ushort)(c * Math.Log(1 + image[x, y], degree));
                }
            }
            return image;
        }

        /// <summary>
        /// Градационное гамма-преобразование
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="c">Коэффициент при гамма-преобразовании</param>
        /// <param name="degree">Степень гамма-преобразования</param>
        /// <returns></returns>
        private static ushort[,] GammaTransformation(ushort[,] image, double c, double degree)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = (ushort)(c * Math.Pow(image[x, y], degree));
                }
            }
            return image;
        }
    }
}
