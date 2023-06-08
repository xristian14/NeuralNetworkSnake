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
using System.IO;
using LiveCharts;
using LiveCharts.Wpf;
using CNTK;

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
        private bool _isFixedtimeSimulation = true;
        public bool IsFixedtimeSimulation
        {
            get { return _isFixedtimeSimulation; }
            set
            {
                _isFixedtimeSimulation = value;
                OnPropertyChanged();
            }
        }
        private int _realtimeDelay = 50;
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
        private int _fixedDuration = 43200;
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
        private string _populationSize = "50";
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
                    }
                }
                OnPropertyChanged();
            }
        }
        private string _mutationPercent = "0,01";
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
        private string _fitnessScaleRate = "0";
        public string FitnessScaleRate
        {
            get { return _fitnessScaleRate; }
            set
            {
                if (double.TryParse(value, out double res))
                {
                    _fitnessScaleRate = value;
                }
                OnPropertyChanged();
            }
        }
        private string _testsCount = "3";
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
        private bool _isLoadSavedNetwork = false;
        public bool IsLoadSavedNetwork
        {
            get { return _isLoadSavedNetwork; }
            set
            {
                _isLoadSavedNetwork = value;
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
                    if (HiddenLayersCount >= 4)
                    {
                        layers[4] = int.Parse(FourthHiddenLayerCountNeurons);
                    }
                    if (HiddenLayersCount >= 5)
                    {
                        layers[5] = int.Parse(FifthHiddenLayerCountNeurons);
                    }
                    layers[layers.Length - 1] = 3;

                    if (_isGeneticLearning)
                    {
                        int[] layers2 = new int[layers.Length - 1];
                        for (int i = 0; i < layers2.Length; i++)
                        {
                            layers2[i] = layers[i + 1];
                        }

                        AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro./*Leaky*/ReLuActivationFunction(), layers[0], layers2);
                        if (IsLoadSavedNetwork)
                        {
                            string s = System.IO.File.ReadAllText("bestChromosome.json");
                            activationNetwork = AForgeExtensions.Features.ConvertSerializeFormatToActNet(System.Text.Json.JsonSerializer.Deserialize<AForgeExtensions.Neuro.ActivationNetworkSerializeFormat>(s));
                            layers = new int[activationNetwork.Layers.Length + 1];
                            layers[0] = activationNetwork.Layers[0].InputsCount;
                            for (int k = 0; k < activationNetwork.Layers.Length; k++)
                            {
                                layers[k + 1] = activationNetwork.Layers[k].Neurons.Length;
                            }
                        }
                        
                        int populationSize = int.Parse(PopulationSize);
                        AForgeExtensions.Neuro.Learning.GeneticLearningNoTeacher geneticLearningNoTeacher = new AForgeExtensions.Neuro.Learning.GeneticLearningNoTeacher(activationNetwork, populationSize, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(true, true), -2, 2, new List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings>() { new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(50, double.Parse(MutationPercent), double.Parse(FitnessScaleRate)) });
                        _simulation = new Simulation(geneticLearningNoTeacher, int.Parse(TestsCount), RealtimeDelay, FixedDuration, int.Parse(BoardSize), int.Parse(ApplesCount));
                        
                    }
                    else
                    {
                        int[] layers2 = new int[layers.Length - 1];
                        for(int i = 0; i < layers2.Length; i++)
                        {
                            layers2[i] = layers[i + 1];
                        }
                        AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-0.1f, 0.1f, new AForgeExtensions.Neuro.LeakyReLuActivationFunction(), layers[0], layers2);
                        if (IsLoadSavedNetwork)
                        {
                            string s = System.IO.File.ReadAllText("deepQLearningNetwork.json");
                            activationNetwork = AForgeExtensions.Features.ConvertSerializeFormatToActNet(System.Text.Json.JsonSerializer.Deserialize<AForgeExtensions.Neuro.ActivationNetworkSerializeFormat>(s));
                            layers = new int[activationNetwork.Layers.Length + 1];
                            layers[0] = activationNetwork.Layers[0].InputsCount;
                            for (int k = 0; k < activationNetwork.Layers.Length; k++)
                            {
                                layers[k + 1] = activationNetwork.Layers[k].Neurons.Length;
                            }
                        }
                        AForgeExtensions.Neuro.Learning.DeepQLearning deepQLearning = new AForgeExtensions.Neuro.Learning.DeepQLearning(activationNetwork);
                        deepQLearning.GeneticLearningTeacher.MutateMinValue = -2;
                        deepQLearning.GeneticLearningTeacher.MutateMaxValue = 2;
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
                    if (!_isFixedtimeSimulation)
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
            /*AForge.Neuro.ActivationNetwork activationNetwork0 = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-2f, 2f, new AForgeExtensions.Neuro.LeakyReLuActivationFunction(), 4, 64, 64, 3);
            AForge.Neuro.ActivationNetwork activationNetwork1 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork2 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork3 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork4 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork5 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork6 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork7 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            AForge.Neuro.ActivationNetwork activationNetwork8 = AForgeExtensions.Neuro.ActivationNetworkFeatures.CloneActivationNetwork(activationNetwork0);
            //AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro.ReLuActivationFunction(), 4, 4, 3);
            AForge.Neuro.Learning.BackPropagationLearning backPropagationLearning = new AForge.Neuro.Learning.BackPropagationLearning(activationNetwork0);

            List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings> stepsSettings0 = new List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings>() { new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.0025, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.00125, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000625, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000313, 0.5) };
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher0_0 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork0, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings0);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher0_1 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork1, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings0);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher0_2 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork2, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings0);
            geneticLearningTeacher0_0.SelectionMethod.FlexibleTargetSourceLength = 3;
            geneticLearningTeacher0_1.SelectionMethod.FlexibleTargetSourceLength = 3;
            geneticLearningTeacher0_2.SelectionMethod.FlexibleTargetSourceLength = 3;

            List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings> stepsSettings1 = new List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings>() { new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.0025, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.00125, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000625, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000313, 0.5) };
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher1_0 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork3, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings1);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher1_1 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork4, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings1);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher1_2 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork5, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings1);
            geneticLearningTeacher1_0.SelectionMethod.FlexibleTargetSourceLength = 4;
            geneticLearningTeacher1_1.SelectionMethod.FlexibleTargetSourceLength = 4;
            geneticLearningTeacher1_2.SelectionMethod.FlexibleTargetSourceLength = 4;

            List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings> stepsSettings2 = new List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings>() { new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.0025, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.00125, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000625, 0.5), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(150, 0.000313, 0.5) };
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher2_0 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork6, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings2);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher2_1 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork7, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings2);
            AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher2_2 = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork8, 100, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, stepsSettings2);
            geneticLearningTeacher2_0.SelectionMethod.FlexibleTargetSourceLength = 5;
            geneticLearningTeacher2_1.SelectionMethod.FlexibleTargetSourceLength = 5;
            geneticLearningTeacher2_2.SelectionMethod.FlexibleTargetSourceLength = 5;

            AForgeExtensions.Neuro.MSELossFunction mSELossFunction = new AForgeExtensions.Neuro.MSELossFunction();
            List<double[]> inputs = new List<double[]> { new double[4] { 0.1, 1, 0.88, 0.2 }, new double[4] { 1, 0.5, 0.04, 0.6 }, new double[4] { 0.5, 0.3, 0.4, 0.01 } };
            List<double[]> desiredOutputs = new List<double[]> { new double[3] { 0.51515, -1, -0.707070 }, new double[3] { 0.3, -0.5, -0.05 }, new double[3] { 0.9, 0.5, -0.3 } };
            List<double[]> otputsBefore = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork0, inputs);
            List<double[]> otputsBefore2 = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork3, inputs);
            List<double[]> otputsBefore3 = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork6, inputs);
            double lossBefore = mSELossFunction.Calculate(otputsBefore, desiredOutputs);

            geneticLearningTeacher0_0.Run(inputs, desiredOutputs);
            geneticLearningTeacher0_1.Run(inputs, desiredOutputs);
            geneticLearningTeacher0_2.Run(inputs, desiredOutputs);

            geneticLearningTeacher1_0.Run(inputs, desiredOutputs);
            geneticLearningTeacher1_1.Run(inputs, desiredOutputs);
            geneticLearningTeacher1_2.Run(inputs, desiredOutputs);

            geneticLearningTeacher2_0.Run(inputs, desiredOutputs);
            geneticLearningTeacher2_1.Run(inputs, desiredOutputs);
            geneticLearningTeacher2_2.Run(inputs, desiredOutputs);

            List<double[]> otputsAfter = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork0, inputs);
            List<double[]> otputsAfter2 = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork3, inputs);
            List<double[]> otputsAfter3 = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork6, inputs);
            double lossAfter = mSELossFunction.Calculate(otputsAfter, desiredOutputs);

            string[] minProgressionStr = geneticLearningTeacher1_0.MinFitnessProgression.Select(a => a.ToString()).ToArray();
            File.WriteAllLines("savedMinProgression.txt", minProgressionStr);

            string[] targetHistoryStr = geneticLearningTeacher1_0.SelectionMethod.TargetHistory.Select(a => a.ToString()).ToArray();
            File.WriteAllLines("savedTargetHistory.txt", targetHistoryStr);

            int inputLength = 616;
            AForge.Neuro.ActivationNetwork activationNetworkLarge = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForgeExtensions.Neuro.ReLuActivationFunction(), inputLength, 128, 128, 128, 3);
            
            double[] inputLarge = new double[inputLength];
            for (int i = 0; i < inputLarge.Length; i++)
            {
                inputLarge[i] = Features.GetRandDouble(0, 1);
            }
            Stopwatch stopwatch = new Stopwatch();
            double[] outputLarge = new double[0];
            stopwatch.Start();
            for (int i = 0; i < 17520; i++)
            {
                outputLarge = activationNetworkLarge.Compute(inputLarge);
            }
            stopwatch.Stop();


            //string s = System.Text.Json.JsonSerializer.Serialize(activationNetworkLarge);
            string sOutput = Newtonsoft.Json.JsonConvert.SerializeObject(activationNetworkLarge);
            File.WriteAllText("file1.json", sOutput);
            string sInput = File.ReadAllText("file1.json");
            AForge.Neuro.ActivationNetwork activationNetworkLarge2 = Newtonsoft.Json.JsonConvert.DeserializeObject<AForge.Neuro.ActivationNetwork>(sInput);
            double[] outputLarge2 = activationNetworkLarge2.Compute(inputLarge);*/
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












        
        private ChartValues<double> _chartValues1;
        public ChartValues<double> ChartValues1
        {
            get { return _chartValues1; }
            set
            {
                _chartValues1 = value;
                OnPropertyChanged();
            }
        }
        private ChartValues<double> _chartValues2;
        public ChartValues<double> ChartValues2
        {
            get { return _chartValues2; }
            set
            {
                _chartValues2 = value;
                OnPropertyChanged();
            }
        }

        public ICommand ButtonTest_Click
        {
            get
            {
                return new DelegateCommand((obj) =>
                {
                    /*AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-2f, 2f, new AForgeExtensions.Neuro.LeakyReLuActivationFunction(), 4, 64, 64, 3);

                    AForgeExtensions.Neuro.Learning.GeneticLearningTeacher geneticLearningTeacher = new AForgeExtensions.Neuro.Learning.GeneticLearningTeacher(activationNetwork, 5, new AForgeExtensions.Neuro.MSELossFunction(), new AForgeExtensions.Neuro.Learning.GeneticLearning.RouletteWheelSelection(false, true), -2, 2, new List<AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings>() { new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(100, 0.0025, 0), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(100, 0.00125, 0), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(100, 0.000625, 0), new AForgeExtensions.Neuro.Learning.GeneticLearning.StepsSettings(100, 0.000313, 0) });

                    List<double[]> inputs = new List<double[]> { new double[4] { 0.1, 1, 0.88, 0.2 }, new double[4] { 1, 0.5, 0.04, 0.6 }, new double[4] { 0.5, 0.3, 0.4, 0.01 } };
                    List<double[]> desiredOutputs = new List<double[]> { new double[3] { 0.51515, -1, -0.707070 }, new double[3] { 0.3, -0.5, -0.05 }, new double[3] { 0.9, 0.5, -0.3 } };
                    List<double[]> otputsBefore = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork, inputs);
                    geneticLearningTeacher.Run(inputs, desiredOutputs);
                    List<double[]> otputsAfter = AForgeExtensions.Neuro.ActivationNetworkFeatures.ActivationNetworkCompute(activationNetwork, inputs);
                    ChartValues1 = new ChartValues<double>(_simulation.GeneticLearningNoTeacher.MaxFitnessProgression);
                    ChartValues2 = new ChartValues<double>(_simulation.GeneticLearningNoTeacher.SelectionMethod.TargetHistory);*/

                    /*ChartValues1 = new ChartValues<double>(_simulation.GeneticLearningNoTeacher.MaxFitnessProgression);
                    ChartValues2 = new ChartValues<double>(_simulation.GeneticLearningNoTeacher.SelectionMethod.TargetHistory);*/



                    TestFunc();



                    int iterations = 10000;
                    // initialize input and output values
                    double[][] input = new double[4][] {
                        new double[] {0, 0}, new double[] {0, 1},
                        new double[] {1, 0}, new double[] {1, 1}
                    };
                    double[][] output = new double[4][] {
                        new double[] {0}, new double[] {1},
                        new double[] {1}, new double[] {0}
                    };
                    // create neural network
                    AForge.Neuro.ActivationNetwork activationNetwork = AForgeExtensions.Neuro.ActivationNetworkFeatures.BuildRandom(-1f, 1f, new AForge.Neuro.SigmoidFunction(), 2, 2, 1);
                    AForge.Neuro.Learning.BackPropagationLearning teacher = new AForge.Neuro.Learning.BackPropagationLearning(activationNetwork);
                    double[] aforgeLosses = new double[iterations];
                    for(int i = 0; i < iterations; i++)
                    {
                        aforgeLosses[i] = teacher.RunEpoch(input, output);
                    }
                    double[] o0 = activationNetwork.Compute(input[0]);
                    double[] o1 = activationNetwork.Compute(input[1]);
                    double[] o2 = activationNetwork.Compute(input[2]);
                    double[] o3 = activationNetwork.Compute(input[3]);


                    int inputDim = 2;
                    int hiddenDim = 8;
                    int numOutputClasses = 1;
                    DeviceDescriptor device = DeviceDescriptor.CPUDevice;
                    Variable inputVariable1 = Variable.InputVariable(new int[] { inputDim }, DataType.Float);
                    Variable outputVariable2 = Variable.InputVariable(new int[] { numOutputClasses }, DataType.Float);

                    var weightParam1 = new Parameter(new int[] { hiddenDim, inputDim }, DataType.Float, 1, device, "w");
                    var biasParam1 = new Parameter(new int[] { hiddenDim }, DataType.Float, 0, device, "b");
                    var classifierOutput0 = CNTKLib.Sigmoid(CNTKLib.Plus(CNTKLib.Times(weightParam1, inputVariable1), biasParam1));

                    var weightParam2 = new Parameter(new int[] { numOutputClasses, hiddenDim }, DataType.Float, 1, device, "ww");
                    var biasParam2 = new Parameter(new int[] { numOutputClasses }, DataType.Float, 0, device, "bb");
                    //var classifierOutput1 = CNTKLib.Sigmoid(CNTKLib.Times(weightParam2, classifierOutput0) + biasParam2);
                    var classifierOutput1 = CNTKLib.Sigmoid(CNTKLib.Plus(CNTKLib.Times(weightParam2, classifierOutput0), biasParam2));



                    //var loss = CNTKLib.CrossEntropyWithSoftmax(classifierOutput1, outputVariable2);
                    var loss = CNTKLib.BinaryCrossEntropy(classifierOutput1, outputVariable2);
                    var evalError = CNTKLib.ClassificationError(classifierOutput1, outputVariable2);

                    // prepare for training
                    CNTK.TrainingParameterScheduleDouble learningRatePerSample = new CNTK.TrainingParameterScheduleDouble(0.01, 1);
                    IList<Learner> parameterLearners = new List<Learner>() { Learner.SGDLearner(classifierOutput1.Parameters(), learningRatePerSample) };
                    var trainer = Trainer.CreateTrainer(classifierOutput1, loss, loss, parameterLearners);

                    float[] inputValuesArr = new float[input.Length * input[0].Length];
                    for (int i = 0; i < input.Length; i++)
                    {
                        for(int k = 0; k < input[i].Length; k++)
                        {
                            inputValuesArr[i * input[i].Length + k] = (float)input[i][k];
                        }
                    }
                    float[] outputValuesArr = new float[output.Length * output[0].Length];
                    for (int i = 0; i < output.Length; i++)
                    {
                        for (int k = 0; k < output[i].Length; k++)
                        {
                            outputValuesArr[i * output[i].Length + k] = (float)output[i][k];
                        }
                    }
                    Value inputValues= Value.CreateBatch<float>(new int[] { inputDim }, inputValuesArr, device);
                    Value outputValues = Value.CreateBatch<float>(new int[] { numOutputClasses }, outputValuesArr, device);

                    // train the model
                    for (int minibatchCount = 0; minibatchCount < iterations; minibatchCount++)
                    {
                        //TODO: sweepEnd should be set properly instead of false.
#pragma warning disable 618
                        trainer.TrainMinibatch(new Dictionary<Variable, Value>() { { inputVariable1, inputValues }, { outputVariable2, outputValues } }, device);
#pragma warning restore 618
                        //TestHelper.PrintTrainingProgress(trainer, minibatchCount, updatePerMinibatches);
                    }

                    var inputDataMap = new Dictionary<Variable, Value>() { { inputVariable1, inputValues } };
                    var outputDataMap = new Dictionary<Variable, Value>() { { classifierOutput1.Output, null } };
                    classifierOutput1.Evaluate(inputDataMap, outputDataMap, device);
                    var outputValue = outputDataMap[classifierOutput1.Output];
                    IList<IList<float>> actualLabelSoftMax = outputValue.GetDenseData<float>(classifierOutput1.Output);









                    NDShape nDShape = new NDShape(1, 4);
                    Function logisticModel = CreateLogisticModel(Variable.InputVariable(nDShape, DataType.Float), 4);
                    LogisticRegression.TrainAndEvaluate(DeviceDescriptor.CPUDevice);




                    
                }, (obj) => true);
            }
        }
        private Function CreateLogisticModel(Variable input, int numOutputClasses)
        {
            Parameter bias = new Parameter(new int[] { numOutputClasses }, DataType.Float, 0);
            Parameter weights = new Parameter(new int[] { input.Shape[0], numOutputClasses }, DataType.Float,
              CNTKLib.GlorotUniformInitializer(
                CNTKLib.DefaultParamInitScale,
                CNTKLib.SentinelValueForInferParamInitRank,
                CNTKLib.SentinelValueForInferParamInitRank, 1));
            Function z = CNTKLib.Plus(bias, CNTKLib.Times(weights, input));
            Function logisticClassifier = CNTKLib.Sigmoid(z, "LogisticClassifier");
            return logisticClassifier;
        }
        private void TestFunc()
        {
            var inputs = new[] { new[] { 0.0f, 0.0f }, new[] { 1.0f, 0.0f }, new[] { 0.0f, 1.0f }, new[] { 1.0f, 1.0f } };
            var expected = new[] { new[] { 0.0f }, new[] { 1.0f }, new[] { 1.0f }, new[] { 0.0f } };

            var device = DeviceDescriptor.CPUDevice;

            const int inputDimensions = 2;
            const int hiddenDimensions = 2;
            const int outputDimensions = 1;

            double initMin = -0.15;
            double initMax = 0.15;
            double normalMean = 0.0;
            double standartDev = 0.25;
            Random random = new Random();
            uint seed = (uint)random.Next(1, 10000);

            var inputVariable = Variable.InputVariable(new[] { inputDimensions }, DataType.Float);
            var outputVariable = Variable.InputVariable(new[] { outputDimensions }, DataType.Float);

            //var hiddenWeights = new Parameter(NDArrayView.RandomUniform<float>(new[] { hiddenDimensions, inputDimensions }, initMin, initMax, 1, device));
            //var hiddenBias = new Parameter(NDArrayView.RandomUniform<float>(new[] { hiddenDimensions }, initMin, initMax, 1, device));
            var hiddenWeights = new Parameter(NDArrayView.RandomNormal<float>(new[] { hiddenDimensions, inputDimensions }, normalMean, standartDev, seed++, device));
            var hiddenBias = new Parameter(NDArrayView.RandomNormal<float>(new[] { hiddenDimensions }, normalMean, standartDev, seed++, device));
            var hidden = CNTKLib.Sigmoid(CNTKLib.Plus(hiddenBias, CNTKLib.Times(hiddenWeights, inputVariable)));

            //var outWeights = new Parameter(NDArrayView.RandomUniform<float>(new[] { outputDimensions, hiddenDimensions }, initMin, initMax, 1, device)); 
            //var outBias = new Parameter(NDArrayView.RandomUniform<float>(new[] { outputDimensions }, initMin, initMax, 1, device));
            var outWeights = new Parameter(NDArrayView.RandomNormal<float>(new[] { outputDimensions, hiddenDimensions }, normalMean, standartDev, seed++, device));
            var outBias = new Parameter(NDArrayView.RandomNormal<float>(new[] { outputDimensions }, normalMean, standartDev, seed++, device));
            var prediction = CNTKLib.Sigmoid(CNTKLib.Plus(outBias, CNTKLib.Times(outWeights, hidden)));



            NDArrayView hiddenWeightsArrayViewBefore = hiddenWeights.Value();
            Value hiddenWeightValueBefore = new Value(hiddenWeightsArrayViewBefore);
            IList<IList<float>> hiddenWeightDataBefore = hiddenWeightValueBefore.GetDenseData<float>(hiddenWeights);

            NDArrayView hiddenBiasArrayViewBefore = hiddenBias.Value();
            Value hiddenBiasValueBefore = new Value(hiddenBiasArrayViewBefore);
            IList<IList<float>> hiddenBiasDataBefore = hiddenBiasValueBefore.GetDenseData<float>(hiddenBias);

            NDArrayView outWeightsArrayViewBefore = outWeights.Value();
            Value outWeightsValueBefore = new Value(outWeightsArrayViewBefore);
            IList<IList<float>> outWeightsDataBefore = outWeightsValueBefore.GetDenseData<float>(outWeights);

            NDArrayView outBiasArrayViewBefore = outBias.Value();
            Value outBiasValueBefore = new Value(outBiasArrayViewBefore);
            IList<IList<float>> outBiasDataBefore = outBiasValueBefore.GetDenseData<float>(outBias);


            var trainingLoss = CNTKLib.BinaryCrossEntropy(prediction, outputVariable);

            /*var schedule = new TrainingParameterScheduleDouble(0.05);
            var momentum = CNTKLib.MomentumAsTimeConstantSchedule(new DoubleVector(new[] { 1.0, 10.0, 100.0 }));
            var learner = CNTKLib.AdamLearner(new ParameterVector(prediction.Parameters().Select(o => o).ToList()), schedule, momentum);
            var trainer = CNTKLib.CreateTrainer(prediction, trainingLoss, trainingLoss, new LearnerVector(new[] { learner }));*/

            TrainingParameterScheduleDouble learningRatePerSample = new TrainingParameterScheduleDouble(0.01, 1);
            IList<Learner> parameterLearners = new List<Learner>() { Learner.SGDLearner(prediction.Parameters(), learningRatePerSample) };
            var trainer = Trainer.CreateTrainer(prediction, trainingLoss, trainingLoss, parameterLearners);

            const int minibatchSize = 40;

            var features = new float[minibatchSize][];
            var labels = new float[minibatchSize][];

            const int numMinibatchesToTrain = 4000;
            var trainIndex = 0;

            for (var i = 0; i < numMinibatchesToTrain; ++i)
            {
                for (var j = 0; j < minibatchSize; ++j)
                {
                    features[j] = inputs[trainIndex];
                    labels[j] = expected[trainIndex];
                    trainIndex = (trainIndex + 1) % inputs.Length;
                }
                var batchInput = Value.Create(new[] { inputDimensions }, features.Select(o => new NDArrayView(new[] { inputDimensions }, o, device)), device);
                var batchLabels = Value.Create(new[] { outputDimensions }, labels.Select(o => new NDArrayView(new[] { outputDimensions }, o, device)), device);
                var minibatchBindings = new Dictionary<Variable, Value>
                            {
                                {inputVariable, batchInput},
                                {outputVariable, batchLabels}
                            };

                trainer.TrainMinibatch(minibatchBindings, true, device);
            }

            float[] inputValuesArr = new float[inputs.Length * inputs[0].Length];
            for (int i = 0; i < inputs.Length; i++)
            {
                for (int k = 0; k < inputs[i].Length; k++)
                {
                    inputValuesArr[i * inputs[i].Length + k] = inputs[i][k];
                }
            }
            Value inputValues = Value.CreateBatch<float>(new int[] { inputDimensions }, inputValuesArr, device);
            var inputDataMap = new Dictionary<Variable, Value>() { { inputVariable, inputValues } };
            var outputDataMap = new Dictionary<Variable, Value>() { { prediction.Output, null } };
            prediction.Evaluate(inputDataMap, outputDataMap, device);
            var outputValue = outputDataMap[prediction.Output];
            IList<IList<float>> actualLabelSoftMax = outputValue.GetDenseData<float>(prediction.Output);


            NDArrayView hiddenWeightsArrayViewAfter = hiddenWeights.Value();
            Value hiddenWeightValueAfter = new Value(hiddenWeightsArrayViewAfter);
            IList<IList<float>> hiddenWeightDataAfter = hiddenWeightValueAfter.GetDenseData<float>(hiddenWeights);

            NDArrayView hiddenBiasArrayViewAfter = hiddenBias.Value();
            Value hiddenBiasValueAfter = new Value(hiddenBiasArrayViewAfter);
            IList<IList<float>> hiddenBiasDataAfter = hiddenBiasValueAfter.GetDenseData<float>(hiddenBias);

            NDArrayView outWeightsArrayViewAfter = outWeights.Value();
            Value outWeightsValueAfter = new Value(outWeightsArrayViewAfter);
            IList<IList<float>> outWeightsDataAfter = outWeightsValueAfter.GetDenseData<float>(outWeights);

            NDArrayView outBiasArrayViewAfter = outBias.Value();
            Value outBiasValueAfter = new Value(outBiasArrayViewAfter);
            IList<IList<float>> outBiasDataAfter = outBiasValueAfter.GetDenseData<float>(outBias);
            int y = 0;
        }
    }
}
