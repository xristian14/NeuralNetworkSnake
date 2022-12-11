using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class SnakeHistory
    {
        public SnakeHistory()
        {
            SnakeCoordinates = new List<SnakeCoordinate>();
        }
        public List<SnakeCoordinate> SnakeCoordinates { get; set; }
    }
}
