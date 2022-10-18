using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetworkSnake
{
    public static class Rand
    {
        public static Random _random = new Random();
        public static int GetInt(int min, int max)
        {
            return _random.Next(min, max);
        }
        public static float GetFloat(float min, float max)
        {
            return (float)_random.NextDouble() * (max - min) + min;
        }
        public static double GetDouble(float min, float max)
        {
            return _random.NextDouble() * (max - min) + min;
        }
    }
}
