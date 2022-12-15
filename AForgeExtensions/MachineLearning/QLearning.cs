using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public class QLearning
    {
        public QLearning(int states, int actions, AForge.MachineLearning.IExplorationPolicy explorationPolicy)
        {
            Random random = new Random();
            StatesCount = states;
            _actions = actions;
            ExplorationPolicy = explorationPolicy;
            LearningRate = 0.5;
            DiscountFactor = 0.9;
            _qvalues = new double[states][];
            for (int i = 0; i < states; i++)
            {
                _qvalues[i] = new double[actions];
                for (int k = 0; k < actions; k++)
                {
                    //_qvalues[i][k] = 0;
                    _qvalues[i][k] = random.NextDouble();
                }
            }
        }
        private int _actions;
        private double[][] _qvalues;
        public int ActionsCount { get { return _actions; } }
        public double[][] Qvalues { get { return _qvalues; } }
        public int StatesCount { get; }
        public AForge.MachineLearning.IExplorationPolicy ExplorationPolicy { get; set; }
        public double LearningRate { get; set; }
        public double DiscountFactor { get; set; }
        public int GetAction(int state)
        {
            return ExplorationPolicy.ChooseAction(_qvalues[state]);
        }
        public void UpdateState(int previousState, int action, double reward, int nextState)
        {
            _qvalues[previousState][action] += LearningRate * (reward + DiscountFactor * _qvalues[nextState].Max() - _qvalues[previousState][action]);
        }
    }
}
