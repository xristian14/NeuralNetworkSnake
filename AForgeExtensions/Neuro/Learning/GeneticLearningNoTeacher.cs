using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning
{
    public class GeneticLearningNoTeacher : GeneticLearningBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stepsSettings">Настройки шагов обучения. Каждый шаг обладает шансом мутации, длительностью в поколениях, и степенью выделения максимальных значений приспособленности. По завершению одного шага, и при переходе на следующий, будет создана новая популяция из лучшей хромосомы за весь период обучения. Если обучение завершено, при следующем вызове Run() будет создано новое поколение на основе настроек последнего элемента в списке stepsSettings. Для обучения по шагам с начала, вызовите метод ResetStepsSettingsNumber().</param>
        public GeneticLearningNoTeacher(AForge.Neuro.ActivationNetwork network, int populationSize, ILossFunction lossFunction, GeneticLearning.SelectionMethodBase selectionMethod, double mutateMinValue, double mutateMaxValue, List<GeneticLearning.StepsSettings> stepsSettings)
        {
            _network = network;
            _populationSize = populationSize;
            _mutateMinValue = mutateMinValue;
            _mutateMaxValue = mutateMaxValue;
            _lossFunction = lossFunction;
            _selectionMethod = selectionMethod;
            _mutationRate = 1;
            _genomeLength = GetGenomeLength(network);
            _crossoverRate = 1;
            _randomRateInitialPopulation = 0;
            _stepsSettings = stepsSettings;
            ResetStepsSettingsNumber();
        }
        private void SetFitness(double[] populationFitness)
        {
            for (int i = 0; i < _population.Length; i++)
            {
                _population[i].Fitness = populationFitness[i];
            }
        }
        /// <summary>
        /// Генерирует следующее поколение, на основе значений приспосоленности текущего поколения
        /// </summary>
        /// <param name="popultionFitness">Значения приспособленности текущего поколения</param>
        /// <returns>true если все шаги обучения завершены, и false в противном случае</returns>
        public bool Run(double[] populationFitness)
        {
            MutatePopulation(_population);
            SetFitness(populationFitness);
            ConvertFitness();
            UpdateBestChromosome();
            _stepGenerationNumber++;
            if(_stepGenerationNumber >= _stepsSettings[_stepNumber].GenerationsDuration && _stepNumber < _stepsSettings.Count - 1)
            {
                _stepGenerationNumber = 0;
                _stepNumber++;
                SpawnInitialPopulation(_bestChromosome.Network);
            }
            else
            {
                _population = SelectionPopulation(_population);
                _population = CrossOverPopulation(_population);
                MutatePopulation(_population);
            }
            return IsFinish();
        }
    }
}
