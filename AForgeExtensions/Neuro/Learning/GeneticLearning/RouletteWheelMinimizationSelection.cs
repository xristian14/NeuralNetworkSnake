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
            double fintessSum = population.Sum(a => a.Fitness);
            for (int i = 0; i < newPopulationSize; i++)
            {
                double randFitnessSum = _random.NextDouble();
                int k = 0;
                double sum = 0;
                while (k < population.Length && sum < randFitnessSum)
                {
                    sum += 1 - population[k].Fitness / fintessSum;
                    k++;
                }
                int index = k - 1;
                newPopulation[i] = population[index];
            }
            return newPopulation;
        }
    }
}
