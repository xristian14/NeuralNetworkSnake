using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public class SameActivationFunction : AForge.Neuro.IActivationFunction, ICloneable
    {
        public SameActivationFunction()
        {

        }
        public object Clone()
        {
            return new SameActivationFunction();
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
            return value;
        }
    }
}
