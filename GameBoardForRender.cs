using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class GameBoardForRender : ViewModelBase
    {
        public GameBoardForRender(ObservableCollection<BoardCell> boardCells, SnakesForRender snakesForRender, ApplesForRender applesForRender)
        {
            BoardCells = boardCells;
            SnakesForRender = snakesForRender;
            ApplesForRender = applesForRender;
        }
        private ObservableCollection<BoardCell> _boardCells = new ObservableCollection<BoardCell>();
        public ObservableCollection<BoardCell> BoardCells
        {
            get { return _boardCells; }
            set
            {
                _boardCells = value;
                OnPropertyChanged();
            }
        }
        private SnakesForRender _snakesForRender = new SnakesForRender();
        public SnakesForRender SnakesForRender
        {
            get { return _snakesForRender; }
            set
            {
                _snakesForRender = value;
                OnPropertyChanged();
            }
        }
        private ApplesForRender _applesForRender = new ApplesForRender();
        public ApplesForRender ApplesForRender
        {
            get { return _applesForRender; }
            set
            {
                _applesForRender = value;
                OnPropertyChanged();
            }
        }
    }
}
