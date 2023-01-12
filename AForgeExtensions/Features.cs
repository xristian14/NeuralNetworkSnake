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
        /// Переводит объект типа AForge.Neuro.ActivationNetwork в объект типа AForgeExtensions.Neuro.ActivationNetworkSerializeFormat, который можно сериализовать и десериализовать
        /// </summary>
        public static Neuro.ActivationNetworkSerializeFormat ConvertActNetToSerializeFormat(AForge.Neuro.ActivationNetwork activationNetwork)
        {
            Neuro.ActivationNetworkSerializeFormat activationNetworkSerializeFormat = new Neuro.ActivationNetworkSerializeFormat();
            activationNetworkSerializeFormat.InputNeuronsCount = activationNetwork.InputsCount;
            activationNetworkSerializeFormat.OtherLayersNeuronsCount = new int[activationNetwork.Layers.Length];
            activationNetworkSerializeFormat.OtherLayersActivationFunction = new string[activationNetwork.Layers.Length];
            activationNetworkSerializeFormat.Weights = new double[activationNetwork.Layers.Length][][];
            activationNetworkSerializeFormat.Biases = new double[activationNetwork.Layers.Length][];
            for (int i = 0; i < activationNetwork.Layers.Length; i++)
            {
                activationNetworkSerializeFormat.OtherLayersNeuronsCount[i] = activationNetwork.Layers[i].Neurons.Length;
                activationNetworkSerializeFormat.OtherLayersActivationFunction[i] = ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[0]).ActivationFunction.GetType().ToString();
                activationNetworkSerializeFormat.Weights[i] = new double[activationNetwork.Layers[i].Neurons.Length][];
                activationNetworkSerializeFormat.Biases[i] = new double[activationNetwork.Layers[i].Neurons.Length];
                for (int n = 0; n < activationNetwork.Layers[i].Neurons.Length; n++)
                {
                    activationNetworkSerializeFormat.Weights[i][n] = new double[activationNetwork.Layers[i].Neurons[n].Weights.Length];
                    for (int w = 0; w < activationNetwork.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        activationNetworkSerializeFormat.Weights[i][n][w] = activationNetwork.Layers[i].Neurons[n].Weights[w];
                    }
                    activationNetworkSerializeFormat.Biases[i][n] = ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[n]).Threshold;
                }
            }
            return activationNetworkSerializeFormat;
        }
        /// <summary>
        /// Переводит объект типа AForgeExtensions.Neuro.ActivationNetworkSerializeFormat в объект типа AForge.Neuro.ActivationNetwork
        /// </summary>
        public static AForge.Neuro.ActivationNetwork ConvertSerializeFormatToActNet(Neuro.ActivationNetworkSerializeFormat activationNetworkSerializeFormat)
        {
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(new AForge.Neuro.SigmoidFunction(), activationNetworkSerializeFormat.InputNeuronsCount, activationNetworkSerializeFormat.OtherLayersNeuronsCount);
            AForge.Neuro.IActivationFunction[] activationFunctions = new AForge.Neuro.IActivationFunction[activationNetworkSerializeFormat.OtherLayersActivationFunction.Length];
            for(int i = 0; i < activationNetworkSerializeFormat.Weights.Length; i++)
            {
                AForge.Neuro.IActivationFunction activationFunction = (AForge.Neuro.IActivationFunction)Activator.CreateInstance(null, activationNetworkSerializeFormat.OtherLayersActivationFunction[i]).Unwrap();
                for (int n = 0; n < activationNetwork.Layers[i].Neurons.Length; n++)
                {
                    for (int w = 0; w < activationNetwork.Layers[i].Neurons[n].Weights.Length; w++)
                    {
                        activationNetwork.Layers[i].Neurons[n].Weights[w] = activationNetworkSerializeFormat.Weights[i][n][w];
                    }
                    ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[n]).Threshold = activationNetworkSerializeFormat.Biases[i][n];
                    ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[i].Neurons[n]).ActivationFunction = activationFunction;
                }
            }
            return activationNetwork;
        }
        /// <summary>
        /// Возвращает массив, сумма занчений которого равняется outputSum
        /// </summary>
        public static double[] SoftMax(double[] input, double outputSum)
        {
            double[] output = new double[input.Length];
            double inputSum = input.Sum();
            for (int i = 0; i < input.Length; i++)
            {
                output[i] = input[i] / inputSum * outputSum;
            }
            return output;
        }
    }
}
