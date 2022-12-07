using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class LeaderboardItem
    {
        public LeaderboardItem(double score, int eatenApples, int age, NeuralNetworkUnit neuralNetworkUnit)
        {
            Score = score;
            EatenApples = eatenApples;
            Age = age;
            NeuralNetworkUnit = neuralNetworkUnit;
        }
        private double _score;
        public double Score
        {
            get { return _score; }
            private set
            {
                _score = value;
            }
        }
        private int _eatenApples;
        public int EatenApples
        {
            get { return _eatenApples; }
            private set
            {
                _eatenApples = value;
            }
        }
        private int _age;
        public int Age
        {
            get { return _age; }
            private set
            {
                _age = value;
            }
        }
        private NeuralNetworkUnit _neuralNetworkUnit;
        public NeuralNetworkUnit NeuralNetworkUnit
        {
            get { return _neuralNetworkUnit; }
            private set
{
                _neuralNetworkUnit = value;
            }
        }
    }
}
