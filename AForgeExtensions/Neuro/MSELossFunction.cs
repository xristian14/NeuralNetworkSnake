using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AForgeExtensions.Neuro
{
    /// <summary>
    /// Функция потерь среднеквадратичного отклонения.
    /// </summary>
    public class MSELossFunction : ILossFunction
    {
        public MSELossFunction()
        {

        }
        /// <summary>
        /// Возвращает среднеквадратичную ошибку.
        /// </summary>
        public double Calculate(double[][] actualOutputs, double[][] desiredOutputs)
        {
            double totalSum = 0;
            for(int i = 0; i < actualOutputs.Length; i++)
            {
                for(int k = 0; k < actualOutputs[i].Length; k++)
                {
                    totalSum += Math.Pow(actualOutputs[i][k] - desiredOutputs[i][k], 2);
                }
            }
            return totalSum / actualOutputs.Length;
        }
    }
}
