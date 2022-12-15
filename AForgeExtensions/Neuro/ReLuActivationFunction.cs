using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public class ReLuActivationFunction : AForge.Neuro.IActivationFunction, ICloneable
    {
        public ReLuActivationFunction()
        {

        }
        public object Clone()
        {
            return new ReLuActivationFunction();
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
