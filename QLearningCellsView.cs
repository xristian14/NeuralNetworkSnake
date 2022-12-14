using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class QLearningCellsView : ViewModelBase
    {
        public QLearningCellsView(int top, int left, int width, int height)
        {
            Top = top;
            Left = left;
            Width = width;
            Height = height;
        }
        private int _reward = 0;
        public int Reward
        {
            get { return _reward; }
            set
            {
                _reward = value;
                OnPropertyChanged();
            }
        }
        private double _upQvalue = 0;
        public double UpQvalue
        {
            get { return _upQvalue; }
            set
            {
                _upQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _rightQvalue = 0;
        public double RightQvalue
        {
            get { return _rightQvalue; }
            set
            {
                _rightQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _leftQvalue = 0;
        public double LeftQvalue
        {
            get { return _leftQvalue; }
            set
            {
                _leftQvalue = value;
                OnPropertyChanged();
            }
        }
        private double _downQvalue = 0;
        public double DownQvalue
        {
            get { return _downQvalue; }
            set
            {
                _downQvalue = value;
                OnPropertyChanged();
            }
        }
        private int _top = 0;
        public int Top
        {
            get { return _top; }
            set
            {
                _top = value;
                OnPropertyChanged();
            }
        }
        public int _left { get; set; }
        public int Left
        {
            get { return _left; }
            set
            {
                _left = value;
                OnPropertyChanged();
            }
        }
        public int _width { get; set; }
        public int Width
        {
            get { return _width; }
            set
            {
                _width = value;
                OnPropertyChanged();
            }
        }
        public int _height { get; set; }
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }
    }
}
