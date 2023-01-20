using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public class RouletteWheelSelection : SelectionMethodBase
    {
        public RouletteWheelSelection(bool isFitnessMaximization, bool isFlexibleTarget, double fixedTarget = 0)
        {
            _isFitnessMaximization = isFitnessMaximization;
            _isFlexibleTarget = isFlexibleTarget;
            _target = fixedTarget;
        }
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public override Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            Chromosome[] newPopulation = new Chromosome[newPopulationSize];
            double softMaxSum = 100;
            double[] targetCloseness = new double[population.Length]; //близость приспособленности к цели селекции
            for(int i = 0; i < population.Length; i++)
            {
                targetCloseness[i] = Math.Abs(_target - population[i].ScaledFitness);
            }
            targetCloseness = AForgeExtensions.Features.SoftMax(targetCloseness, softMaxSum);
            for (int i = 0; i < targetCloseness.Length; i++)
            {
                targetCloseness[i] = 1 / targetCloseness[i];
            }
            double targetClosenessSum = targetCloseness.Sum();
            for (int i = 0; i < newPopulationSize; i++)
            {
                double random = _random.NextDouble() * targetClosenessSum;
                int k = 0;
                double sum = 0;
                while (k < targetCloseness.Length && sum < random)
                {
                    sum += targetCloseness[k];
                    k++;
                }
                int index = k > 0 ? k - 1 : k;
                newPopulation[i] = population[index];
            }
            return newPopulation;
        }
    }
}
