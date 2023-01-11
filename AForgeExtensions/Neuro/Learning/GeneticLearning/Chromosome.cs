using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro.Learning.GeneticLearning
{
    public class Chromosome : ICloneable
    {
        public Chromosome(AForge.Neuro.ActivationNetwork network)
        {
            _network = network;
        }
        private AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private double _fitness = 0;
        public double Fitness { get { return _fitness; } set { _fitness = value; } }
        private double _scaledFitness = 0;
        public double ScaledFitness { get { return _scaledFitness; } set { _scaledFitness = value; } } //смасштабированное значение приспособленности.
        public object Clone()
        {
            Chromosome chromosome = new Chromosome(ActivationNetworkFeatures.CloneActivationNetwork(_network));
            chromosome.Fitness = _fitness;
            chromosome.ScaledFitness = _scaledFitness;
            return chromosome;
        }
    }
}
