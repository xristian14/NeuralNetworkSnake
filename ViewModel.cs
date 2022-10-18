using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class ViewModel : ViewModelBase
    {
        public ViewModel()
        {
            int[] a = new int[3] { 2, 3, 4 };
            NeuralNetworkUnit neuralNetworkUnit = new NeuralNetworkUnit(a);
            NeuralNetworkEngine.FillRandomly(neuralNetworkUnit);
        }
        private int _hiddenLayersCount = 1;
        public int HiddenLayersCount
        {
            get { return _hiddenLayersCount; }
            set
            {
                _hiddenLayersCount = value;
                OnPropertyChanged();
            }
        }
        private string _firstHiddenLayerCountNeurons = "3";
        public string FirstHiddenLayerCountNeurons
        {
            get { return _firstHiddenLayerCountNeurons; }
            set
            {
                _firstHiddenLayerCountNeurons = value;
                OnPropertyChanged();
            }
        }
        private string _secondHiddenLayerCountNeurons = "3";
        public string SecondHiddenLayerCountNeurons
        {
            get { return _secondHiddenLayerCountNeurons; }
            set
            {
                _secondHiddenLayerCountNeurons = value;
                OnPropertyChanged();
            }
        }
        private string _thirdHiddenLayerCountNeurons = "3";
        public string ThirdHiddenLayerCountNeurons
        {
            get { return _thirdHiddenLayerCountNeurons; }
            set
            {
                _thirdHiddenLayerCountNeurons = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<string> _algorithmLearningCombobox = new ObservableCollection<string>() { "Генетический алгоритм", "Deep Q-Learning" };
        public ObservableCollection<string> AlgorithmLearningCombobox
        {
            get { return _algorithmLearningCombobox; }
            set
            {
                _algorithmLearningCombobox = value;
                OnPropertyChanged();
            }
        }
        private string _selectedAlgorithmLearningCombobox;
        public string SelectedAlgorithmLearningCombobox
        {
            get { return _selectedAlgorithmLearningCombobox; }
            set
            {
                _selectedAlgorithmLearningCombobox = value;
                OnPropertyChanged();
            }
        }
        private string _layersText;
        public string LayersText
        {
            get { return _layersText; }
            set
            {
                _layersText = value;
                OnPropertyChanged();
            }
        }
        private int _age = 1000000000;
        public int Age
        {
            get { return _age; }
            set
            {
                _age = value;
                OnPropertyChanged();
            }
        }
        private string _boardSize = "13";
        public string BoardSize
        {
            get { return _boardSize; }
            set
            {
                _boardSize = value;
                OnPropertyChanged();
            }
        }
        private string _applesCount = "1";
        public string ApplesCount
        {
            get { return _applesCount; }
            set
            {
                _applesCount = value;
                OnPropertyChanged();
            }
        }
        private bool _isRealtimeSimulation = true;
        public bool IsRealtimeSimulation
        {
            get { return _isRealtimeSimulation; }
            set
            {
                _isRealtimeSimulation = value;
                OnPropertyChanged();
            }
        }
        private int _realtimeDelay = 100;
        public int RealtimeDelay
        {
            get { return _realtimeDelay; }
            set
            {
                _realtimeDelay = value;
                OnPropertyChanged();
            }
        }
        private int _fixedDuration = 30;
        public int FixedDuration
        {
            get { return _fixedDuration; }
            set
            {
                _fixedDuration = value;
                OnPropertyChanged();
            }
        }
        private int _remainingTime = 30;
        public int RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                OnPropertyChanged();
            }
        }

        /*генетический алгоритм*/
        private int _populationSize = 50;
        public int PopulationSize
        {
            get { return _populationSize; }
            set
            {
                _populationSize = value;
                OnPropertyChanged();
            }
        }
        private int _mutationPercent = 1;
        public int MutationPercent
        {
            get { return _mutationPercent; }
            set
            {
                _mutationPercent = value;
                OnPropertyChanged();
            }
        }
    }
}
