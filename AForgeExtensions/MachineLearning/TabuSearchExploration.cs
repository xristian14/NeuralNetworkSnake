using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public class TabuSearchExploration : AForge.MachineLearning.IExplorationPolicy
    {
        public TabuSearchExploration(int actions, AForge.MachineLearning.IExplorationPolicy basePolicy)
        {
            _actions = actions;
            _basePolicy = basePolicy;
            _tabuActions = new int[actions];
            _tabuActionsDecrease = new int[actions];
        }
        private int _actions;
        private int[] _tabuActions;
        private int[] _tabuActionsDecrease;
        private AForge.MachineLearning.IExplorationPolicy _basePolicy;
        public AForge.MachineLearning.IExplorationPolicy BasePolicy { get { return _basePolicy; } set { _basePolicy = value; } }
        private void DecreaseTabuActionsPrepare()
        {
            for (int i = 0; i < _actions; i++)
            {
                if(_tabuActions[i] > 0)
                {
                    _tabuActionsDecrease[i]++;
                }
            }
        }
        private void DecreaseTabuActions()
        {
            for (int i = 0; i < _actions; i++)
            {
                if(_tabuActionsDecrease[i] > 0)
                {
                    _tabuActions[i]--;
                    _tabuActionsDecrease[i]--;
                }
            }
        }
        public int ChooseAction(double[] actionEstimates)
        {
            DecreaseTabuActions(); //вызываем отложенное уменьшение длительности _tabuActions
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
            DecreaseTabuActionsPrepare();
            return action;
        }
        public void ResetTabuList()
        {
            for(int i = 0; i < _actions; i++)
            {
                _tabuActions[i] = 0;
                _tabuActionsDecrease[i] = 0;
            }
        }
        public void SetTabuAction(int action, int tabuTime)
        {
            _tabuActions[action] = tabuTime;
        }
    }
}
