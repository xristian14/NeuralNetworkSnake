using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningTeacher
    {
        /// <summary>
        /// Инициализирует экземпляр GeneticLearningTeacher. Функция приспособленности - наименьшее значение средней ошибки
        /// </summary>
        public GeneticLearningTeacher(int populationSize, AForge.Neuro.ActivationNetwork network)
        {
            _populationSize = populationSize;
            _network = network;
            _lossFunction = new AForgeExtensions.Neuro.MSELossFunction();
            _selectionMethod = new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection();
            _mutationRate = 0.1;
            _mutationProbability = 0.001;
            _crossoverRate = 0.75;
        }
        private AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private double _mutationRate;
        /// <summary>
        /// Количество особей, участвующих в мутации [0,1]
        /// </summary>
        public double MutationRate { get { return _mutationRate; } set { _mutationRate = value; } }
        private double _mutationProbability;
        /// <summary>
        /// Вероятность мутации одного гена [0,1]
        /// </summary>
        public double MutationProbability { get { return _mutationProbability; } set { _mutationProbability = value; } }
        private double _crossoverRate;
        /// <summary>
        /// Количество особей, участвующих в скрещивании [0,1]
        /// </summary>
        public double CrossoverRate { get { return _crossoverRate; } set { _crossoverRate = value; } }
        private double[][] _inputs;
        private double[][] _desiredOutputs;
        private int _populationSize;
        public int PopulationSize { get { return _populationSize; } }
        private AForgeExtensions.Neuro.ILossFunction _lossFunction;
        /// <summary>
        /// Функция расчета ошибки (по умолчанию MSELossFunction)
        /// </summary>
        public AForgeExtensions.Neuro.ILossFunction LossFunction { get { return _lossFunction; } set { _lossFunction = value; } }
        private AForgeExtensions.Neuro.Learning.GeneticLearning.ISelectionMethod _selectionMethod;
        /// <summary>
        /// Метод селекции: отбора особей в новое поколение после скрещивания и мутаций (по умолчанию RouletteWheelSelection)
        /// </summary>
        public AForgeExtensions.Neuro.Learning.GeneticLearning.ISelectionMethod SelectionMethod { get { return _selectionMethod; } set { _selectionMethod = value; } }
        private AForgeExtensions.Neuro.Learning.GeneticLearning.Chromosome _bestChromosome;
        private AForgeExtensions.Neuro.Learning.GeneticLearning.Chromosome[] _population;
        private void SpawnInitialPopulation()
        {
            _population = new AForgeExtensions.Neuro.Learning.GeneticLearning.Chromosome[_populationSize];
            for (int i = 0; i < _populationSize; i++)
            {
                _population[i] = new AForgeExtensions.Neuro.Learning.GeneticLearning.Chromosome(AForgeExtensions.Features.CloneActivationNetwork(_network));
                if(i == 0)
                {
                    FitnessCalculate(_population[i]);
                }
                else
                {
                    _population[i].Fitness = _population[0].Fitness;
                }
            }
        }
        private void FitnessCalculate(AForgeExtensions.Neuro.Learning.GeneticLearning.Chromosome chromosome)
        {
            double lossSum = 0;
            for(int i = 0; i < _inputs.Length; i++)
            {
                double[] actualOutput = chromosome.Network.Compute(_inputs[i]);
                lossSum += _lossFunction.Calculate(actualOutput, _desiredOutputs[i]);
            }
            chromosome.Fitness = lossSum / _inputs.Length;
        }
        private void PopulationFitnessCalculate()
        {
            for(int i = 0; i < _populationSize; i++)
            {
                FitnessCalculate(_population[i]);
            }
        }
        public double Run(double[][] inputs, double[][] desiredOutputs)
        {
            _inputs = inputs;
            _desiredOutputs = desiredOutputs;
            SpawnInitialPopulation();
            _bestChromosome = _population[0];

        }
    }
}
