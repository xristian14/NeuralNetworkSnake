using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public class QLearning
    {
        /// <summary>
        /// Инициализирует экземпляр QLearning, заполняя массив qvalues нулями
        /// </summary>
        public QLearning(int states, int actions, AForgeExtensions.MachineLearning.IExplorationPolicy explorationPolicy)
        {
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
                    _qvalues[i][k] = 0;
                }
            }
        }
        /// <summary>
        /// Инициализирует экземпляр QLearning, заполняя массив qvalues случайными значениями, от min до max
        /// </summary>
        public QLearning(int states, int actions, AForgeExtensions.MachineLearning.IExplorationPolicy explorationPolicy, double min, double max)
        {
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
                    _qvalues[i][k] = AForgeExtensions.Features.GetRandDouble(min, max);
                }
            }
        }
        private int _actions;
        private double[][] _qvalues;
        public int ActionsCount { get { return _actions; } }
        public double[][] Qvalues { get { return _qvalues; } }
        public int StatesCount { get; }
        public AForgeExtensions.MachineLearning.IExplorationPolicy ExplorationPolicy { get; set; }
        public double LearningRate { get; set; }
        public double DiscountFactor { get; set; }
        public int GetAction(int state)
        {
            return ExplorationPolicy.ChooseAction(_qvalues[state]);
        }
        public void UpdateState(int previousState, int action, double reward, int nextState)
        {
            bool isTabuSearchExploration = true;
            AForgeExtensions.MachineLearning.TabuSearchExploration tabuSearchExploration = new TabuSearchExploration(1, ExplorationPolicy);
            try
            {
                tabuSearchExploration = (AForgeExtensions.MachineLearning.TabuSearchExploration)ExplorationPolicy;
            }
            catch
            {
                isTabuSearchExploration = false;
            }
            double bestActionQvalue = _qvalues[nextState].Max();
            if (isTabuSearchExploration)
            {
                double previousEpsilon = tabuSearchExploration.BasePolicy.SetEpsilon(0);
                int bestAction = ExplorationPolicy.ChooseAction(_qvalues[nextState]);
                bestActionQvalue = _qvalues[nextState][bestAction];
                tabuSearchExploration.BasePolicy.SetEpsilon(previousEpsilon);
            }
            _qvalues[previousState][action] += LearningRate * (reward + DiscountFactor * bestActionQvalue - _qvalues[previousState][action]);
        }
    }
}
