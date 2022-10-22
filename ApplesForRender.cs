using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class ApplesForRender : ViewModelBase
    {
        private ObservableCollection<SnakeForRender> _applesCoordinates = new ObservableCollection<SnakeForRender>();
        public ObservableCollection<SnakeForRender> ApplesCoordinates
        {
            get { return _applesCoordinates; }
            set
            {
                _applesCoordinates = value;
                OnPropertyChanged();
            }
        }
    }
}
