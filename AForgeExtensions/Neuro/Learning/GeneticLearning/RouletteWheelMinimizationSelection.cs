using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    /// <summary>
    /// Выбор хромосом в новую популяцию осуществляется случайным образом, чем НИЖЕ значение приспособленности, тем выше шанс быть выбранным. Значение приспособленности прямо пропорционально шансу быть выбранным.
    /// </summary>
    public class RouletteWheelMinimizationSelection : ISelectionMethod
    {
        public RouletteWheelMinimizationSelection()
        {

        }
        private static Random _random = new Random();
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            Chromosome[] newPopulation = new Chromosome[newPopulationSize];
            double fintessSum = population.Sum(a => 1 / a.Fitness);
            for (int i = 0; i < newPopulationSize; i++)
            {
                double randFitness = _random.NextDouble() * fintessSum;
                int k = 0;
                double sum = 0;
                while (k < population.Length && sum < randFitness)
                {
                    sum += 1 / population[k].Fitness;
                    k++;
                }
                int index = k - 1;
                newPopulation[i] = population[index];
            }
            return newPopulation;
        }
    }
}
