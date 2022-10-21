using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public void MoveSnake(int xOffset,int yOffset)
        {
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
                    int tailTipX = SnakeCoordinates[0].X;
                    int tailTipY = SnakeCoordinates[0].Y;
                    BoardCellsInfo[tailTipX, tailTipY].IsSnake = false;
                    SnakeCoordinates.RemoveAt(0);
                }
                else //съели яблоко
                {
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
