using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public class StepsSettings
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="generationsDuration">длительность в поколениях</param>
        /// <param name="mutationProbability">Вероятность мутации одного гена [0,1]</param>
        /// <param name="fitnessStretchRate">Показатель растягивания значений приспособленности. Показатель позволяет собирать значения около минимума (для максимизации значения приспособленности) или максимума (для минимизации значения приспособленности), с целью выделить лучшие результаты и дать им больше шанса быть отобранными в процессе селекции [0,∞]. 0 - приспособленность не изменяется. Пример(для максимизации приспособленности): значения приспособленности и результаты преобразования для разных FitnessStretchRate: [20, 50, 90, 99, 100]: 0-> [20, 50, 90, 99, 100]; 0.5-> [20, 38.4, 85.5, 98.5, 100]; 1-> [20, 31.3, 81.3, 98, 100]; 2-> [20, 24.2, 73.6, 97, 100]; 5-> [20, 20.2, 55.9, 94.2, 100]; 10-> [20, 20, 38.4, 89.6, 100]; 20-> [20, 20, 24.8, 81.4, 100].</param>
        public StepsSettings(int generationsDuration, double mutationProbability, double fitnessStretchRate = 0)
        {
            _generationsDuration = generationsDuration;
            _mutationProbability = mutationProbability;
            _fitnessStretchRate = fitnessStretchRate;
        }
        private int _generationsDuration;
        /// <summary>
        /// Длительность шага обучения в поколениях
        /// </summary>
        public int GenerationsDuration { get { return _generationsDuration; } set { _generationsDuration = value; } }
        private double _mutationProbability;
        /// <summary>
        /// Вероятность мутации одного гена [0,1]
        /// </summary>
        public double MutationProbability { get { return _mutationProbability; } set { _mutationProbability = value; } }
        private double _fitnessStretchRate;
        /// <summary>
        /// Показатель растягивания значений приспособленности. Показатель позволяет собирать значения около минимума (для максимизации значения приспособленности) или максимума (для минимизации значения приспособленности), с целью выделить лучшие результаты и дать им больше шанса быть отобранными в процессе селекции [0,∞]. 0 - приспособленность не изменяется. Пример(для максимизации приспособленности): значения приспособленности и результаты преобразования для разных FitnessStretchRate: [20, 50, 90, 99, 100]: 0-> [20, 50, 90, 99, 100]; 0.5-> [20, 38.4, 85.5, 98.5, 100]; 1-> [20, 31.3, 81.3, 98, 100]; 2-> [20, 24.2, 73.6, 97, 100]; 5-> [20, 20.2, 55.9, 94.2, 100]; 10-> [20, 20, 38.4, 89.6, 100]; 20-> [20, 20, 24.8, 81.4, 100].
        /// </summary>
        public double FitnessStretchRate { get { return _fitnessStretchRate; } set { _fitnessStretchRate = value; } }
    }
}
