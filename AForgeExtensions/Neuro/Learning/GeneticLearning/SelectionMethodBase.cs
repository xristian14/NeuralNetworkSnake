using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public abstract class SelectionMethodBase : ISelectionMethod
    {
        protected Random _random = new Random();
        protected bool _isFlexibleTarget;
        /// <summary>
        /// Цель отбора гибкая или фиксированная.
        /// </summary>
        public bool IsFlexibleTarget { get { return _isFlexibleTarget; } }
        protected double _flexibleTargetScaleRate;
        /// <summary>
        /// Показатель масштабирования гибкой цели селекции. (На сколько умножить лучший результат при максимизации для вычисления цели, или на сколько)
        /// </summary>
        public double FlexibleTargetScaleRate { get { return _flexibleTargetScaleRate; } }
        protected bool _isFitnessMaximization;
        /// <summary>
        /// Значение приспособленности максимизируется или минимизируется
        /// </summary>
        public bool IsFitnessMaximization { get { return _isFitnessMaximization; } }
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public virtual Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            return new Chromosome[0];
        }
    }
}
