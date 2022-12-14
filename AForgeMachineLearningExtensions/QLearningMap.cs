using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeMachineLearningExtensions
{
    /// <summary>
    /// Класс описывает экземпляр карты
    /// </summary>
    class QLearningMap
    {
        public QLearningMap(bool isWall, bool isCliff, double reward)
        {
            IsWall = isWall;
            IsCliff = isCliff;
            Reward = reward;
        }
        public bool IsWall { get; private set; }
        public bool IsCliff { get; private set; }
        public double Reward { get; private set; }
    }
}
