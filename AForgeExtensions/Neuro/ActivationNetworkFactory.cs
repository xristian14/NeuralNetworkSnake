using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public static class ActivationNetworkFactory
    {
        /// <summary>
        /// Инициализирует новый экземпляр AForge.Neuro.ActivationNetwork со случайными значениями весов и смещений в диапазоне от -1 до 1
        /// </summary>
        /// <param name="function">Функция активации</param>
        /// <param name="outputLayerFunction">Функция активации выходного слоя</param>
        /// <param name="inputsCount"></param>
        /// <param name="neuronsCount"></param>
        /// <returns></returns>
        public static AForge.Neuro.ActivationNetwork BuildRandFromNegativeOneToOne(AForge.Neuro.IActivationFunction function, AForge.Neuro.IActivationFunction outputLayerFunction, int inputsCount, params int[] neuronsCount)
        {
            AForge.Neuro.Neuron.RandRange = new AForge.Range(-1f, 1f);
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(function, inputsCount, neuronsCount);
            int lastLayerIndex = activationNetwork.Layers.Length - 1;
            for (int i = 0; i < activationNetwork.Layers[lastLayerIndex].Neurons.Length; i++)
            {
                ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers[lastLayerIndex].Neurons[i]).ActivationFunction = outputLayerFunction;
            }
            return activationNetwork;
        }
    }
}
