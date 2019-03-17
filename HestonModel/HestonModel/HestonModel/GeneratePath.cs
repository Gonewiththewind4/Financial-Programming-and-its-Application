using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace HestonModel
{
    /// <summary>
    /// generate path for MonteCarlo method
    /// </summary>
    class GeneratePath
    {
        private double Theta;
        private double Kappa;
        private double sigma;
        private double Rho;
        private double V0;
        /// <summary>
        /// default parameters
        /// </summary>
        public GeneratePath()
        {

            Theta = 0.06;
            Kappa = 2;
            sigma = 0.4;
            Rho = 0.5;
            V0 = 0.04;

        }
        /// <summary>
        /// get real data to generate path
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        public GeneratePath(double Kappa, double Theta, double sigma, double Rho, double V0)
        {

            this.Theta = Theta;
            this.Kappa = Kappa;
            this.sigma = sigma;
            this.Rho = Rho;
            this.V0 = V0;
        }
        /// <summary>
        /// create single path and return only one data:S(T)
        /// </summary>
        /// <param name="price">initial price</param>
        /// <param name="Strike">StrikePrice</param>
        /// <param name="OptionexerciseT">Manuity</param>
        /// <param name="N">time step</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>S(T)</returns>
        public double CreatePathSingleT(double price, double Strike, double OptionexerciseT, int N, double r)
        {

            double tau = OptionexerciseT / N;
            double Alpha = (4.0 * Kappa * Theta - Math.Pow(sigma, 2)) / 8.0;
            double Belta = -Kappa * 0.5;
            double Gamma = sigma * 0.5;
            double x1 = Normal.Sample(0, 1);
            double x2 = Normal.Sample(0, 1);
            double temp1 = 0;
            double temp2 = 0;
            double yn0 = Math.Sqrt(V0);
            double[] S = new double[N + 1];
            double[] yn = new double[N + 1];
            double[] Z1 = new double[N + 1];
            double[] Z2 = new double[N + 1];
            S[0] = price;
            yn[0] = yn0;
            Z1[0] = Math.Sqrt(tau) * x1;
            Z2[0] = Math.Sqrt(tau) * (Rho * x1 + Math.Sqrt(1 - Math.Pow(Rho, 2)) * x2);
            for (int i = 1; i <= N; i++)
            {

                x1 = Normal.Sample(0, 1);
                x2 = Normal.Sample(0, 1);
                Z1[i] = Math.Sqrt(tau) * x1;
                Z2[i] = Math.Sqrt(tau) * (Rho * x1 + Math.Sqrt(1 - Math.Pow(Rho, 2)) * x2);
                temp1 = (1 - Belta * tau);
                temp2 = Math.Abs(yn[i - 1]) + Gamma * Z2[i];
                S[i] = S[i - 1] + r * S[i - 1] * tau + Math.Abs(yn[i - 1]) * S[i - 1] * Z1[i];
                yn[i] = temp2 / temp1 * 0.5 + Math.Sqrt(Math.Pow(temp2 / temp1 * 0.5, 2) + Alpha * tau / temp1);
            }
            return S[N];

        }
        /// <summary>
        /// create a path and return a path other than a data
        /// </summary>
        /// <param name="price">initial price</param>
        /// <param name="Strike">StrikePrice</param>
        /// <param name="OptionexerciseT">Manuity</param>
        /// <param name="N">time step</param>
        /// <param name="r">risk-free-rate</param>
        /// <returns>the whole price path</returns>

        public double[] CreatePathMultiT(double price, double Strike, double OptionexerciseT, int N, double r)
        {
            double tau = OptionexerciseT / N;
            double Alpha = (4.0 * Kappa * Theta - Math.Pow(sigma, 2)) / 8.0;
            double Belta = -Kappa * 0.5;
            double Gamma = sigma * 0.5;
            double[] x1 = new double[N + 1];
            Normal.Samples(x1, 0, 1);
            double[] x2 = new double[N + 1];
            Normal.Samples(x2, 0, 1);
            double yn0 = Math.Sqrt(V0);
            double[] S = new double[N + 1];
            double[] yn = new double[N + 1];
            double[] Z1 = new double[N + 1];
            double[] Z2 = new double[N + 1];
            S[0] = price;
            yn[0] = yn0;
            Z1[0] = Math.Sqrt(tau) * x1[0];
            Z2[0] = Math.Sqrt(tau) * (Rho * x1[0] + Math.Sqrt(1 - Math.Pow(Rho, 2)) * x2[0]);
            for (int i = 1; i <= N; i++)
            {

                Z1[i] = Math.Sqrt(tau) * x1[i];
                Z2[i] = Math.Sqrt(tau) * (Rho * x1[i] + Math.Sqrt(1 - Math.Pow(Rho, 2)) * x2[i]);
                S[i] = S[i - 1] + r * S[i - 1] * tau + Math.Abs(yn[i - 1]) * S[i - 1] * Z1[i];
                yn[i] = (yn[i - 1] + Gamma * Z2[i]) / ((1 - Belta * tau) * 2.0) +
                         Math.Sqrt(Math.Pow((yn[i - 1] + Gamma * Z2[i]) /
                        ((1 - Belta * tau) * 2.0), 2) + Alpha * tau / (1 - Belta * tau));
            }
            return S;
        }
    }
}
