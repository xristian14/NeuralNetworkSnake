using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions
{
    public static class Features
    {
        /// <summary>
        /// Возвращает индекс наибольшего элемента в последовательности
        /// </summary>
        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            int maxIndex = -1;
            T maxValue = default(T);

            int index = 0;
            foreach (T value in sequence)
            {
                if (value.CompareTo(maxValue) > 0 || maxIndex == -1)
                {
                    maxIndex = index;
                    maxValue = value;
                }
                index++;
            }
            return maxIndex;
        }
        private static Random _random = new Random();
        public static double GetRandDouble(double min, double max)
        {
            return _random.NextDouble() * (max - min) + min;
        }
        /// <summary>
        /// Создает и возвращает полную копию экземпляра AForge.Neuro.ActivationNetwork
        /// </summary>
        public static AForge.Neuro.ActivationNetwork CloneActivationNetwork(AForge.Neuro.ActivationNetwork inputNetwork)
        {
            int[] layers = new int[inputNetwork.Layers.Length];
            for(int i = 0; i < layers.Length; i++)
            {
                layers[i] = inputNetwork.Layers[i].Neurons.Length;
            }
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[0].Neurons[0]).ActivationFunction, inputNetwork.InputsCount, layers);
            for(int i = 0; i < activationNetwork.Layers.Length; i++)
            {
                for (int n = 0; n < activationNetwork.Layers[i].Neurons.Length; n++)
                {
                    inputNetwork.Layers[i].Neurons[n].Weights.CopyTo(activationNetwork.Layers[i].Neurons[n].Weights, 0);
                    ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[n]).Threshold = ((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[i].Neurons[n]).Threshold;
                    ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[n]).ActivationFunction = ((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[i].Neurons[n]).ActivationFunction;
                }
            }
            return activationNetwork;
        }
        /// <summary>
        /// Заполняет значения весов и смещений случайными значениями от randomMin до randomMax
        /// </summary>
        public static void FillRandomlyActivationNetwork(AForge.Neuro.ActivationNetwork inputNetwork, double randomMin, double randomMax)
        {
            for (int i = 0; i < inputNetwork.Layers.Length; i++)
            {
                for (int n = 0; n < inputNetwork.Layers[i].Neurons.Length; n++)
                {
                    for (int w = 0; w < inputNetwork.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        inputNetwork.Layers[i].Neurons[n].Weights[w] = GetRandDouble(randomMin, randomMax);
                    }
                    ((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[i].Neurons[n]).Threshold = GetRandDouble(randomMin, randomMax);
                }
            }
        }
        /// <summary>
        /// Копирует значения весов и смещений из sourceNetwork в targetNetwork
        /// </summary>
        public static void CopyActivationNetworkWeightsBiases(AForge.Neuro.ActivationNetwork sourceNetwork, AForge.Neuro.ActivationNetwork targetNetwork)
        {
            for (int i = 0; i < sourceNetwork.Layers.Length; i++)
            {
                for (int n = 0; n < sourceNetwork.Layers[i].Neurons.Length; n++)
                {
                    sourceNetwork.Layers[i].Neurons[n].Weights.CopyTo(targetNetwork.Layers[i].Neurons[n].Weights, 0);
                    ((AForge.Neuro.ActivationNeuron)targetNetwork.Layers[i].Neurons[n]).Threshold = ((AForge.Neuro.ActivationNeuron)sourceNetwork.Layers[i].Neurons[n]).Threshold;
                }
            }
        }
        /// <summary>
        /// возвращает массив, сумма значений которого равняется 1
        /// </summary>
        public static double[] SoftMaxArray(double[] input)
        {
            double[] output = new double[input.Length];
            double inputSum = input.Sum();
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] / inputSum;
            }
            return output;
        }
    }
}
