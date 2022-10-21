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
        public GameBoard(int boardSize)
        {
            BoardCellsInfo = new BoardCellInfo[boardSize, boardSize];
            for(int i = 0; i < boardSize; i++)
            {
                for (int k = 0; k < boardSize; k++)
                {
                    BoardCellsInfo[i, k] = new BoardCellInfo();
                }
            }
            FreeCellsCount = boardSize * boardSize;
            //генерируем начальную позицию змейки
            Random random = new Random();
            SnakeCoordinates.Add(new SnakeCoordinate(random.Next(1, boardSize - 1), random.Next(1, boardSize - 1))); //начальная координата
            int nextX = SnakeCoordinates[0].X;
            int nextY = SnakeCoordinates[0].Y;
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
        }
        public int FreeCellsCount;
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

    }
}
