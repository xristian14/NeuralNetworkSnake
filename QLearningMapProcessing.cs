using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class QLearningMapProcessing
    {
        public QLearningMapProcessing(QLearningMap[,] map, NeuralNetworkSnake.PointInt startPoint, NeuralNetworkSnake.PointInt destinationPoint, AForgeExtensions.MachineLearning.QLearning qLearning)
        {
            _map = map;
            _mapRowsCount = map.GetLength(0);
            _mapColumnCount = map.GetLength(1);
            _startPoint = startPoint;
            _currentPoint = startPoint;
            _destinationPoint = destinationPoint;
            _qLearning = qLearning;
            UpdateTabuActions();
        }
        private AForgeExtensions.MachineLearning.QLearning _qLearning;
        public AForgeExtensions.MachineLearning.QLearning QLearning { get { return _qLearning; } }
        private int _mapRowsCount;
        public int MapRowsCoun { get { return _mapRowsCount; } }
        private int _mapColumnCount;
        public int MapColumnCount { get { return _mapColumnCount; } }
        private QLearningMap[,] _map;
        public QLearningMap[,] Map { get { return _map; } }
        private NeuralNetworkSnake.PointInt _startPoint;
        public NeuralNetworkSnake.PointInt StartPoint { get { return _startPoint; } }
        private NeuralNetworkSnake.PointInt _currentPoint;
        public NeuralNetworkSnake.PointInt CurrentPoint { get { return _currentPoint; } }
        private NeuralNetworkSnake.PointInt _destinationPoint;
        public NeuralNetworkSnake.PointInt DestinationPoint { get { return _destinationPoint; } }
        private int PointIntToState(NeuralNetworkSnake.PointInt pointInt)
        {
            return _mapColumnCount * pointInt.X + pointInt.Y;
        }
        private NeuralNetworkSnake.PointInt StateToPointInt(int state)
        {
            return new NeuralNetworkSnake.PointInt((int)Math.Truncate((double)state / _mapColumnCount), state % _mapColumnCount);
        }
        /// <summary>
        /// Устанавливает в табу действия, невозможные для текущего положения на карте, на 1 ход
        /// </summary>
        private void UpdateTabuActions()
        {
            ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).ResetTabuList();
            int time = 2;
            //up
            if (_currentPoint.X == 0)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(0, time);
            }
            else if (_map[_currentPoint.X - 1, _currentPoint.Y].IsWall)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(0, time);
            }
            //right
            if (_currentPoint.Y == _mapColumnCount - 1)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(1, time);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y + 1].IsWall)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(1, time);
            }
            //left
            if (_currentPoint.Y == 0)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(2, time);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y - 1].IsWall)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(2, time);
            }
            //down
            if (_currentPoint.X == _mapRowsCount - 1)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(3, time);
            }
            else if (_map[_currentPoint.X + 1, _currentPoint.Y].IsWall)
            {
                ((AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(3, time);
            }

        }
        private int GetNextState(NeuralNetworkSnake.PointInt previousPoint, int action)
        {
            int nextState = 0;
            if (action == 0)
            {
                nextState = PointIntToState(new NeuralNetworkSnake.PointInt(previousPoint.X - 1, previousPoint.Y)); //up
            }
            else if (action == 1)
            {
                nextState = PointIntToState(new NeuralNetworkSnake.PointInt(previousPoint.X, previousPoint.Y + 1)); //right
            }
            else if (action == 2)
            {
                nextState = PointIntToState(new NeuralNetworkSnake.PointInt(previousPoint.X, previousPoint.Y - 1)); //left
            }
            else if (action == 3)
            {
                nextState = PointIntToState(new NeuralNetworkSnake.PointInt(previousPoint.X + 1, previousPoint.Y)); //down
            }
            return nextState;
        }
        /// <summary>
        /// Возвращает точку, на которую перешел.
        /// </summary>
        public NeuralNetworkSnake.PointInt Move()
        {
            if (_map[_currentPoint.X, _currentPoint.Y].IsCliff || (_currentPoint.X == _destinationPoint.X && _currentPoint.Y == _destinationPoint.Y)) //если попали в обрыв или в точку назначения, переходим в начало
            {
                _currentPoint = _startPoint;
                UpdateTabuActions();
            }
            else
            {
                int previousState = PointIntToState(_currentPoint);
                int previousAction = _qLearning.GetAction(previousState);
                int nextState = GetNextState(_currentPoint, previousAction);
                NeuralNetworkSnake.PointInt nextPoint = StateToPointInt(nextState);
                _currentPoint = nextPoint;
                UpdateTabuActions();
                _qLearning.UpdateState(previousState, previousAction, Map[nextPoint.X, nextPoint.Y].Reward, nextState);
            }
            return _currentPoint;
        }
    }
}
