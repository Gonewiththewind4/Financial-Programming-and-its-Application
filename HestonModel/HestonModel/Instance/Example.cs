using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    /// <summary>
    /// This class,with HestonCmdLine program, are used to debug the code and finish the tasks.
    /// In this class, we don't use the methods in Heston.cs, but implement the other class with calculating function directly. 
    /// </summary>
    public class Example
    {
        /// <summary>
        /// This method is used to Test Heston Model Calibration task2.5/2.6
        /// </summary>
        public static void TestHestonModelCalibration(double Kappa, double Theta, double Sigma, double Rho, double V0,double rate)
        {
            double r = rate;
            double kappa = Kappa;
            double theta = Theta;
            double sigma = Sigma;
            double rho = Rho;
            double v0 = V0;

            HestonModelFormula model = new HestonModelFormula(kappa, theta, sigma, rho, v0);
            double[] S = new double[] { 100, 100, 100, 100, 100 };
            double[] optionExerciseTimes = new double[] { 1, 1, 2, 2, 1.5 };
            double[] optionStrikes = new double[] { 80, 90, 80, 100, 100 };
            double[] prices = new double[] { 25.72, 18.93, 30.49, 19.36, 16.58 };
            double[] resultParameter = new double[5];
            HestonCalibrator calibrator = new HestonCalibrator(100, r, 1e-3, 1000);
            calibrator.SetGuessParameters(kappa, theta, sigma, rho, v0);
            for (int i = 0; i < prices.Length; ++i)
            {
                calibrator.AddObservedOption(optionStrikes[i], optionExerciseTimes[i], prices[i]);
            }

            resultParameter = calibrator.Calibrate();

            double error = 0;
            CalibrationOutcome outcome = CalibrationOutcome.NotStarted;
            calibrator.GetCalibrationStatus(ref outcome, ref error);
            Console.WriteLine("Calibration outcome: {0} and error: {1}", outcome, error);

        }
        /// <summary>
        /// task2.2
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelFormula(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {

            double T1 = 1;
            double T2 = 2;
            double T3 = 3;
            double T4 = 4;
            double T5 = 15;
            double C1, C2, C3, C4, C5;

            Console.WriteLine("HestonModelFormula");

            HestonModelFormula Task2 = new HestonModelFormula(Kappa, Theta, sigma, Rho, V0);// (1.5768, 0.0398, 0.5751, -0.5711, 0.0175);//(1.5757,0.15656,0.57829,-0.57717,0.01843);//(1.5768, 0.0398, 0.5751, -0.5711, 0.0175);

            C1 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T1);
            C2 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T2);
            C3 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T3);
            C4 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T4);
            C5 = Task2.CalculateCallEuropeanOptionPrice(S, K, r, T5);
            Console.WriteLine(C1);
            Console.WriteLine(C2);
            Console.WriteLine(C3);
            Console.WriteLine(C4);
            Console.WriteLine(C5);
        }
        /// <summary>
        /// task2.3
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelMonteCarlo(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {
            double T1 = 1;
            double T2 = 2;
            double T3 = 3;
            double T4 = 4;
            double T5 = 15;
            double C1, C2, C3, C4, C5;

            Console.WriteLine("MonteCarlo");

            HestonModelMonteCarlo Task3 = new HestonModelMonteCarlo(Kappa, Theta, sigma, Rho, V0);// (2, 0.06, 0.4, 0.5, 0.04);
            C1 = Task3.CalculateEuropeanCallOption(S, K, T1, 10000, 365, r);
            Console.WriteLine(C1);
            C2 = Task3.CalculateEuropeanCallOption(S, K, T2, 10000, 365, r);
            Console.WriteLine(C2);
            C3 = Task3.CalculateEuropeanCallOption(S, K, T3, 10000, 365, r);
            Console.WriteLine(C3);
            C4 = Task3.CalculateEuropeanCallOption(S, K, T4, 10000, 365, r);
            Console.WriteLine(C4);
            C5 = Task3.CalculateEuropeanCallOption(S, K, T5, 10000, 365, r);
            Console.WriteLine(C5);
        }
        /// <summary>
        /// task2.7
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelMonteCarloAsian(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {


            double C1, C2, C3;
            double[] TT1 = { 0.75, 1 };
            double[] TT2 = { 0.25, 0.50, 0.75, 1, 1.25, 1.5, 1.75 };
            double[] TT3 = { 1.0, 2.0, 3.0 };
            Console.WriteLine("Asian");
            HestonModelMonteCarlo Task7 = new HestonModelMonteCarlo(Kappa, Theta, sigma, Rho, V0);
            C1 = Task7.CalculateAsianCallOptionPrice(S, K, TT1, 1.0, 365, 100000, r);
            Console.WriteLine(C1);

            C2 = Task7.CalculateAsianCallOptionPrice(S, K, TT2, 2.0, 365, 100000, r);
            Console.WriteLine(C2);

            C3 = Task7.CalculateAsianCallOptionPrice(S, K, TT3, 3.0, 365, 100000, r);
            Console.WriteLine(C3);

        }
        /// <summary>
        /// task2.8
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelMonteCarloLookBack(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {

            double C1, C2, C3, C4, C5;
            int div = 365;
            Console.WriteLine("lookback");
            HestonModelMonteCarlo Task8 = new HestonModelMonteCarlo(Kappa, Theta, sigma, Rho, V0);
            C1 = Task8.CalculateLookbackOption(S, K, 1, div, 10000, r);
            Console.WriteLine(C1);
            C2 = Task8.CalculateLookbackOption(S, K, 3, div, 10000, r);
            Console.WriteLine(C2);
            C3 = Task8.CalculateLookbackOption(S, K, 5, div, 10000, r);
            Console.WriteLine(C3);
            C4 = Task8.CalculateLookbackOption(S, K, 7, div, 10000, r);
            Console.WriteLine(C4);
            C5 = Task8.CalculateLookbackOption(S, K, 9, div, 10000, r);
            Console.WriteLine(C5);

        }
        /// <summary>
        /// task exploration
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelMonteCarloShoutOption(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {
            double T1 = 1;
            double T2 = 2;
            double T3 = 3;
            double T4 = 4;
            double T5 = 15;
            double C1, C2, C3, C4, C5;

            Console.WriteLine("MonteCarloShoutOption");

            ExoticOptionExploration Task9 = new ExoticOptionExploration(Kappa, Theta, sigma, Rho, V0);// (2, 0.06, 0.4, 0.5, 0.04);
            C1 = Task9.CalculateCallShoutOption(S, K, r, T1, 10000, 365);
            Console.WriteLine(C1);
            C2 = Task9.CalculateCallShoutOption(S, K, r, T2, 10000, 365);
            Console.WriteLine(C2);
            C3 = Task9.CalculateCallShoutOption(S, K, r, T3, 10000, 365);
            Console.WriteLine(C3);
            C4 = Task9.CalculateCallShoutOption(S, K, r, T4, 10000, 365);
            Console.WriteLine(C4);
            C5 = Task9.CalculateCallShoutOption(S, K, r, T5, 10000, 365);
            Console.WriteLine(C5);

        }
        /// <summary>
        /// task exploration
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        /// <param name="S">initialprice</param>
        /// <param name="K">strikeprice</param>
        /// <param name="r">risk-free-rate</param>
        public static void TestHestonModelMonteCarloRainbowOption(double Kappa, double Theta, double sigma, double Rho, double V0, double S, double K, double r)
        {
            double T1 = 1;
            double T2 = 2;
            double T3 = 3;
            double T4 = 4;
            double T5 = 15;
            double C1, C2, C3, C4, C5;
            double[] price = new double[] { 100, 100, 100 };
            
            Console.WriteLine("MonteCarloRainbowOption");

            ExoticOptionExploration Task10 = new ExoticOptionExploration(Kappa, Theta, sigma, Rho, V0);// (2, 0.06, 0.4, 0.5, 0.04);
            C1 = Task10.CalculateCallRainBowOption(price, K, r, T1, 10000, 365);
            Console.WriteLine(C1);
            C2 = Task10.CalculateCallRainBowOption(price, K, r, T2, 10000, 365);
            Console.WriteLine(C2);
            C3 = Task10.CalculateCallRainBowOption(price, K, r, T3, 10000, 365);
            Console.WriteLine(C3);
            C4 = Task10.CalculateCallRainBowOption(price, K, r, T4, 10000, 365);
            Console.WriteLine(C4);
            C5 = Task10.CalculateCallRainBowOption(price, K, r, T5, 10000, 365);
            Console.WriteLine(C5);

        }
    }
}
