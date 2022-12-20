using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    public static class Features
    {
        public static Random _random = new Random();
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
        public static int GetRandInt(int min, int max)
        {
            return _random.Next(min, max);
        }
        public static float GetRandFloat(float min, float max)
        {
            return (float)_random.NextDouble() * (max - min) + min;
        }
        public static double GetRandDouble(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }
        public static double[] SoftMaxVector(float[] inputVector)
        {
            double[] outputVector = new double[inputVector.Length];
            double vectorSum = 0;
            for (int i = 0; i < inputVector.Length; i++)
            {
                vectorSum += Math.Pow(Math.E, inputVector[i]);
            }
            for (int i = 0; i < inputVector.Length; i++)
            {
                outputVector[i] = Math.Pow(Math.E, inputVector[i]) / vectorSum;
            }
            return outputVector;
        }
        public static double[] SoftMaxVector(double[] inputVector) //возвращает вектор, сумма значений которого равняется 1
        {
            double[] outputVector = new double[inputVector.Length];
            double inputVectorSum = inputVector.Sum();
            for (int i = 0; i < inputVector.Length; i++)
            {
                outputVector[i] = inputVector[i] / inputVectorSum;
            }
            return outputVector;
        }
        public static double DegreeToRadian(double degree)
        {
            return degree * Math.PI / 180;
        }
        public static Vector<float> ReLuVector(Vector<float> vector)
        {
            for(int i = 0; i < vector.Count; i++)
            {
                vector[i] = ReLuFloat(vector[i]);
            }
            return vector;
        }
        public static float ReLuFloat(float value)
        {
            return Math.Max(0, value);
        }
    }
}
