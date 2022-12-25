using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public class ActivationNetworkSerializeFormat
    {
        public double[][][] Weights;
        public double[][] Biases;
        public int InputNeuronsCount;
        public int[] OtherLayersNeuronsCount;
        public string[] OtherLayersActivationFunction; //название типа, функции активации
    }
}
