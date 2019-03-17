using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using MathNet.Numerics.Distributions;
using HestonModel;
using System.Collections.Generic;


namespace HestonModel
{
    /// <summary>
    /// In this class implement the Monte-Carlo method and implementEuropean option, Asian option pricing as well as Lookback option pricing
    /// </summary>
    /// <param name="Kappa">Mean reversion speed in Heston model</param>
    /// <param name="Theta">The long-term mean in Heston model</param>
    /// <param name="Sigma">The vol of vol in Heston model</param>
    /// <param name="Rho">Initial variance in Heston model</param>
    /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
    /// <returns>Option price</returns>
    public class HestonModelMonteCarlo
    {
        
        private double Theta;
        private double Kappa;
        private double sigma;
        private double Rho;
        private double V0;

        public HestonModelMonteCarlo()
        {
            Theta = 0.06;
            Kappa = 2;
            sigma = 0.4;
            Rho = 0.5;
            V0 = 0.04;
        }
        public HestonModelMonteCarlo(double Kappa, double Theta, double sigma, double Rho, double V0)
        {
            this.Theta = Theta;
            this.Kappa = Kappa;
            this.sigma = sigma;
            this.Rho = Rho;
            this.V0 = V0;
        }
        /// <summary>
        /// this method is used to calculate European call option price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>European call option price</returns>
        public double CalculateEuropeanCallOption(double S, double K, double T, int n, int N, double r)
        {
            if (2 * Kappa * Theta < Math.Pow(sigma, 2))
                throw new System.ArgumentException("Need to Satisfy Feller condition");
            if (S <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T , n , N> 0");
            double Sn = 0;
            GeneratePath pathS = new GeneratePath(Kappa, Theta, sigma, Rho, V0);
            for (int j = 1; j <= n; j++)
            {
                Sn = Sn + Math.Max(pathS.CreatePathSingleT(S, K, T, N, r) - K, 0);
            }
            return Sn / n * Math.Exp(-r * (T));
        }
        /// <summary>
        /// this method is used to calculate European put option price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>European put option price</returns>
        public double CalculateEuropeanPutOptionPrice(double S, double K, double T, int n, int N, double r)
        {
            if (S <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T ,n ,N> 0");

            return CalculateEuropeanCallOption(S, K, T, n, N, r) - S + K * Math.Exp(-r * (T));
        }


        //Asian option
        /// <summary>
        /// check if input is standard.
        /// </summary>
        /// <param name="T"></param>
        /// <param name="exerciseT"></param>
        private void CheckAsianOptionInputs(IEnumerable<double> T, double exerciseT)
        {
            if (T.Count() == 0)
                throw new System.ArgumentException("Need at least one monitoring date for Asian option.");

            if (T.ElementAt(0) <= 0)
                throw new System.ArgumentException("The first monitoring date must be positive.");


            for (int i = 1; i < T.Count(); ++i)
            {
                if (T.ElementAt(i-1) >= T.ElementAt(i))
                    throw new System.ArgumentException("Monitoring dates must be increasing.");
            }

            if (T.ElementAt(T.Count() - 1) > exerciseT)
                throw new System.ArgumentException("Last monitoring dates must not be greater than the exercise time.");

        }
        /// <summary>
        /// This methods is used to calculate Asian Call option price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">time dived array</param>
        /// <param name="exerciseT">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>Asian Call option price</returns>
        public double CalculateAsianCallOptionPrice(double S, double K, IEnumerable<double> T, double exerciseT, int N, int n, double r)
        {
            if (2 * Kappa * Theta < Math.Pow(sigma, 2))
                throw new System.ArgumentException("Need Feller condition");
            if (S <= 0 || K <= 0 || exerciseT <= 0)
                throw new System.ArgumentException("Need S, K, T > 0");

            CheckAsianOptionInputs(T, exerciseT);

            Func<double, double> callPayoff = (x) => Math.Max(x - K, 0);
            return CalculateAsianOptionPrice(S, K, T, exerciseT, N, n, callPayoff, r);
        }
        /// <summary>
        /// This methods is used to calculate Asian put option price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">time dived array</param>
        /// <param name="exerciseT">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>Asian put option price</returns>
        public double CalculateAsianPutOptionPrice(double S, double K, IEnumerable<double> T, double exerciseT, int N, int n, double r)
        {
            
            if (2 * Kappa * Theta < Math.Pow(sigma, 2))
                throw new System.ArgumentException("Need Feller condition");
            if (S <= 0 || K <= 0 || exerciseT <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T > 0");

            CheckAsianOptionInputs(T, exerciseT);

            Func<double, double> putPayoff = (x) => Math.Max(K - x, 0);
            return CalculateAsianOptionPrice(S, K, T, exerciseT, N, n, putPayoff, r);
        }

        private double CalculateAsianOptionPrice(double S, double K, IEnumerable<double> T, double exerciseT, int N,
                                                 int n, Func<double, double> payoffFn, double r)
        {
            if (2 * Kappa * Theta < Math.Pow(sigma, 2))
                throw new System.ArgumentException("Need Feller condition");
            int M = T.Count();
            // We generate the 'path' for the T_i that we need
            double price = S;
            // the is the average for Asian option price
            double[] pathsOfS = new double[N + 1];
            double tau = exerciseT / N;
            // this is the MC average
            GeneratePath pathS = new GeneratePath(Kappa, Theta, sigma, Rho, V0);
            double[] MM = new double[M];
            double[] averagePayoff = new double[n];
            double temp = 0;
            for (int i = 0; i < n; i++)
            {
                pathsOfS = pathS.CreatePathMultiT(S, K, exerciseT, N, r);

                for (int m = 0; m < M; m++)
                {
                    int time = Convert.ToInt32(Math.Round(T.ElementAt(m) / tau));
                    MM[m] = pathsOfS[time];
                }
                temp = MM.Sum();
                averagePayoff[i] = Math.Exp(-r * exerciseT) * payoffFn(temp / M);
            }
            return averagePayoff.Sum() / averagePayoff.Length;
        }
        /// <summary>
        /// Lookback option pricing
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="exerciseT">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>Lookback option price</returns>
        public double CalculateLookbackOption(double S, double K, double exerciseT, int N, int n, double r)
        {

            if (2 * Kappa * Theta < Math.Pow(sigma, 2))
                throw new System.ArgumentException("Need Feller condition");
            double[] pathsOfS = new double[N + 1];
            double tau = exerciseT / N;
            double payoff;
            double temp = 0;
            // this is the MC average
            double averagePayoff = 0;
            GeneratePath pathS = new GeneratePath(Kappa, Theta, sigma, Rho, V0);
            for (int i = 0; i < n; ++i)
            {
                pathsOfS = pathS.CreatePathMultiT(S, K, exerciseT, N, r);
                temp = pathsOfS.Min();
                payoff = pathsOfS[N] - pathsOfS.Min();
                averagePayoff += payoff;
            }
            averagePayoff *= 1.0 / n;

            return Math.Exp(-r * exerciseT) * averagePayoff;
        }
    }
}
