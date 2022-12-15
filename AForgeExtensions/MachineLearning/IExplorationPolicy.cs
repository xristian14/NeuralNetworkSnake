using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.MachineLearning
{
    public interface IExplorationPolicy
    {
        int ChooseAction(double[] actionEstimates);
        double SetEpsilon(double epsilon);
    }
}
