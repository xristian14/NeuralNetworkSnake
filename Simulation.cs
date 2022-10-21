﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class Simulation
    {
        public Simulation(GeneticLearning geneticLearning,GameBoard[] gameBoardsGeneticLearning, int pauseMillisecDelay)
        {
            _geneticLearning = geneticLearning;
            _gameBoardsGeneticLearning = gameBoardsGeneticLearning;
            PauseMillisecDelay = pauseMillisecDelay;
            IsComplete = false;
        }
        private GeneticLearning _geneticLearning;
        private GameBoard[] _gameBoardsGeneticLearning;
        private readonly object locker = new object();
        private bool _isComplete { get; set; } //завершена ли симуляция
        public bool IsComplete //реализация потокобезопасного получения и установки свойства
        {
            get { lock (locker) { return _isComplete; } }
            set { lock (locker) { _isComplete = value; } }
        }
        private int _pauseMillisecDelay { get; set; }
        public int PauseMillisecDelay
        {
            get { lock (locker) { return _pauseMillisecDelay; } }
            set { lock (locker) { _pauseMillisecDelay = value; } }
        }
        private void Simulate()
        {

        }
        public void RealTimeSimulate(Simulation simulation)
        {

        }
        public void FixedTimeSimulate(Simulation simulation, double secDuration)
        {

        }
    }
}
