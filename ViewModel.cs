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
using System.ComponentModel;
using System.Collections.Specialized;
using System.Threading;

namespace NeuralNetworkSnake
{
    class ViewModel : ViewModelBase
    {
        private static ViewModel _instance;
        private ViewModel()
        {
            MapSelection.Add("Карта Обрыв");
            MapSelection.Add("Карта Приключение");
            SelectedMapSelection = MapSelection[0];
            SelectedAlgorithmLearningCombobox = AlgorithmLearningCombobox[0];
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
        private int _hiddenLayersCount = 2;
        public int HiddenLayersCount
        {
            get { return _hiddenLayersCount; }
            set
            {
                _hiddenLayersCount = value;
                OnPropertyChanged();
            }
        }
        private string _firstHiddenLayerCountNeurons = "12";
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
        private string _secondHiddenLayerCountNeurons = "12";
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
        private string _thirdHiddenLayerCountNeurons = "12";
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
        private string _fourthHiddenLayerCountNeurons = "12";
        public string FourthHiddenLayerCountNeurons
        {
            get { return _fourthHiddenLayerCountNeurons; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    _fourthHiddenLayerCountNeurons = value;
                }
                OnPropertyChanged();
            }
        }
        private string _fifthHiddenLayerCountNeurons = "12";
        public string FifthHiddenLayerCountNeurons
        {
            get { return _fifthHiddenLayerCountNeurons; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    _fifthHiddenLayerCountNeurons = value;
                }
                OnPropertyChanged();
            }
        }
        private bool _isGeneticLearning = false;
        public bool IsGeneticLearning //выбрано генетическое обучение змейки. Если нет, то DeepQLearning
        {
            get { return _isGeneticLearning; }
            set
            {
                _isGeneticLearning = value;
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
        private double _boardCellSize = 11;
        public double BoardCellSize
        {
            get { return _boardCellSize; }
            private set
            {
                _boardCellSize = value;
                OnPropertyChanged();
            }
        }
        private string _boardSize = "10";
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
                }
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

        public void CreateBoardCells(int boardSize)
        {
            BoardCells.Clear();
            for (int i = 0; i < boardSize; i++)
            {
                for (int k = 0; k < boardSize; k++)
                {
                    BoardCells.Add(new BoardCell(BoardCellSize));
                }
            }
        }

        private ObservableCollection<string> _inputsInfo = new ObservableCollection<string>();
        public ObservableCollection<string> InputsInfo
        {
            get { return _inputsInfo; }
            set
            {
                _inputsInfo = value;
                OnPropertyChanged();
            }
        }
        public void UpdateInputsInfo(Vector<float> inputs)
        {
            InputsInfo.Clear();
            for (int i = 0; i < inputs.Count; i++)
            {
                if(i == 0 || i == inputs.Count / 3 || i== inputs.Count / 3 * 2)
                {
                    InputsInfo.Add("");
                }
                InputsInfo.Add(inputs[i].ToString());
            }
        }

        private ObservableCollection<GameBoardForRender> _gameBoardsForRender = new ObservableCollection<GameBoardForRender>();
        public ObservableCollection<GameBoardForRender> GameBoardsForRender
        {
            get { return _gameBoardsForRender; }
            set
            {
                _gameBoardsForRender = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<GenerationLeaderboardItem> _generationLeaderboard = new ObservableCollection<GenerationLeaderboardItem>();
        public ObservableCollection<GenerationLeaderboardItem> GenerationLeaderboard
        {
            get { return _generationLeaderboard; }
            set
            {
                _generationLeaderboard = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<LeaderboardItem> _leaderboard = new ObservableCollection<LeaderboardItem>();
        public ObservableCollection<LeaderboardItem> Leaderboard
        {
            get { return _leaderboard; }
            set
            {
                _leaderboard = value;
                OnPropertyChanged();
            }
        }

        /*генетический алгоритм*/
        private string _populationSize = "100";
        public string PopulationSize
        {
            get { return _populationSize; }
            set
            {
                if(int.TryParse(value, out int res))
                {
                    if(res > 0)
                    {
                        _populationSize = value;
                        if (_simulation != null)
                        {
                            _simulation.SetPopulationSize(res);
                        }
                    }
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
                    if(res >= 0 && res <= 100)
                    {
                        _mutationPercent = value;
                        if (_simulation != null)
                        {
                            _simulation.SetMutationPercent(res);
                        }
                    }
                }
                OnPropertyChanged();
            }
        }
        private string _testsCount = "10";
        public string TestsCount //количество тестов для одной нейросети, нужно чтобы провести несколько тестов для одной змейки, и на основе общего результата за все тесты выбирать пары для скрещивания. Один удачный тест может лишить потомства более совершенную нейронную сеть, у которой тест сложился неудачно
        {
            get { return _testsCount; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    if(res > 0)
                    {
                        _testsCount = value;
                    }
                }
                OnPropertyChanged();
            }
        }
        private string _passedToNewGenerationCount = "3";
        public string PassedToNewGenerationCount //Количество лучших результатов, передаваемых в новое поколение без изменений
        {
            get { return _passedToNewGenerationCount; }
            set
            {
                if (int.TryParse(value, out int res))
                {
                    if(res > 0 && res < int.Parse(PopulationSize))
                    {
                        _passedToNewGenerationCount = value;
                    }
                }
                OnPropertyChanged();
            }
        }
        private string _сurrentTestNumber = "0";
        public string CurrentTestNumber //номер теста в текущем поколении
        {
            get { return _сurrentTestNumber; }
            set
            {
                _сurrentTestNumber = value;
                OnPropertyChanged();
            }
        }
        private string _epsilonDeepQLearning = "0,1";
        public string EpsilonDeepQLearning
        {
            get { return _epsilonDeepQLearning; }
            set
            {
                if (double.TryParse(value, out double res))
                {
                    if (res >= 0 && res <= 100)
                    {
                        _epsilonDeepQLearning = value;
                        if (_simulation != null)
                        {
                            _simulation.EpsilonDeepQLearning = res;
                        }
                    }
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
                    if (AlgorithmLearningCombobox.IndexOf(SelectedAlgorithmLearningCombobox) == 0)
                    {
                        _isGeneticLearning = true;
                    }
                    else
                    {
                        _isGeneticLearning = false;
                    }

                    
                    int[] layers = new int[2 + HiddenLayersCount];
                    layers[0] = 42;
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
                    if (HiddenLayersCount >= 3)
                    {
                        layers[4] = int.Parse(FourthHiddenLayerCountNeurons);
                    }
                    if (HiddenLayersCount >= 3)
                    {
                        layers[5] = int.Parse(FifthHiddenLayerCountNeurons);
                    }
                    layers[layers.Length - 1] = 3;

                    if (_isGeneticLearning)
                    {
                        int populationSize = int.Parse(PopulationSize);
                        GeneticLearning geneticLearning = new GeneticLearning(populationSize, int.Parse(MutationPercent), int.Parse(TestsCount), int.Parse(PassedToNewGenerationCount), layers);
                        _simulation = new Simulation(geneticLearning, RealtimeDelay, FixedDuration, int.Parse(BoardSize), int.Parse(ApplesCount));
                    }
                    else
                    {
                        int[] layers2 = new int[layers.Length - 1];
                        for(int i = 0; i < layers2.Length; i++)
                        {
                            layers2[i] = layers[i + 1];
                        }
                        AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-0.1f, 0.1f, new AForgeExtensions.Neuro.LeakyReLuActivationFunction(), layers[0], layers2);
                        //AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFactory.BuildRandom(-0.1f, 0.1f, new AForgeExtensions.Neuro.ReLuActivationFunction(), layers[0], layers2);
                        //AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFactory.BuildRandom(0f, 0.01f, new AForge.Neuro.BipolarSigmoidFunction(), layers[0], layers2);
                        AForgeExtensions.Neuro.Learning.DeepQLearning deepQLearning = new AForgeExtensions.Neuro.Learning.DeepQLearning(activationNetwork);
                        _simulation = new Simulation(deepQLearning, RealtimeDelay, FixedDuration, int.Parse(BoardSize), int.Parse(ApplesCount));
                    }

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









        private ObservableCollection<string> _mapSelection = new ObservableCollection<string>();
        public ObservableCollection<string> MapSelection
        {
            get { return _mapSelection; }
            set
            {
                _mapSelection = value;
                OnPropertyChanged();
            }
        }
        private string _selectedMapSelection;
        public string SelectedMapSelection
        {
            get { return _selectedMapSelection; }
            set
            {
                _selectedMapSelection = value;
                OnPropertyChanged();
            }
        }

        private int _qLearningCellWidth = 61;
        private QLearningCellView _qLearningSelectedCellView;
        public QLearningCellView QLearningSelectedCellView
        {
            get { return _qLearningSelectedCellView; }
            set
            {
                _qLearningSelectedCellView = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<QLearningCellView> _qLearningCellsView = new ObservableCollection<QLearningCellView>();
        public ObservableCollection<QLearningCellView> QLearningCellsView
        {
            get { return _qLearningCellsView; }
            set
            {
                _qLearningCellsView = value;
                OnPropertyChanged();
            }
        }
        private void CreateQLearningCellsView(QLearningMap[,] qLearningMap, System.Drawing.Point[] destinationPoints)
        {
            QLearningCellsView.Clear();
            //System.Windows.Media.Brush ordinaryBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 201, 255, 201));
            System.Windows.Media.Brush ordinaryBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            System.Windows.Media.Brush wallBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 226, 216, 114));
            System.Windows.Media.Brush cliffBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 128, 163, 255));
            System.Windows.Media.Brush destinationBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 135, 135));
            QLearningSelectedCellView = new QLearningCellView(ordinaryBrush, 0, 0, 0, 0, 0, 0, 0);
            for (int i = 0; i < qLearningMap.GetLength(0); i++)
            {
                for (int k = 0; k < qLearningMap.GetLength(1); k++)
                {
                    System.Windows.Media.Brush brush = ordinaryBrush;
                    if (qLearningMap[i, k].IsWall)
                    {
                        brush = wallBrush;
                    }
                    else if(qLearningMap[i, k].IsCliff)
                    {
                        brush = cliffBrush;
                    }
                    for (int u = 0; u < destinationPoints.Length; u++)
                    {
                        if (i == destinationPoints[u].X && k == destinationPoints[u].Y)
                        {
                            brush = destinationBrush;
                            break;
                        }
                    }
                    QLearningCellsView.Add(new QLearningCellView(brush, qLearningMap[i, k].Reward, 0, 0, 0, 0, _qLearningCellWidth * i, _qLearningCellWidth * k));
                }
            }
        }
        private void UpdateQLearningCellView(System.Drawing.Point updateCellPoint)
        {
            int state = PointToState(updateCellPoint, _qLearningMapProcessing.MapColumnCount);
            QLearningCellsView[state].UpQvalue = _qLearningMapProcessing.QLearning.Qvalues[state][0];
            QLearningCellsView[state].RightQvalue = _qLearningMapProcessing.QLearning.Qvalues[state][1];
            QLearningCellsView[state].LeftQvalue = _qLearningMapProcessing.QLearning.Qvalues[state][2];
            QLearningCellsView[state].DownQvalue = _qLearningMapProcessing.QLearning.Qvalues[state][3];
        }
        private void UpdateQLearningSelectedCellView(System.Drawing.Point updateCellPoint)
        {
            QLearningSelectedCellView.Left = _qLearningCellWidth * updateCellPoint.Y;
            QLearningSelectedCellView.Top = _qLearningCellWidth * updateCellPoint.X;
        }
        private int PointToState(System.Drawing.Point point, int mapColumnCount)
        {
            return mapColumnCount * point.X + point.Y;
        }
        private QLearningMapProcessing _qLearningMapProcessing;
        private void CreateQLearningMapProcessing()
        {
            QLearningMap[,] qLearningMap = new QLearningMap[1, 1];
            System.Drawing.Point startPoint = new System.Drawing.Point(0, 0);
            System.Drawing.Point[] destinationPoints = new System.Drawing.Point[1];
            int wallReward = 0;
            double ordinaryReward = 0;
            int cliffReward = -50;
            int[] destinationRewards = new int[2] { 1, 3 };
            List<System.Drawing.Point> wallsList = new List<System.Drawing.Point>();
            List<System.Drawing.Point> cliffList = new List<System.Drawing.Point>();
            if (MapSelection.IndexOf(SelectedMapSelection) == 0)
            {
                qLearningMap = new QLearningMap[6, 12];
                startPoint = new System.Drawing.Point(qLearningMap.GetLength(0) - 2, 1);
                destinationPoints = new System.Drawing.Point[1] { new System.Drawing.Point(qLearningMap.GetLength(0) - 2, qLearningMap.GetLength(1) - 2) };
                for (int i = 0; i < qLearningMap.GetLength(0); i++)
                {
                    for (int k = 0; k < qLearningMap.GetLength(1); k++)
                    {
                        if (i == 0 || i == qLearningMap.GetLength(0) - 1) //стена слева и справа от поля
                        {
                            qLearningMap[i, k] = new QLearningMap(true, false, wallReward);
                        }
                        else if (k == 0 || k == qLearningMap.GetLength(1) - 1) //стена сверху и снизу от поля
                        {
                            qLearningMap[i, k] = new QLearningMap(true, false, wallReward);
                        }
                        else if (i == qLearningMap.GetLength(0) - 2 && k > 1 && k < qLearningMap.GetLength(1) - 1) //обрыв
                        {
                            qLearningMap[i, k] = new QLearningMap(false, true, cliffReward);
                        }
                        else //обычная клетка
                        {
                            qLearningMap[i, k] = new QLearningMap(false, false, ordinaryReward);
                        }
                        for(int u = 0; u < destinationPoints.Length; u++)
                        {
                            if (i == destinationPoints[u].X && k == destinationPoints[u].Y) //клетка назначения
                            {
                                qLearningMap[i, k] = new QLearningMap(false, false, destinationRewards[u]);
                                break;
                            }
                        }
                        
                    }
                }
            }
            else
            {
                qLearningMap = new QLearningMap[12, 26];
                startPoint = new System.Drawing.Point(qLearningMap.GetLength(0) - 1, 2);
                destinationPoints = new System.Drawing.Point[2] { new System.Drawing.Point(3, 8), new System.Drawing.Point(11, 19) };
                //destinationPoints = new System.Drawing.Point[1] { new System.Drawing.Point(11, 19) };
                wallsList.Add(new System.Drawing.Point(0, 1));
                wallsList.Add(new System.Drawing.Point(0, 2));
                wallsList.Add(new System.Drawing.Point(0, 3));
                wallsList.Add(new System.Drawing.Point(0, 4));
                wallsList.Add(new System.Drawing.Point(0, 5));
                wallsList.Add(new System.Drawing.Point(0, 21));
                wallsList.Add(new System.Drawing.Point(1, 5));
                wallsList.Add(new System.Drawing.Point(1, 19));
                wallsList.Add(new System.Drawing.Point(1, 21));
                wallsList.Add(new System.Drawing.Point(1, 23));
                wallsList.Add(new System.Drawing.Point(1, 24));
                wallsList.Add(new System.Drawing.Point(2, 0));
                wallsList.Add(new System.Drawing.Point(2, 1));
                wallsList.Add(new System.Drawing.Point(2, 2));
                wallsList.Add(new System.Drawing.Point(2, 3));
                wallsList.Add(new System.Drawing.Point(2, 5));
                wallsList.Add(new System.Drawing.Point(2, 7));
                wallsList.Add(new System.Drawing.Point(2, 8));
                wallsList.Add(new System.Drawing.Point(2, 19));
                wallsList.Add(new System.Drawing.Point(2, 23));
                wallsList.Add(new System.Drawing.Point(3, 7));
                wallsList.Add(new System.Drawing.Point(3, 19));
                wallsList.Add(new System.Drawing.Point(3, 20));
                wallsList.Add(new System.Drawing.Point(3, 21));
                wallsList.Add(new System.Drawing.Point(3, 22));
                wallsList.Add(new System.Drawing.Point(3, 23));
                wallsList.Add(new System.Drawing.Point(3, 25));
                wallsList.Add(new System.Drawing.Point(4, 4));
                wallsList.Add(new System.Drawing.Point(4, 12));
                wallsList.Add(new System.Drawing.Point(4, 13));
                wallsList.Add(new System.Drawing.Point(5, 2));
                wallsList.Add(new System.Drawing.Point(5, 4));
                wallsList.Add(new System.Drawing.Point(5, 16));
                wallsList.Add(new System.Drawing.Point(6, 2));
                wallsList.Add(new System.Drawing.Point(6, 3));
                wallsList.Add(new System.Drawing.Point(6, 4));
                wallsList.Add(new System.Drawing.Point(6, 21));
                wallsList.Add(new System.Drawing.Point(6, 22));
                wallsList.Add(new System.Drawing.Point(6, 23));
                wallsList.Add(new System.Drawing.Point(7, 2));
                wallsList.Add(new System.Drawing.Point(7, 3));
                wallsList.Add(new System.Drawing.Point(7, 16));
                wallsList.Add(new System.Drawing.Point(8, 3));
                wallsList.Add(new System.Drawing.Point(8, 16));
                wallsList.Add(new System.Drawing.Point(8, 23));
                wallsList.Add(new System.Drawing.Point(8, 24));
                wallsList.Add(new System.Drawing.Point(8, 25));
                wallsList.Add(new System.Drawing.Point(9, 16));
                wallsList.Add(new System.Drawing.Point(10, 8));
                wallsList.Add(new System.Drawing.Point(10, 16));
                wallsList.Add(new System.Drawing.Point(11, 8));
                wallsList.Add(new System.Drawing.Point(11, 9));
                wallsList.Add(new System.Drawing.Point(11, 16));
                cliffList.Add(new System.Drawing.Point(0, 0));
                cliffList.Add(new System.Drawing.Point(1, 16));
                cliffList.Add(new System.Drawing.Point(1, 17));
                cliffList.Add(new System.Drawing.Point(1, 8));
                cliffList.Add(new System.Drawing.Point(1, 9));
                cliffList.Add(new System.Drawing.Point(2, 9));
                cliffList.Add(new System.Drawing.Point(2, 12));
                cliffList.Add(new System.Drawing.Point(2, 16));
                cliffList.Add(new System.Drawing.Point(2, 17));
                cliffList.Add(new System.Drawing.Point(3, 12));
                cliffList.Add(new System.Drawing.Point(5, 17));
                cliffList.Add(new System.Drawing.Point(5, 18));
                cliffList.Add(new System.Drawing.Point(5, 19));
                cliffList.Add(new System.Drawing.Point(5, 20));
                cliffList.Add(new System.Drawing.Point(6, 9));
                cliffList.Add(new System.Drawing.Point(6, 20));
                cliffList.Add(new System.Drawing.Point(7, 8));
                cliffList.Add(new System.Drawing.Point(7, 9));
                cliffList.Add(new System.Drawing.Point(7, 10));
                cliffList.Add(new System.Drawing.Point(7, 17));
                cliffList.Add(new System.Drawing.Point(7, 18));
                cliffList.Add(new System.Drawing.Point(7, 20));
                cliffList.Add(new System.Drawing.Point(8, 6));
                cliffList.Add(new System.Drawing.Point(8, 7));
                cliffList.Add(new System.Drawing.Point(8, 8));
                cliffList.Add(new System.Drawing.Point(8, 9));
                cliffList.Add(new System.Drawing.Point(8, 20));
                cliffList.Add(new System.Drawing.Point(9, 7));
                cliffList.Add(new System.Drawing.Point(9, 8));
                cliffList.Add(new System.Drawing.Point(9, 9));
                cliffList.Add(new System.Drawing.Point(9, 18));
                cliffList.Add(new System.Drawing.Point(9, 19));
                cliffList.Add(new System.Drawing.Point(9, 20));
                cliffList.Add(new System.Drawing.Point(10, 13));
                cliffList.Add(new System.Drawing.Point(11, 13));
                for (int i = 0; i < qLearningMap.GetLength(0); i++)
                {
                    for (int k = 0; k < qLearningMap.GetLength(1); k++)
                    {
                        if(wallsList.Exists(a => a.X == i && a.Y == k)) //стена
                        {
                            qLearningMap[i, k] = new QLearningMap(true, false, wallReward);
                        }
                        else if(cliffList.Exists(a => a.X == i && a.Y == k)) //обрыв
                        {
                            qLearningMap[i, k] = new QLearningMap(false, true, cliffReward);
                        }
                        else //обычная клетка
                        {
                            qLearningMap[i, k] = new QLearningMap(false, false, ordinaryReward);
                        }
                        for (int u = 0; u < destinationPoints.Length; u++)
                        {
                            if (i == destinationPoints[u].X && k == destinationPoints[u].Y) //клетка назначения
                            {
                                qLearningMap[i, k] = new QLearningMap(false, false, destinationRewards[u]);
                                break;
                            }
                        }
                    }
                }
            }
            
            CreateQLearningCellsView(qLearningMap, destinationPoints);
            int[] fixedStates = new int[destinationPoints.Length + cliffList.Count + wallsList.Count];
            int h = 0;
            while(h < destinationPoints.Length)
            {
                fixedStates[h] = PointToState(destinationPoints[h], qLearningMap.GetLength(1));
                h++;
            }
            while(h < cliffList.Count + destinationPoints.Length)
            {
                fixedStates[h] = PointToState(cliffList[h - destinationPoints.Length], qLearningMap.GetLength(1));
                h++;
            }
            while(h < wallsList.Count + cliffList.Count + destinationPoints.Length)
            {
                fixedStates[h] = PointToState(wallsList[h - cliffList.Count - destinationPoints.Length], qLearningMap.GetLength(1));
                h++;
            }
            AForgeExtensions.MachineLearning.EpsilonGreedyExploration epsilonGreedyExploration = new AForgeExtensions.MachineLearning.EpsilonGreedyExploration(0.1);
            AForgeExtensions.MachineLearning.TabuSearchExploration tabuSearchExploration = new AForgeExtensions.MachineLearning.TabuSearchExploration(4, epsilonGreedyExploration);
            //AForgeExtensions.MachineLearning.QLearning qLearning = new AForgeExtensions.MachineLearning.QLearning(qLearningMap.GetLength(1) * qLearningMap.GetLength(0), 4, tabuSearchExploration, 0, 0.1, fixedStates, 0);
            AForgeExtensions.MachineLearning.QLearning qLearning = new AForgeExtensions.MachineLearning.QLearning(qLearningMap.GetLength(1) * qLearningMap.GetLength(0), 4, tabuSearchExploration);
            qLearning.DiscountFactor = 0.99;
            _qLearningMapProcessing = new QLearningMapProcessing(qLearningMap, startPoint, destinationPoints, qLearning);
            AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro.LeakyReLuActivationFunction(), 4, 12, 12, 3);
            //AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro.ReLuActivationFunction(), 4, 4, 3);
            AForge.Neuro.Learning.BackPropagationLearning backPropagationLearning = new AForge.Neuro.Learning.BackPropagationLearning(activationNetwork);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork, 100, 1000, -1, 1);
            geneticLearningTeacher.MutationRate = 0.75;
            AForgeExtensions.Neuro.MSELossFunction mSELossFunction = new AForgeExtensions.Neuro.MSELossFunction();
            List<double[]> inputs = new List<double[]> { new double[4] { 0.1, 1, 0.88, 0.2 }, new double[4] { 1, 0.5, 0.04, 0.6 }, new double[4] { 0.5, 0.3, 0.4, 0.01 } };
            List<double[]> desiredOutputs = new List<double[]> { new double[3] { 0.51515, -1, -0.707070 }, new double[3] { 0.3, -0.5, -0.05 }, new double[3] { 0.9, 0.5, -0.3 } };
            List<double[]> otputsBefore = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork, inputs);
            double lossBefore = mSELossFunction.Calculate(otputsBefore, desiredOutputs);
            double loss = geneticLearningTeacher.Run(inputs, desiredOutputs);
            List<double[]> otputsAfter = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork, inputs);
            double lossAfter = mSELossFunction.Calculate(otputsAfter, desiredOutputs);
            //отрисовываем все qvalues
            for (int x = 0; x < qLearningMap.GetLength(0); x++)
            {
                for(int y = 0; y < qLearningMap.GetLength(1); y++)
                {
                    UpdateQLearningCellView(new System.Drawing.Point(x, y));
                }
            }
        }
        private CancellationTokenSource _qLearningCancellationTokenSource;

        private void QLearningRun(CancellationToken token, bool isEnumeration)
        {
            while (!token.IsCancellationRequested)
            {
                System.Drawing.Point previousPoint = _qLearningMapProcessing.CurrentPoint;
                System.Drawing.Point nextPoint = isEnumeration ? _qLearningMapProcessing.MoveEnumeration() : _qLearningMapProcessing.Move();
                DispatcherInvoke((Action)(() => {
                    UpdateQLearningCellView(previousPoint);
                    UpdateQLearningSelectedCellView(nextPoint);
                }));
                Thread.Sleep(RealtimeDelay);
            }
        }

        public ICommand StartQlearning_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if(_qLearningMapProcessing == null)
                    {
                        CreateQLearningMapProcessing();
                    }
                    _qLearningCancellationTokenSource = new CancellationTokenSource();
                    CancellationToken token = _qLearningCancellationTokenSource.Token;
                    Task.Run(() => QLearningRun(token, false)); //запускаем в отдельном потоке чтобы форма обновлялась
                }, (obj) => true);
            }
        }
        public ICommand StartEnumerationQlearning_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    if(_qLearningMapProcessing == null)
                    {
                        CreateQLearningMapProcessing();
                    }
                    _qLearningCancellationTokenSource = new CancellationTokenSource();
                    CancellationToken token = _qLearningCancellationTokenSource.Token;
                    Task.Run(() => QLearningRun(token, true)); //запускаем в отдельном потоке чтобы форма обновлялась
                }, (obj) => true);
            }
        }
        public ICommand StopQlearning_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    _qLearningCancellationTokenSource.Cancel();
                }, (obj) => true);
            }
        }
        public ICommand CreateMapQlearning_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    CreateQLearningMapProcessing();
                }, (obj) => true);
            }
        }
        private double _previousEpsilon = 0;
        public ICommand EpsilonOnOffQlearning_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    _previousEpsilon = ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearningMapProcessing.QLearning.ExplorationPolicy).BasePolicy.SetEpsilon(_previousEpsilon);
                }, (obj) => true);
            }
        }












        private bool _isFirstTest = true;
        private NeuralNetworkUnit _neuralNetworkUnitTest;
        Matrix<float> _matrix3;
        Vector<float> _vector1;
        AForge.Neuro.ActivationNetwork network;
        AForge.Neuro.ActivationNetwork network2;
        AForge.Neuro.ActivationNetwork network3;
        double[] array1;
        public void testFunc()
        {
            int[] a = new int[3] { 10, 10, 3 };
            if (_isFirstTest)
            {
                _isFirstTest = false;
                //create neural network
                //network = new AForge.Neuro.ActivationNetwork(new AForge.Neuro.SigmoidFunction(), a[0], a[1], a[2], a[3]);
                //network = new AForge.Neuro.ActivationNetwork(new AForgeNeuroExtensions.ReLuActivationFunction(), a[0], a[1], a[2]);
                network = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro.ReLuActivationFunction(), a[0], a[1], a[2]);
                network2 = new AForge.Neuro.ActivationNetwork(new AForgeExtensions.Neuro.ReLuActivationFunction(), a[0], a[1], a[2]);
                //create teacher
                AForge.Neuro.Learning.BackPropagationLearning backPropagationLearning = new AForge.Neuro.Learning.BackPropagationLearning(network);
                // loop
                /*while (!needToStop)
                {
                    // run epoch of learning procedure
                    double error = backPropagationLearning.RunEpoch(input, output);
                    // check error value to see if we need to stop
                    // ...
                }*/






                _neuralNetworkUnitTest = NeuralNetworkUnit.CreateNeuralNetworkUnitRandomly(a);

                for(int i = 0; i < network.Layers.Length; i++)
                {
                    for(int k = 0; k < a[i + 1]; k++)
                    {
                        int neuonCount = network.Layers[i].Neurons[k].Weights.Length;
                        for(int u = 0; u < neuonCount; u++)
                        {
                            network.Layers[i].Neurons[k].Weights[u] = _neuralNetworkUnitTest.Weights[i][k, u];
                            network2.Layers[i].Neurons[k].Weights[u] = _neuralNetworkUnitTest.Weights[i][k, u];
                        }
                        ((AForge.Neuro.ActivationNeuron)network.Layers[i].Neurons[k]).Threshold = _neuralNetworkUnitTest.Biases[i][k];
                        ((AForge.Neuro.ActivationNeuron)network2.Layers[i].Neurons[k]).Threshold = _neuralNetworkUnitTest.Biases[i][k];
                    }
                }

                array1 = new double[a[1]];
                float[] array2 = new float[a[1]];
                for(int i = 0; i < a[1]; i++)
                {
                    array1[i] = Features.GetRandDouble(0, 1);
                    array2[i] = (float)array1[i];
                }
                _vector1 = Vector<float>.Build.Dense(array2);
            }

            int iteration = 100;
            
            Stopwatch stopwatch1 = new Stopwatch();
            stopwatch1.Start();
            for (int i = 0; i < iteration; i++)
            {
                _ = _neuralNetworkUnitTest.ForwardPropagation(_vector1);
            }
            stopwatch1.Stop();

            Stopwatch stopwatch4 = new Stopwatch();
            stopwatch4.Start();
            for (int i = 0; i < iteration; i++)
            {
                _ = network.Compute(array1);
            }
            stopwatch4.Stop();

            //LayersText = "stopwatch1=" + stopwatch1.ElapsedMilliseconds + "мс  stopwatch4 = " + stopwatch4.ElapsedMilliseconds + "мс";
            LayersText = "_neuralNetworkUnitTest.ForwardPropagation(_vector1)= " + Environment.NewLine + _neuralNetworkUnitTest.ForwardPropagation(_vector1);
            double[] rr = network.Compute(array1);
            LayersText += "network.Compute(array1)= ";
            for(int i = 0; i < rr.Length; i++)
            {
                LayersText += Environment.NewLine + "," + rr[i];
            }
            rr = network2.Compute(array1);
            LayersText += Environment.NewLine + "network2.Compute(array1)= ";
            for(int i = 0; i < rr.Length; i++)
            {
                LayersText += Environment.NewLine + "," + rr[i];
            }
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
