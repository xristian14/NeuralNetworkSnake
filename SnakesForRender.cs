using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class SnakesForRender : ViewModelBase
    {
        public bool IsGameOver = false;
        private ObservableCollection<SnakeForRender> _snakesCoordinate = new ObservableCollection<SnakeForRender>();
        public ObservableCollection<SnakeForRender> SnakesCoordinate
        {
            get { return _snakesCoordinate; }
            set
            {
                _snakesCoordinate = value;
                OnPropertyChanged();
            }
        }
    }
}
