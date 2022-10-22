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
        public Vector<float>[] Biases;
        public int[] Layers;
        private NeuralNetworkUnit(int[] layers)
        {
            Layers = layers;
            Weights = new Matrix<float>[layers.Length - 1];
            Biases = new Vector<float>[layers.Length - 1];
            for(int i = 0; i < layers.Length - 1; i++)
            {
                Weights[i] = Matrix<float>.Build.Dense(layers[i + 1], layers[i]);
                Biases[i] = Vector<float>.Build.Dense(layers[i + 1], 0);
            }
        }
        public static NeuralNetworkUnit CreateNeuralNetworkUnitRandomly(int[] layers)
        {
            NeuralNetworkUnit neuralNetworkUnit = new NeuralNetworkUnit(layers);
            for (int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int k = 0; k < neuralNetworkUnit.Weights[i].RowCount; k++)
                {
                    for (int n = 0; n < neuralNetworkUnit.Weights[i].ColumnCount; n++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = Features.GetRandFloat(-1, 1);
                    }
                    neuralNetworkUnit.Biases[i][k] = Features.GetRandFloat(-1, 1);
                }
            }
            return neuralNetworkUnit;
        }
        public static NeuralNetworkUnit CreateNeuralNetworkUnitZero(int[] layers)
        {
            NeuralNetworkUnit neuralNetworkUnit = new NeuralNetworkUnit(layers);
            for (int i = 0; i < neuralNetworkUnit.Weights.Length; i++)
            {
                for (int k = 0; k < neuralNetworkUnit.Weights[i].RowCount; k++)
                {
                    for (int n = 0; n < neuralNetworkUnit.Weights[i].ColumnCount; n++)
                    {
                        neuralNetworkUnit.Weights[i][k, n] = 0;
                    }
                    neuralNetworkUnit.Biases[i][k] = 0;
                }
            }
            return neuralNetworkUnit;
        }
        public Vector<float> ForwardPropagation(Vector<float> inputs)
        {
            Vector<float> outputs = Weights[0] * inputs;
            outputs = outputs + Biases[0];
            outputs = Features.ReLuVector(outputs);
            inputs = outputs;
            for (int i = 1; i < Weights.Length; i++)
            {
                outputs = Weights[i] * inputs;
                outputs = outputs + Biases[i];
                if(i < Weights.Length - 1)
                {
                    outputs = Features.ReLuVector(outputs);
                }
                inputs = outputs;
            }
            return outputs;
        }
    }
}
