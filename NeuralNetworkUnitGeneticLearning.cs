using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class NeuralNetworkUnitGeneticLearning
    {
        public NeuralNetworkUnitGeneticLearning(NeuralNetworkUnit neuralNetworkUnit)
        {
            NeuralNetworkUnit = neuralNetworkUnit;
            Rating = 0;
        }
        public NeuralNetworkUnit NeuralNetworkUnit;
        public double Rating;
        public int ApplesCount;
    }
}
