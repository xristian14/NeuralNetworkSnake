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
        public GeneticLearningTeacher(AForge.Neuro.ActivationNetwork network, int populationSize, int generationsCount, double mutateMinValue, double mutateMaxValue)
        {
            _network = network;
            _populationSize = populationSize;
            _generationsCount = generationsCount;
            _mutateMinValue = mutateMinValue;
            _mutateMaxValue = mutateMaxValue;
            _lossFunction = new MSELossFunction();
            _selectionMethod = new GeneticLearning.RouletteWheelMinimizationSelection();
            _mutationRate = 0.1;
            _genomeLength = GetGenomeLength(network);
            _mutationProbability = 1.0 / _genomeLength;
            _crossoverRate = 0.75;
            _randomRateInitialPopulation = 0;
        }
        private Random _random = new Random();
        private AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private int _generationsCount;
        /// <summary>
        /// Количество поколений
        /// </summary>
        public int GenerationsCount { get { return _generationsCount; } set { _generationsCount = value; } }
        private double _mutateMinValue;
        /// <summary>
        /// Минимальное значение, которое может быть выбрано в результате мутации
        /// </summary>
        public double MutateMinValue { get { return _mutateMinValue; } set { _mutateMinValue = value; } }
        private double _mutateMaxValue;
        /// <summary>
        /// Максимальное значение, которое может быть выбрано в результате мутации
        /// </summary>
        public double MutateMaxValue { get { return _mutateMaxValue; } set { _mutateMaxValue = value; } }
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
        private List<double> _minFitnessProgression = new List<double>();
        /// <summary>
        /// Минимальное значение приспособленности, для всех поколений
        /// </summary>
        public List<double> MinFitnessProgression { get { return _minFitnessProgression; } }
        private List<double> _averageFitnessProgression = new List<double>();
        /// <summary>
        /// Среднее значение приспособленности, для всех поколений
        /// </summary>
        public List<double> AverageFitnessProgression { get { return _averageFitnessProgression; } }
        private List<double> _maxFitnessProgression = new List<double>();
        /// <summary>
        /// Максимальное значение приспособленности, для всех поколений
        /// </summary>
        public List<double> MaxFitnessProgression { get { return _maxFitnessProgression; } }
        private double _randomRateInitialPopulation;
        /// <summary>
        /// Количество случайно сгенерированных особей в начальной популяции [0,1]. Хотя бы одна особь будет с оригинальной AForge.Neuro.ActivationNetwork
        /// </summary>
        public double RandomRateInitialPopulation { get { return _randomRateInitialPopulation; } set { _randomRateInitialPopulation = value; } }
        private double[][] _inputs;
        private double[][] _desiredOutputs;
        private int _populationSize;
        public int PopulationSize { get { return _populationSize; } }
        private int _genomeLength;
        /// <summary>
        /// Длина генома нейронной сети
        /// </summary>
        public int GenomeLength { get { return _genomeLength; } }
        private ILossFunction _lossFunction;
        /// <summary>
        /// Функция расчета ошибки (по умолчанию MSELossFunction)
        /// </summary>
        public ILossFunction LossFunction { get { return _lossFunction; } set { _lossFunction = value; } }
        private GeneticLearning.ISelectionMethod _selectionMethod;
        /// <summary>
        /// Метод селекции: отбора особей в новое поколение после скрещивания и мутаций (по умолчанию RouletteWheelMinimizationSelection)
        /// </summary>
        public GeneticLearning.ISelectionMethod SelectionMethod { get { return _selectionMethod; } set { _selectionMethod = value; } }
        private GeneticLearning.Chromosome _bestChromosome;
        private GeneticLearning.Chromosome[] _population;
        private int GetGenomeLength(AForge.Neuro.ActivationNetwork network)
        {
            int length = 0;
            for (int i = 0; i < network.Layers.Length; i++)
            {
                length += network.Layers[i].Neurons.Length * network.Layers[i].Neurons[0].Weights.Length; //веса
                length += network.Layers[i].Neurons.Length; //смещение
            }
            return length;
        }
        private void SpawnInitialPopulation()
        {
            _population = new GeneticLearning.Chromosome[_populationSize];
            bool isOriginalNetwork = false;
            for (int i = 0; i < _populationSize; i++)
            {
                if(_random.NextDouble() < _randomRateInitialPopulation)
                {
                    AForge.Neuro.ActivationNetwork randNetwork = Features.CloneActivationNetwork(_network);
                    Features.FillRandomlyActivationNetwork(randNetwork, _mutateMinValue, _mutateMaxValue);
                    _population[i] = new GeneticLearning.Chromosome(randNetwork);
                }
                else
                {
                    isOriginalNetwork = true;
                    _population[i] = new GeneticLearning.Chromosome(Features.CloneActivationNetwork(_network));
                }
            }
            if (!isOriginalNetwork)
            {
                _population[0] = new GeneticLearning.Chromosome(Features.CloneActivationNetwork(_network));
            }
            _bestChromosome = _population[0];
        }
        private GeneticLearning.Chromosome[] ClonePopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                newPopulation[i] = (GeneticLearning.Chromosome)population[i].Clone();
            }
            return newPopulation;
        }
        private void FitnessCalculate(GeneticLearning.Chromosome chromosome)
        {
            double lossSum = 0;
            for(int i = 0; i < _inputs.Length; i++)
            {
                double[] actualOutput = chromosome.Network.Compute(_inputs[i]);
                lossSum += _lossFunction.Calculate(actualOutput, _desiredOutputs[i]);
            }
            chromosome.Fitness = lossSum / _inputs.Length;
        }
        private void GenerationFitnessCalculate()
        {
            for(int i = 0; i < _population.Length; i++)
            {
                FitnessCalculate(_population[i]);
            }

            double minFitness = _population.Min(a => a.Fitness);
            if(minFitness < _bestChromosome.Fitness)
            {
                double min = _population[0].Fitness;
                int index = 0;
                for(int i = 1; i < _population.Length; i++)
                {
                    if(_population[i].Fitness < min)
                    {
                        min = _population[i].Fitness;
                        index = i;
                    }
                }
                _bestChromosome = _population[index];
            }
            _minFitnessProgression.Add(minFitness);
            _averageFitnessProgression.Add(_population.Average(a => a.Fitness));
            _maxFitnessProgression.Add(_population.Max(a => a.Fitness));
        }
        private GeneticLearning.Chromosome CrossOverPair(GeneticLearning.Chromosome chromosome1, GeneticLearning.Chromosome chromosome2)
        {
            GeneticLearning.Chromosome childChromosome = (GeneticLearning.Chromosome)chromosome1.Clone();
            for (int i = 0; i < childChromosome.Network.Layers.Length; i++)
            {
                for (int n = 0; n < childChromosome.Network.Layers[i].Neurons.Length; n++)
                {
                    for(int w = 0; w < childChromosome.Network.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        if (_random.NextDouble() < 0.5)
                        {
                            childChromosome.Network.Layers[i].Neurons[n].Weights[w] = chromosome2.Network.Layers[i].Neurons[n].Weights[w];
                        }
                    }
                    if (_random.NextDouble() < 0.5)
                    {
                        ((AForge.Neuro.ActivationNeuron)childChromosome.Network.Layers[i].Neurons[n]).Threshold = ((AForge.Neuro.ActivationNeuron)chromosome2.Network.Layers[i].Neurons[n]).Threshold;
                    }
                }
            }
            return childChromosome;
        }
        private GeneticLearning.Chromosome[] CrossOverPopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            int crossOverPairsCount = (int)Math.Round(population.Length * _crossoverRate);
            GeneticLearning.Chromosome[] crossOverPairs = _selectionMethod.ApplySelection(population, crossOverPairsCount * 2);
            for(int i = 0; i < crossOverPairsCount; i++)
            {
                newPopulation[i] = CrossOverPair(crossOverPairs[i * 2], crossOverPairs[i * 2 + 1]);
            }
            GeneticLearning.Chromosome[] chromosomesWithoutCrossover = _selectionMethod.ApplySelection(population, population.Length - crossOverPairsCount);
            for(int i = 0; i < population.Length - crossOverPairsCount; i++)
            {
                newPopulation[i + crossOverPairsCount] = (GeneticLearning.Chromosome)chromosomesWithoutCrossover[i].Clone();
            }
            return newPopulation;
        }
        private void MutateChromosome(GeneticLearning.Chromosome chromosome)
        {
            for (int i = 0; i < chromosome.Network.Layers.Length; i++)
            {
                for (int n = 0; n < chromosome.Network.Layers[i].Neurons.Length; n++)
                {
                    for (int w = 0; w < chromosome.Network.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        if (_random.NextDouble() < _mutationProbability)
                        {
                            chromosome.Network.Layers[i].Neurons[n].Weights[w] = Features.GetRandDouble(_mutateMinValue, _mutateMaxValue);
                        }
                    }
                    if (_random.NextDouble() < _mutationProbability)
                    {
                        ((AForge.Neuro.ActivationNeuron)chromosome.Network.Layers[i].Neurons[n]).Threshold = Features.GetRandDouble(_mutateMinValue, _mutateMaxValue);
                    }
                }
            }
        }
        private void MutatePopulation(GeneticLearning.Chromosome[] population)
        {
            for(int i = 0; i < population.Length; i++)
            {
                if(_random.NextDouble() < MutationRate)
                {
                    MutateChromosome(population[i]);
                }
            }
        }
        private GeneticLearning.Chromosome[] SelectionPopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            GeneticLearning.Chromosome[] selectionPopulation = _selectionMethod.ApplySelection(population, population.Length);
            for(int i = 0; i < newPopulation.Length; i++)
            {
                newPopulation[i] = (GeneticLearning.Chromosome)selectionPopulation[i].Clone();
            }
            return newPopulation;
        }
        /// <summary>
        /// Ищет настройки нейронной сети с минимальным значением ошибки, и копирует веса и смещения лучшей модели в нейронную сеть, переданную в конструктор
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="desiredOutputs"></param>
        /// <returns></returns>
        public double Run(double[][] inputs, double[][] desiredOutputs)
        {
            _inputs = inputs;
            _desiredOutputs = desiredOutputs;
            _minFitnessProgression.Clear();
            _averageFitnessProgression.Clear();
            _maxFitnessProgression.Clear();
            SpawnInitialPopulation();
            GenerationFitnessCalculate();
            for (int i = 0; i < _generationsCount; i++)
            {
                _population = CrossOverPopulation(_population);
                MutatePopulation(_population);
                GenerationFitnessCalculate();
                _population = SelectionPopulation(_population);
            }
            Features.CopyActivationNetworkWeightsBiases(_bestChromosome.Network, _network);
            return _bestChromosome.Fitness;
        }
    }
}
