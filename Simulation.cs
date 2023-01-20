using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace NeuralNetworkSnake
{
    class Simulation : ViewModelBase
    {
        public Simulation(AForgeExtensions.Neuro.Learning.GeneticLearningNoTeacher geneticLearningNoTeacher, int testsCount, int pauseMillisecDelay, int fixedDuration, int boardSize, int applesCount)
        {
            _isGeneticLearning = true;
            _geneticLearningNoTeacher = geneticLearningNoTeacher;
            _geneticLearningNoTeacher.SpawnInitialPopulation(_geneticLearningNoTeacher.Network);
            _testsCount = testsCount;
            _currentTestNumber = 1;
            PauseMillisecDelay = pauseMillisecDelay;
            FixedDuration = fixedDuration;
            BoardSize = boardSize;
            ApplesCount = applesCount;
            IsRunning = false;
            Age = 0;
            DispatcherInvoke((Action)(() => {
                ViewModel.getInstance().Age = Age;
            }));
            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearningNoTeacher.PopulationSize, BoardSize, ApplesCount);
            CreateSnakesInfo();
            DispatcherInvoke((Action)(() => {
                CreateGameBoardForRender();
                CreateGenerationLeaderBoard();
            }));
        }
        public Simulation(AForgeExtensions.Neuro.Learning.DeepQLearning deepQLearning, int pauseMillisecDelay, int fixedDuration, int boardSize, int applesCount)
        {
            _isGeneticLearning = false;
            _deepQLearning = deepQLearning;
            PauseMillisecDelay = pauseMillisecDelay;
            FixedDuration = fixedDuration;
            BoardSize = boardSize;
            ApplesCount = applesCount;
            IsRunning = false;
            Age = 0;
            _epsilonDeepQLearning = double.Parse(ViewModel.getInstance().EpsilonDeepQLearning);
            DispatcherInvoke((Action)(() => {
                ViewModel.getInstance().Age = Age;
            }));
            _gameBoardsGeneticLearning = CreateGameBoards(1, BoardSize, ApplesCount);
            DispatcherInvoke((Action)(() => {
                CreateGameBoardForRender();
            }));
        }
        private bool _isGeneticLearning;
        private Random _random = new Random();
        private AForgeExtensions.Neuro.Learning.GeneticLearningNoTeacher _geneticLearningNoTeacher;
        public AForgeExtensions.Neuro.Learning.GeneticLearningNoTeacher GeneticLearningNoTeacher { get { return _geneticLearningNoTeacher; } }
        private int _testsCount;
        private int _currentTestNumber;
        private SnakeInfo[] _snakesInfo;
        private int snakeInfoCurrentId = 0;
        private AForgeExtensions.Neuro.Learning.DeepQLearning _deepQLearning;
        public AForgeExtensions.Neuro.Learning.DeepQLearning DeepQLearning { get { return _deepQLearning; } }
        private GameBoard[] _gameBoardsGeneticLearning;
        private readonly object locker = new object();
        private bool _isRunning { get; set; } //выполняется ли симуляция
        public bool IsRunning //реализация потокобезопасного получения и установки свойства
        {
            get { lock (locker) { return _isRunning; } }
            set { lock (locker) { _isRunning = value; } }
        }
        private int _pauseMillisecDelay { get; set; }
        public int PauseMillisecDelay
        {
            get { lock (locker) { return _pauseMillisecDelay; } }
            set { lock (locker) { _pauseMillisecDelay = value; } }
        }
        private int _fixedDuration { get; set; }
        public int FixedDuration
        {
            get { lock (locker) { return _fixedDuration; } }
            set { lock (locker) { _fixedDuration = value; } }
        }
        private int _boardSize { get; set; }
        public int BoardSize
        {
            get { lock (locker) { return _boardSize; } }
            set { lock (locker) { _boardSize = value; } }
        }
        private int _applesCount { get; set; }
        public int ApplesCount
        {
            get { lock (locker) { return _applesCount; } }
            set { lock (locker) { _applesCount = value; } }
        }
        private double _epsilonDeepQLearning { get; set; }
        public double EpsilonDeepQLearning
        {
            get { lock (locker) { return _epsilonDeepQLearning; } }
            set { lock (locker) { _epsilonDeepQLearning = value; } }
        }
        private int Age = 0;
        public void SetMutationPercent(double mutationPercent)
        {
            //_geneticLearningNoTeacher.MutationProbability = mutationPercent;
        }
        private void CreateSnakesInfo()
        {
            _snakesInfo = new SnakeInfo[_gameBoardsGeneticLearning.Length];
            for(int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                snakeInfoCurrentId++;
                _snakesInfo[i] = new SnakeInfo { Id = snakeInfoCurrentId, EatenApples = 0, Score = 0, TotalRating = 0 };
            }
        }
        private void UpdateSnakesInfo()
        {
            for (int i = 0; i < _snakesInfo.Length; i++)
            {
                _snakesInfo[i].Score = _gameBoardsGeneticLearning[i].Score;
                _snakesInfo[i].EatenApples = _gameBoardsGeneticLearning[i].EatenApples;
            }
        }
        private GameBoard[] CreateGameBoards(int count, int boardSize, int applesCount)
        {
            GameBoard[] gameBoards = new GameBoard[count];
            for (int i = 0; i < count; i++)
            {
                gameBoards[i] = new GameBoard(boardSize, applesCount);
            }
            return gameBoards;
        }
        private void CreateGenerationLeaderBoard()
        {
            ViewModel viewModel = ViewModel.getInstance();
            viewModel.GenerationLeaderboard.Clear();
            for(int i = 0; i < _snakesInfo.Length; i++)
            {
                viewModel.GenerationLeaderboard.Add(new GenerationLeaderboardItem { Id = _snakesInfo[i].Id, TotalScore = _snakesInfo[i].TotalRating, Score = _snakesInfo[i].Score, EatenApples = _snakesInfo[i].EatenApples });
            }
        }
        private void UpdateGenerationLeaderBoard()
        {
            ViewModel viewModel = ViewModel.getInstance();
            for(int i = 0; i < viewModel.GenerationLeaderboard.Count; i++)
            {
                int index = Array.FindIndex(_snakesInfo, a => a.Id == viewModel.GenerationLeaderboard[i].Id);
                viewModel.GenerationLeaderboard[i].TotalScore = _snakesInfo[index].TotalRating;
                viewModel.GenerationLeaderboard[i].Score = _gameBoardsGeneticLearning[index].Score;
                viewModel.GenerationLeaderboard[i].EatenApples = _gameBoardsGeneticLearning[index].EatenApples;
            }
            viewModel.GenerationLeaderboard = new ObservableCollection<GenerationLeaderboardItem>(viewModel.GenerationLeaderboard.OrderByDescending(a => a.Score));
        }
        private void UpdateCurrentTestNumber() //обновляет номер текущего теста для представления
        {
            ViewModel viewModel = ViewModel.getInstance();
            viewModel.CurrentTestNumber = _currentTestNumber.ToString() + "/" + _testsCount.ToString();
        }
        private void CreateGameBoardForRender()
        {
            ViewModel viewModel = ViewModel.getInstance();
            viewModel.GameBoardsForRender.Clear();
            viewModel.CreateBoardCells(_gameBoardsGeneticLearning[0].BoardSize);
            for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                viewModel.GameBoardsForRender.Add(new GameBoardForRender(viewModel.BoardCells, new SnakesForRender(), new ApplesForRender()));
            }
        }
        private void UpdateGameBoardForRender()
        {
            ViewModel viewModel = ViewModel.getInstance();
            for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                if (!viewModel.GameBoardsForRender[i].SnakesForRender.IsGameOver)
                {
                    //viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Clear();
                    //viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Clear();
                    if (_gameBoardsGeneticLearning[i].GetIsGameOver())
                    {
                        viewModel.GameBoardsForRender[i].SnakesForRender.IsGameOver = true;
                    }
                    //положение змейки
                    if (_gameBoardsGeneticLearning[i].SnakeCoordinates.Count == viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Count)
                    {
                        int newTop = (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates.Last().Y * viewModel.BoardCellSize);
                        int newLeft = (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates.Last().X * viewModel.BoardCellSize);
                        viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.RemoveAt(0);
                        SnakeForRender snakeForRender = new SnakeForRender(newTop, newLeft, (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                        viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Add(snakeForRender);
                    }
                    else
                    {
                        while (_gameBoardsGeneticLearning[i].SnakeCoordinates.Count > viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Count)
                        {
                            int index = viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Count;
                            SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                            viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Add(snakeForRender);
                        }
                    }
                    //положение яблока
                    viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Clear();
                    while (_gameBoardsGeneticLearning[i].AppleCoordinates.Count > viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Count)
                    {
                        int index = viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Count;
                        SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                        viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Add(snakeForRender);
                    }
                    if (viewModel.GameBoardsForRender[i].SnakesForRender.IsGameOver)
                    {
                        viewModel.GameBoardsForRender[i].SnakesForRender.SnakesCoordinate.Clear();
                        viewModel.GameBoardsForRender[i].ApplesForRender.ApplesCoordinates.Clear();
                    }
                }
            }
        }
        private void SetGeneticLearningRating()
        {
            for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                _snakesInfo[i].TotalRating += _gameBoardsGeneticLearning[i].Score;
            }
        }
        private bool SimulateOneStepGeneticLearning()
        {
            bool isAllGameOver = true;
            for(int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                if (!_gameBoardsGeneticLearning[i].GetIsGameOver())
                {
                    isAllGameOver = false;
                    double[] inputs = _gameBoardsGeneticLearning[i].GetInputs();
                    double[] outputs = _geneticLearningNoTeacher.Population[i].Network.Compute(inputs);
                    int xOffset = 0;
                    int yOffset = 0;

                    int indexMaximum = Features.MaxIndex(outputs);
                    if (_gameBoardsGeneticLearning[i].IsSnakeGoUp())
                    {
                        if(indexMaximum == 0)
                        {
                            xOffset = -1;
                        }
                        if (indexMaximum == 1)
                        {
                            yOffset = -1;
                        }
                        if (indexMaximum == 2)
                        {
                            xOffset = 1;
                        }
                    }
                    if (_gameBoardsGeneticLearning[i].IsSnakeGoRight())
                    {
                        if(indexMaximum == 0)
                        {
                            yOffset = -1;
                        }
                        if (indexMaximum == 1)
                        {
                            xOffset = 1;
                        }
                        if (indexMaximum == 2)
                        {
                            yOffset = 1;
                        }
                    }
                    if (_gameBoardsGeneticLearning[i].IsSnakeGoDown())
                    {
                        if(indexMaximum == 0)
                        {
                            xOffset = 1;
                        }
                        if (indexMaximum == 1)
                        {
                            yOffset = 1;
                        }
                        if (indexMaximum == 2)
                        {
                            xOffset = -1;
                        }
                    }
                    if (_gameBoardsGeneticLearning[i].IsSnakeGoLeft())
                    {
                        if(indexMaximum == 0)
                        {
                            yOffset = 1;
                        }
                        if (indexMaximum == 1)
                        {
                            xOffset = -1;
                        }
                        if (indexMaximum == 2)
                        {
                            yOffset = -1;
                        }
                    }
                    _gameBoardsGeneticLearning[i].MoveSnake(xOffset, yOffset);
                }
            }
            return isAllGameOver;
        }
        private int _reward1StateActions = 0; //base
        private int _reward2StateActions = 0; //apple
        private int _reward3StateActions = 0; //gameOver
        private bool SimulateOneStepDeepQLearning()
        {
            bool isAllGameOver = true;
            if (!_gameBoardsGeneticLearning[0].GetIsGameOver())
            {
                isAllGameOver = false;
                double[] inputs = _gameBoardsGeneticLearning[0].GetInputs();
                double[] outputs = _deepQLearning.Network.Compute(inputs);
                int chosenAction = AForgeExtensions.Features.MaxIndex(outputs);
                if (_random.NextDouble() < _epsilonDeepQLearning)
                {
                    int randAction = _random.Next(0, 2);
                    if (chosenAction <= randAction)
                    {
                        randAction++;
                    }
                    chosenAction = randAction;
                }

                int xOffset = 0;
                int yOffset = 0;
                if (_gameBoardsGeneticLearning[0].IsSnakeGoUp())
                {
                    if (chosenAction == 0)
                    {
                        xOffset = -1;
                    }
                    if (chosenAction == 1)
                    {
                        yOffset = -1;
                    }
                    if (chosenAction == 2)
                    {
                        xOffset = 1;
                    }
                }
                if (_gameBoardsGeneticLearning[0].IsSnakeGoRight())
                {
                    if (chosenAction == 0)
                    {
                        yOffset = -1;
                    }
                    if (chosenAction == 1)
                    {
                        xOffset = 1;
                    }
                    if (chosenAction == 2)
                    {
                        yOffset = 1;
                    }
                }
                if (_gameBoardsGeneticLearning[0].IsSnakeGoDown())
                {
                    if (chosenAction == 0)
                    {
                        xOffset = 1;
                    }
                    if (chosenAction == 1)
                    {
                        yOffset = 1;
                    }
                    if (chosenAction == 2)
                    {
                        xOffset = -1;
                    }
                }
                if (_gameBoardsGeneticLearning[0].IsSnakeGoLeft())
                {
                    if (chosenAction == 0)
                    {
                        yOffset = 1;
                    }
                    if (chosenAction == 1)
                    {
                        xOffset = -1;
                    }
                    if (chosenAction == 2)
                    {
                        yOffset = -1;
                    }
                }

                bool isApple = false;
                bool isGameOver = false;
                double baseReward = 0;
                double appleReward = 1;
                double gameOverReward = -14;
                double reward = baseReward;
                int newX = _gameBoardsGeneticLearning[0].SnakeCoordinates[_gameBoardsGeneticLearning[0].SnakeCoordinates.Count - 1].X + xOffset;
                int newY = _gameBoardsGeneticLearning[0].SnakeCoordinates[_gameBoardsGeneticLearning[0].SnakeCoordinates.Count - 1].Y + yOffset;
                if(newX < _gameBoardsGeneticLearning[0].BoardCellsInfo.GetLength(0) && newX >= 0 && newY < _gameBoardsGeneticLearning[0].BoardCellsInfo.GetLength(1) && newY >= 0) //если не врезались в стенку
                {
                    if (_gameBoardsGeneticLearning[0].BoardCellsInfo[newX, newY].IsApple)
                    {
                        reward = appleReward;
                        isApple = true;
                    }
                }
                _gameBoardsGeneticLearning[0].MoveSnake(xOffset, yOffset);
                if (_gameBoardsGeneticLearning[0].GetIsGameOver())
                {
                    if(isApple == false) //если игра закончилась из-за того что змейка заполнила все поле, не нужно указывать за это отрицательную награду
                    {
                        reward = gameOverReward;
                        isGameOver = true;
                    }
                }
                double[] inputs2 = _gameBoardsGeneticLearning[0].GetInputs();
                int poolLength = 200;
                double minPart = 0.12;
                int reward1Reservation = (int)Math.Round(poolLength * minPart) - _reward1StateActions; //base
                reward1Reservation = reward1Reservation < 0 ? 0 : reward1Reservation;
                int reward2Reservation = (int)Math.Round(poolLength * minPart) - _reward2StateActions; //apple
                reward2Reservation = reward2Reservation < 0 ? 0 : reward2Reservation;
                int reward3Reservation = (int)Math.Round(poolLength * minPart) - _reward3StateActions; //gameOver
                reward3Reservation = reward3Reservation < 0 ? 0 : reward3Reservation;
                int freeCount = (poolLength - _deepQLearning.PoolStateActionUpdateLength) - (reward1Reservation + reward2Reservation + reward3Reservation);
                bool isAddStateAction = false;
                if(_random.NextDouble() < 0.1)
                {
                    isAddStateAction = true;
                }
                /*if (isApple)
                {
                    if(freeCount > 0 || reward2Reservation > 0)
                    {
                        isAddStateAction = true;
                        _reward2StateActions++;
                    }
                }
                else if (isGameOver)
                {
                    if (freeCount > 0 || reward3Reservation > 0)
                    {
                        isAddStateAction = true;
                        _reward3StateActions++;
                    }
                }
                else
                {
                    if (freeCount > 0 || reward1Reservation > 0)
                    {
                        isAddStateAction = true;
                        _reward1StateActions++;
                    }
                }*/
                if (isAddStateAction)
                {
                    _deepQLearning.AddStateActionUpdate(inputs, outputs, chosenAction, reward, !isGameOver/*!(isApple || isGameOver)*/, inputs2);
                    if (_deepQLearning.PoolStateActionUpdateLength == poolLength)
                    {
                        _deepQLearning.UpdateState();
                        _reward1StateActions = 0; //base
                        _reward2StateActions = 0; //apple
                        _reward3StateActions = 0; //gameOver
                        AForgeExtensions.Neuro.ActivationNetworkSerializeFormat activationNetworkSerializeFormat = AForgeExtensions.Features.ConvertActNetToSerializeFormat(_deepQLearning.Network);
                        string s = System.Text.Json.JsonSerializer.Serialize(activationNetworkSerializeFormat);
                        System.IO.File.WriteAllText("deepQLearningNetwork.json", s);
                    }
                }
            }
            return isAllGameOver;
        }
        public void RealTimeSimulate()
        {
            if (_isGeneticLearning)
            {
                DispatcherInvoke((Action)(() => {
                    UpdateCurrentTestNumber();
                    CreateGenerationLeaderBoard();
                }));
            }
            IsRunning = true;
            while (IsRunning)
            {
                if (_isGeneticLearning)
                {
                    if (SimulateOneStepGeneticLearning())
                    {
                        _currentTestNumber++; //указываем что перешли на следующий раунд тестов нейросетей
                        SetGeneticLearningRating();
                        if (_currentTestNumber > _testsCount) //если выполнили все тесты для данных нейросетей, генерируем новое поколение
                        {
                            _currentTestNumber = 1;

                            AForgeExtensions.Neuro.ActivationNetworkSerializeFormat activationNetworkSerializeFormat = AForgeExtensions.Features.ConvertActNetToSerializeFormat(_geneticLearningNoTeacher.BestChromosome.Network);
                            string s = System.Text.Json.JsonSerializer.Serialize(activationNetworkSerializeFormat);
                            System.IO.File.WriteAllText("bestChromosome.json", s);

                            double[] populationFitness = new double[_geneticLearningNoTeacher.PopulationSize];
                            for(int i = 0; i < _geneticLearningNoTeacher.PopulationSize; i++)
                            {
                                populationFitness[i] = _snakesInfo[i].TotalRating / _testsCount;
                            }
                            _geneticLearningNoTeacher.Run(populationFitness);
                            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearningNoTeacher.PopulationSize, BoardSize, ApplesCount);
                            CreateSnakesInfo();
                            Age++;
                            DispatcherInvoke((Action)(() => {
                                ViewModel.getInstance().Age = Age;
                            }));
                        }
                        else //иначе создаем новые поля для нейронных сетей
                        {
                            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearningNoTeacher.PopulationSize, BoardSize, ApplesCount);
                        }
                        DispatcherInvoke((Action)(() => {
                            CreateGameBoardForRender();
                            CreateGenerationLeaderBoard();
                            UpdateCurrentTestNumber();
                        }));
                    }
                    else
                    {
                        DispatcherInvoke((Action)(() => {
                            UpdateGenerationLeaderBoard();
                        }));
                    }
                    UpdateSnakesInfo();
                    /*for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++) //вывод inputs
                    {
                        if (!_gameBoardsGeneticLearning[i].GetIsGameOver())
                        {
                            if (ViewModel.getInstance().IsRealtimeSimulation)
                            {
                                DispatcherInvoke((Action)(() => {
                                    ViewModel.getInstance().UpdateInputsInfo(_gameBoardsGeneticLearning[i].GetInputs());
                                }));
                                break;
                            }
                        }
                    }*/
                    DispatcherInvoke((Action)(() => {
                        UpdateGameBoardForRender();
                    }));
                }
                else
                {
                    if (SimulateOneStepDeepQLearning())
                    {
                        _gameBoardsGeneticLearning = CreateGameBoards(1, BoardSize, ApplesCount);
                        Age++;
                        DispatcherInvoke((Action)(() => {
                            ViewModel.getInstance().Age = Age;
                        }));
                        DispatcherInvoke((Action)(() => {
                            CreateGameBoardForRender();
                        }));
                    }
                    DispatcherInvoke((Action)(() => {
                        UpdateGameBoardForRender();
                    }));
                }
                Thread.Sleep(PauseMillisecDelay);
            }
        }
        public void FixedTimeSimulate()
        {
            if (_isGeneticLearning)
            {
                DispatcherInvoke((Action)(() => {
                    UpdateCurrentTestNumber();
                }));
            }
            IsRunning = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long lastRemaining = FixedDuration - stopwatch.ElapsedMilliseconds / 1000;
            while (IsRunning)
            {
                if (_isGeneticLearning)
                {
                    if (SimulateOneStepGeneticLearning())
                    {
                        _currentTestNumber++; //указываем что перешли на следующий раунд тестов нейросетей
                        SetGeneticLearningRating();
                        if (_currentTestNumber > _testsCount) //если выполнили все тесты для данных нейросетей, генерируем новое поколение
                        {
                            _currentTestNumber = 1;

                            AForgeExtensions.Neuro.ActivationNetworkSerializeFormat activationNetworkSerializeFormat = AForgeExtensions.Features.ConvertActNetToSerializeFormat(_geneticLearningNoTeacher.BestChromosome.Network);
                            string s = System.Text.Json.JsonSerializer.Serialize(activationNetworkSerializeFormat);
                            System.IO.File.WriteAllText("bestChromosome.json", s);

                            double[] populationFitness = new double[_geneticLearningNoTeacher.PopulationSize];
                            for (int i = 0; i < _geneticLearningNoTeacher.PopulationSize; i++)
                            {
                                populationFitness[i] = _snakesInfo[i].TotalRating / _testsCount;
                            }
                            _geneticLearningNoTeacher.Run(populationFitness);
                            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearningNoTeacher.PopulationSize, BoardSize, ApplesCount);
                            CreateSnakesInfo();
                            Age++;
                            DispatcherInvoke((Action)(() => {
                                ViewModel.getInstance().Age = Age;
                            }));
                        }
                        else //иначе создаем новые поля для нейронных сетей
                        {
                            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearningNoTeacher.PopulationSize, BoardSize, ApplesCount);
                        }
                        DispatcherInvoke((Action)(() => {
                            UpdateCurrentTestNumber();
                        }));
                    }
                    UpdateSnakesInfo();
                }
                else
                {
                    if (SimulateOneStepDeepQLearning())
                    {
                        _gameBoardsGeneticLearning = CreateGameBoards(1, BoardSize, ApplesCount);
                        Age++;
                        DispatcherInvoke((Action)(() => {
                            ViewModel.getInstance().Age = Age;
                        }));
                    }
                }

                if (FixedDuration - stopwatch.ElapsedMilliseconds / 1000 != lastRemaining)
                {
                    lastRemaining = FixedDuration - stopwatch.ElapsedMilliseconds / 1000;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().RemainingTime = lastRemaining;
                    }));
                }
                if (lastRemaining <= 0)
                {
                    IsRunning = false;
                }
            }
        }
    }
}
