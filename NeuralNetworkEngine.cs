using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    public static class NeuralNetworkEngine
    {
        public static void FillRandomly(NeuralNetworkUnit neuralNetworkUnit)
        {
            for(int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int n = 0; n < neuralNetworkUnit.Weights[i].Columns; n++)
                {
                    for (int k = 0; k < neuralNetworkUnit.Weights[i].Rows; k++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = Rand.GetFloat(-1, 1);
                    }
                    neuralNetworkUnit.Biases[i][n] = Rand.GetFloat(-1, 1);
                }
            }
        }
        public static void FillZero(NeuralNetworkUnit neuralNetworkUnit)
        {
            for(int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int n = 0; n < neuralNetworkUnit.Weights[i].Columns; n++)
                {
                    for (int k = 0; k < neuralNetworkUnit.Weights[i].Rows; k++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = 0;
                    }
                    neuralNetworkUnit.Biases[i][n] = 0;
                }
            }
        }
        public static double[] SoftMaxVector(float[] inputVector)
        {
            double[] outputVector = new double[inputVector.Length];
            double vectorSum = 0;
            for(int i = 0; i < inputVector.Length; i++)
            {
                vectorSum += Math.Pow(Math.E, inputVector[i]);
            }
            for(int i = 0; i < inputVector.Length; i++)
            {
                outputVector[i] = Math.Pow(Math.E, inputVector[i]) / vectorSum;
            }
            return outputVector;
        }
        public static double[] SoftMaxVector(double[] inputVector)
        {
            double[] outputVector = new double[inputVector.Length];
            double vectorSum = 0;
            for(int i = 0; i < inputVector.Length; i++)
            {
                vectorSum += Math.Pow(Math.E, inputVector[i]);
            }
            for(int i = 0; i < inputVector.Length; i++)
            {
                outputVector[i] = Math.Pow(Math.E, inputVector[i]) / vectorSum;
            }
            return outputVector;
        }
    }
}
