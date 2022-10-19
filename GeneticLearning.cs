using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class GeneticLearning
    {
        public GeneticLearning(int populationSize, int[] layers)
        {
            Layers = layers;
            MutationPercent = 0;
            PopulationSize = populationSize;
            Population = new NeuralNetworkUnitGeneticLearning[populationSize];
            for(int i = 0; i < populationSize; i++)
            {
                Population[i] = new NeuralNetworkUnitGeneticLearning(new NeuralNetworkUnit(layers));
                NeuralNetworkEngine.FillRandomly(Population[i].NeuralNetworkUnit);
            }
        }
        private int[] Layers;
        private double MutationPercent;
        private int PopulationSize;
        public NeuralNetworkUnitGeneticLearning[] Population;
        public void SetMutationPercent(double mutationPercent)
        {
            MutationPercent = mutationPercent;
        }
        public void SetPopulationSize(int populationSize)
        {
            PopulationSize = populationSize;
        }
        public void SpawnNewGeneration()
        {
            double[] populationRatings = new double[PopulationSize];
            for(int i = 0; i < PopulationSize; i++)
            {
                populationRatings[i] = Population[i].Rating;
            }
            double[] softMaxRatings = NeuralNetworkEngine.SoftMaxVector(populationRatings);
            //генерируем новую популяцию
            NeuralNetworkUnitGeneticLearning[] newPopulation = new NeuralNetworkUnitGeneticLearning[PopulationSize];
            for (int i = 0; i < PopulationSize; i++)
            {
                newPopulation[i] = new NeuralNetworkUnitGeneticLearning(new NeuralNetworkUnit(Layers));
                
                double softMaxRatingParent = Rand.GetDouble(0, 1);
                int k = 0;
                double sum = 0;
                while(k < PopulationSize && sum < softMaxRatingParent)
                {
                    sum += softMaxRatings[k];
                    k++;
                }
                int indexFirstParent = k - 1;
                softMaxRatingParent = Rand.GetDouble(0, 1);
                k = 0;
                sum = 0;
                while (k < PopulationSize && sum < softMaxRatingParent)
                {
                    sum += softMaxRatings[k];
                    k++;
                }
                int indexSecondParent = k - 1;


            }
        }
        private NeuralNetworkUnit Crossing(NeuralNetworkUnit parent1, NeuralNetworkUnit parent2)
        {
            NeuralNetworkUnit child = new NeuralNetworkUnit(Layers);

        }
    }
}
