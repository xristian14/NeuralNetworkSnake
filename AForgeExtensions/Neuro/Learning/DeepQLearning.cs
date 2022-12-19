using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class DeepQLearning
    {
        /// <summary>
        /// Инициализирует экземпляр DeepQLearning.
        /// </summary>
        public DeepQLearning(AForge.Neuro.ActivationNetwork network)
        {
            _network = network;
            _targetNetwork = ActivationNetworkFeatures.CloneActivationNetwork(_network);
            _backPropagationLearning = new AForge.Neuro.Learning.BackPropagationLearning(_network);
            _backPropagationLearning.LearningRate = 0.3;
            _learningRate = 0.5;
            _discountFactor = 0.9;
            _targetNetworkUpdateTime = 100;
            _targetNetworkUpdateTimeElapsed = 0;
        }
        private AForge.Neuro.ActivationNetwork _network; //основная нейронная сеть
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private AForge.Neuro.ActivationNetwork _targetNetwork; //целевая нейронная сеть, из неё будут браться значения будующих наград
        private AForge.Neuro.Learning.BackPropagationLearning _backPropagationLearning;
        private int _targetNetworkUpdateTime;
        private int _targetNetworkUpdateTimeElapsed;
        private int _targetNetworkUpdateTotalNumber = 0;
        /// <summary>
        /// Количество итераций UpdateState(), через которое основная нейронная сеть копируется в целевую. (по умолчанию 100)
        /// </summary>
        public int TargetNetworkUpdateTime { get { return _targetNetworkUpdateTime; } set { _targetNetworkUpdateTime = value; } }
        private double _learningRate;
        /// <summary>
        /// Скорость обучения (по умолчанию 0.5).
        /// </summary>
        public double LearningRate { get { return _learningRate; } set { _learningRate = value; } }
        private double _discountFactor;
        /// <summary>
        /// Фактор дисконтирования (по умолчанию 0.99), в соответствии с которым размер будующей награды будет уменьшен.
        /// </summary>
        public double DiscountFactor { get { return _discountFactor; } set { _discountFactor = value; } }
        public void UpdateState(double[] previousStateInput, double[] previousStateOutput, int action, double reward, double[] nextStateInput)
        {
            if (previousStateOutput.Max() > 40)
            {
                int y = 0;
            }
            double[] nextStateOutput = _targetNetwork.Compute(nextStateInput);
            double previousQvalue = previousStateOutput[action];
            double updatedQvalue = previousQvalue + _learningRate * (reward + _discountFactor * nextStateOutput.Max() - previousQvalue);
            double[] updatedPreviousStateOutput = new double[previousStateOutput.Length];
            previousStateOutput.CopyTo(updatedPreviousStateOutput, 0);
            updatedPreviousStateOutput[action] = updatedQvalue;
            _backPropagationLearning.Run(previousStateInput, updatedPreviousStateOutput);

            _targetNetworkUpdateTimeElapsed++;
            //если совершили достаточно итераций обновления состояния, обновляем целевую нейронную сеть
            if (_targetNetworkUpdateTimeElapsed >= _targetNetworkUpdateTime)
            {
                _targetNetworkUpdateTimeElapsed = 0;
                _targetNetwork = ActivationNetworkFeatures.CloneActivationNetwork(_network);
                _targetNetworkUpdateTotalNumber++;
            }
            if (_targetNetworkUpdateTotalNumber > 400)
            {
                int y = 0;
            }
        }
    }
}
