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
        public static int GetRandInt(int min, int max)
        {
            return _random.Next(min, max);
        }
        public static float GetRandFloat(float min, float max)
        {
            return (float)_random.NextDouble() * (max - min) + min;
        }
        public static double GetRandDouble(float min, float max)
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
        public static double[] SoftMaxVector(double[] inputVector)
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
    }
}
