using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HestonModel
{
    /// <summary>
    /// This class is used to implement Heston Model in Monto-Carlo Method to price exotic derivatives, including rainbow option and shout Option
    /// </summary>
    /// <param name="Kappa">Mean reversion speed in Heston model</param>
    /// <param name="Theta">The long-term mean in Heston model</param>
    /// <param name="Sigma">The vol of vol in Heston model</param>
    /// <param name="Rho">Initial variance in Heston model</param>
    /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
    /// <returns>Option price</returns>
    class ExoticOptionExploration
    {
        private double theta;
        private double kappa;
        private double sigma;
        private double rho;
        private double v0;
        private HestonModelMonteCarlo MonteCarlo;
        /// <summary>
        /// default parameters
        /// </summary>
        public ExoticOptionExploration()
        {
            this.MonteCarlo = new HestonModelMonteCarlo();
            this.theta = 0.06;
            this.kappa = 2;
            this.sigma = 0.4;
            this.rho = 0.5;
            this.v0 = 0.04;
        }
        /// <summary>
        /// interface for data in class
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        public ExoticOptionExploration(double Kappa, double Theta, double Sigma, double Rho, double V0)
        {
            this.MonteCarlo = new HestonModelMonteCarlo(Kappa,Theta,Sigma,Rho,V0);
            this.theta = Theta;
            this.kappa = Kappa;
            this.sigma = Sigma;
            this.rho = Rho;
            this.v0 = V0;
        }

        /// <summary>
        /// calculate call shout option
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>call shout option price</returns>
        public double CalculateCallShoutOption(double S, double K, double r, double T, int n, int N)
        {
            if (S <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T , n , N> 0");
            GeneratePath pathS = new GeneratePath(kappa, theta, sigma, rho, v0);
            double[] pathOfS = new double[N+1];
            double callPrice = 0;
            for (int j = 1; j <= n; j++)
            {

                pathOfS = pathS.CreatePathMultiT(S, K, T, N, r);
                callPrice += Math.Max(Math.Max(pathOfS[N] - K, pathOfS.Max() - K), 0) - (pathOfS[N] - K);
            }
            callPrice *= 1.0 / n;
            return Math.Exp(-r * T) * callPrice;
        }
        /// <summary>
        /// calculate put shout option
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>put shout option price</returns>
        public double CalculatePutShoutOption(double S, double K, double r, double T, int n, int N)
        {
            if (S <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T ,n ,N> 0");

            return CalculateCallShoutOption(S, K, r, T, n, N) - S + K * Math.Exp(-r * (T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns></returns>
        public double CalculateCallRainBowOption(double[] S, double K, double r, double T, int n, int N)
        {
            if (S.Min() <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T , n , N> 0");
            GeneratePath pathS = new GeneratePath(kappa, theta, sigma, rho, v0);
            double[] pathOfS = new double[N + 1];
            double[] callPriceS = new double[n];
            double callPrice = 0;
            double[] Price = new double[S.Length];
            for (int j = 0; j < n; j++)
            {
                for (int i = 0; i < S.Length; i++)
                {
                    pathOfS = pathS.CreatePathMultiT(S[i], K, T, N, r);
                    Price[i] = pathOfS[N];
                }
                callPriceS[j] = Math.Max(Price.Max() - K, 0);
            }
            callPrice = callPriceS.Sum()*1.0 / n;
            return Math.Exp(-r * T) * callPrice;
        }
        /// <summary>
        /// Calculate Put RainBow Option
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="T">Manuity</param>
        /// <param name="N">time steps</param>
        /// <param name="n">Monte-Carlo number</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>Put RainBow Option price</returns>
        public double CalculatePutRainBowOption(double[] S, double K, double r, double T, int n, int N)
        {
            if (S.Min() <= 0 || K <= 0 || T <= 0 || n <= 0 || N <= 0)
                throw new System.ArgumentException("Need S, K, T ,n ,N> 0");
            GeneratePath pathS = new GeneratePath(kappa, theta, sigma, rho, v0);
            double[] pathOfS = new double[N + 1];
            double putPrice = 0;
            for (int j = 1; j <= n; j++)
            {
                for (int i = 0; i < S.Length; i++)
                {
                    S[i] = pathS.CreatePathMultiT(S[i], K, T, N, r)[N];
                }
                putPrice += Math.Max(K- S.Max() , 0);
            }
            putPrice *= 1.0 / n;
            return Math.Exp(-r * T) * putPrice;
        }

    }
}
