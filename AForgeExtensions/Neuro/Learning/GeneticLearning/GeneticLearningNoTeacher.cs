using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningNoTeacher : GeneticLearningBase
    {
        /// <summary>
        /// Инициализирует экземпляр GeneticLearningNoTeacher
        /// </summary>
        public GeneticLearningNoTeacher(AForge.Neuro.ActivationNetwork network, int populationSize, ILossFunction lossFunction, GeneticLearning.SelectionMethodBase selectionMethod, double mutateMinValue, double mutateMaxValue)
        {
            _network = network;
            _populationSize = populationSize;
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
        private void SetFitness(double[] popultionFitness)
        {
            for (int i = 0; i < _population.Length; i++)
            {
                _population[i].Fitness = popultionFitness[i];
            }
        }
        /// <summary>
        /// Генерирует следующее поколение, на основе значений приспосоленности текущего
        /// </summary>
        /// <param name="popultionFitness">Значения приспособленности текущего поколения</param>
        /// <returns>Значение приспособленности лучшей особи за все поколения</returns>
        public double Run(double[] popultionFitness)
        {
            SetFitness(popultionFitness);
            UpdateBestChromosome();
            _population = SelectionPopulation(_population);
            _population = CrossOverPopulation(_population);
            MutatePopulation(_population);
            return _bestChromosome.Fitness;
        }
    }
}
