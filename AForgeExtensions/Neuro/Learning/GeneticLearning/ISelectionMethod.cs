using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public interface ISelectionMethod
    {
        Chromosome[] ApplySelection(Chromosome[] population);
    }
}
