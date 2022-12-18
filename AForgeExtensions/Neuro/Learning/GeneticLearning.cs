using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningTeacher
    {
        /// <summary>
        /// Инициализирует экземпляр GeneticLearningTeacher. Функция приспособленности - наименьшее значение средней ошибки
        /// </summary>
        public GeneticLearningTeacher(AForge.Neuro.ActivationNetwork network, AForgeExtensions.Neuro.Learning.GeneticLearning.ISelectionMethod selectionMethod)
        {
            _network = network;
            _selectionMethod = selectionMethod;
            _mutationRate = 0.01;
            _mutationProbability = 0.001;
            _crossoverRate = 0.75;
        }
        private AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private double _mutationRate;
        /// <summary>
        /// Количество особей, участвующих в мутации [0,1]
        /// </summary>
        public double MutationRate { get { return _mutationRate; } set { _mutationRate = value; } }
        private double _mutationProbability;
        /// <summary>
        /// Вероятность мутации одного гена мутации [0,1]
        /// </summary>
        public double MutationProbability { get { return _mutationProbability; } set { _mutationProbability = value; } }
        private double _crossoverRate;
        /// <summary>
        /// Количество особей, участвующих в скрещивании [0,1]
        /// </summary>
        public double CrossoverRate { get { return _crossoverRate; } set { _crossoverRate = value; } }
        private double _populationSize;
        public double PopulationSize { get { return _populationSize; } }
        private AForgeExtensions.Neuro.Learning.GeneticLearning.ISelectionMethod _selectionMethod;
        /// <summary>
        /// Метод селекции: отбора особей в новое поколение после скрещивания и мутаций
        /// </summary>
        public AForgeExtensions.Neuro.Learning.GeneticLearning.ISelectionMethod SelectionMethod { get { return _selectionMethod; } set { _selectionMethod = value; } }
        
    }
}
