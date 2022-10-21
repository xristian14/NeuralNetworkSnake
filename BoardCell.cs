using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NeuralNetworkSnake
{
    class BoardCell
    {
        public BoardCell(double cellSize)
        {
            CellSize = cellSize;
        }
        public double CellSize { get; set; }
    }
}
