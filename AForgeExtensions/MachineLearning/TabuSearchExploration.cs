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
        }
        private int _actions;
        private int[] _tabuActions;
        private AForge.MachineLearning.IExplorationPolicy _basePolicy;
        public AForge.MachineLearning.IExplorationPolicy BasePolicy { get { return _basePolicy; } set { _basePolicy = value; } }
        public int ChooseAction(double[] actionEstimates)
        {
            return 0;
        }
        public void ResetTabuList()
        {

        }
        public void SetTabuAction(int action, int tabuTime)
        {

        }
    }
}
