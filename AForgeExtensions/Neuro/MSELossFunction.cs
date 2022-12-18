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
        public double Calculate(double[] actualOutput, double[] desiredOutput)
        {
            double sum = 0;
            for(int i = 0; i < actualOutput.Length; i++)
            {
                sum += Math.Pow(actualOutput[i] - desiredOutput[i], 2);
            }
            return sum / actualOutput.Length;
        }
    }
}
