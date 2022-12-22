﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public class SelectionMethodBase : ISelectionMethod
    {
        protected Random _random = new Random();
        protected bool _isFitnessMaximization;
        /// <summary>
        /// Значение приспособленности максимизируется или минимизируется
        /// </summary>
        public bool IsFitnessMaximization { get { return _isFitnessMaximization; } }
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            return new Chromosome[0];
        }
    }
}
