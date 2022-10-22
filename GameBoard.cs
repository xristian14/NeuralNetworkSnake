using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            SnakeCoordinates.Add(new SnakeCoordinate(nextX, nextY));
            BoardCellsInfo[nextX, nextY].IsSnake = true;
            //генерируем яблоки
            for(int i = 0; i < appleCount; i++)
            {
                AppleGenerate();
            }
            Score = _oneStepScore;
            _maxStepsWithoutApples = (int)Math.Round(boardSize * 2.5);
        }
        private double _oneStepScore = 0.001;
        private int _stepsWithoutApples = 0;
        private int _maxStepsWithoutApples = 0;
        private double _appleScore = 1;
        private double _score;
        public double Score
        {
            get { return _score; }
            private set { _score = value; }
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
                AppleCoordinates.Add(new AppleCoordinate(currentX, currentY));
                BoardCellsInfo[currentX, currentY].IsApple = true;
            }
        }
        public bool IsSnakeGoUp()
        {
            if(SnakeCoordinates[SnakeCoordinates.Count-1].Y > SnakeCoordinates[SnakeCoordinates.Count - 2].Y)
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
            if(SnakeCoordinates[SnakeCoordinates.Count-1].Y < SnakeCoordinates[SnakeCoordinates.Count - 2].Y)
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
            Vector<float> inputs = Vector<float>.Build.Dense(48, 0);
            int lastIndex = 0;
            for (int i = 0; i < 16; i++) //расстояние до стенки
            {
                float distance = 0;
                if(i % 4 == 0) //если под прямым углом
                {
                    switch (i % 4)
                    {
                        case 0:
                            distance = SnakeCoordinates[SnakeCoordinates.Count - 1].Y; //to up
                            break;
                        case 1:
                            distance = BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].X; //to right
                            break;
                        case 2:
                            distance = BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].Y; //to down
                            break;
                        case 3:
                            distance = SnakeCoordinates[SnakeCoordinates.Count - 1].X; //to left
                            break;
                    }
                }
                else //если под наклоном
                {
                    float angle = 360 / 16 * i;
                    if(angle > 0 && angle < 90)
                    {
                        float distanceToUp = SnakeCoordinates[SnakeCoordinates.Count - 1].Y / (float)Math.Cos(angle);
                        float distanceToRight = (BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].X) / (float)Math.Cos(90 - angle);
                        distance = Math.Min(distanceToUp, distanceToRight);
                    }
                    else if (angle > 90 && angle < 180)
                    {
                        float distanceToRight = (BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].X) / (float)Math.Cos(angle - 90);
                        float distanceToDown = BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].Y / (float)Math.Cos(180 - angle);
                        distance = Math.Min(distanceToRight, distanceToDown);
                    }
                    else if (angle > 180 && angle < 270)
                    {
                        float distanceToDown = BoardSize - SnakeCoordinates[SnakeCoordinates.Count - 1].Y / (float)Math.Cos(angle - 180);
                        float distanceToLeft = SnakeCoordinates[SnakeCoordinates.Count - 1].X / (float)Math.Cos(270 - angle);
                        distance = Math.Min(distanceToDown, distanceToLeft);
                    }
                    else if (angle > 270 && angle < 360)
                    {
                        float distanceToLeft = SnakeCoordinates[SnakeCoordinates.Count - 1].X / (float)Math.Cos(angle - 270);
                        float distanceToUp = SnakeCoordinates[SnakeCoordinates.Count - 1].Y / (float)Math.Cos(360 - angle);
                        distance = Math.Min(distanceToLeft, distanceToUp);
                    }
                }
                inputs[lastIndex + i] = 1 / distance;
            }
            lastIndex += 16;
            //расстояние до яблок
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
            double[] angles = new double[13] { 224, 202, 180, 157.5, 135, 112.5, 90, 67.5, 45, 22.5, 0, 338, 316 }; //углы наклона лучей, оносительно оси X
            for(int i = 0; i < angles.Length; i++)
            {
                //расстояние до стенки
                double angle = (angles[i] + correctionAngle) - Math.Truncate((angles[i] + correctionAngle) / 360) * 360; //приводим результирующий угол в диапазон от 0 до 360
                double distanceToWall = 0;
                //если угол - вертикальная или горизонтальная линия, вычисляем расстояние до стенки по прямой
                if(angle - 0 < 0.1 || angle - 90 < 0.1 || angle - 180 < 0.1 || angle - 270 < 0.1)
                {
                    if(angle - 0 < 0.1) //до правой стенки
                    {
                        distanceToWall = BoardSize - headCenterX;
                    }
                    if(angle - 90 < 0.1) //до верхней стенки
                    {
                        distanceToWall = headCenterY;
                    }
                    if(angle - 180 < 0.1) //до левой стенки
                    {
                        distanceToWall = headCenterX;
                    }
                    if(angle - 270 < 0.1) //до нижней стенки
                    {
                        distanceToWall = BoardSize - headCenterY;
                    }
                }
                else //иначе вычисляем расстояние до стенок к которым направлен вектор
                {
                    //определяем в какой из четырех углов квадрата направлен вектора
                    double angleToUpRight = Math.Abs(angle - 45);
                    double angleToUpLeft = Math.Abs(angle - 135);
                    double angleToDownLeft = Math.Abs(angle - 225);
                    double angleToDownRight = Math.Abs(angle - 315);

                    bool isToUpLeft = angleToUpLeft < angleToUpRight && angleToUpLeft < angleToDownLeft && angleToUpLeft < angleToDownRight;
                    bool isToUpRight = angleToUpRight < angleToUpLeft && angleToUpRight < angleToDownLeft && angleToUpRight < angleToDownRight;
                    bool isToDownRight = angleToDownRight < angleToUpRight && angleToDownRight < angleToUpLeft && angleToDownRight < angleToDownLeft;
                    bool isToDownLeft = angleToDownLeft < angleToUpRight && angleToDownLeft < angleToUpLeft && angleToDownLeft < angleToDownRight;

                    //расстояние до верхней стенки
                    double distanceToUp = 0;
                    if(isToUpLeft || isToUpRight)
                    {
                        double y = BoardSize;
                        double x = (y - InvertCoordinate(headCenterY)) / Math.Tan(Features.DegreeToRadian(angle)) + headCenterX;
                        distanceToUp = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    //расстояние до правой стенки
                    double distanceToRight = 0;
                    if(isToUpRight || isToDownRight)
                    {
                        double x = BoardSize;
                        double y = Math.Tan(Features.DegreeToRadian(angle)) * (x - headCenterX) + InvertCoordinate(headCenterY);
                        distanceToRight = Math.Sqrt(Math.Pow(x - headCenterX, 2) + Math.Pow(y - InvertCoordinate(headCenterY), 2));
                    }

                    //расстояние до нижней стенки
                    double distanceToDown = 0;
                    if(isToDownRight || isToDownLeft)
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

            }
        }
        public double InvertCoordinate(double coordinate)
        {
            return BoardSize - coordinate;
        }
        public void MoveSnake(int xOffset,int yOffset)
        {
            Score += _oneStepScore;
            int newX = SnakeCoordinates[SnakeCoordinates.Count - 1].X + xOffset;
            int newY = SnakeCoordinates[SnakeCoordinates.Count - 1].Y + yOffset;
            if(newX >= BoardCellsInfo.GetLength(0) || newY >= BoardCellsInfo.GetLength(1)) //если врезались в стенку
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
                    Score += Math.Round(_appleScore - _appleScore * ((double)_stepsWithoutApples / _maxStepsWithoutApples * 0.4), 3); //чем быстрее добрались до яблока, тем больше счета получим. На максимальном количестве ходов снижается 40% счета от яблока
                    BoardCellsInfo[newX, newY].IsApple = false;
                    AppleGenerate(); //генерируем новое яблоко
                }
                if (BoardCellsInfo[newX, newY].IsSnake) //если врезались в хвост
                {
                    IsGameOver = true;
                }
                BoardCellsInfo[newX, newY].IsSnake = true;
            }
        }
    }
}
