using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class NeuralNetworkUnitGeneticLearning
    {
        public NeuralNetworkUnitGeneticLearning(NeuralNetworkUnit neuralNetworkUnit, int id)
        {
            NeuralNetworkUnit = neuralNetworkUnit;
            Id = id;
            TotalRating = 0;
        }
        private int _id;
        public int Id
        {
            get { return _id; }
            private set { _id = value; }
        }
        public NeuralNetworkUnit NeuralNetworkUnit;
        public double TotalRating; //общий счет за все тесты
    }
}
