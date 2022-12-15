using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeMachineLearningExtensions
{
    class QLearningMapProcessing
    {
        public QLearningMapProcessing(QLearningMap[,] map, AForge.MachineLearning.QLearning qLearning)
        {
            _map = map;
            _qLearning = qLearning;
        }
        private AForge.MachineLearning.QLearning _qLearning;
        private QLearningMap[,] _map;
    }
}
