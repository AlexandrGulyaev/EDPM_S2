using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sem2Lab1
{
    class Analysis
    {
        public static double getMin(double[,] x)
        {
            double min = x[0, 0];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    if (x[i, j] < min)
                    {
                        min = x[i, j];
                    }
                }
            }
            return min;
        }

        public static double getMax(double[,] x)
        {
            double max = x[0, 0];
            for (int i = 0; i < x.GetLength(0); i++)
            {
                for (int j = 0; j < x.GetLength(1); j++)
                {
                    if (x[i, j] > max)
                    {
                        max = x[i, j];
                    }
                }
            }
            return max;
        }

        /// <summary>
        /// Найти индекс максимального значения
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static int getMaxIndex(double[] x)
        {
            int index = 0;
            double max = x[0];

            for (int i = 1; i < x.Count(); i++)
            {
                if (x[i] > max)
                {
                    max = x[i];
                    index = i;
                }
            }

            return index;
        }

        /// <summary>
        /// Среднее значение
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double mean(double[] x)
        {
            double summ = 0;
            for (int i = 0; i < x.Count(); i++)
            {
                summ += x[i];
            }
            return summ / x.Count();
        }


        /// <summary>
        /// Автокорелляционная функция
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double AKF(double[] x, int L)
        {
            double xmean = mean(x);
            double sum1 = 0;
            double sum2 = 0;
            for (int k = 0; k < x.Count() - L - 1; k++)
            {
                sum1 += (x[k] - xmean) * (x[k + L] - xmean);
            }
            for (int k = 0; k < x.Count() - 1; k++)
            {
                sum2 += Math.Pow(x[k] - xmean, 2);
            }
            return sum1 / sum2;
        }
    }
}
