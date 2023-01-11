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
        /// <param name="generationsDuration">Длительность в поколениях.</param>
        /// <param name="mutationProbability">Вероятность мутации одного гена [0,1].</param>
        /// <param name="fitnessScaleRate">Показатель масштабирования значений приспособленности [0,∞]. 0 - отсутствие маштабирования, 1 - масштабирование в 2 раза, 2 - масштабирование в 3 раза. Для максимизации приспособленности, точка масштабирования - максимальная приспособленность, для минимизации точка масштабирования - минимальная приспособленость. Примеры: (FitnessScaleRate=1, Минимизация) [0.5, 0.6, 1]->[0.5, 0.7, 1.5]; (FitnessScaleRate=2, Максимизация) [1, 2, 3]->[-3, 0, 3]; (FitnessScaleRate=0.5, Максимизация) [0.5, 0.6, 0.7]->[0.4, 0.55, 0.7].</param>
        public StepsSettings(int generationsDuration, double mutationProbability, double fitnessScaleRate = 0)
        {
            _generationsDuration = generationsDuration;
            _mutationProbability = mutationProbability;
            _fitnessScaleRate = fitnessScaleRate;
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
        private double _fitnessScaleRate;
        /// <summary>
        /// Показатель масштабирования значений приспособленности [0,∞]. 0 - отсутствие маштабирования, 1 - масштабирование в 2 раза, 2 - масштабирование в 3 раза. Для максимизации приспособленности, точка масштабирования - максимальная приспособленность, для минимизации точка масштабирования - минимальная приспособленость. Примеры: (FitnessScaleRate=1, Минимизация) [0.5, 0.6, 1]->[0.5, 0.7, 1.5]; (FitnessScaleRate=2, Максимизация) [1, 2, 3]->[-3, 0, 3]; (FitnessScaleRate=0.5, Максимизация) [0.5, 0.6, 0.7]->[0.4, 0.55, 0.7].
        /// </summary>
        public double FitnessScaleRate { get { return _fitnessScaleRate; } set { _fitnessScaleRate = value; } }
    }
}
