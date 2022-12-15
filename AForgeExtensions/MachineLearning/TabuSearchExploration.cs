using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public class TabuSearchExploration : AForgeExtensions.MachineLearning.IExplorationPolicy
    {
        public TabuSearchExploration(int actions, AForgeExtensions.MachineLearning.IExplorationPolicy basePolicy)
        {
            _actions = actions;
            _basePolicy = basePolicy;
            _tabuActions = new int[actions];
        }
        private int _actions;
        private int[] _tabuActions;
        public int[] TabuActions { get { return _tabuActions; } }
        private AForgeExtensions.MachineLearning.IExplorationPolicy _basePolicy;
        public AForgeExtensions.MachineLearning.IExplorationPolicy BasePolicy { get { return _basePolicy; } set { _basePolicy = value; } }
        public double SetEpsilon(double epsilon)
        {
            return 0.0;
        }
        private void DecreaseTabuActions()
        {
            for (int i = 0; i < _actions; i++)
            {
                if(_tabuActions[i] > 0)
                {
                    _tabuActions[i]--;
                }
            }
        }
        public int ChooseAction(double[] actionEstimates)
        {
            double[] nonTabuActionEstimates = new double[_tabuActions.Where(a => a == 0).Count()];
            int index = 0;
            for(int i = 0; i < actionEstimates.Length; i++)
            {
                if(_tabuActions[i] == 0)
                {
                    nonTabuActionEstimates[index] = actionEstimates[i];
                    index++;
                }
            }
            int action = _basePolicy.ChooseAction(nonTabuActionEstimates);
            for(int i = 0; i <= action; i++)
            {
                if(_tabuActions[i] > 0)
                {
                    action++;
                }
            }
            DecreaseTabuActions();
            return action;
        }
        public void ResetTabuList()
        {
            for(int i = 0; i < _actions; i++)
            {
                _tabuActions[i] = 0;
            }
        }
        public void SetTabuAction(int action, int tabuTime)
        {
            _tabuActions[action] = tabuTime;
        }
    }
}
