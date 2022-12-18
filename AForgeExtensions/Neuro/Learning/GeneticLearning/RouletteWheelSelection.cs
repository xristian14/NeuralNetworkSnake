using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public class RouletteWheelSelection : ISelectionMethod
    {
        public RouletteWheelSelection()
        {

        }
        private static Random _random = new Random();
        public Chromosome[] ApplySelection(Chromosome[] population)
        {
            Chromosome[] newPopulation = new Chromosome[population.Length];
            double fintessSum = population.Sum(a => a.Fitness);
            for (int i = 0; i < population.Length; i++)
            {
                double randFitnessSum = _random.NextDouble() * fintessSum;
                int k = 0;
                double sum = 0;
                while (k < population.Length && sum < randFitnessSum)
                {
                    sum += population[k].Fitness;
                    k++;
                }
                int index = k - 1;
                newPopulation[i] = (Chromosome)population[index].Clone();
            }
            return newPopulation;
        }
    }
}
