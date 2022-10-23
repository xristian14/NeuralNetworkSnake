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
            Random random = new Random();
            SnakeCoordinates.Add(new SnakeCoordinate(random.Next(1, boardSize - 1), random.Next(1, boardSize - 1))); //начальная координата
            SnakeCoordinates[0].X = 5;
            SnakeCoordinates[0].Y = 6;

            int nextX = SnakeCoordinates[0].X;
            int nextY = SnakeCoordinates[0].Y;

            BoardCellsInfo[nextX, nextY].IsSnake = true;
            int direction = random.Next(0, 3);
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

            nextX = 5;
            nextY = 5;

            SnakeCoordinates.Add(new SnakeCoordinate(nextX, nextY));
            BoardCellsInfo[nextX, nextY].IsSnake = true;
            //генерируем яблоки
            for(int i = 0; i < appleCount; i++)
            {
                AppleGenerate();
            }
            Score = _oneStepScore;
            _appleScoreReduceMaxSteps = (int)Math.Round(boardSize * 2.5);
            _maxStepsWithoutApples = (int)Math.Round((double)boardSize * boardSize);
        }
        private double _oneStepScore = 0.001;
        private int _stepsWithoutApples = 0;
        private int _maxStepsWithoutApples = 0;
        private double _appleScore = 1; //базовая стоимость съеденного яблока
        private int _appleScoreReduceMaxSteps = 0; //количество шагов, на котором стоимость яблока будет максимально уменьшена
        private double _appleScoreReduce = 0.4; //величина от стоимости яблока, на которое уменьшится его стоимость при количестве шагов _appleScoreReduceMaxSteps
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
        private bool IsGameOver = false;
        private int BoardSize;
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
                                    currentY = 0;
                                    currentX--;
                                }
                            }
                        }
                    }
                    direction = !direction;
                }

                currentX = 3;
                currentY = 3;

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
            return IsGameOver;
        }
        public Vector<float> GetInputs()
        {
            double[] angles = new double[13] { 224, 202, 180, 157.5, 135, 112.5, 90, 67.5, 45, 22.5, 0, 338, 316 }; //углы наклона лучей, оносительно оси X
            Vector<float> inputs = Vector<float>.Build.Dense(angles.Length * 3, 0);
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
                inputs[i] = (float)(1 / distanceToWall);

                //расстояние до ближайшего яблока
                double minDistanceToApple = 0;
                bool isAppleWasFind = false;
                for (int k = 0; k < AppleCoordinates.Count; k++)
                {
                    //координаты 4 вершин квадрата яблока
                    Point[] points = new Point[4] { new Point(AppleCoordinates[k].X, InvertCoordinate(AppleCoordinates[k].Y)), new Point(AppleCoordinates[k].X, InvertCoordinate(AppleCoordinates[k].Y + 1)), new Point(AppleCoordinates[k].X + 1, InvertCoordinate(AppleCoordinates[k].Y + 1)), new Point(AppleCoordinates[k].X + 1, InvertCoordinate(AppleCoordinates[k].Y)) };

                    





                    if (isStraightRight || isStraightUp || isStraightLeft || isStraightDown) //если угол - вертикальная или горизонтальная линия, вычисляем расстояние до яблока по прямой
                    {
                        bool isStraightAppleFind = false;
                        double distanceToApple = 0;
                        if (isStraightRight) //до правого яблока
                        {
                            if(InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && headCenterX < points[0].X)
                            {
                                distanceToApple = points[0].X - headCenterX;
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightUp) //до верхнего яблока
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && InvertCoordinate(headCenterY) < points[0].Y)
                            {
                                distanceToApple = points[0].Y - InvertCoordinate(headCenterY);
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightLeft) //до левого яблока
                        {
                            if (InvertCoordinate(headCenterY) > points[0].Y && InvertCoordinate(headCenterY) < points[1].Y && headCenterX > points[0].X)
                            {
                                distanceToApple = headCenterX - points[2].X;
                                isStraightAppleFind = true;
                            }
                        }
                        if (isStraightDown) //до нижнего яблока
                        {
                            if (headCenterX > points[0].X && headCenterX < points[2].X && InvertCoordinate(headCenterY) > points[1].Y)
                            {
                                distanceToApple = points[0].Y - InvertCoordinate(headCenterY);
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
                            double yVectorForX1Distance = Math.Sqrt(Math.Pow(points[0].X - headCenterX, 2) + Math.Pow(yVectorForX1 - InvertCoordinate(headCenterY), 2));
                            double yVectorForX2Distance = Math.Sqrt(Math.Pow(points[2].X - headCenterX, 2) + Math.Pow(yVectorForX2 - InvertCoordinate(headCenterY), 2));
                            double distanceToApple = Math.Min(yVectorForX1Distance, yVectorForX2Distance);
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
                inputs[angles.Length + i] = isAppleWasFind ? (float)(1 / minDistanceToApple) : 0;

                //расстояние до ближайшего хвоста
                double minDistanceToTail = 0;
                bool isTailWasFind = false;
                for (int k = 0; k < SnakeCoordinates.Count - 1; k++)
                {
                    //координаты 4 вершин тела змейки
                    Point[] points = new Point[4] { new Point(SnakeCoordinates[k].X, InvertCoordinate(SnakeCoordinates[k].Y)), new Point(SnakeCoordinates[k].X, InvertCoordinate(SnakeCoordinates[k].Y + 1)), new Point(SnakeCoordinates[k].X + 1, InvertCoordinate(SnakeCoordinates[k].Y + 1)), new Point(SnakeCoordinates[k].X + 1, InvertCoordinate(SnakeCoordinates[k].Y)) };

                    double yVectorForX1 = Math.Tan(Features.DegreeToRadian(angle)) * (points[0].X - headCenterX) + InvertCoordinate(headCenterY); //y=tan(angle)*(x-x0)+y0
                    double yVectorForX2 = Math.Tan(Features.DegreeToRadian(angle)) * (points[2].X - headCenterX) + InvertCoordinate(headCenterY);
                    //если значения вектора для всех X квадрата всегда больше или всегда меньше значений Y квадрата, значит вектор не пересекает квадрат
                    bool vectorDirection = yVectorForX1 > points[0].Y;
                    bool isVectorDirectionDifferent = false;
                    isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[0].Y ? isVectorDirectionDifferent : false;
                    isVectorDirectionDifferent = vectorDirection == yVectorForX1 > points[1].Y ? isVectorDirectionDifferent : false;
                    isVectorDirectionDifferent = vectorDirection == yVectorForX2 > points[1].Y ? isVectorDirectionDifferent : false;
                    if (isVectorDirectionDifferent) //пересекли квадрат
                    {
                        double yVectorForX1Distance = Math.Sqrt(Math.Pow(points[0].X - headCenterX, 2) + Math.Pow(yVectorForX1 - InvertCoordinate(headCenterY), 2));
                        double yVectorForX2Distance = Math.Sqrt(Math.Pow(points[2].X - headCenterX, 2) + Math.Pow(yVectorForX2 - InvertCoordinate(headCenterY), 2));
                        double distanceToTail = Math.Min(yVectorForX1Distance, yVectorForX2Distance);
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
                inputs[angles.Length * 2 + i] = isTailWasFind ? (float)(1 / minDistanceToTail) : 0;
            }
            return inputs;
        }
        public double InvertCoordinate(double coordinate)
        {
            return BoardSize - coordinate;
        }
        public void MoveSnake(int xOffset,int yOffset)
        {
            if(Math.Abs(xOffset) > 0 && Math.Abs(yOffset) > 0)
            {
                int Y = 0;
            }
            Score += _oneStepScore;
            int newX = SnakeCoordinates[SnakeCoordinates.Count - 1].X + xOffset;
            int newY = SnakeCoordinates[SnakeCoordinates.Count - 1].Y + yOffset;
            if(newX >= BoardCellsInfo.GetLength(0) || newX < 0 || newY >= BoardCellsInfo.GetLength(1) || newY < 0) //если врезались в стенку
            {
                IsGameOver = true;
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
                    EatenApples++;
                    Score += Math.Round(_appleScore - _appleScore * ((double)_stepsWithoutApples / _appleScoreReduceMaxSteps * _appleScoreReduce), 3); //чем быстрее добрались до яблока, тем больше счета получим
                    BoardCellsInfo[newX, newY].IsApple = false;
                    _stepsWithoutApples = 0;
                    AppleCoordinates.Remove(AppleCoordinates.Where(a => a.X == newX && a.Y == newY).First()); //удаляем съеденное яблоко
                    AppleGenerate(); //генерируем новое яблоко
                }
                if (BoardCellsInfo[newX, newY].IsSnake) //если врезались в хвост
                {
                    IsGameOver = true;
                }
                if(_stepsWithoutApples >= _maxStepsWithoutApples)
                {
                    IsGameOver = true;
                }
                BoardCellsInfo[newX, newY].IsSnake = true;
            }
        }
    }
}
