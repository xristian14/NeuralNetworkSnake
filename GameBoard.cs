using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MathNet.Numerics.LinearAlgebra;

namespace NeuralNetworkSnake
{
    class GameBoard : ViewModelBase
    {
        public GameBoard(int boardSize, int appleCount)
        {
            BoardCellsInfo = new BoardCellInfo[boardSize, boardSize];
            for(int i = 0; i < boardSize; i++)
            {
                for (int k = 0; k < boardSize; k++)
                {
                    BoardCellsInfo[i, k] = new BoardCellInfo();
                }
            }
            BoardSize = boardSize;
            CellsCount = boardSize * boardSize;
            AppleCount = appleCount;
            //генерируем начальную позицию змейки
            
            SnakeCoordinates.Add(new SnakeCoordinate(_random.Next(1, boardSize - 1), _random.Next(1, boardSize - 1))); //начальная координата

            int nextX = SnakeCoordinates[0].X;
            int nextY = SnakeCoordinates[0].Y;
            BoardCellsInfo[nextX, nextY].IsSnake = true;
            int direction = _random.Next(0, 3);
            switch (direction) //направление от начальной координаты
            {
                case 0: //направо
                    nextX++;
                    break;
                case 1: //вниз
                    nextY++;
                    break;
                case 2: //налево
                    nextX--;
                    break;
                case 3: //вверх
                    nextY--;
                    break;
            }
            SnakeCoordinates.Add(new SnakeCoordinate(nextX, nextY));
            AddSnakeHistories(SnakeCoordinates);
            BoardCellsInfo[nextX, nextY].IsSnake = true;
            //генерируем яблоки
            for(int i = 0; i < appleCount; i++)
            {
                AppleGenerate();
            }
            Score = _baseOneStepScore;
            _appleScoreReduceMaxSteps = (int)Math.Round((double)boardSize * boardSize / 3);
            _maxStepsWithoutApples = (int)Math.Round((double)boardSize * boardSize);
            _nextOneStepScore = _baseOneStepScore;
            _nextAppleScore = _baseAppleScore;
        }
        Random _random = new Random();
        private double _baseOneStepScore = 0.005; //базовая стоимость шага змейки
        private double _oneStepScoreMultiply = 0; //множитель, на который увеличивается стоимость одного шага змейки, при съедении яблока
        private double _nextOneStepScore = 0; //стоимость следующего шага змейки
        private List<SnakeHistory> _snakeHistories = new List<SnakeHistory>(); //история позиций змейки. Нужно чтобы при повторении позиции, указать что игра окончена, т.к. змейка зациклилась
        private int _stepsWithoutApples = 0; //количество шагов, с момента съедения последнего яблока
        public int StepsWithoutApples
        {
            get { return _stepsWithoutApples; }
        }
        private int _maxStepsWithoutApples = 0; //максимально допустимое количество шагов, с момента съедения последнего яблока
        public int MaxStepsWithoutApples
        {
            get { return _maxStepsWithoutApples; }
        }
        private double _baseAppleScore = 1; //базовая стоимость съеденного яблока
        private double _appleScoreMultiply = 0.1; //множитель, на который увеличивается стоимость следующего яблока
        private double _nextAppleScore = 0; //стоимость следующего съеденного яблока
        private int _appleScoreReduceMaxSteps = 0; //количество шагов, на котором стоимость яблока будет максимально уменьшена
        private double _appleScoreReduce = 0; //величина от стоимости яблока, на которое уменьшится его стоимость при количестве шагов _appleScoreReduceMaxSteps
        private double _score;
        public double Score
        {
            get { return _score; }
            private set { _score = value; }
        }
        private int _eatenApples = 0;
        public int EatenApples
        {
            get { return _eatenApples; }
            private set { _eatenApples = value; }
        }
        private bool _isGameOver = false;
        private int _boardSize;
        public int BoardSize
        {
            get { return _boardSize; }
            private set { _boardSize = value; }
        }
        private int CellsCount;
        private int AppleCount;
        public BoardCellInfo[,] BoardCellsInfo;
        private ObservableCollection<SnakeCoordinate> _snakeCoordinates = new ObservableCollection<SnakeCoordinate>();
        public ObservableCollection<SnakeCoordinate> SnakeCoordinates
        {
            get { return _snakeCoordinates; }
            set
            {
                _snakeCoordinates = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<AppleCoordinate> _appleCoordinates = new ObservableCollection<AppleCoordinate>();
        public ObservableCollection<AppleCoordinate> AppleCoordinates
        {
            get { return _appleCoordinates; }
            set
            {
                _appleCoordinates = value;
                OnPropertyChanged();
            }
        }
        private void AddSnakeHistories(ObservableCollection<SnakeCoordinate> snakeCoordinates)
        {
            SnakeHistory snakeHistory = new SnakeHistory();
            snakeHistory.SnakeCoordinates.AddRange(snakeCoordinates);
            _snakeHistories.Add(snakeHistory);
        }
        private bool FindSnakeHistory(ObservableCollection<SnakeCoordinate> snakeCoordinates)
        {
            bool isEqual = false;
            foreach(SnakeHistory snakeHistory in _snakeHistories)
            {
                isEqual = true;
                for(int i = 0; i < snakeHistory.SnakeCoordinates.Count; i++)
                {
                    if(!(snakeCoordinates[i].X == snakeHistory.SnakeCoordinates[i].X && snakeCoordinates[i].Y == snakeHistory.SnakeCoordinates[i].Y)) //если координаты не совпадают, выходим из цикла сравнения координат
                    {
                        isEqual = false;
                        break;
                    }
                }
                if (isEqual)
                {
                    return true;
                }
            }
            return isEqual;
        }
        private void AppleGenerate()
        {
            if(CellsCount - (SnakeCoordinates.Count + AppleCoordinates.Count) > 0) //если есть свободные клетки
            {
                int x = Features.GetRandInt(0, BoardSize);
                int y = Features.GetRandInt(0, BoardSize);
                bool direction = Features.GetRandInt(0, 1) == 1 ? true : false; //направление поиска пустой ячейки, в конец или в начало
                bool isFind = false;
                int currentX = x;
                int currentY = y;
                while (!isFind)
                {
                    if (currentX >= BoardSize) //если в цикле поиска вперед, мы не нашли, возвращаемся к стартовой координате
                    {
                        currentX = x;
                        currentY = y;
                    }
                    if (currentX < 0) //если в цикле поиска назад, мы не нашли, возвращаемся к стартовой координате
                    {
                        currentX = x;
                        currentY = y;
                    }
                    if (direction)
                    {
                        while (!isFind && currentX < BoardSize)
                        {
                            if (!BoardCellsInfo[currentX, currentY].IsApple && !BoardCellsInfo[currentX, currentY].IsSnake)
                            {
                                isFind = true;
                            }
                            else
                            {
                                currentY++;
                                if (currentY >= BoardSize)
                                {
                                    currentY = 0;
                                    currentX++;
                                }
                            }
                        }
                    }
                    else
                    {
                        while (!isFind && currentX >= 0)
                        {
                            if (!BoardCellsInfo[currentX, currentY].IsApple && !BoardCellsInfo[currentX, currentY].IsSnake)
                            {
                                isFind = true;
                            }
                            else
                            {
                                currentY--;
                                if (currentY < 0)
                                {
                                    currentY = BoardSize - 1;
                                    currentX--;
                                }
                            }
                        }
                    }
                    direction = !direction;
                }

                AppleCoordinates.Add(new AppleCoordinate(currentX, currentY));
                BoardCellsInfo[currentX, currentY].IsApple = true;
            }
        }
        public bool IsSnakeGoUp()
        {
            if(SnakeCoordinates[SnakeCoordinates.Count-1].Y < SnakeCoordinates[SnakeCoordinates.Count - 2].Y)
                return true;
            else
                return false;
        }
        public bool IsSnakeGoRight()
        {
            if(SnakeCoordinates[SnakeCoordinates.Count-1].X > SnakeCoordinates[SnakeCoordinates.Count - 2].X)
                return true;
            else
                return false;
        }
        public bool IsSnakeGoDown()
        {
            if(SnakeCoordinates[SnakeCoordinates.Count-1].Y > SnakeCoordinates[SnakeCoordinates.Count - 2].Y)
                return true;
            else
                return false;
        }
        public bool IsSnakeGoLeft()
        {
            if(SnakeCoordinates[SnakeCoordinates.Count-1].X < SnakeCoordinates[SnakeCoordinates.Count - 2].X)
                return true;
            else
                return false;
        }
        public bool GetIsGameOver()
        {
            return _isGameOver;
        }
        /// <summary>
        /// Возвращает количество свободных клеток в замкнутой области, к которой принадлежит точка
        /// </summary>
        private int GetSquare(System.Drawing.Point point)
        {
            if (point.X < BoardCellsInfo.GetLength(0) && point.X > 0 && point.Y < BoardCellsInfo.GetLength(1) && point.Y > 0) //если не врезались в стенку
            {
                if (BoardCellsInfo[point.X, point.Y].IsSnake) //врезались в хвост
                {
                    return 0;
                }
            }
            else //врезались в стенку
            {
                return 0;
            }
            List<System.Drawing.Point> historyPoints = new List<System.Drawing.Point>();
            historyPoints.Add(point);
            List<System.Drawing.Point> visitedPoints = new List<System.Drawing.Point>();
            visitedPoints.Add(point);
            System.Drawing.Point currentPoint = new System.Drawing.Point(point.X, point.Y);
            while (historyPoints.Count > 0)
            {
                //определяем направление
                System.Drawing.Point[] directionsPoints = new System.Drawing.Point[4];
                directionsPoints[0] = new System.Drawing.Point(currentPoint.X - 1, currentPoint.Y); //вверх
                directionsPoints[1] = new System.Drawing.Point(currentPoint.X, currentPoint.Y + 1); //вправо
                directionsPoints[2] = new System.Drawing.Point(currentPoint.X + 1, currentPoint.Y); //вниз
                directionsPoints[3] = new System.Drawing.Point(currentPoint.X, currentPoint.Y - 1); //влево
                bool isFree = false;
                int index = 0;
                while(index < 4 && !isFree)
                {
                    isFree = false;
                    if (directionsPoints[index].X < BoardCellsInfo.GetLength(0) && directionsPoints[index].X > 0 && directionsPoints[index].Y < BoardCellsInfo.GetLength(1) && directionsPoints[index].Y > 0) //не врезались в стенку
                    {
                        if (!BoardCellsInfo[directionsPoints[index].X, directionsPoints[index].Y].IsSnake) //не врезались в хвост
                        {
                            if (!visitedPoints.Any(a => a.Equals(directionsPoints[index]))) //точка отсутствует в списке посещаемых
                            {
                                isFree = true;
                            }
                        }
                    }
                    index++;
                }
                index--;
                if (isFree)
                {
                    currentPoint = directionsPoints[index];
                    historyPoints.Add(new System.Drawing.Point(currentPoint.X, currentPoint.Y));
                    visitedPoints.Add(new System.Drawing.Point(currentPoint.X, currentPoint.Y));
                }
                else
                {
                    historyPoints.RemoveAt(historyPoints.Count - 1);
                    if(historyPoints.Count > 0)
                    {
                        currentPoint = historyPoints.Last();
                    }
                }
            }
            return visitedPoints.Count;
        }
        public Vector<float> GetInputs()
        {
            double[] angles = new double[13] { 224, 202, 180, 157.5, 135, 112.5, 90, 67.5, 45, 22.5, 0, 338, 316 }; //углы наклона лучей, оносительно оси X
            Vector<float> inputs = Vector<float>.Build.Dense(angles.Length * 3 + 3, 0);
            double headCenterX = SnakeCoordinates[SnakeCoordinates.Count - 1].X + 0.5;
            double headCenterY = SnakeCoordinates[SnakeCoordinates.Count - 1].Y + 0.5;
            //определяем угол изменения вектора взгляда, исходя из направления змейки
            double correctionAngle = 0;
            if (IsSnakeGoRight())
            {
                correctionAngle = 270;
            }
            if (IsSnakeGoDown())
            {
                correctionAngle = 180;
            }
            if (IsSnakeGoLeft())
            {
                correctionAngle = 90;
            }
            for (int i = 0; i < angles.Length; i++)
            {
                double angle = (angles[i] + correctionAngle) - Math.Truncate((angles[i] + correctionAngle) / 360) * 360; //приводим результирующий угол в диапазон от 0 до 360
                //определяем, является ли вектор вертикальной или горизонтальной линией
                bool isStraightRight = Math.Abs(angle - 0) < 0.1; //вектор - прямая линия вверх
                bool isStraightUp = Math.Abs(angle - 90) < 0.1;
                bool isStraightLeft = Math.Abs(angle - 180) < 0.1;
                bool isStraightDown = Math.Abs(angle - 270) < 0.1;
                //определяем близость направленности вектора к углам
                double angleToUpRight = Math.Abs(angle - 45);
                double angleToUpLeft = Math.Abs(angle - 135);
                double angleToDownLeft = Math.Abs(angle - 225);
                double angleToDownRight = Math.Abs(angle - 315);
                //определяем в какой из четырех углов квадрата направлен вектор
                bool isToUpLeft = angleToUpLeft < angleToUpRight && angleToUpLeft < angleToDownLeft && angleToUpLeft < angleToDownRight;
                bool isToUpRight = angleToUpRight < angleToUpLeft && angleToUpRight < angleToDownLeft && angleToUpRight < angleToDownRight;
                bool isToDownRight = angleToDownRight < angleToUpRight && angleToDownRight < angleToUpLeft && angleToDownRight < angleToDownLeft;
                bool isToDownLeft = angleToDownLeft < angleToUpRight && angleToDownLeft < angleToUpLeft && angleToDownLeft < angleToDownRight;
                //расстояние до стенки
                double distanceToWall = 0;
                if (isStraightRight || isStraightUp || isStraightLeft || isStraightDown) //если угол - вертикальная или горизонтальная линия, вычисляем расстояние до стенки по прямой
                {
                    if (isStraightRight) //до правой стенки
                    {
                        distanceToWall = BoardSize - headCenterX;
                    }
                    if (isStraightUp) //до верхней стенки
                    {
                        distanceToWall = headCenterY;
                    }
                    if (isStraightLeft) //до левой стенки
                    {
                        distanceToWall = headCenterX;
                    }
                    if (isStraightDown) //до нижней стенки
                    {
                        distanceToWall = BoardSize - headCenterY;
                    }
                }
                else //иначе вычисляем расстояние до стенок к которым направлен вектор
                {
                    //расстояние до верхней стенки
                    double distanceToUp = 0;
                    if (isToUpLeft || isToUpRight)
                    {
                        double y = BoardSize;
                        double x = (y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX; //x=(y-y0)/tan(angle)+x0
                        distanceToUp = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    //расстояние до правой стенки
                    double distanceToRight = 0;
                    if (isToUpRight || isToDownRight)
                    {
                        double x = BoardSize;
                        double y = Math.Tan(Features.DegreeToRadian(angle)) * (x - headCenterX) + InvertCoordinate(headCenterY); //y=tan(angle)*(x-x0)+y0
                        distanceToRight = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    //расстояние до нижней стенки
                    double distanceToDown = 0;
                    if (isToDownRight || isToDownLeft)
                    {
                        double y = 0;
                        double x = (y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX;
                        distanceToDown = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    //расстояние до левой стенки
                    double distanceToLeft = 0;
                    if (isToDownLeft || isToUpLeft)
                    {
                        double x = 0;
                        double y = Math.Tan(Features.DegreeToRadian(angle)) * (x - headCenterX) + InvertCoordinate(headCenterY);
                        distanceToLeft = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    if (isToUpLeft)
                    {
                        distanceToWall = Math.Min(distanceToUp, distanceToLeft);
                    }
                    else if (isToUpRight)
                    {
                        distanceToWall = Math.Min(distanceToUp, distanceToRight);
                    }
                    else if (isToDownRight)
                    {
                        distanceToWall = Math.Min(distanceToRight, distanceToDown);
                    }
                    else if (isToDownLeft)
                    {
                        distanceToWall = Math.Min(distanceToDown, distanceToLeft);
                    }
                }
                inputs[i] = (float)(1 / (distanceToWall - 0.5 + 1/**/)); //-0.5 потому что мы считаем расстояние из середины клетки, то есть оно получается на 0.5 больше. +1 потому что на расстоянии 0 значение активации должно быть 1, а для этого нужно прибавить 1, т.к. иначе будет деление на 0 и получится бесконечность

                //расстояние до ближайшего яблока
                double minDistanceToApple = 0;
                bool isAppleWasFind = false;
                for (int k = 0; k < AppleCoordinates.Count; k++)
                {
                    //координаты 4 вершин квадрата яблока
                    Point[] points = new Point[4] { new Point(AppleCoordinates[k].X, InvertCoordinate(AppleCoordinates[k].Y + 1)), new Point(AppleCoordinates[k].X, InvertCoordinate(AppleCoordinates[k].Y)), new Point(AppleCoordinates[k].X + 1, InvertCoordinate(AppleCoordinates[k].Y)), new Point(AppleCoordinates[k].X + 1, InvertCoordinate(AppleCoordinates[k].Y + 1)) };
                    bool isAppleToRightHead = headCenterX < points[0].X; //яблоко правее головы
                    bool isAppleToUpHead = InvertCoordinate(headCenterY) < points[0].Y; //яблоко выше головы
                    bool isAppleToLeftHead = headCenterX > points[2].X; //яблоко левее головы
                    bool isAppleToDownHead = InvertCoordinate(headCenterY) > points[1].Y; //яблоко ниже головы

                    if (isStraightRight || isStraightUp || isStraightLeft || isStraightDown) //если угол - вертикальная или горизонтальная линия, вычисляем расстояние до яблока по прямой
                    {
                        bool isStraightAppleFind = false;
                        double distanceToApple = 0;
                        if (isStraightRight) //до правого яблока
                        {
                            if(InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && isAppleToRightHead)
                            {
                                distanceToApple = points[0].X - headCenterX;
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightUp) //до верхнего яблока
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && isAppleToUpHead)
                            {
                                distanceToApple = points[0].Y - InvertCoordinate(headCenterY);
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightLeft) //до левого яблока
                        {
                            if (InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && isAppleToLeftHead)
                            {
                                distanceToApple = headCenterX - points[2].X;
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightDown) //до нижнего яблока
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && isAppleToDownHead)
                            {
                                distanceToApple = InvertCoordinate(headCenterY) - points[1].Y;
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightAppleFind)
                        {
                            if (isAppleWasFind)
                            {
                                minDistanceToApple = Math.Min(minDistanceToApple, distanceToApple);
                            }
                            else
                            {
                                minDistanceToApple = distanceToApple;
                            }
                            isAppleWasFind = true;
                        }
                    }
                    else //иначе вычисляем расстояние до яблок к которым направлен вектор под углом
                    {
                        bool isAppleReverseToVector = false; //находится ли яблоко в противоположном направлении от вектора. Ведь в этом случае уравнение прямой определит что яблоко лежит на ней
                        if((isToUpLeft && isAppleToDownHead) || (isToUpLeft && isAppleToRightHead))
                        {
                            isAppleReverseToVector = true;
                        }
                        if ((isToUpRight && isAppleToDownHead) || (isToUpRight && isAppleToLeftHead))
                        {
                            isAppleReverseToVector = true;
                        }
                        if ((isToDownRight && isAppleToUpHead) || (isToDownRight && isAppleToLeftHead))
                        {
                            isAppleReverseToVector = true;
                        }
                        if ((isToDownLeft && isAppleToUpHead) || (isToDownLeft && isAppleToRightHead))
                        {
                            isAppleReverseToVector = true;
                        }

                        if (!isAppleReverseToVector) //если вектор и яблоко не в противоположных направлениях проверяем пересекает ли вектор яблоко, иначе пропускаем это яблоко, т.к. формула может дать пересечение с противоположной стороны
                        {
                            double xVectorForY1 = (points[0].Y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX; //x=(y-y0)/tan(angle)+x0  -  значение X для функции линии в точке Y
                            double xVectorForY2 = (points[1].Y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX;

                            double yVectorForX1 = Math.Tan(Features.DegreeToRadian(angle)) * (points[0].X - headCenterX) + InvertCoordinate(headCenterY); //y=tan(angle)*(x-x0)+y0  -  значение Y для функции линии в точке X
                            double yVectorForX2 = Math.Tan(Features.DegreeToRadian(angle)) * (points[2].X - headCenterX) + InvertCoordinate(headCenterY);
                            //если значения вектора для всех X квадрата всегда больше или всегда меньше значений Y квадрата, значит вектор не пересекает квадрат
                            bool vectorDirection = yVectorForX1 > points[0].Y;
                            bool isVectorDirectionDifferent = false;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[0].Y ? isVectorDirectionDifferent : true;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX1 > points[1].Y ? isVectorDirectionDifferent : true;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[1].Y ? isVectorDirectionDifferent : true;
                            if (isVectorDirectionDifferent) //пересекли квадрат
                            {
                                //мы вычислили координаты точек вектора под углом angle, в которых этот вектор пересекает 4 линии квадрата: верхнюю горизонтальную линию (xVectorForY2), нижнюю горизонтальную (xVectorForY1), левую вертикальную (yVectorForX1) и правую вертиальную (yVectorForX2)
                                //теперь нам нужно найти минимальную дистанцию от начала вектора до этих точек, среди тех точек которые лежат в координатах квадрата

                                double xVectorForY1Distance = Math.Sqrt(Math.Pow(xVectorForY1 - headCenterX, 2) + Math.Pow(points[0].Y - InvertCoordinate(headCenterY), 2));
                                double xVectorForY2Distance = Math.Sqrt(Math.Pow(xVectorForY2 - headCenterX, 2) + Math.Pow(points[1].Y - InvertCoordinate(headCenterY), 2));

                                double yVectorForX1Distance = Math.Sqrt(Math.Pow(points[0].X - headCenterX, 2) + Math.Pow(yVectorForX1 - InvertCoordinate(headCenterY), 2));
                                double yVectorForX2Distance = Math.Sqrt(Math.Pow(points[2].X - headCenterX, 2) + Math.Pow(yVectorForX2 - InvertCoordinate(headCenterY), 2));

                                bool isDistanceInit = false;
                                double distanceToApple = xVectorForY2Distance;
                                if(xVectorForY2 >= points[0].X && xVectorForY2 <= points[2].X) //находится ли X координата на верхней горизонтальной линии в пределах координат квадрата
                                {
                                    isDistanceInit = true;
                                }
                                if (xVectorForY1 >= points[0].X && xVectorForY1 <= points[2].X) //находится ли X координата на нижней горизонтальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToApple = Math.Min(distanceToApple, xVectorForY1Distance);
                                    }
                                    else
                                    {
                                        distanceToApple = xVectorForY1Distance;
                                        isDistanceInit = true;
                                    }
                                }
                                if (yVectorForX1 >= points[0].Y && yVectorForX1 <= points[1].Y) //находится ли Y координата на левой вертикальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToApple = Math.Min(distanceToApple, yVectorForX1Distance);
                                    }
                                    else
                                    {
                                        distanceToApple = yVectorForX1Distance;
                                        isDistanceInit = true;
                                    }
                                }
                                if (yVectorForX2 >= points[0].Y && yVectorForX2 <= points[1].Y) //находится ли Y координата на правой вертикальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToApple = Math.Min(distanceToApple, yVectorForX2Distance);
                                    }
                                    else
                                    {
                                        distanceToApple = yVectorForX2Distance;
                                        isDistanceInit = true;
                                    }
                                }

                                if (isAppleWasFind)
                                {
                                    minDistanceToApple = Math.Min(minDistanceToApple, distanceToApple);
                                }
                                else
                                {
                                    minDistanceToApple = distanceToApple;
                                }
                                isAppleWasFind = true;
                            }
                        }
                    }
                }
                inputs[angles.Length + i] = isAppleWasFind ? (float)(1 / (minDistanceToApple - 0.5 + 1)) : 0;

                //расстояние до ближайшего хвоста
                double minDistanceToTail = 0;
                bool isTailWasFind = false;

                for (int k = 0; k < SnakeCoordinates.Count - 1; k++)
                {
                    //координаты 4 вершин квадрата хвоста
                    Point[] points = new Point[4] { new Point(SnakeCoordinates[k].X, InvertCoordinate(SnakeCoordinates[k].Y + 1)), new Point(SnakeCoordinates[k].X, InvertCoordinate(SnakeCoordinates[k].Y)), new Point(SnakeCoordinates[k].X + 1, InvertCoordinate(SnakeCoordinates[k].Y)), new Point(SnakeCoordinates[k].X + 1, InvertCoordinate(SnakeCoordinates[k].Y + 1)) };
                    bool isTailToRightHead = headCenterX < points[0].X; //хвост правее головы
                    bool isTailToUpHead = InvertCoordinate(headCenterY) < points[0].Y; //хвост выше головы
                    bool isTailToLeftHead = headCenterX > points[2].X; //хвост левее головы
                    bool isTailToDownHead = InvertCoordinate(headCenterY) > points[1].Y; //хвост ниже головы

                    if (isStraightRight || isStraightUp || isStraightLeft || isStraightDown) //если угол - вертикальная или горизонтальная линия, вычисляем расстояние до хвоста по прямой
                    {
                        bool isStraightTailFind = false;
                        double distanceToTail = 0;
                        if (isStraightRight) //до правого хвоста
                        {
                            if (InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && isTailToRightHead)
                            {
                                distanceToTail = points[0].X - headCenterX;
                                isStraightTailFind = true;
                            }
                        }
                        if (isStraightUp) //до верхнего хвоста
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && isTailToUpHead)
                            {
                                distanceToTail = points[0].Y - InvertCoordinate(headCenterY);
                                isStraightTailFind = true;
                            }
                        }
                        if (isStraightLeft) //до левого хвоста
                        {
                            if (InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && isTailToLeftHead)
                            {
                                distanceToTail = headCenterX - points[2].X;
                                isStraightTailFind = true;
                            }
                        }
                        if (isStraightDown) //до нижнего хвоста
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && isTailToDownHead)
                            {
                                distanceToTail = InvertCoordinate(headCenterY) - points[1].Y;
                                isStraightTailFind = true;
                            }
                        }
                        if (isStraightTailFind)
                        {
                            if (isTailWasFind)
                            {
                                minDistanceToTail = Math.Min(minDistanceToTail, distanceToTail);
                            }
                            else
                            {
                                minDistanceToTail = distanceToTail;
                            }
                            isTailWasFind = true;
                        }
                    }
                    else //иначе вычисляем расстояние до хвостов к которым направлен вектор под углом
                    {
                        bool isTailReverseToVector = false; //находится ли хвост в противоположном направлении от вектора. Ведь в этом случае уравнение прямой определит что хвост лежит на ней
                        if ((isToUpLeft && isTailToDownHead) || (isToUpLeft && isTailToRightHead))
                        {
                            isTailReverseToVector = true;
                        }
                        if ((isToUpRight && isTailToDownHead) || (isToUpRight && isTailToLeftHead))
                        {
                            isTailReverseToVector = true;
                        }
                        if ((isToDownRight && isTailToUpHead) || (isToDownRight && isTailToLeftHead))
                        {
                            isTailReverseToVector = true;
                        }
                        if ((isToDownLeft && isTailToUpHead) || (isToDownLeft && isTailToRightHead))
                        {
                            isTailReverseToVector = true;
                        }

                        if (!isTailReverseToVector) //если вектор и хвост не в противоположных направлениях проверяем пересекает ли вектор хвост, иначе пропускаем этот хвост, т.к. формула может дать пересечение с противоположной стороны
                        {
                            double xVectorForY1 = (points[0].Y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX; //x=(y-y0)/tan(angle)+x0  -  значение X для функции линии в точке Y
                            double xVectorForY2 = (points[1].Y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX;

                            double yVectorForX1 = Math.Tan(Features.DegreeToRadian(angle)) * (points[0].X - headCenterX) + InvertCoordinate(headCenterY); //y=tan(angle)*(x-x0)+y0  -  значение Y для функции линии в точке X
                            double yVectorForX2 = Math.Tan(Features.DegreeToRadian(angle)) * (points[2].X - headCenterX) + InvertCoordinate(headCenterY);
                            //если значения вектора для всех X квадрата всегда больше или всегда меньше значений Y квадрата, значит вектор не пересекает квадрат
                            bool vectorDirection = yVectorForX1 > points[0].Y;
                            bool isVectorDirectionDifferent = false;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[0].Y ? isVectorDirectionDifferent : true;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX1 > points[1].Y ? isVectorDirectionDifferent : true;
                            isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[1].Y ? isVectorDirectionDifferent : true;
                            if (isVectorDirectionDifferent) //пересекли квадрат
                            {
                                //мы вычислили координаты точек вектора под углом angle, в которых этот вектор пересекает 4 линии квадрата: верхнюю горизонтальную линию (xVectorForY2), нижнюю горизонтальную (xVectorForY1), левую вертикальную (yVectorForX1) и правую вертиальную (yVectorForX2)
                                //теперь нам нужно найти минимальную дистанцию от начала вектора до этих точек, среди тех точек которые лежат в координатах квадрата

                                double xVectorForY1Distance = Math.Sqrt(Math.Pow(xVectorForY1 - headCenterX, 2) + Math.Pow(points[0].Y - InvertCoordinate(headCenterY), 2));
                                double xVectorForY2Distance = Math.Sqrt(Math.Pow(xVectorForY2 - headCenterX, 2) + Math.Pow(points[1].Y - InvertCoordinate(headCenterY), 2));

                                double yVectorForX1Distance = Math.Sqrt(Math.Pow(points[0].X - headCenterX, 2) + Math.Pow(yVectorForX1 - InvertCoordinate(headCenterY), 2));
                                double yVectorForX2Distance = Math.Sqrt(Math.Pow(points[2].X - headCenterX, 2) + Math.Pow(yVectorForX2 - InvertCoordinate(headCenterY), 2));

                                bool isDistanceInit = false;
                                double distanceToTail = xVectorForY2Distance;
                                if (xVectorForY2 >= points[0].X && xVectorForY2 <= points[2].X) //находится ли X координата на верхней горизонтальной линии в пределах координат квадрата
                                {
                                    isDistanceInit = true;
                                }
                                if (xVectorForY1 >= points[0].X && xVectorForY1 <= points[2].X) //находится ли X координата на нижней горизонтальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToTail = Math.Min(distanceToTail, xVectorForY1Distance);
                                    }
                                    else
                                    {
                                        distanceToTail = xVectorForY1Distance;
                                        isDistanceInit = true;
                                    }
                                }
                                if (yVectorForX1 >= points[0].Y && yVectorForX1 <= points[1].Y) //находится ли Y координата на левой вертикальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToTail = Math.Min(distanceToTail, yVectorForX1Distance);
                                    }
                                    else
                                    {
                                        distanceToTail = yVectorForX1Distance;
                                        isDistanceInit = true;
                                    }
                                }
                                if (yVectorForX2 >= points[0].Y && yVectorForX2 <= points[1].Y) //находится ли Y координата на правой вертикальной линии в пределах координат квадрата
                                {
                                    if (isDistanceInit)
                                    {
                                        distanceToTail = Math.Min(distanceToTail, yVectorForX2Distance);
                                    }
                                    else
                                    {
                                        distanceToTail = yVectorForX2Distance;
                                        isDistanceInit = true;
                                    }
                                }

                                if (isTailWasFind)
                                {
                                    minDistanceToTail = Math.Min(minDistanceToTail, distanceToTail);
                                }
                                else
                                {
                                    minDistanceToTail = distanceToTail;
                                }
                                isTailWasFind = true;
                            }
                        }
                    }
                }
                inputs[angles.Length * 2 + i] = isTailWasFind ? (float)(1 / (minDistanceToTail - 0.5 + 1)) : 0;
            }
            return inputs;
        }
        public double InvertCoordinate(double coordinate)
        {
            return BoardSize - coordinate;
        }
        public void MoveSnake(int xOffset,int yOffset)
        {
            Score += _nextOneStepScore;
            int newX = SnakeCoordinates[SnakeCoordinates.Count - 1].X + xOffset;
            int newY = SnakeCoordinates[SnakeCoordinates.Count - 1].Y + yOffset;
            if(newX >= BoardCellsInfo.GetLength(0) || newX < 0 || newY >= BoardCellsInfo.GetLength(1) || newY < 0) //если врезались в стенку
            {
                _isGameOver = true;
            }
            else
            {
                SnakeCoordinates.Add(new SnakeCoordinate(newX, newY));
                if (!BoardCellsInfo[newX, newY].IsApple)
                {
                    _stepsWithoutApples++;
                    int tailTipX = SnakeCoordinates[0].X;
                    int tailTipY = SnakeCoordinates[0].Y;
                    BoardCellsInfo[tailTipX, tailTipY].IsSnake = false;
                    SnakeCoordinates.RemoveAt(0);
                }
                else //съели яблоко
                {
                    _snakeHistories.Clear();
                    if(SnakeCoordinates.Count == BoardSize * BoardSize) //если после съедения яблока, змейка заполняет все игровое поле, игра закончилась
                    {
                        _isGameOver = true;
                    }
                    EatenApples++;
                    Score += Math.Round(_nextAppleScore - _nextAppleScore * ((double)_stepsWithoutApples / _appleScoreReduceMaxSteps * _appleScoreReduce), 3); //чем быстрее добрались до яблока, тем больше счета получим
                    _nextOneStepScore += _nextOneStepScore * _oneStepScoreMultiply; //увеличиваем стоимость следующего шага змейки
                    _nextAppleScore += _nextAppleScore * _appleScoreMultiply; //увеличиваем стоимость следующего яблока
                    _stepsWithoutApples = 0;
                    AppleCoordinates.Remove(AppleCoordinates.Where(a => a.X == newX && a.Y == newY).First()); //удаляем съеденное яблоко
                }
                if (BoardCellsInfo[newX, newY].IsSnake) //если врезались в хвост
                {
                    _isGameOver = true;
                }
                else if (FindSnakeHistory(SnakeCoordinates))
                {
                    _isGameOver = true;
                }
                else
                {
                    AddSnakeHistories(SnakeCoordinates);
                }
                /*if(_stepsWithoutApples >= _maxStepsWithoutApples)
                {
                    _isGameOver = true;
                }*/
                BoardCellsInfo[newX, newY].IsSnake = true;
                if(BoardCellsInfo[newX, newY].IsApple) //генерируем новое яблоко, если съели. И делаем это после того как установим что в новой позиции есть тело змейки, т.к. иначе яблоко может быть сгенерировано в этой позиции
                {
                    BoardCellsInfo[newX, newY].IsApple = false;
                    AppleGenerate(); //генерируем новое яблоко
                }
            }
        }
    }
}
