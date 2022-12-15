using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeMachineLearningExtensions
{
    class QLearningMapProcessing
    {
        public QLearningMapProcessing(QLearningMap[,] map, NeuralNetworkSnake.PointInt startPoint, NeuralNetworkSnake.PointInt destinationPoint, AForge.MachineLearning.QLearning qLearning, AForgeMachineLearningExtensions.QLearning myqLearning, bool isMyqLearning)
        {
            _map = map;
            _mapRowsCount = map.GetLength(0);
            _mapColumnCount = map.GetLength(1);
            _startPoint = startPoint;
            _currentPoint = startPoint;
            _destinationPoint = destinationPoint;
            _qLearning = qLearning;
            _myqLearning = myqLearning;
            _isMyqLearning = isMyqLearning;
        }
        private bool _isMyqLearning;
        private AForge.MachineLearning.QLearning _qLearning;
        public AForge.MachineLearning.QLearning QLearning { get { return _qLearning; } }
        private AForgeMachineLearningExtensions.QLearning _myqLearning;
        public AForgeMachineLearningExtensions.QLearning MyqLearning { get { return _myqLearning; } }
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
            //up
            if(_currentPoint.X == 0)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(0, 1);
            }
            else if(_map[_currentPoint.X - 1, _currentPoint.Y].IsWall)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(0, 1);
            }
            //right
            if (_currentPoint.Y == _mapColumnCount - 1)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(1, 1);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y + 1].IsWall)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(1, 1);
            }
            //left
            if (_currentPoint.Y == 0)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(2, 1);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y - 1].IsWall)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(2, 1);
            }
            //down
            if (_currentPoint.X == _mapRowsCount - 1)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(3, 1);
            }
            else if (_map[_currentPoint.X + 1, _currentPoint.Y].IsWall)
            {
                ((AForge.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy).SetTabuAction(3, 1);
            }

        }
        /// <summary>
        /// Возвращает точку, на которую перешел.
        /// </summary>
        public NeuralNetworkSnake.PointInt Move()
        {
            if(_map[_currentPoint.X, _currentPoint.Y].IsCliff || (_currentPoint.X == _destinationPoint.X && _currentPoint.Y == _destinationPoint.Y)) //если попали в обрыв или в точку назначения, переходим в начало
            {
                _currentPoint = _startPoint;
            }
            else
            {
                UpdateTabuActions();
                int previousState = PointIntToState(_currentPoint);
                int previousAction = _isMyqLearning ? _myqLearning.GetAction(previousState) : _qLearning.GetAction(previousState);
                int nextState = 0;
                if (previousAction == 0)
                {
                    nextState = PointIntToState(new NeuralNetworkSnake.PointInt(_currentPoint.X - 1, _currentPoint.Y)); //up
                }
                else if (previousAction == 1)
                {
                    nextState = PointIntToState(new NeuralNetworkSnake.PointInt(_currentPoint.X, _currentPoint.Y + 1)); //right
                }
                else if (previousAction == 2)
                {
                    nextState = PointIntToState(new NeuralNetworkSnake.PointInt(_currentPoint.X, _currentPoint.Y - 1)); //left
                }
                else if (previousAction == 3)
                {
                    nextState = PointIntToState(new NeuralNetworkSnake.PointInt(_currentPoint.X + 1, _currentPoint.Y)); //down
                }
                if (nextState == 0)
                {
                    int y = 0;
                }
                NeuralNetworkSnake.PointInt nextPoint = StateToPointInt(nextState);
                if (_isMyqLearning)
                {
                    _myqLearning.UpdateState(previousState, previousAction, Map[nextPoint.X, nextPoint.Y].Reward, nextState);
                }
                else
                {
                    _qLearning.UpdateState(previousState, previousAction, Map[nextPoint.X, nextPoint.Y].Reward, nextState);
                }
                _currentPoint = nextPoint;
            }
            return _currentPoint;
        }
    }
}
