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
            NewPopulationSize = populationSize;
            _populationSize = populationSize;
            Population = new NeuralNetworkUnitGeneticLearning[_populationSize];
            for(int i = 0; i < _populationSize; i++)
            {
                Population[i] = new NeuralNetworkUnitGeneticLearning(NeuralNetworkUnit.CreateNeuralNetworkUnitRandomly(layers));
            }
        }
        private int[] Layers;
        private readonly object locker = new object();
        private double _mutationPercent { get; set; }
        public double MutationPercent //реализация потокобезопасного получения и установки свойства
        {
            get { lock (locker) { return _mutationPercent; } }
            private set { lock (locker) { _mutationPercent = value; } }
        }
        private int _populationSize;

        private int _newPopulationSize { get; set; }
        public int NewPopulationSize //реализация потокобезопасного получения и установки свойства
        {
            get { lock (locker) { return _newPopulationSize; } }
            private set { lock (locker) { _newPopulationSize = value; } }
        }
        public NeuralNetworkUnitGeneticLearning[] Population;
        public int GetPopulationSize()
        {
            return _populationSize;
        }
        public void SetMutationPercent(double mutationPercent)
        {
            MutationPercent = mutationPercent;
        }
        public void SetNewPopulationSize(int populationSize)
        {
            NewPopulationSize = populationSize;
        }
        public void SpawnNewGeneration()
        {
            _populationSize = NewPopulationSize;
            double[] populationRatings = new double[_populationSize];
            for(int i = 0; i < _populationSize; i++)
            {
                populationRatings[i] = Population[i].Rating;
            }
            double[] softMaxRatings = Features.SoftMaxVector(populationRatings);
            //генерируем новую популяцию
            NeuralNetworkUnitGeneticLearning[] newPopulation = new NeuralNetworkUnitGeneticLearning[_populationSize];
            for (int i = 0; i < _populationSize; i++)
            {
                double softMaxRatingParent = Features.GetRandDouble(0, 1);
                int k = 0;
                double sum = 0;
                while(k < _populationSize && sum < softMaxRatingParent)
                {
                    sum += softMaxRatings[k];
                    k++;
                }
                int indexFirstParent = k - 1;
                softMaxRatingParent = Features.GetRandDouble(0, 1);
                k = 0;
                sum = 0;
                while (k < _populationSize && sum < softMaxRatingParent)
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
                for (int k = 0; k < parent1.Weights[i].RowCount; k++)
                {
                    for (int n = 0; n < parent1.Weights[i].ColumnCount; n++)
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
                        child.Biases[i][k] = parent1.Biases[i][k];
                    else
                        child.Biases[i][k] = parent2.Biases[i][k];
                    if (Features.GetRandDouble(0, 100) < MutationPercent)
                        child.Biases[i][k] = Features.GetRandFloat(-1, 1);
                }
            }
            return child;
        }
    }
}
