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
            _backPropagationLearning.LearningRate = 0.5;
            _learningRate = 0.5;
            _discountFactor = 0.98;
            _targetNetworkUpdateTime = 0;
            _targetNetworkUpdateTimeElapsed = 0;

            _geneticLearningTeacher = new GeneticLearningTeacher(network, 100, 400, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelMinimizationSelection(), -1, 1);
        }
        private AForge.Neuro.ActivationNetwork _network; //основная нейронная сеть
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private AForge.Neuro.ActivationNetwork _targetNetwork; //целевая нейронная сеть, из неё будут браться значения будующих наград
        private AForge.Neuro.Learning.BackPropagationLearning _backPropagationLearning;
        private GeneticLearningTeacher _geneticLearningTeacher;
        public GeneticLearningTeacher GeneticLearningTeacher { get { return _geneticLearningTeacher; } }
        private int _targetNetworkUpdateTime;
        private int _targetNetworkUpdateTimeElapsed;
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
        public int PoolStateActionUpdateLength { get { return _inputs.Count; } }
        private List<double[]> _inputs = new List<double[]>();
        private List<double[]> _desiredOutputs = new List<double[]>();
        /// <summary>
        /// Добавляет в пул значений для обновления весов и смещений, значения входа и желаемого выхода. При вызове UpdateState() будут обновляться веса и смещения для минимизации ошибки всего пула значений
        /// </summary>
        /// <param name="previousStateInput">Вход предыдущего состояния</param>
        /// <param name="previousStateOutput">Выход предыдущего состояния</param>
        /// <param name="action">Действие</param>
        /// <param name="reward">Награда</param>
        /// <param name="isNextReward">Нужно ли учитывать в qvalue награду за следующее действие</param>
        /// <param name="nextStateInput">Вход следующего состояния (после действия action) (для isNextReward=false значения не имеет)</param>
        public void AddStateActionUpdate(double[] previousStateInput, double[] previousStateOutput, int action, double reward, bool isNextReward, double[] nextStateInput)
        {
            double[] desiredOutput = new double[previousStateOutput.Length];
            previousStateOutput.CopyTo(desiredOutput, 0);
            double previousQvalue = previousStateOutput[action];
            double nextStateReward = 0;
            if (isNextReward)
            {
                double[] nextStateOutput = _targetNetwork.Compute(nextStateInput);
                nextStateReward = nextStateOutput.Max();
            }
            double updatedQvalue = previousQvalue + _learningRate * (reward + _discountFactor * nextStateReward - previousQvalue);
            desiredOutput[action] = updatedQvalue;
            _inputs.Add(previousStateInput);
            _desiredOutputs.Add(desiredOutput);
        }
        /// <summary>
        /// Выполняет обновления весов для минимизации ошибки пула значений входов и желаемых выходов. Очищает пул значений для обновления весов и смещений.
        /// </summary>
        public void UpdateState()
        {
            List<double[]> outputsBefore = ActivationNetworkFeatures.ActivationNetworkCompute(_network, _inputs);
            _geneticLearningTeacher.Run(_inputs, _desiredOutputs);
            //_backPropagationLearning.RunEpoch(_inputs.ToArray(), _desiredOutputs.ToArray());
            List<double[]> outputsAfter = ActivationNetworkFeatures.ActivationNetworkCompute(_network, _inputs);
            int u = 0;
            _inputs.Clear();
            _desiredOutputs.Clear();
            _targetNetworkUpdateTimeElapsed++;
            //если совершили достаточно итераций обновления состояния, обновляем целевую нейронную сеть
            if (_targetNetworkUpdateTimeElapsed >= _targetNetworkUpdateTime)
            {
                _targetNetworkUpdateTimeElapsed = 0;
                _targetNetwork = ActivationNetworkFeatures.CloneActivationNetwork(_network);
            }
        }
    }
}
