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
            int length = 0;
            for(int i = 0; i < network.Layers.Length; i++)
            {
                length += network.Layers[i].Neurons.Length * network.Layers[i].Neurons[0].Weights.Length; //веса
                length += network.Layers[i].Neurons.Length; //смещение
            }
            _length = length;
        }
        private AForge.Neuro.ActivationNetwork _network;
        public AForge.Neuro.ActivationNetwork Network { get { return _network; } }
        private double _fitness = 0;
        public double Fitness { get { return _fitness; } set { _fitness = value; } }
        private double _length;
        /// <summary>
        /// Длина генома (количество значений).
        /// </summary>
        public double Length { get { return _length; } }
        public object Clone()
        {
            Chromosome chromosome = new Chromosome(AForgeExtensions.Features.CloneActivationNetwork(_network));
            chromosome.Fitness = _fitness;
            return chromosome;
        }
    }
}
