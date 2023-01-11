using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    /// <summary>
    /// Выбор хромосом в новую популяцию осуществляется случайным образом, чем ВЫШЕ значение приспособленности, тем выше шанс быть выбранным. Значение приспособленности прямо пропорционально шансу быть выбранным.
    /// </summary>
    public class RouletteWheelSelection : SelectionMethodBase
    {
        public RouletteWheelSelection()
        {
            _isFitnessMaximization = true;
        }
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public override Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            Chromosome[] newPopulation = new Chromosome[newPopulationSize];
            double fintessSum = population.Sum(a => a.ScaledFitness);
            for (int i = 0; i < newPopulationSize; i++)
            {
                double randFitness = _random.NextDouble() * fintessSum;
                int k = 0;
                double sum = 0;
                while (k < population.Length && sum < randFitness)
                {
                    sum += population[k].ScaledFitness;
                    k++;
                }
                int index = k > 0 ? k - 1 : k;
                newPopulation[i] = population[index];
            }
            return newPopulation;
        }
    }
}
