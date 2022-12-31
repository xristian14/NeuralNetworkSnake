using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    [Serializable]
    public class ActivationNetworkSerializeFormat
    {
        public ActivationNetworkSerializeFormat()
        {

        }
        public double[][][] Weights { get; set; }
        public double[][] Biases { get; set; }
        public int InputNeuronsCount { get; set; }
        public int[] OtherLayersNeuronsCount { get; set; }
        public string[] OtherLayersActivationFunction { get; set; } //название типа, функции активации
    }
}
