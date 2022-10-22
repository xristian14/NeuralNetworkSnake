using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetworkSnake
{
    public class NeuralNetworkUnit
    {
        public Matrix<float>[] Weights;
        public float[][] Biases;
        public int[] Layers;
        private NeuralNetworkUnit(int[] layers)
        {
            Layers = layers;
            Weights = new Matrix<float>[layers.Length - 1];
            Biases = new float[layers.Length - 1][];
            for(int i = 0; i < layers.Length - 1; i++)
            {
                Weights[i] = Matrix<float>.Build.Dense(layers[i], layers[i + 1]);
                Biases[i] = new float[layers[i + 1]];
            }
        }
        public static NeuralNetworkUnit CreateNeuralNetworkUnitRandomly(int[] layers)
        {
            NeuralNetworkUnit neuralNetworkUnit = new NeuralNetworkUnit(layers);
            for (int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int n = 0; n < neuralNetworkUnit.Weights[i].ColumnCount; n++)
                {
                    for (int k = 0; k < neuralNetworkUnit.Weights[i].RowCount; k++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = Features.GetRandFloat(-1, 1);
                    }
                    neuralNetworkUnit.Biases[i][n] = Features.GetRandFloat(-1, 1);
                }
            }
            return neuralNetworkUnit;
        }
        public static NeuralNetworkUnit CreateNeuralNetworkUnitZero(int[] layers)
        {
            NeuralNetworkUnit neuralNetworkUnit = new NeuralNetworkUnit(layers);
            for (int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int n = 0; n < neuralNetworkUnit.Weights[i].ColumnCount; n++)
                {
                    for (int k = 0; k < neuralNetworkUnit.Weights[i].RowCount; k++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = 0;
                    }
                    neuralNetworkUnit.Biases[i][n] = 0;
                }
            }
            return neuralNetworkUnit;
        }
        public Vector<float> ForwardPropagation(Vector<float> inputs)
        {
            Vector<float> outputs = inputs * Weights[0];
            inputs = outputs;
            for (int i = 2; i < Layers.Length; i++)
            {
                outputs = inputs * Weights[i - 1];
                inputs = outputs;
            }
            return outputs;
        }
    }
}
