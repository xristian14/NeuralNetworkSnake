﻿using System;
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
        public Simulation(GeneticLearning geneticLearning, int pauseMillisecDelay, int fixedDuration, int boardSize, int applesCount)
        {
            _geneticLearning = geneticLearning;
            PauseMillisecDelay = pauseMillisecDelay;
            FixedDuration = fixedDuration;
            BoardSize = boardSize;
            ApplesCount = applesCount;
            IsRunning = false;
            Age = 0;
            DispatcherInvoke((Action)(() => {
                ViewModel.getInstance().Age = Age;
            }));
            _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.GetPopulationSize(), BoardSize, ApplesCount);
            DispatcherInvoke((Action)(() => {
                CreateLeaderBoard();
            }));
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
            _geneticLearning.SetMutationPercent(mutationPercent);
        }
        public void SetPopulationSize(int size)
        {
            _geneticLearning.SetNewPopulationSize(size);
        }
        private GameBoard[] CreateGameBoards(int count, int boardSize, int applesCount)
        {
            GameBoard[] gameBoards = new GameBoard[count];
            for (int i = 0; i < count; i++)
            {
                gameBoards[i] = new GameBoard(boardSize, applesCount);
            }
            DispatcherInvoke((Action)(() => {
                int countGameBoard = count;
                ViewModel viewModel = ViewModel.getInstance();
                viewModel.SnakesForRenders.Clear();
                viewModel.ApplesForRenders.Clear();
                for (int i = 0; i < countGameBoard; i++)
                {
                    viewModel.SnakesForRenders.Add(new SnakesForRender());
                    viewModel.ApplesForRenders.Add(new ApplesForRender());
                }
            }));
            return gameBoards;
        }
        private void CreateLeaderBoard()
        {
            ViewModel viewModel = ViewModel.getInstance();
            viewModel.Leaderboard.Clear();
            for(int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                viewModel.Leaderboard.Add(new LeaderboardItem { Score = _gameBoardsGeneticLearning[i].Score, EatenApples = _gameBoardsGeneticLearning[i].EatenApples, LostMoves = _gameBoardsGeneticLearning[i].MaxStepsWithoutApples - _gameBoardsGeneticLearning[i].StepsWithoutApples });
            }
        }
        private void UpdateLeaderBoard()
        {
            ViewModel viewModel = ViewModel.getInstance();
            for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
            {
                viewModel.Leaderboard[i].Score = _gameBoardsGeneticLearning[i].Score;
                viewModel.Leaderboard[i].EatenApples = _gameBoardsGeneticLearning[i].EatenApples;
                viewModel.Leaderboard[i].LostMoves = _gameBoardsGeneticLearning[i].MaxStepsWithoutApples - _gameBoardsGeneticLearning[i].StepsWithoutApples;
            }
            viewModel.Leaderboard = new ObservableCollection<LeaderboardItem>(viewModel.Leaderboard.OrderByDescending(a => a.Score));
        }
        private void UpdateViewModelSnakeAppleCoordinates()
        {
            DispatcherInvoke((Action)(() => {
                ViewModel viewModel = ViewModel.getInstance();
                int u = 0;
                for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
                {
                    if (!viewModel.SnakesForRenders[i].IsGameOver)
                    {
                        if (_gameBoardsGeneticLearning[i].GetIsGameOver())
                        {
                            viewModel.SnakesForRenders[i].IsGameOver = true;
                        }
                        //положение змейки
                        if(_gameBoardsGeneticLearning[i].SnakeCoordinates.Count == viewModel.SnakesForRenders[i].SnakesCoordinate.Count)
                        {
                            int newTop = (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates.Last().Y * viewModel.BoardCellSize);
                            int newLeft = (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates.Last().X * viewModel.BoardCellSize);
                            viewModel.SnakesForRenders[i].SnakesCoordinate.RemoveAt(0);
                            SnakeForRender snakeForRender = new SnakeForRender(newTop, newLeft, (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                            viewModel.SnakesForRenders[i].SnakesCoordinate.Add(snakeForRender);
                        }
                        else
                        {
                            while (_gameBoardsGeneticLearning[i].SnakeCoordinates.Count > viewModel.SnakesForRenders[i].SnakesCoordinate.Count)
                            {
                                int index = viewModel.SnakesForRenders[i].SnakesCoordinate.Count;
                                SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].SnakeCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                                viewModel.SnakesForRenders[i].SnakesCoordinate.Add(snakeForRender);
                            }
                        }
                        //положение яблока
                        viewModel.ApplesForRenders[i].ApplesCoordinates.Clear();
                        while (_gameBoardsGeneticLearning[i].AppleCoordinates.Count > viewModel.ApplesForRenders[i].ApplesCoordinates.Count)
                        {
                            int index = viewModel.ApplesForRenders[i].ApplesCoordinates.Count;
                            SnakeForRender snakeForRender = new SnakeForRender((int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].Y * viewModel.BoardCellSize), (int)(_gameBoardsGeneticLearning[i].AppleCoordinates[index].X * viewModel.BoardCellSize), (int)viewModel.BoardCellSize, (int)viewModel.BoardCellSize);
                            viewModel.ApplesForRenders[i].ApplesCoordinates.Add(snakeForRender);
                        }
                        if (viewModel.SnakesForRenders[i].IsGameOver)
                        {
                            viewModel.SnakesForRenders[i].SnakesCoordinate.Clear();
                            viewModel.ApplesForRenders[i].ApplesCoordinates.Clear();
                        }
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
                    Vector<float> inputs = _gameBoardsGeneticLearning[i].GetInputs();
                    Vector<float> outputs = _geneticLearning.Population[i].NeuralNetworkUnit.ForwardPropagation(inputs);
                    int xOffset = 0;
                    int yOffset = 0;

                    int indexMaximum = outputs.MaximumIndex();
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
        public void RealTimeSimulate()
        {
            IsRunning = true;
            while (IsRunning)
            {
                double maxRating = 0;
                for(int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
                {
                    if(_gameBoardsGeneticLearning[i].Score > maxRating && _gameBoardsGeneticLearning[i].EatenApples == 0)
                    {
                        maxRating = _gameBoardsGeneticLearning[i].Score;
                    }
                }
                if(maxRating > 0.050)
                {
                    int y = 0;
                }
                if (SimulateOneStep())
                {
                    SetGeneticLearningRating();
                    _geneticLearning.SpawnNewGeneration();
                    _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.GetPopulationSize(), BoardSize, ApplesCount);
                    Age++;
                    DispatcherInvoke((Action)(() => {
                        CreateLeaderBoard();
                        ViewModel.getInstance().Age = Age;
                    }));
                }
                else
                {
                    DispatcherInvoke((Action)(() => {
                        UpdateLeaderBoard();
                    }));
                }



                for (int i = 0; i < _gameBoardsGeneticLearning.Length; i++)
                {
                    if (!_gameBoardsGeneticLearning[i].GetIsGameOver())
                    {
                        if (ViewModel.getInstance().IsRealtimeSimulation) //вывод inputs
                        {
                            DispatcherInvoke((Action)(() => {
                                ViewModel.getInstance().UpdateInputsInfo(_gameBoardsGeneticLearning[i].GetInputs());
                            }));
                            break;
                        }
                    }
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
            long lastRemaining = FixedDuration - stopwatch.ElapsedMilliseconds / 1000;
            while (IsRunning)
            {
                if (SimulateOneStep())
                {
                    SetGeneticLearningRating();
                    _geneticLearning.SpawnNewGeneration();
                    _gameBoardsGeneticLearning = CreateGameBoards(_geneticLearning.GetPopulationSize(), BoardSize, ApplesCount);
                    Age++;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().Age = Age;
                    }));
                }
                if(FixedDuration - stopwatch.ElapsedMilliseconds / 1000 != lastRemaining)
                {
                    lastRemaining = FixedDuration - stopwatch.ElapsedMilliseconds / 1000;
                    DispatcherInvoke((Action)(() => {
                        ViewModel.getInstance().RemainingTime = lastRemaining;
                    }));
                }
                if(lastRemaining <= 0)
                {
                    IsRunning = false;
                }
            }
        }
    }
}
