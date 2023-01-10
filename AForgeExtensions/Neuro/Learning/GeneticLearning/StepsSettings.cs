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
        /// <param name="fitnessMaxHighlightRate">Степень выделения максимальных значений приспособленности [0,∞]. 0 - не выделять. Примеры (степень приспособленности, и получившиеся значения приспособленности в результате выделения максимумов): 0: [1, 9, 10]->[1, 9,1 0]; 1: [1, 9, 10]->[0.1, 8.1, 10]; 2: [1, 9, 10]->[0.01, 7.29, 10]; 0.5: [5, 50, 99, 100]->[1.12, 35.36,9 98.5, 100]; 1: [5, 50, 99, 100]->[0.25, 25, 98, 100]; 2: [5, 50, 99, 100]->[0.01, 12.5, 97, 100]; 5: [5, 50, 99, 100]->[0, 1.56, 94.15, 100]; 10: [5, 50, 99, 100]->[0, 0.05, 89.53, 100]; 20: [5, 50, 99, 100]->[0, 0, 80.97, 100];</param>
        public StepsSettings(int generationsDuration, double mutationProbability, double fitnessMaxHighlightRate = 0)
        {
            _generationsDuration = generationsDuration;
            _mutationProbability = mutationProbability;
            _fitnessMaxHighlightRate = fitnessMaxHighlightRate;
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
        private double _fitnessMaxHighlightRate;
        /// <summary>
        /// Степень выделения максимальных значений приспособленности [0,∞]. 0 - не выделять. Примеры (степень приспособленности, и получившиеся значения приспособленности в результате выделения максимумов): 0: [1, 9, 10]->[1, 9,1 0]; 1: [1, 9, 10]->[0.1, 8.1, 10]; 2: [1, 9, 10]->[0.01, 7.29, 10]; 0.5: [5, 50, 99, 100]->[1.12, 35.36,9 98.5, 100]; 1: [5, 50, 99, 100]->[0.25, 25, 98, 100]; 2: [5, 50, 99, 100]->[0.01, 12.5, 97, 100]; 5: [5, 50, 99, 100]->[0, 1.56, 94.15, 100]; 10: [5, 50, 99, 100]->[0, 0.05, 89.53, 100]; 20: [5, 50, 99, 100]->[0, 0, 80.97, 100];
        /// </summary>
        public double FitnessMaxHighlightRate { get { return _fitnessMaxHighlightRate; } set { _fitnessMaxHighlightRate = value; } }
    }
}
