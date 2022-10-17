using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace NeuralNetworkSnake
{
    class ViewModelBase : INotifyPropertyChanged
    {
        public ViewModelBase()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            DispatcherInvoke((Action)(() =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }));
        }

        public Dispatcher _dispatcher;

        public void DispatcherInvoke(Action action)
        {
            _dispatcher.Invoke(action);
        }
    }
}
