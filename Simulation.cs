using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

namespace NeuralNetworkSnake
{
    class Simulation : ViewModelBase
    {
        public Simulation(GeneticLearning geneticLearning, int pauseMillisecDelay, int boardSize, int applesCount)
        {
            _geneticLearning = geneticLearning;
            PauseMillisecDelay = pauseMillisecDelay;
            BoardSize = boardSize;
            ApplesCount = applesCount;
            IsRunning = false;
            Age = 0;
            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.PopulationSize, BoardSize, ApplesCount);
        }
        private GeneticLearning _geneticLearning;
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
        private int Age = 0;
        public void SetMutationPercent(double mutationPercent)
        {
            _geneticLearning.MutationPercent = mutationPercent;
        }
        public void SetPopulationSize(int size)
        {
            _geneticLearning.PopulationSize = size;
        }
        private GameBoard[] CreateGameBoards(int count, int boardSize, int applesCount)
        {
            GameBoard[] gameBoards = new GameBoard[count];
            for (int i = 0; i < count; i++)
            {
                gameBoards[i] = new GameBoard(boardSize, applesCount);
                DispatcherInvoke((Action)(() => {
                    ViewModel.getInstance().SnakesForRenders.Add(new SnakesForRender());
                    ViewModel.getInstance().ApplesForRenders.Add(new ApplesForRender());
                }));
            }
            return gameBoards;
        }
        private void UpdateViewModelSnakeAppleCoordinates()
        {
            ViewModel viewModel = ViewModel.getInstance();
            DispatcherInvoke((Action)(() => {
                for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
                {
                    while(_gameBoardsGeneticLearning[i].SnakeCoordinates.Count > viewModel.SnakesForRenders[i].SnakesCoordinate.Count)
                    {
                        int index = viewModel.SnakesForRenders[i].SnakesCoordinate.Count;
                        SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                        viewModel.SnakesForRenders[i].SnakesCoordinate.Add(snakeForRender);
                    }
                    while (_gameBoardsGeneticLearning[i].AppleCoordinates.Count > viewModel.ApplesForRenders[i].ApplesCoordinates.Count)
                    {
                        int index = viewModel.ApplesForRenders[i].ApplesCoordinates.Count;
                        SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                        viewModel.ApplesForRenders[i].ApplesCoordinates.Add(snakeForRender);
                    }
                }
            }));
        }
        private void SetGeneticLearningRating()
        {
            for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                _geneticLearning.Population[i].Rating = _gameBoardsGeneticLearning[i].Score;
            }
        }
        private bool SimulateOneStep()
        {
            bool isAllGameOver = true;
            for(int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                if (!_gameBoardsGeneticLearning[i].GetIsGameOver())
                {
                    isAllGameOver = false;
                    Vector<float> outputs = _geneticLearning.Population[i].NeuralNetworkUnit.ForwardPropagation(_gameBoardsGeneticLearning[i].GetInputs());
                    int xOffset = 0;
                    int yOffset = 0;
                    int indexMaximum = outputs.AbsoluteMaximumIndex();
                    if (_gameBoardsGeneticLearning[i].IsSnakeGoUp())
                    {
                        if(indexMaximum == 0)
                        {
                            xOffset = -1;
                        }
                        if (indexMaximum == 1)
                        {
                            yOffset = 1;
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
                            yOffset = 1;
                        }
                        if (indexMaximum == 1)
                        {
                            xOffset = 1;
                        }
                        if (indexMaximum == 2)
                        {
                            yOffset = -1;
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
                            yOffset = -1;
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
                            yOffset = -1;
                        }
                        if (indexMaximum == 1)
                        {
                            xOffset = -1;
                        }
                        if (indexMaximum == 2)
                        {
                            yOffset = 1;
                        }
                    }
                    _gameBoardsGeneticLearning[i].MoveSnake(xOffset, yOffset);
                }
            }
            return isAllGameOver;
        }
        public void RealTimeSimulate()
        {
            IsRunning = true;
            while (IsRunning)
            {
                if (SimulateOneStep())
                {
                    SetGeneticLearningRating();
                    _geneticLearning.SpawnNewGeneration();
                    _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.PopulationSize, BoardSize, ApplesCount);
                    Age++;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().Age = Age;
                    }));
                }
                UpdateViewModelSnakeAppleCoordinates();
                Thread.Sleep(PauseMillisecDelay);
            }
        }
        public void FixedTimeSimulate()
        {
            IsRunning = true;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            long lastRemaining = stopwatch.ElapsedMilliseconds / 1000;
            while (IsRunning)
            {
                if (SimulateOneStep())
                {
                    SetGeneticLearningRating();
                    _geneticLearning.SpawnNewGeneration();
                    _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.PopulationSize, BoardSize, ApplesCount);
                    Age++;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().Age = Age;
                    }));
                }
                if(stopwatch.ElapsedMilliseconds / 1000 != lastRemaining)
                {
                    lastRemaining = stopwatch.ElapsedMilliseconds / 1000;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().RemainingTime = lastRemaining;
                    }));
                }
                if(lastRemaining >= FixedDuration)
                {
                    IsRunning = false;
                }
            }
        }
    }
}
