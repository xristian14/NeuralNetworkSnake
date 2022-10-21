using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class BoardCellInfo
    {
        public BoardCellInfo()
        {
            IsSnake = false;
            isApple = false;
        }
        public bool IsSnake { get; set; }
        public bool isApple { get; set; }
    }
}
