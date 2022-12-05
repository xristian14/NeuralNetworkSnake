using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class LeaderboardItem : ViewModelBase
    {
        private double _score;
        public double Score
        {
            get { return _score; }
            set
            {
                _score = value;
                OnPropertyChanged();
            }
        }
        private int _applesCount;
        public int ApplesCount
        {
            get { return _applesCount; }
            set
            {
                _applesCount = value;
                OnPropertyChanged();
            }
        }
        private int _lostMoves;
        public int LostMoves
        {
            get { return _lostMoves; }
            set
            {
                _lostMoves = value;
                OnPropertyChanged();
            }
        }
    }
}
