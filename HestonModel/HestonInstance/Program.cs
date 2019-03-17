using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HestonModel;


namespace HestonCmdLine
{
    class Program
    {
        /// <summary>
        /// implement all the task
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            double S = 100;
            double K = 100;
            Example.TestHestonModelFormula(1.5768, 0.0398, 0.5751, -0.5711, 0.0175, S, K, 0.025);
            Example.TestHestonModelFormula(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelMonteCarlo(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelFormula(2, 0.06, 0.3, 0.56, 0.04, S, K, 0.15);
            Example.TestHestonModelMonteCarlo(2, 0.06, 0.3, 0.56, 0.04, S, K, 0.15);
            Example.TestHestonModelMonteCarloAsian(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelMonteCarloLookBack(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelMonteCarloShoutOption(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelMonteCarloRainbowOption(2, 0.06, 0.4, 0.5, 0.04, S, K, 0.1);
            Example.TestHestonModelCalibration(1.5768, 0.0398, 0.5751, -0.5711, 0.0175, 0.025);
            Example.TestHestonModelCalibration(2, 0.06, 0.4, 0.5, 0, 0.1);
            Console.ReadKey();
        }
    }
}