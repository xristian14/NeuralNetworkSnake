using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    /// <summary>
    /// Класс описывает экземпляр ячейки для отображения в представлении, из которых состоит поле для алгоритма QLearning
    /// </summary>
    class QLearningCellView : ViewModelBase
    {
        public QLearningCellView(System.Windows.Media.Brush backgroundBrush, double reward, double upQvalue, double rightQvalue, double leftQvalue, double downQvalue, int top, int left)
        {
            BackgroundBrush = backgroundBrush;
            Reward = reward;
            UpQvalue = upQvalue;
            RightQvalue = rightQvalue;
            LeftQvalue = leftQvalue;
            DownQvalue = downQvalue;
            Top = top;
            Left = left;
        }
        private System.Windows.Media.Brush _backgroundBrush;
        public System.Windows.Media.Brush BackgroundBrush
        {
            get { return _backgroundBrush; }
            set
            {
                _backgroundBrush = value;
                OnPropertyChanged();
            }
        }
        private double _reward;
        public double Reward
        {
            get { return _reward; }
            set
            {
                _reward = value;
                OnPropertyChanged();
            }
        }
        private double _upQvalue;
        public double UpQvalue
        {
            get { return _upQvalue; }
            set
            {
                _upQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _rightQvalue;
        public double RightQvalue
        {
            get { return _rightQvalue; }
            set
            {
                _rightQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _leftQvalue;
        public double LeftQvalue
        {
            get { return _leftQvalue; }
            set
            {
                _leftQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _downQvalue;
        public double DownQvalue
        {
            get { return _downQvalue; }
            set
            {
                _downQvalue = value;
                OnPropertyChanged();
            }
        }
        private int _top;
        public int Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }
        private int _left;
        public int Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }
    }
}
