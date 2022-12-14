using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeNeuroExtensions
{
    public class ActivationNetwork
    {
        private ActivationNetwork()
        {

        }
        /// <summary>
        /// Инициализирует новый экземпляр AForge.Neuro.ActivationNetwork со случайными значениями весов и смещений в диапазоне от -1 до 1
        /// </summary>
        public static AForge.Neuro.ActivationNetwork BuildRandFromNegativeOneToOne(AForge.Neuro.IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            AForge.Neuro.Neuron.RandRange = new AForge.Range(-1f, 1f);
            return new AForge.Neuro.ActivationNetwork(function, inputsCount, neuronsCount);
        }
    }
}
