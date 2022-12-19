using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions
{
    public static class Features
    {
        /// <summary>
        /// Возвращает индекс наибольшего элемента в последовательности
        /// </summary>
        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T);

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
        private static Random _random = new Random();
        public static double GetRandDouble(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }
        /// <summary>
        /// возвращает массив, сумма значений которого равняется 1
        /// </summary>
        public static double[] SoftMaxArray(double[] input)
        {
            double[] output = new double[input.Length];
            double inputSum = input.Sum();
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] / inputSum;
            }
            return output;
        }
    }
}
