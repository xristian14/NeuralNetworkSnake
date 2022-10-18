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
                        neuralNetworkUnit.Weights[i].Cells[k, n] = Rand.GetFloat(-1, 1);
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
                        neuralNetworkUnit.Weights[i].Cells[k, n] = 0;
                    }
                    neuralNetworkUnit.Biases[i][n] = 0;
                }
            }
        }
    }
}
