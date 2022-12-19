using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public static class ActivationNetworkFeatures
    {
        /// <summary>
        /// Инициализирует новый экземпляр AForge.Neuro.ActivationNetwork со случайными значениями весов и смещений в диапазоне от randomMin до randomMax
        /// </summary>
        public static AForge.Neuro.ActivationNetwork BuildRandom(float randomMin, float randomMax, AForge.Neuro.IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            AForge.Neuro.Neuron.RandRange = new AForge.Range(randomMin, randomMax);
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(function, inputsCount, neuronsCount);
            return activationNetwork;
        }
        /// <summary>
        /// Вычисляет работу нейросети для массива массивов входных значений
        /// </summary>
        public static double[][] ActivationNetworkCompute(AForge.Neuro.ActivationNetwork network, double[][] inputs)
        {
            double[][] outputs = new double[inputs.Length][];
            for(int i = 0; i < inputs.Length; i++)
            {
                outputs[i] = network.Compute(inputs[i]);
            }
            return outputs;
        }
        /// <summary>
        /// Создает и возвращает полную копию экземпляра AForge.Neuro.ActivationNetwork
        /// </summary>
        public static AForge.Neuro.ActivationNetwork CloneActivationNetwork(AForge.Neuro.ActivationNetwork inputNetwork)
        {
            int[] layers = new int[inputNetwork.Layers.Length];
            for (int i = 0; i < layers.Length; i++)
            {
                layers[i] = inputNetwork.Layers[i].Neurons.Length;
            }
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[0].Neurons[0]).ActivationFunction, inputNetwork.InputsCount, layers);
            for (int i = 0; i < activationNetwork.Layers.Length; i++)
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
                        inputNetwork.Layers[i].Neurons[n].Weights[w] = Features.GetRandDouble(randomMin, randomMax);
                    }
                    ((AForge.Neuro.ActivationNeuron)inputNetwork.Layers[i].Neurons[n]).Threshold = Features.GetRandDouble(randomMin, randomMax);
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
    }
}
