using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public class EpsilonGreedyExploration : AForgeExtensions.MachineLearning.IExplorationPolicy
    {
        public EpsilonGreedyExploration(double epsilon)
        {
            _epsilon = epsilon;
        }
        private Random _random = new Random();
        private double _epsilon;
        public double Epsilon { get { return _epsilon; } set { _epsilon = value; } }
        public double SetEpsilon(double epsilon)
        {
            double previousEpsilon = Epsilon;
            Epsilon = epsilon;
            return previousEpsilon;
        }
        public int ChooseAction(double[] actionEstimates)
        {
            int bestAction = AForgeExtensions.Features.MaxIndex(actionEstimates);
            if(actionEstimates.Length > 1)
            {
                if (_random.NextDouble() < _epsilon)
                {
                    int randAction = _random.Next(0, actionEstimates.Length - 1);
                    if(bestAction <= randAction)
                    {
                        randAction++;
                    }
                    return randAction;
                }
            }
            return bestAction;
        }
    }
}
