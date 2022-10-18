using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    public class NeuralNetworkUnit
    {
        public Matrix[] Weights;
        public float[][] Biases;
        public NeuralNetworkUnit(int[] layers)
        {
            Weights = new Matrix[layers.Length - 1];
            Biases = new float[layers.Length - 1][];
            for(int i = 0; i < layers.Length - 1; i++)
            {
                Weights[i] = new Matrix(layers[i], layers[i + 1]);
                Biases[i] = new float[layers[i + 1]];
            }
        }
    }
}
