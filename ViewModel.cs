using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows;

namespace NeuralNetworkSnake
{
    class ViewModel : ViewModelBase
    {
        private static ViewModel _instance;
        private ViewModel()
        {
            BoardSizeInt = int.Parse(BoardSize);
            UpdateBoardCells();
        }
        public static ViewModel getInstance()
        {
            if (_instance == null)
            {
                _instance = new ViewModel();
            }
            return _instance;
        }
        
        private Simulation _simulation;
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
        private string _firstHiddenLayerCountNeurons = "20";
        public string FirstHiddenLayerCountNeurons
        {
            get { return _firstHiddenLayerCountNeurons; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    _firstHiddenLayerCountNeurons = value;
                }
                OnPropertyChanged();
            }
        }
        private string _secondHiddenLayerCountNeurons = "20";
        public string SecondHiddenLayerCountNeurons
        {
            get { return _secondHiddenLayerCountNeurons; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    _secondHiddenLayerCountNeurons = value;
                }
                OnPropertyChanged();
            }
        }
        private string _thirdHiddenLayerCountNeurons = "20";
        public string ThirdHiddenLayerCountNeurons
        {
            get { return _thirdHiddenLayerCountNeurons; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    _thirdHiddenLayerCountNeurons = value;
                }
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
        private double _boardCellSize = 14;
        public double BoardCellSize
        {
            get { return _boardCellSize; }
            private set
            {
                _boardCellSize = value;
                OnPropertyChanged();
            }
        }
        private string _boardSize = "13";
        public string BoardSize
        {
            get { return _boardSize; }
            set
            {
                if(int.TryParse(value, out int res))
                {
                    if (res < 51 && res > 2)
                    {
                        _boardSize = value;
                    }
                    else if(res > 50)
                    {
                        _boardSize = 50.ToString();
                    }
                    else if(res < 3)
                    {
                        _boardSize = 3.ToString();
                    }
                    BoardSizeInt = int.Parse(_boardSize);
                    UpdateBoardCells();
                }
                OnPropertyChanged();
            }
        }
        private int _boardSizeInt;
        public int BoardSizeInt
        {
            get { return _boardSizeInt; }
            set
            {
                _boardSizeInt = value;
                OnPropertyChanged();
            }
        }
        private string _applesCount = "1";
        public string ApplesCount
        {
            get { return _applesCount; }
            set
            {
                if(int.TryParse(value, out int res))
                {
                    _applesCount = value;
                }
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
                if (_simulation != null)
                {
                    _simulation.PauseMillisecDelay = value;
                }
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
                if (_simulation != null)
                {
                    _simulation.FixedDuration = value;
                }
                OnPropertyChanged();
            }
        }
        private long _remainingTime = 0;
        public long RemainingTime
        {
            get { return _remainingTime; }
            set
            {
                _remainingTime = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<BoardCell> _boardCells = new ObservableCollection<BoardCell>();
        public ObservableCollection<BoardCell> BoardCells
        {
            get { return _boardCells; }
            set
            {
                _boardCells = value;
                OnPropertyChanged();
            }
        }

        private void UpdateBoardCells()
        {
            BoardCells.Clear();
            int boardSize = int.Parse(BoardSize);
            for(int i = 0; i < boardSize; i++)
            {
                for(int k = 0; k < boardSize; k++)
                {
                    BoardCells.Add(new BoardCell(BoardCellSize));
                }
            }
        }

        private ObservableCollection<SnakesForRender> _snakesForRenders = new ObservableCollection<SnakesForRender>();
        public ObservableCollection<SnakesForRender> SnakesForRenders
        {
            get { return _snakesForRenders; }
            set
            {
                _snakesForRenders = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<ApplesForRender> _applesForRenders = new ObservableCollection<ApplesForRender>();
        public ObservableCollection<ApplesForRender> ApplesForRenders
        {
            get { return _applesForRenders; }
            set
            {
                _applesForRenders = value;
                OnPropertyChanged();
            }
        }

        /*генетический алгоритм*/
        private string _populationSize = "50";
        public string PopulationSize
        {
            get { return _populationSize; }
            set
            {
                if(int.TryParse(value, out int res))
                {
                    _populationSize = value;
                }
                OnPropertyChanged();
            }
        }
        private string _mutationPercent = "1";
        public string MutationPercent
        {
            get { return _mutationPercent; }
            set
            {
                if (double.TryParse(value, out double res))
                {
                    _populationSize = value;
                }
                OnPropertyChanged();
            }
        }


        public ICommand CreateNeuralNetwork_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    int[] layers = new int[2 + HiddenLayersCount];
                    layers[0] = 39;
                    if(HiddenLayersCount >= 1)
                    {
                        layers[1] = int.Parse(FirstHiddenLayerCountNeurons);
                    }
                    if (HiddenLayersCount >= 2)
                    {
                        layers[2] = int.Parse(SecondHiddenLayerCountNeurons);
                    }
                    if (HiddenLayersCount >= 3)
                    {
                        layers[3] = int.Parse(ThirdHiddenLayerCountNeurons);
                    }
                    layers[layers.Length - 1] = 3;
                    int populationSize = int.Parse(PopulationSize);

                    SnakesForRenders.Clear();
                    ApplesForRenders.Clear();
                    GeneticLearning geneticLearning = new GeneticLearning(populationSize, layers);
                    _simulation = new Simulation(geneticLearning, RealtimeDelay, BoardSizeInt, int.Parse(ApplesCount));

                    LayersText = layers[0].ToString();
                    for (int i = 1; i < layers.Length; i++)
                    {
                        LayersText += "-" + layers[i].ToString();
                    }

                }, (obj) => true);
            }
        }
        public ICommand StartSimulation_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if (IsRealtimeSimulation)
                    {
                        Task.Run(() => _simulation.RealTimeSimulate()); //запускаем в отдельном потоке чтобы форма обновлялась
                    }
                    else
                    {
                        Task.Run(() => _simulation.FixedTimeSimulate()); //запускаем в отдельном потоке чтобы форма обновлялась
                    }
                }, (obj) => true);
            }
        }
        public ICommand StopSimulation_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    _simulation.IsRunning = false;
                }, (obj) => true);
            }
        }






        private bool _isFirstTest = true;
        private NeuralNetworkUnit _neuralNetworkUnitTest;
        Matrix<float> _matrix3;
        Vector<float> _vector1;
        public void testFunc()
        {
            if (_isFirstTest)
            {
                _isFirstTest = false;
                int[] a = new int[3] { 440000, 100, 1 };
                _neuralNetworkUnitTest = NeuralNetworkUnit.CreateNeuralNetworkUnitRandomly(a);

                _matrix3 = Matrix<float>.Build.Random(a[0], a[1]);
                _vector1 = Vector<float>.Build.Random(a[1]);
            }

            int iteration = 1;
            
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            /*for (int i = 0; i < iteration; i++)
            {
                _ = _neuralNetworkUnitTest.Weights[0] * _neuralNetworkUnitTest.Weights[1];
            }*/
            stopwatch1.Stop();

            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            for (int i = 0; i < iteration; i++)
            {
                _ = _matrix3 * _vector1;
            }
            stopwatch4.Stop();

            FirstHiddenLayerCountNeurons = "stopwatch1=" + stopwatch1.ElapsedMilliseconds + "мс  stopwatch4 = " + stopwatch4.ElapsedMilliseconds + "мс";
            int y = 0;
        }

        public ICommand ButtonTest_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    testFunc();
                }, (obj) => true);
            }
        }
    }
}
