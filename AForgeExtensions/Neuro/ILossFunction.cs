using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    public interface ILossFunction
    {
        double Calculate(double[] actualOutput, double[] desiredOutput);
    }
}
