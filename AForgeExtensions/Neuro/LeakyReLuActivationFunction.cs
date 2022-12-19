using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public class LeakyReLuActivationFunction : AForge.Neuro.IActivationFunction, ICloneable
    {
        public LeakyReLuActivationFunction()
        {

        }
        private double _negativeSlope = 0.1;
        /// <summary>
        /// Наклон отрицательного значения. (Множитель, на который умножается отрицательное значение)
        /// </summary>
        public double NegativeSlope { get { return _negativeSlope; } set { _negativeSlope = value; } }
        public object Clone()
        {
            return new LeakyReLuActivationFunction();
        }
        public double Derivative(double value)
        {
            return value >= 0 ? 1 : _negativeSlope;
        }
        public double Derivative2(double value)
        {
            return value >= 0 ? 1 : _negativeSlope;
        }
        public double Function(double value)
        {
            return Math.Max(0, value) + _negativeSlope * Math.Min(0, value);
        }
    }
}
