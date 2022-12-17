using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class QLearningMapProcessing
    {
        public QLearningMapProcessing(QLearningMap[,] map, System.Drawing.Point startPoint, System.Drawing.Point[] destinationPoints, AForgeExtensions.MachineLearning.QLearning qLearning)
        {
            _map = map;
            _mapRowsCount = map.GetLength(0);
            _mapColumnCount = map.GetLength(1);
            _startPoint = startPoint;
            _currentPoint = startPoint;
            _destinationPoints = destinationPoints;
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
        private System.Drawing.Point _startPoint;
        public System.Drawing.Point StartPoint { get { return _startPoint; } }
        private System.Drawing.Point _currentPoint;
        public System.Drawing.Point CurrentPoint { get { return _currentPoint; } }
        private System.Drawing.Point[] _destinationPoints;
        public System.Drawing.Point[] DestinationPoint { get { return _destinationPoints; } }
        private int PointToState(System.Drawing.Point point)
        {
            return _mapColumnCount * point.X + point.Y;
        }
        private System.Drawing.Point StateToPoint(int state)
        {
            return new System.Drawing.Point((int)Math.Truncate((double)state / _mapColumnCount), state % _mapColumnCount);
        }
        /// <summary>
        /// Устанавливает в табу действия, невозможные для текущего положения на карте, на 1 ход
        /// </summary>
        private void UpdateTabuActions()
        {
            AForgeExtensions.MachineLearning.TabuSearchExploration tabu = (AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy;
            tabu.ResetTabuList();
            int time = 2;
            //up
            if (_currentPoint.X == 0)
            {
                tabu.SetTabuAction(0, time);
            }
            else if (_map[_currentPoint.X - 1, _currentPoint.Y].IsWall)
            {
                tabu.SetTabuAction(0, time);
            }
            //right
            if (_currentPoint.Y == _mapColumnCount - 1)
            {
                tabu.SetTabuAction(1, time);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y + 1].IsWall)
            {
                tabu.SetTabuAction(1, time);
            }
            //left
            if (_currentPoint.Y == 0)
            {
                tabu.SetTabuAction(2, time);
            }
            else if (_map[_currentPoint.X, _currentPoint.Y - 1].IsWall)
            {
                tabu.SetTabuAction(2, time);
            }
            //down
            if (_currentPoint.X == _mapRowsCount - 1)
            {
                tabu.SetTabuAction(3, time);
            }
            else if (_map[_currentPoint.X + 1, _currentPoint.Y].IsWall)
            {
                tabu.SetTabuAction(3, time);
            }

        }
        private int GetNextState(System.Drawing.Point previousPoint, int action)
        {
            int nextState = 0;
            if (action == 0)
            {
                nextState = PointToState(new System.Drawing.Point(previousPoint.X - 1, previousPoint.Y)); //up
            }
            else if (action == 1)
            {
                nextState = PointToState(new System.Drawing.Point(previousPoint.X, previousPoint.Y + 1)); //right
            }
            else if (action == 2)
            {
                nextState = PointToState(new System.Drawing.Point(previousPoint.X, previousPoint.Y - 1)); //left
            }
            else if (action == 3)
            {
                nextState = PointToState(new System.Drawing.Point(previousPoint.X + 1, previousPoint.Y)); //down
            }
            return nextState;
        }
        private bool IsDestinationPoint(System.Drawing.Point point)
        {
            for(int i = 0; i < _destinationPoints.Length; i++)
            {
                if(_destinationPoints[i].X == point.X && _destinationPoints[i].Y == point.Y)
                {
                    return true;
                }
            }
            return false;
        }
        private int _nextAction = -1;
        /// <summary>
        /// Возвращает точку, на которую перешел. Перебирает все действия для всех состояний
        /// </summary>
        public System.Drawing.Point MoveEnumeration()
        {
            while (_map[_currentPoint.X, _currentPoint.Y].IsCliff || _map[_currentPoint.X, _currentPoint.Y].IsWall || IsDestinationPoint(_currentPoint))
            {
                _currentPoint.Y++;
                if (_currentPoint.Y >= _mapColumnCount)
                {
                    _currentPoint.X++;
                    _currentPoint.Y = 0;
                    if (_currentPoint.X >= _mapRowsCount)
                    {
                        _currentPoint.X = 0;
                    }
                }
            }
            UpdateTabuActions();
            _nextAction++;
            System.Drawing.Point nextPoint = _currentPoint;
            if (_nextAction < _qLearning.ActionsCount)
            {
                AForgeExtensions.MachineLearning.TabuSearchExploration tabu = (AForgeExtensions.MachineLearning.TabuSearchExploration)_qLearning.ExplorationPolicy;
                if (tabu.TabuActions[_nextAction] == 0)
                {
                    int previousState = PointToState(_currentPoint);
                    int previousAction = _nextAction;
                    int nextState = GetNextState(_currentPoint, previousAction);
                    nextPoint = StateToPoint(nextState);
                    System.Drawing.Point savePoint = _currentPoint;
                    _currentPoint = nextPoint;
                    UpdateTabuActions();
                    _qLearning.UpdateState(previousState, previousAction, Map[nextPoint.X, nextPoint.Y].Reward, nextState);
                    _currentPoint = savePoint;
                }
            }
            else
            {
                _nextAction = -1;
                _currentPoint.Y++;
                if (_currentPoint.Y >= _mapColumnCount)
                {
                    _currentPoint.X++;
                    _currentPoint.Y = 0;
                    if (_currentPoint.X >= _mapRowsCount)
                    {
                        _currentPoint.X = 0;
                    }
                }
            }
            
            return nextPoint;
        }
        /// <summary>
        /// Возвращает точку, на которую перешел
        /// </summary>
        public System.Drawing.Point Move()
        {
            if (_map[_currentPoint.X, _currentPoint.Y].IsCliff || IsDestinationPoint(_currentPoint)) //если попали в обрыв или в точку назначения, переходим в начало
            {
                _currentPoint = _startPoint;
                UpdateTabuActions();
                return _currentPoint;
            }
            int previousState = PointToState(_currentPoint);
            int previousAction = _qLearning.GetAction(previousState);
            int nextState = GetNextState(_currentPoint, previousAction);
            System.Drawing.Point nextPoint = StateToPoint(nextState);
            _currentPoint = nextPoint;
            UpdateTabuActions();
            _qLearning.UpdateState(previousState, previousAction, Map[nextPoint.X, nextPoint.Y].Reward, nextState);
            return _currentPoint;
        }
    }
}
