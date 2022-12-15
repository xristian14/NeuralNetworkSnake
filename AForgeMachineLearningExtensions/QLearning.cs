using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeMachineLearningExtensions
{
    class QLearning
    {
        public QLearning(int states, int actions, AForge.MachineLearning.IExplorationPolicy explorationPolicy)
        {
            StatesCount = states;
            _actions = actions;
            ExplorationPolicy = explorationPolicy;
            LearningRate = 0.5;
            DiscountFactor = 0.9;
        }
        private int _actions;
        private double[][] _qvalues;
        public int ActionsCount { get { return _actions; } }
        public int StatesCount { get; }
        public AForge.MachineLearning.IExplorationPolicy ExplorationPolicy { get; set; }
        public double LearningRate { get; set; }
        public double DiscountFactor { get; set; }
        public int GetAction(int state)
        {
            int bestAction = ExplorationPolicy.ChooseAction(_qvalues[state]);
            if()
        }
        public void UpdateState(int previousState, int action, double reward, int nextState)
        {

        }
    }
}
