﻿using System;
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
        public static AForge.Neuro.ActivationNetwork BuildRandom(float randomMin, float randomMax, AForge.Neuro.IActivationFunction function, int inputsCount, params int[] neuronsCount)
        {
            AForge.Neuro.Neuron.RandRange = new AForge.Range(randomMin, randomMax);
            AForge.Neuro.ActivationNetwork activationNetwork = new AForge.Neuro.ActivationNetwork(function, inputsCount, neuronsCount);
            return activationNetwork;
        }
    }
}