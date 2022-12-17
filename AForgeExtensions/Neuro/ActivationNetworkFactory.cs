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
        /// <param name="activationFunction">Функция активации</param>
        /// <param name="outputActivationFunction">Функция активации выходного слоя</param>
        /// <param name="inputsCount"></param>
        /// <param name="neuronsCount"></param>
        /// <returns></returns>
        public static AForge.Neuro.ActivationNetwork BuildRandFromNegativeOneToOne(AForge.Neuro.IActivationFunction activationFunction, AForge.Neuro.IActivationFunction outputActivationFunction, int inputsCount, params int[] neuronsCount)
        {
            AForge.Neuro.Neuron.RandRange = new AForge.Range(-1f, 1f);
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(activationFunction, inputsCount, neuronsCount);
            for (int i = 0; i < activationNetwork.Layers.Last().Neurons.Length; i++)
            {
                ((AForge.Neuro.ActivationNeuron)activationNetwork.Layers.Last().Neurons[i]).ActivationFunction = outputActivationFunction;
            }
            return activationNetwork;
        }
    }
}
