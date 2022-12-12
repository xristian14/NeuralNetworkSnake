using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    class ReLuFunction : AForge.Neuro.IActivationFunction, ICloneable
    {
        public ReLuFunction()
        {

        }
        public object Clone()
        {
            return new ReLuFunction();
        }
        public double Derivative(double x)
        {
            return x;
        }
        public double Derivative2(double y)
        {
            return y;
        }
        public double Function(double value)
        {
            return Math.Max(0, value);
        }
    }
}
