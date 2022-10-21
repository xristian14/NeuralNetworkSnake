using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

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
                Population[i] = new NeuralNetworkUnitGeneticLearning(NeuralNetworkUnit.CreateNeuralNetworkUnitRandomly(layers));
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
            double[] softMaxRatings = Features.SoftMaxVector(populationRatings);
            //генерируем новую популяцию
            NeuralNetworkUnitGeneticLearning[] newPopulation = new NeuralNetworkUnitGeneticLearning[PopulationSize];
            for (int i = 0; i < PopulationSize; i++)
            {
                double softMaxRatingParent = Features.GetRandDouble(0, 1);
                int k = 0;
                double sum = 0;
                while(k < PopulationSize && sum < softMaxRatingParent)
                {
                    sum += softMaxRatings[k];
                    k++;
                }
                int indexFirstParent = k - 1;
                softMaxRatingParent = Features.GetRandDouble(0, 1);
                k = 0;
                sum = 0;
                while (k < PopulationSize && sum < softMaxRatingParent)
                {
                    sum += softMaxRatings[k];
                    k++;
                }
                int indexSecondParent = k - 1;
                newPopulation[i] = new NeuralNetworkUnitGeneticLearning(Crossing(Population[indexFirstParent].NeuralNetworkUnit, Population[indexSecondParent].NeuralNetworkUnit));
            }
            Population = newPopulation;
        }
        private NeuralNetworkUnit Crossing(NeuralNetworkUnit parent1, NeuralNetworkUnit parent2)
        {
            NeuralNetworkUnit child = NeuralNetworkUnit.CreateNeuralNetworkUnitZero(Layers);
            for(int i = 0; i < parent1.Weights.Length; i++)
            {
                for (int n = 0; n < parent1.Weights[i].ColumnCount; n++)
                {
                    for (int k = 0; k < parent1.Weights[i].RowCount; k++)
                    {
                        //веса
                        if(Features.GetRandFloat(0, 1) < 0.5f)
                            child.Weights[i][k, n] = parent1.Weights[i][k, n];
                        else
                            child.Weights[i][k, n] = parent2.Weights[i][k, n];
                        if(Features.GetRandDouble(0, 100) < MutationPercent)
                            child.Weights[i][k, n] = Features.GetRandFloat(-1, 1);
                    }
                    //смещение
                    if (Features.GetRandFloat(0, 1) < 0.5f)
                        child.Biases[i][n] = parent1.Biases[i][n];
                    else
                        child.Biases[i][n] = parent2.Biases[i][n];
                    if (Features.GetRandDouble(0, 100) < MutationPercent)
                        child.Biases[i][n] = Features.GetRandFloat(-1, 1);
                }
            }
            return child;
        }
    }
}
