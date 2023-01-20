using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public abstract class SelectionMethodBase
    {
        protected Random _random = new Random();
        protected double _target;
        /// <summary>
        /// Цель отбора.
        /// </summary>
        public double Target { get { return _target; } }
        protected List<double> _targetHistory = new List<double>();
        /// <summary>
        /// История цели отбора.
        /// </summary>
        public List<double> TargetHistory { get { return _targetHistory; } }
        protected bool _isFlexibleTarget;
        /// <summary>
        /// Цель отбора гибкая или фиксированная.
        /// </summary>
        public bool IsFlexibleTarget { get { return _isFlexibleTarget; } }
        protected double _flexibleTargetMultiply = 4;
        /// <summary>
        /// Множитель для определения цели селекции. (Умножается на среднее изменение лучшей приспособленности за FlexibleTargetSourceLength поколений, после чего добавляется (максимизация) или вычитается (минимизация) из лучшего значения приспособленности текущего поколения).
        /// </summary>
        public double FlexibleTargetMultiply { get { return _flexibleTargetMultiply; } set { _flexibleTargetMultiply = value; } }
        protected int _flexibleTargetSourceLength = 5;
        /// <summary>
        /// Количество поколений, за которое будет определяться среднее изменение лучшей приспособленности. (Значение изменения добавляется в список последних изменений только если оно не 0).
        /// </summary>
        public int FlexibleTargetSourceLength { get { return _flexibleTargetSourceLength; } set { _flexibleTargetSourceLength = value; } }
        protected bool _isFitnessMaximization;
        /// <summary>
        /// Значение приспособленности максимизируется или минимизируется
        /// </summary>
        public bool IsFitnessMaximization { get { return _isFitnessMaximization; } }
        /// <summary>
        /// Значения лучшей приспособленности, на осоновании которых будет определяться цель селекции.
        /// </summary>
        protected List<double> _targetSourceBestFitness = new List<double>();
        /// <summary>
        /// Сбрасывает значения полей, сформированных в результате обучения.
        /// </summary>
        public void ResetSelection()
        {
            _targetSourceBestFitness.Clear();
            _targetHistory.Clear();
        }
        /// <summary>
        /// Добавляет значение лучшей приспособленности для формирования гибкой цели отбора, и обновляет цель отбора.
        /// </summary>
        public void AddTargetSource(double bestFitness)
        {
            if(_targetSourceBestFitness.Count == 0)
            {
                _targetSourceBestFitness.Add(bestFitness);
                _target = bestFitness + (_isFitnessMaximization ? (bestFitness + 1) * 0.3 : -((bestFitness + 1) * 0.3));
                _targetHistory.Add(_target);
            }
            else
            {
                double difference = Math.Abs(_targetSourceBestFitness.Last() - bestFitness);
                if (difference > 0.000000000000001)
                {
                    if(_targetSourceBestFitness.Count == _flexibleTargetSourceLength)
                    {
                        _targetSourceBestFitness.RemoveAt(0);
                    }
                    _targetSourceBestFitness.Add(bestFitness);
                    double bestFitnessAverage = _targetSourceBestFitness.Average();
                    double changing = 0;
                    for(int i = 0; i < _targetSourceBestFitness.Count; i++)
                    {
                        changing += Math.Abs(_targetSourceBestFitness[i] - bestFitnessAverage);
                    }
                    changing /= _targetSourceBestFitness.Count;
                    if (_isFitnessMaximization)
                    {
                        _target = _targetSourceBestFitness.Last() + changing * _flexibleTargetMultiply;
                    }
                    else
                    {
                        _target = _targetSourceBestFitness.Last() - changing * _flexibleTargetMultiply;
                    }
                    _targetHistory.Add(_target);
                }
            }
        }
        /// <summary>
        /// Возвращает массив хромосом, которые были выбраны в результате селекции. Результирующий массив содержит ссылки на хромосомы исходного массива.
        /// </summary>
        public virtual Chromosome[] ApplySelection(Chromosome[] population, int newPopulationSize)
        {
            return new Chromosome[0];
        }
    }
}
