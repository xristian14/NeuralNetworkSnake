using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningTeacher : GeneticLearningBase
    {
        /// <summary>
        /// Инициализирует экземпляр GeneticLearningTeacher.
        /// </summary>
        public GeneticLearningTeacher(AForge.Neuro.ActivationNetwork network, int populationSize, int generationsCount, ILossFunction lossFunction, GeneticLearning.SelectionMethodBase selectionMethod, double mutateMinValue, double mutateMaxValue)
        {
            _network = network;
            _populationSize = populationSize;
            _generationsCount = generationsCount;
            _mutateMinValue = mutateMinValue;
            _mutateMaxValue = mutateMaxValue;
            _lossFunction = lossFunction;//new MSELossFunction();
            _selectionMethod = selectionMethod;//new GeneticLearning.RouletteWheelMinimizationSelection();
            _mutationRate = 0.75;
            _genomeLength = GetGenomeLength(network);
            _mutationProbability = 1.0 / _genomeLength;
            _crossoverRate = 0.75;
            _randomRateInitialPopulation = 0;
        }
        private int _generationsCount;
        /// <summary>
        /// Количество поколений
        /// </summary>
        public int GenerationsCount { get { return _generationsCount; } set { _generationsCount = value; } }
        private List<double[]> _inputs;
        private List<double[]> _desiredOutputs;
        private void FitnessCalculate(GeneticLearning.Chromosome chromosome)
        {
            List<double[]> outputs = ActivationNetworkFeatures.ActivationNetworkCompute(chromosome.Network, _inputs);
            chromosome.Fitness = _lossFunction.Calculate(outputs, _desiredOutputs);
        }
        private void GenerationFitnessCalculate()
        {
            for(int i = 0; i < _population.Length; i++)
            {
                FitnessCalculate(_population[i]);
            }
        }
        /// <summary>
        /// Ищет настройки нейронной сети с минимальным значением ошибки, и копирует веса и смещения лучшей модели в нейронную сеть, переданную в конструктор
        /// </summary>
        /// <returns>Значение приспособленности лучшей особи за все поколения</returns>
        public double Run(List<double[]> inputs, List<double[]> desiredOutputs)
        {
            _inputs = inputs;
            _desiredOutputs = desiredOutputs;
            _minFitnessProgression.Clear();
            _averageFitnessProgression.Clear();
            _maxFitnessProgression.Clear();
            SpawnInitialPopulation();
            GenerationFitnessCalculate();
            /*System.Diagnostics.Stopwatch stopwatch1 = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch stopwatch2 = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch stopwatch3 = new System.Diagnostics.Stopwatch();
            System.Diagnostics.Stopwatch stopwatch4 = new System.Diagnostics.Stopwatch();*/
            for (int i = 0; i < _generationsCount; i++)
            {
                //stopwatch1.Start();
                _population = CrossOverPopulation(_population);
                //stopwatch1.Stop();
                //stopwatch2.Start();
                MutatePopulation(_population);
                //stopwatch2.Stop();
                //stopwatch3.Start();
                GenerationFitnessCalculate();
                UpdateBestChromosome();
                //stopwatch3.Stop();
                //stopwatch4.Start();
                _population = SelectionPopulation(_population);
                //stopwatch4.Stop();
            }
            ActivationNetworkFeatures.CopyActivationNetworkWeightsBiases(_bestChromosome.Network, _network);
            return _bestChromosome.Fitness;
        }
    }
}
