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
        /// Привести изображение к шкале серости
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ushort[,] ConvertToGrayscale(int[,] image)
        {
            ushort[,] imageGS = new ushort[image.GetLength(0), image.GetLength(1)];
            int min = image[0, 0];
            int max = image[0, 0];

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    min = image[x, y] > min ? min : image[x, y];
                    max = image[x, y] < max ? max : image[x, y];
                }
            }

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    imageGS[x, y] = (ushort)((double)(image[x, y] - min) / (double)(max - min) * 255.0);
                }
            }

            return imageGS;
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
        /// Градационное логарифмическое преобразование
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="c">Коэффициент при логарифмическом преобразовании</param>
        /// <param name="degree">Степень логарифмического преобразования</param>
        /// <returns></returns>
        public static ushort[,] LogarithmicTransformation(short[,] image, double c, double degree)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = (short)(c * Math.Log(1 + image[x, y], degree));
                }
            }

            short min = image[0, 0];
            short max = image[0, 0];
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    min = image[x, y] < min ? image[x, y] : min;
                    max = image[x, y] > max ? image[x, y] : max;
                }
            }

            ushort[,] result = new ushort[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    result[x, y] = (ushort)((double)(image[x,y] - min)/(double)(max - min) * 255.0);
                }
            }

            return result;
        }

        /// <summary>
        /// Градационное гамма-преобразование
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="c">Коэффициент при гамма-преобразовании</param>
        /// <param name="degree">Степень гамма-преобразования</param>
        /// <returns>Гамма-преобразование</returns>
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

        /// <summary>
        /// Получить гистограму изображения
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <returns>Гистограма изображения</returns>
        public static Dictionary<ushort, int> GetHistogram(ushort[,] image)
        {
            Dictionary<ushort, int> histogram = new Dictionary<ushort, int>();
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (histogram.ContainsKey(image[x, y]))
                    {
                        histogram[image[x, y]]++;
                    }
                    else
                    {
                        histogram.Add(image[x, y], 1);
                    }
                }
            }
            int max = histogram.Keys.Max();

            for (ushort i = 0; i <= max; i++)
            {
                if (!histogram.ContainsKey(i)) histogram.Add(i, 0);
            }

            return histogram;
        }

        /// <summary>
        /// Получить функцию распределения
        /// </summary>
        /// <param name="histogram">Гистограма</param>
        /// <param name="r"></param>
        /// <returns></returns>
        public static int[] GetCDF(Dictionary<ushort, int> histogram)
        {
            var dict = histogram.OrderBy(x => x.Key);
            int[] CDF = new int[histogram.Keys.Max() + 1];
            int index = 0;
            foreach (var temp in dict)
            {
                CDF[index] = index == 0 ? temp.Value : CDF[index - 1] + temp.Value;
                index++;
            }
            return CDF;
        }

        /// <summary>
        /// Получить передаточную шкалу
        /// </summary>
        /// <param name="CDF"></param>
        /// <returns></returns>
        public static int[] GetTransferScale(int[] CDF)
        {
            int[] TransferScale = new int[CDF.Count()];
            int max = CDF.Max();
            double coeff = (double)(CDF.Count() - 1) / (double)max;
            for (int i = 0; i < CDF.Count(); i++)
            {
                TransferScale[i] = (int)((double)CDF[i] * coeff);
            }
            return TransferScale;
        }

        public static ushort[,] ReplacePixelsFromCDF(ushort[,] image, int[] CDF)
        {
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    image[x, y] = (ushort)CDF[image[x, y]];
                }
            }
            return image;
        }

        /// <summary>
        /// Получить обратную функцию
        /// </summary>
        /// <param name="function">Исходная функция</param>
        /// <returns>Обратная функция</returns>
        public static int[] GetInverseFunction(int[] function)
        {
            int[] reverse_function = new int[function.Count()];
            for (int i = 0; i < function.Count(); i++)
            {
                reverse_function[function[i]] = i;
                //reverse_function[i] = (int)((double)i / (double)function[i]);
            }
            return reverse_function;
        }
        
        /// <summary>
        /// Действительная часть спектра Фурье
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Re(double[] x, int n)
        {
            int N = x.Count();
            double sum = 0;
            for (int k = 0; k < N; k++)
            {
                sum += x[k] * Math.Cos(2 * Math.PI * n * k / N);
            }
            return sum / (double)N;
        }

        /// <summary>
        /// Мнимая часть спектра Фурье
        /// </summary>
        /// <param name="x"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static double Im(double[] x, int n)
        {
            int N = x.Count();
            double sum = 0;
            for (int k = 0; k < N; k++)
            {
                sum += x[k] * Math.Sin(2 * Math.PI * n * k / N);
            }
            return sum / (double)N;
        }

        /// <summary>
        /// Амплитудный спектр Фурье
        /// </summary>
        /// <param name="N"></param>
        /// <returns></returns>
        public static double[] GetAmplitudeSpectrum(double[] x, bool normal = false)
        {
            int N = x.Count();
            int coeff = normal ? 1 : N;
            double[] ASpectr = new double[N];
            for (int n = 0; n < N; n++)
            {
                ASpectr[n] = Math.Sqrt(Math.Pow(Re(x, n), 2) + Math.Pow(Im(x, n), 2));
            }
            return ASpectr;
        }

        /// <summary>
        /// Получить производную функции
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double[] GetDerivative(double[] x)
        {
            double[] derivative = new double[x.Count() - 1];
            for (int i = 0; i < derivative.Count(); i++)
            {
                derivative[i] = x[i + 1] - x[i];
            }
            return derivative;
        }

        /// <summary>
        /// Низкочастотный фильтр
        /// </summary>
        public static double[] LPF(int m, double fc, double dt)
        {
            double[] d = new double[4] { 0.35577019, 0.2436983, 0.07211497, 0.00630165 };
            double fact = 2 * fc * dt;
            double arg = Math.PI * fact;
            double[] right_part = new double[m + 1];
            right_part[0] = fact;
            for (int i = 1; i <= m; i++)
            {
                right_part[i] = Math.Sin(arg * i) / (Math.PI * i);
            }
            right_part[m] /= 2;
            double sumg = right_part[0];
            for (int i = 1; i <= m; i++)
            {
                double sum = d[0];
                arg = Math.PI * i / m;
                for (int k = 1; k <= 3; k++)
                {
                    sum += 2 * d[k] * Math.Cos(arg * k);
                }
                right_part[i] *= sum;
                sumg += 2 * right_part[i];
            }
            for (int i = 0; i <= m; i++)
            {
                right_part[i] /= sumg;
            }

            List<Double> result = new List<double>();
            result.AddRange(right_part.Reverse().ToList());
            result.RemoveAt(result.Count() - 1);
            result.AddRange(right_part.ToList());

            return result.ToArray();
        }

        /// <summary>
        /// Высокочастотный фильтр
        /// </summary>
        /// <param name="LPF">Низкочастотный фильтр</param>
        /// <returns>Высокочастотный фильтр</returns>
        public static double[] HPF(double[] LPF)
        {
            double[] HPF = new double[LPF.Count()];
            int m = (LPF.Count() - 1) / 2;
            int k = 0;
            foreach (double value in LPF)
            {
                HPF[k] = k == m ? 1 - value : -value;
                k++;
            }
            return HPF;
        }
        

        /// <summary>
        /// Полосно-заградительный фильтр (режекторный фильтр), fc1 < fc2
        /// </summary>
        /// <param name="LPF1">Низкочастотный фильтр с fc1</param>
        /// <param name="LPF2">Низкочастотный фильтр с fc2</param>
        /// <returns>ПЗФ</returns>
        public static double[] BSF(double[] LPF1, double[] LPF2)
        {
            double[] BSF = new double[LPF1.Count()];
            int m = (LPF1.Count() - 1) / 2;
            for (int i = 0; i < LPF1.Count(); i++)
            {
                BSF[i] = i == m ? 1 + LPF1[i] - LPF2[i] : LPF1[i] - LPF2[i];
            }
            return BSF;
        }

        /// <summary>
        /// Свёртка (линейное преобразование)
        /// </summary>
        /// <returns></returns>
        public static double[] Convolution(double[] x, double[] h)
        {
            int size = x.Count() + h.Count();
            double[] y = new double[size];
            for (int k = 0; k < size; k++)
            {
                for (int j = 0; j < h.Count(); j++)
                {
                    if (k >= j && k - j < x.Count())
                        y[k] += x[k - j] * h[j];
                }
            }
            return y;
        }

        /// <summary>
        /// Сгенерировать шум
        /// </summary>
        /// <param name="N">Размер шума</param>
        /// <param name="type">Тип шума:
        ///                    0 - cоль-перец;
        ///                    1 - cлучайный шум;
        ///                    2 - комбинированный шум</param>
        /// <param name="scale">Точность случайного шума (количество знаков после запятой)</param>
        /// <param name="frequency">Частота шума соль-перец (от 0 до 1)</param>
        /// <param name="coeff">Коэффициент случайного шума</param>
        /// <returns>Сгенерированный шум</returns>
        public static double[] getNoise(int N, byte type, int scale = 3, double frequency = 0.4, double coeff = 2)
        {
            double[] noise = new double[N];
            double limit = Math.Pow(10, scale);
            Random rnd = new Random(); ;
            switch (type)
            {
                case 0:
                    for (int i = 0; i < N; i++)
                    {
                        noise[i] = 1;
                    }
                    for (int i = 0; i < frequency * N; i++)
                    {
                        int index = rnd.Next(0, N);
                        noise[index] = rnd.Next(0, 2) == 0 ? -20000 : -10000;
                    }
                    break;
                case 1:
                    for (int i = 0; i < N; i++)
                    {
                        noise[i] = coeff * (double)rnd.Next(-(int)limit, (int)limit + 1) / limit;
                    }
                    break;
                case 2:
                    for (int i = 0; i < N; i++)
                    {
                        noise[i] = coeff * (double)rnd.Next(-(int)limit, (int)limit + 1) / limit;
                    }
                    for (int i = 0; i < frequency * N; i++)
                    {
                        int index = rnd.Next(0, N);
                        noise[index] = rnd.Next(0, 2) == 0 ? -20000 : -10000;
                    }
                    break;
            }
            return noise;
        }
        
        /// <summary>
        /// Обратное преобразование Фурье
        /// </summary>
        /// <param name="fourier"></param>
        /// <returns></returns>
        public static double[] getInverseFourier(double[] fourier)
        {
            double[] inverseFourier = new double[fourier.Count()];

            for (int i = 0; i < fourier.Count(); i++)
            {
                inverseFourier[i] = (Re(fourier, i) + Im(fourier, i));
            }

            return inverseFourier;
        }

        /// <summary>
        /// Получить разность между изображениями
        /// </summary>
        /// <param name="image1">Ресайз</param>
        /// <param name="image2">Оригинал</param>
        /// <returns></returns>
        public static short[,] getDiffImage(ushort[,] image1, ushort[,] image2)
        {
            short[,] Diff = new short[image1.GetLength(0), image1.GetLength(1)];
            double coeff = (double)image2.GetLength(0) / (double)image1.GetLength(0);

            for (int x = 0; x < image1.GetLength(0); x++)
            {
                for (int y = 0; y < image1.GetLength(1); y++)
                {
                    Diff[x, y] = (short)(image1[x, y] - image2[(int)((double)x * coeff), (int)((double)y * coeff)]);
                }
            }

            return Diff;
        }

        /// <summary>
        /// Применить фильтр
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="filter">Фильтр</param>
        /// <param name="mode">Режим фильтрации: 0 - построчно; 1 - постолбцово; 2 - 2D-фильтрация</param>
        /// <returns></returns>
        public static ushort[,] applyFilter(ushort[,] image, double[] filter, byte mode = 0)
        {
            int width = image.GetLength(1);
            int height = image.GetLength(0);

            if (mode == 0 || mode == 2)
            {
                for (int i = 0; i < width; i++)
                {
                    double[] str = new double[height];
                    for (int j = 0; j < height; j++)
                    {
                        str[j] = image[j, i];
                    }

                    str = Convolution(str, filter);
                    int startIndex = (str.Count() - height) / 2;
                    for (int j = startIndex; j < height + startIndex; j++)
                    {
                        image[j - startIndex, i] = (ushort)Math.Abs(str[j]);
                    }
                }
            }
            if (mode == 1 || mode == 2)
            {
                for (int i = 0; i < height; i++)
                {
                    double[] str = new double[width];
                    for (int j = 0; j < width; j++)
                    {
                        str[j] = image[i, j];
                    }

                    str = Convolution(str, filter);
                    int startIndex = (str.Count() - width) / 2;
                    for (int j = startIndex; j < width + startIndex; j++)
                    {
                        image[i, j - startIndex] = (ushort)Math.Abs(str[j]);
                    }
                }
            }
            return image;
        }

        /// <summary>
        /// Применить градиент
        /// </summary>
        /// <param name="image"></param>
        /// <param name="maskX"></param>
        /// <param name="maskY"></param>
        /// <param name="direction">Направление:
        ///       0 - вдоль строк;
        ///       1 - вдоль столбцов;
        ///       2 - двумерное</param>
        /// <returns></returns>
        public static ushort[,] applyGradient(ushort[,] image, int[,] maskX, int[,] maskY, byte direction = 0)
        {
            int[,] Gx = new int[image.GetLength(0), image.GetLength(1)];
            int[,] Gy = new int[image.GetLength(0), image.GetLength(1)];
            int[,] result = new int[image.GetLength(0), image.GetLength(1)];

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (x == 0 || y == 0 || x == image.GetLength(0) - 1 || y == image.GetLength(1) - 1)
                    {
                        result[x, y] = image[x, y];
                    }
                    else
                    {
                        int[,] part = new int[3, 3]
                        {
                            { image[x - 1, y - 1], image[x, y - 1], image[x + 1, y - 1] },
                            {     image[x - 1, y],     image[x, y],     image[x + 1, y] },
                            { image[x - 1, y + 1], image[x, y + 1], image[x + 1, y + 1] }
                        };

                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (direction == 0 || direction == 2)
                                {
                                    Gx[x, y] += part[i, j] * maskX[i, j];
                                }
                                if (direction == 1 || direction == 2)
                                {
                                    Gy[x, y] += part[i, j] * maskY[i, j];
                                }
                            }
                        }
                        result[x, y] = (int)(Math.Sqrt(Math.Pow(Gx[x, y], 2) + Math.Pow(Gy[x, y], 2)));
                    }
                }
            }
            return ConvertToGrayscale(result);
        }

        /// <summary>
        /// Применить морфологическую операцию (дилатацию или эрозию)
        /// </summary>
        /// <param name="image">Изображение</param>
        /// <param name="mask">Маска</param>
        /// <param name="type">true - дилатация
        ///                    false - эрозия</param>
        /// <returns></returns>
        public static ushort[,] ApplyMorphologicalOperation(ushort[,] image, int[,] mask, bool type = true)
        {
            int[,] result = new int[image.GetLength(0), image.GetLength(1)];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    mask[i, j] = mask[i, j] == 1 ? 255 : 0;
                }
            }

            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    if (x == 0 || y == 0 || x == image.GetLength(0) - 1 || y == image.GetLength(1) - 1)
                    {
                        result[x, y] = image[x, y];
                    }
                    else
                    {
                        int[,] part = new int[3, 3]
                        {
                            { image[x - 1, y - 1], image[x, y - 1], image[x + 1, y - 1] },
                            {     image[x - 1, y],     image[x, y],     image[x + 1, y] },
                            { image[x - 1, y + 1], image[x, y + 1], image[x + 1, y + 1] }
                        };

                        int cnt = 0;
                        bool flag = false;
                        for (int i = 0; i < 3; i++)
                        {
                            for (int j = 0; j < 3; j++)
                            {
                                if (mask[i, j] != part[i, j] && type || mask[i, j] == part[i, j] && !type)
                                {
                                    cnt++;
                                    flag = true;
                                    break;
                                }
                                if (flag) break;
                            }
                        }
                        if (type)
                        {
                            result[x, y] = cnt == 0 ? 0 : image[x, y];
                        }
                        else
                        {
                            result[x, y] = cnt == 0 ? 255 : image[x, y];
                        }
                    }
                }
            }

            return ConvertToGrayscale(result);
        }

        /// <summary>
        /// Деление комплексных чисел
        /// </summary>
        /// <param name="Re1"></param>
        /// <param name="Im1"></param>
        /// <param name="Re2"></param>
        /// <param name="Im2"></param>
        /// <param name="Re"></param>
        /// <param name="Im"></param>
        public static void DivisionComplexNumbers(double Re1, double Im1, double Re2, double Im2, out double Re, out double Im)
        {
            Re = (Re1 * Re2 + Im1 * Im2) / (Math.Pow(Re2, 2) + Math.Pow(Im2, 2));
            Im = (Im1 * Re2 - Im2 * Re1) / (Math.Pow(Re2, 2) + Math.Pow(Im2, 2));
        }
        
        /// <summary>
        /// Модуль комплексного числа
        /// </summary>
        /// <param name="Re"></param>
        /// <param name="Im"></param>
        /// <returns></returns>
        public static double AbsComplexNumber(double Re, double Im)
        {
            return Math.Sqrt(Re * Re + Im * Im);
        }

        /// <summary>
        /// Умножение комплексных чисел
        /// </summary>
        /// <param name="Re1"></param>
        /// <param name="Im1"></param>
        /// <param name="Re2"></param>
        /// <param name="Im2"></param>
        /// <param name="Re"></param>
        /// <param name="Im"></param>
        public static void MultiplicationComplexNumbers(double Re1, double Im1, double Re2, double Im2, out double Re, out double Im)
        {
            Re = Re1 * Re2 - Im1 * Im2;
            Im = Im1 * Re2 + Im2 * Re1;
        }

        /// <summary>
        /// Комплексное преобразование Фурье
        /// </summary>
        /// <param name="image"></param>
        /// <param name="Fourier"></param>
        /// <param name="ArrRe"></param>
        /// <param name="ArrIm"></param>
        /// <param name="Is2D"></param>
        /// <param name="mode">Режим работы:
        ///     true - сначала построчный обход, потом постолбцовый;
        ///     false - сначала постолбцовый обход, потом построчный.</param>
        public static void getFourier(double[,] image, out double[,] Fourier, out double[,] ArrRe, out double[,] ArrIm, bool Is2D = true, bool mode = true)
        {
            int width = image.GetLength(0);
            int height = image.GetLength(1);
            double[,] FourierTmp = new double[width, height];
            Fourier = new double[width, height];
            ArrRe = new double[width, height];
            ArrIm = new double[width, height];

            if (mode)
            {
                // Обход по строкам
                for (int y = 0; y < height; y++)
                {
                    // Выделяем строку
                    double[] str = new double[width];
                    for (int x = 0; x < width; x++)
                    {
                        str[x] = image[x, y];
                    }
                    for (int x = 0; x < width; x++)
                    {
                        ArrRe[x, y] = Re(str, x);
                        ArrIm[x, y] = Im(str, x);
                        Fourier[x, y] = ArrRe[x, y] + ArrIm[x, y];
                        FourierTmp[x, y] = ArrRe[x, y] + ArrIm[x, y];
                    }
                }

                if (Is2D)
                {
                    // Обход по столбцам
                    for (int x = 0; x < width; x++)
                    {
                        // Выделяем колонку
                        double[] str = new double[height];
                        for (int y = 0; y < height; y++)
                        {
                            str[y] = FourierTmp[x, y];
                        }
                        for (int y = 0; y < height; y++)
                        {
                            ArrRe[x, y] = Re(str, y);
                            ArrIm[x, y] = Im(str, y);
                            Fourier[x, y] = ArrRe[x, y] + ArrIm[x, y];
                        }
                    }
                }
            }
            else
            {
                // Обход по столбцам
                for (int x = 0; x < width; x++)
                {
                    // Выделяем колонку
                    double[] str = new double[height];
                    for (int y = 0; y < height; y++)
                    {
                        str[y] = image[x, y];
                    }
                    for (int y = 0; y < height; y++)
                    {
                        ArrRe[x, y] = Re(str, y);
                        ArrIm[x, y] = Im(str, y);
                        Fourier[x, y] = ArrRe[x, y] + ArrIm[x, y];
                        FourierTmp[x, y] = ArrRe[x, y] + ArrIm[x, y];
                    }
                }

                if (Is2D)
                {
                    // Обход по строкам
                    for (int y = 0; y < height; y++)
                    {
                        // Выделяем строку
                        double[] str = new double[width];
                        for (int x = 0; x < width; x++)
                        {
                            str[x] = FourierTmp[x, y];
                        }
                        for (int x = 0; x < width; x++)
                        {
                            ArrRe[x, y] = Re(str, x);
                            ArrIm[x, y] = Im(str, x);
                            Fourier[x, y] = ArrRe[x, y] + ArrIm[x, y];
                        }
                    }
                }
            }
        }

        public static void getFourier(ushort[,] image, out double[,] Fourier, out double[,] ArrRe, out double[,] ArrIm, bool Is2D = true, bool mode = true)
        {
            double[,] new_image = new double[image.GetLength(0), image.GetLength(1)];
            for (int x = 0; x < image.GetLength(0); x++)
            {
                for (int y = 0; y < image.GetLength(1); y++)
                {
                    new_image[x, y] = (double)image[x, y];
                }
            }
            getFourier(new_image, out Fourier, out ArrRe, out ArrIm, Is2D, mode);
        }


        /// <summary>
        /// Подавить искажения
        /// </summary>
        /// <param name="ArrRe"></param>
        /// <param name="ArrIm"></param>
        /// <param name="ArrReDist"></param>
        /// <param name="ArrImDist"></param>
        /// <param name="isNoise"></param>
        /// <returns></returns>
        public static double[,] SuppressDistorsion(double[,] ArrRe, double[,] ArrIm, double[,] ArrReDist, double[,] ArrImDist, bool isNoise = false, double alpha = 0.1)
        {
            double[,] Fourier = new double[ArrRe.GetLength(0), ArrRe.GetLength(1)];

            if (!isNoise)
            {
                for (int x = 0; x < ArrRe.GetLength(0); x++)
                {
                    for (int y = 0; y < ArrRe.GetLength(1); y++)
                    {
                        double newRe, newIm;
                        DivisionComplexNumbers(ArrRe[x, y], ArrIm[x, y], ArrReDist[x, 0], ArrImDist[x, 0], out newRe, out newIm);
                        Fourier[x, y] = newRe + newIm;
                    }
                }
            }
            else
            {
                double[,] ArrReAbs = new double[ArrReDist.GetLength(0), ArrReDist.GetLength(1)]
                        , ArrImAbs = new double[ArrReDist.GetLength(0), ArrReDist.GetLength(1)]
                        , ArrImConjugate = new double[ArrReDist.GetLength(0), ArrReDist.GetLength(1)]
                        , SRe = new double[ArrReDist.GetLength(0), ArrReDist.GetLength(1)]
                        , SIm = new double[ArrReDist.GetLength(0), ArrReDist.GetLength(1)];
                for (int x = 0; x < ArrRe.GetLength(0); x++)
                {
                    ArrReAbs[x, 0] = Math.Pow(AbsComplexNumber(ArrReDist[x, 0], ArrImDist[x, 0]), 2);
                    ArrReAbs[x, 0] += alpha * alpha;
                    ArrImConjugate[x, 0] = -ArrImDist[x, 0];
                    DivisionComplexNumbers(ArrReDist[x, 0], ArrImConjugate[x, 0], ArrReAbs[x, 0], ArrImAbs[x, 0], out SRe[x, 0], out SIm[x, 0]);
                }

                for (int x = 0; x < ArrRe.GetLength(0); x++)
                {
                    for (int y = 0; y < ArrRe.GetLength(1); y++)
                    {
                        double newRe, newIm;
                        MultiplicationComplexNumbers(ArrRe[x, y], ArrIm[x, y], SRe[x, 0], SIm[x, 0], out newRe, out newIm);
                        Fourier[x, y] = newRe + newIm;
                    }
                }

            }
            return Fourier;
        }
    }
}
