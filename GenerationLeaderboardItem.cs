using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class GenerationLeaderboardItem : ViewModelBase
    {
        private int _id;
        public int Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }
        private double _totalScore;
        public double TotalScore
        {
            get { return _totalScore; }
            set
            {
                _totalScore = value;
                OnPropertyChanged();
            }
        }
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
        private int _eatenApples;
        public int EatenApples
        {
            get { return _eatenApples; }
            set
            {
                _eatenApples = value;
                OnPropertyChanged();
            }
        }
    }
}
