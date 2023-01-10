using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningBase
    {
        protected Random _random = new Random();
        protected AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        protected double _mutateMinValue;
        /// <summary>
        /// Минимальное значение, которое может быть выбрано в результате мутации
        /// </summary>
        public double MutateMinValue { get { return _mutateMinValue; } set { _mutateMinValue = value; } }
        protected double _mutateMaxValue;
        /// <summary>
        /// Максимальное значение, которое может быть выбрано в результате мутации
        /// </summary>
        public double MutateMaxValue { get { return _mutateMaxValue; } set { _mutateMaxValue = value; } }
        protected double _mutationRate;
        /// <summary>
        /// Количество особей, участвующих в мутации [0,1]
        /// </summary>
        public double MutationRate { get { return _mutationRate; } set { _mutationRate = value; } }
        protected double _crossoverRate;
        /// <summary>
        /// Количество особей, участвующих в скрещивании [0,1]
        /// </summary>
        public double CrossoverRate { get { return _crossoverRate; } set { _crossoverRate = value; } }
        protected List<GeneticLearning.StepsSettings> _stepsSettings;
        /// <summary>
        /// Настройки шагов обучения. Каждый шаг обладает шансом мутации, длительностью в поколениях, и степенью выделения максимальных значений приспособленности. По завершению одного шага, и при переходе на следующий, будет создана новая популяция из лучшей хромосомы за весь период обучения.
        /// </summary>
        public List<GeneticLearning.StepsSettings> StepsSettings { get { return _stepsSettings; } }
        protected int _stepNumber;
        protected int _stepGenerationNumber;
        protected List<double> _minFitnessProgression = new List<double>();
        /// <summary>
        /// Минимальное значение приспособленности, для всех поколений
        /// </summary>
        public List<double> MinFitnessProgression { get { return _minFitnessProgression; } }
        protected List<double> _averageFitnessProgression = new List<double>();
        /// <summary>
        /// Среднее значение приспособленности, для всех поколений
        /// </summary>
        public List<double> AverageFitnessProgression { get { return _averageFitnessProgression; } }
        protected List<double> _maxFitnessProgression = new List<double>();
        /// <summary>
        /// Максимальное значение приспособленности, для всех поколений
        /// </summary>
        public List<double> MaxFitnessProgression { get { return _maxFitnessProgression; } }
        protected double _randomRateInitialPopulation;
        /// <summary>
        /// Количество случайно сгенерированных особей в начальной популяции [0,1]. Хотя бы одна особь будет с оригинальной AForge.Neuro.ActivationNetwork
        /// </summary>
        public double RandomRateInitialPopulation { get { return _randomRateInitialPopulation; } set { _randomRateInitialPopulation = value; } }
        protected int _populationSize;
        public int PopulationSize { get { return _populationSize; } set { _populationSize = value; } }
        protected int _genomeLength;
        /// <summary>
        /// Длина генома нейронной сети
        /// </summary>
        public int GenomeLength { get { return _genomeLength; } }
        protected ILossFunction _lossFunction;
        /// <summary>
        /// Функция расчета ошибки
        /// </summary>
        public ILossFunction LossFunction { get { return _lossFunction; } set { _lossFunction = value; } }
        protected GeneticLearning.SelectionMethodBase _selectionMethod;
        /// <summary>
        /// Метод селекции: отбора особей в новое поколение после скрещивания и мутаций
        /// </summary>
        public GeneticLearning.SelectionMethodBase SelectionMethod { get { return _selectionMethod; } set { _selectionMethod = value; } }
        protected GeneticLearning.Chromosome _bestChromosome;
        /// <summary>
        /// Лучшая особь за все поколения
        /// </summary>
        public GeneticLearning.Chromosome BestChromosome { get { return _bestChromosome; } }
        protected GeneticLearning.Chromosome[] _population;
        public GeneticLearning.Chromosome[] Population { get { return _population; } }
        protected int GetGenomeLength(AForge.Neuro.ActivationNetwork network)
        {
            int length = 0;
            for (int i = 0; i < network.Layers.Length; i++)
            {
                length += network.Layers[i].Neurons.Length * network.Layers[i].Neurons[0].Weights.Length; //веса
                length += network.Layers[i].Neurons.Length; //смещение
            }
            return length;
        }
        /// <summary>
        /// Сбрасывает номер текущего шага обучения и номер поколения, после чего процесс обучения начнется с начального элемента в StepsSettings.
        /// </summary>
        public void ResetStepsSettingsNumber()
        {
            _stepNumber = 0;
            _stepGenerationNumber = 0;
        }
        /// <summary>
        /// Возвращает true если все шаги тестирования пройдены, и false в противном случае.
        /// </summary>
        public bool IsFinish()
        {
            return (_stepNumber == _stepsSettings.Count - 1) && (_stepGenerationNumber > _stepsSettings[_stepNumber].GenerationsDuration);
        }
        /// <summary>
        /// Вычисляет ConvertedFitness для хромосом, и в зависимости от значения FitnessMaxHighlightRate выделяем максимумы приспособленности.
        /// </summary>
        protected void ConvertFitness()
        {
            double max = _population.Max(a => a.Fitness);
            for(int i = 0; i < _population.Length; i++)
            {
                _population[i].ConvertedFitness = _population[i].Fitness * Math.Pow(_population[i].Fitness / max, _stepsSettings[_stepNumber].FitnessMaxHighlightRate);
            }
        }
        /// <summary>
        /// Порождает начальную популяцию. В начальной популяции будут сгенерированы особи со случайными значениями весов и смещений с шансом _randomRateInitialPopulation, их веса и смещения будут в диапазоне от _mutateMinValue до _mutateMaxValue
        /// </summary>
        public void SpawnInitialPopulation(AForge.Neuro.ActivationNetwork network)
        {
            _population = new GeneticLearning.Chromosome[_populationSize];
            bool isOriginalNetwork = false;
            for (int i = 0; i < _populationSize; i++)
            {
                if (_random.NextDouble() < _randomRateInitialPopulation)
                {
                    AForge.Neuro.ActivationNetwork randNetwork = ActivationNetworkFeatures.CloneActivationNetwork(network);
                    ActivationNetworkFeatures.FillRandomlyActivationNetwork(randNetwork, _mutateMinValue, _mutateMaxValue);
                    _population[i] = new GeneticLearning.Chromosome(randNetwork);
                }
                else
                {
                    isOriginalNetwork = true;
                    _population[i] = new GeneticLearning.Chromosome(ActivationNetworkFeatures.CloneActivationNetwork(network));
                }
            }
            if (!isOriginalNetwork)
            {
                _population[0] = new GeneticLearning.Chromosome(ActivationNetworkFeatures.CloneActivationNetwork(network));
            }
            _bestChromosome = _population[0];
        }
        protected GeneticLearning.Chromosome[] ClonePopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            for (int i = 0; i < population.Length; i++)
            {
                newPopulation[i] = (GeneticLearning.Chromosome)population[i].Clone();
            }
            return newPopulation;
        }
        /// <summary>
        /// Обновляет лучшую хромосому, в соответствии с политикой максимизации или минимизации приспособленности в _selectionMethod, а так же добавляет в историю приспособленности минимальное, среднее и максимальное значения приспособленности текущего поколения
        /// </summary>
        protected void UpdateBestChromosome()
        {
            int minFitnessIndex = 0;
            double minFitness = _population[minFitnessIndex].Fitness;
            for (int i = 1; i < _population.Length; i++)
            {
                if (_population[i].Fitness < minFitness)
                {
                    minFitness = _population[i].Fitness;
                    minFitnessIndex = i;
                }
            }

            int maxFitnessIndex = 0;
            double maxFitness = _population[maxFitnessIndex].Fitness;
            for (int i = 1; i < _population.Length; i++)
            {
                if (_population[i].Fitness > maxFitness)
                {
                    maxFitness = _population[i].Fitness;
                    maxFitnessIndex = i;
                }
            }

            if (_selectionMethod.IsFitnessMaximization)
            {
                if (maxFitness > _bestChromosome.Fitness)
                {
                    _bestChromosome = _population[maxFitnessIndex];
                }
            }
            else
            {
                if (minFitness < _bestChromosome.Fitness)
                {
                    _bestChromosome = _population[minFitnessIndex];
                }
            }

            _minFitnessProgression.Add(minFitness);
            _averageFitnessProgression.Add(_population.Average(a => a.Fitness));
            _maxFitnessProgression.Add(maxFitness);
        }
        protected GeneticLearning.Chromosome CrossOverPair(GeneticLearning.Chromosome chromosome1, GeneticLearning.Chromosome chromosome2)
        {
            GeneticLearning.Chromosome childChromosome = (GeneticLearning.Chromosome)chromosome1.Clone();
            for (int i = 0; i < childChromosome.Network.Layers.Length; i++)
            {
                for (int n = 0; n < childChromosome.Network.Layers[i].Neurons.Length; n++)
                {
                    for (int w = 0; w < childChromosome.Network.Layers[i].Neurons[n].Weights.Length; w++)
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
        protected GeneticLearning.Chromosome[] CrossOverPopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            int crossOverPairsCount = (int)Math.Round(population.Length * _crossoverRate);
            GeneticLearning.Chromosome[] crossOverPairs = _selectionMethod.ApplySelection(population, crossOverPairsCount * 2);
            for (int i = 0; i < crossOverPairsCount; i++)
            {
                newPopulation[i] = CrossOverPair(crossOverPairs[i * 2], crossOverPairs[i * 2 + 1]);
            }
            GeneticLearning.Chromosome[] chromosomesWithoutCrossover = _selectionMethod.ApplySelection(population, population.Length - crossOverPairsCount);
            for (int i = 0; i < population.Length - crossOverPairsCount; i++)
            {
                newPopulation[i + crossOverPairsCount] = (GeneticLearning.Chromosome)chromosomesWithoutCrossover[i].Clone();
            }
            return newPopulation;
        }
        protected void MutateChromosome(GeneticLearning.Chromosome chromosome)
        {
            for (int i = 0; i < chromosome.Network.Layers.Length; i++)
            {
                for (int n = 0; n < chromosome.Network.Layers[i].Neurons.Length; n++)
                {
                    for (int w = 0; w < chromosome.Network.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        if (_random.NextDouble() < _stepsSettings[_stepNumber].MutationProbability)
                        {
                            chromosome.Network.Layers[i].Neurons[n].Weights[w] = Features.GetRandDouble(_mutateMinValue, _mutateMaxValue);
                        }
                    }
                    if (_random.NextDouble() < _stepsSettings[_stepNumber].MutationProbability)
                    {
                        ((AForge.Neuro.ActivationNeuron)chromosome.Network.Layers[i].Neurons[n]).Threshold = Features.GetRandDouble(_mutateMinValue, _mutateMaxValue);
                    }
                }
            }
        }
        protected void MutatePopulation(GeneticLearning.Chromosome[] population)
        {
            for (int i = 0; i < population.Length; i++)
            {
                if (_random.NextDouble() < MutationRate)
                {
                    MutateChromosome(population[i]);
                }
            }
        }
        protected GeneticLearning.Chromosome[] SelectionPopulation(GeneticLearning.Chromosome[] population)
        {
            GeneticLearning.Chromosome[] newPopulation = new GeneticLearning.Chromosome[population.Length];
            GeneticLearning.Chromosome[] selectionPopulation = _selectionMethod.ApplySelection(population, population.Length);
            for (int i = 0; i < newPopulation.Length; i++)
            {
                newPopulation[i] = (GeneticLearning.Chromosome)selectionPopulation[i].Clone();
            }
            return newPopulation;
        }
    }
}
