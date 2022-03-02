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
    }
}
