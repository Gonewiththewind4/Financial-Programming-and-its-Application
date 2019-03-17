using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics.Integration;

namespace HestonModel
{
    /// <summary>
    /// This class is use to calculate European option price from Heston Model Formula
    /// </summary>
    /// <param name="Kappa">Mean reversion speed in Heston model</param>
    /// <param name="Theta">The long-term mean in Heston model</param>
    /// <param name="Sigma">The vol of vol in Heston model</param>
    /// <param name="Rho">Initial variance in Heston model</param>
    /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
    /// <returns>Option price</returns>
    public class HestonModelFormula
    {
        public const int numModelParams = 5;
        private Complex i = new Complex(0.0, 1.0);

        private const int kIndex = 0;
        private const int thetaIndex = 1;
        private const int sigmaIndex = 2;
        private const int rhoIndex = 3;
        private const int v0Index = 4;

        public double Theta { get; private set; }
        public double Kappa { get; private set; }
        public double Sigma { get; private set; }
        public double Rho { get; private set; }
        public double V0 { get; private set; }
        public HestonModelFormula()
        {
            Theta = 0.06;
            Kappa = 2;
            Sigma = 0.4;
            Rho = 0.5;
            V0 = 0.04;
        }
       
        public HestonModelFormula(double Kappa, double Theta, double Sigma, double Rho, double V0)
        {
            this.Theta = Theta;
            this.Kappa = Kappa;
            this.Sigma = Sigma;
            this.Rho = Rho;
            this.V0 = V0;
        }
        public HestonModelFormula(double[] paramsArray)
                     : this(paramsArray[kIndex], paramsArray[thetaIndex], paramsArray[sigmaIndex],
                           paramsArray[rhoIndex], paramsArray[v0Index])
        {
        }
        /// <summary>
        /// This method is used to calculate European Call Option Price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <returns>European Call Option Price</returns>
        public double CalculateCallEuropeanOptionPrice(double S, double K, double r, double T)
        {
            return S * P(1, S, K, r, T) - Math.Exp(-r * T) * K * P(2, S, K, r, T);
        }
        /// <summary>
        /// Calculate call European Option price from put option
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <param name="putPrice">European Put Option Price</param>
        /// <returns>European Call Option Price</returns>
        public double GetCallFromPutPrice(double S, double K, double r, double T, double putPrice)
        {
            return putPrice - Math.Exp(-r * T) * K + S;
        }
        /// <summary>
        /// Calculate put European Option price from call option
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <param name="callPrice"></param>
        /// <returns>European put Option Price</returns>
        public double GetPutFromCallPrice(double S, double K, double r, double T, double callPrice)
        {
            return callPrice - S + K * Math.Exp(-r * T);
        }
        /// <summary>
        /// Calculate put European Option price
        /// </summary>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <returns>European put Option Price</returns>
        public double CalculatePutEuropeanOptionPrice(double S, double K, double r, double T)
        {
            if (Sigma <= 0 || T <= 0 || K <= 0 || S <= 0)
                throw new System.ArgumentException("Need sigma > 0, T > 0, K > 0 and S > 0.");

            return GetPutFromCallPrice(S, K, r, T, CalculateCallEuropeanOptionPrice(S, K, r, T));
        }
        /// <summary>
        /// The integrand andget return the real part of the integrand.
        /// </summary>
        /// <param name="Phi">Independent variable</param>
        /// <param name="Pnum">1,2</param>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <returns>The real part of the integrand</returns>
        private double HestonModelPFunc(double Phi, int Pnum, double S, double K, double r, double T)
        {
            double a = Kappa * Theta;
            double x = Math.Log(S);
            Complex b, u, d, g, D, G, C, f, integrand = new Complex();
            if (Pnum == 1)
            {
                u = 0.5;
                b = Kappa - Rho * Sigma;
            }
            else
            {
                u = -0.5;
                b = Kappa;
            }
            //Heston formula avoiding the little Heston trap
            d = Complex.Sqrt(Complex.Pow(Rho * Sigma * i * Phi - b, 2.0) - Sigma
                                        * Sigma * (2.0 * u * i * Phi - Phi * Phi));
            g = (b - Rho * Sigma * i * Phi - d) / (b - Rho * Sigma * i * Phi + d);
            G = (1.0 - g * Complex.Exp(-d * T)) / (1.0 - g);
            C = r * i * Phi * T + a / Sigma / Sigma * ((b - Rho * Sigma * i * Phi - d) * T - 2.0 * Complex.Log(G));
            D = (b - Rho * Sigma * i * Phi - d) / Sigma / Sigma * ((1.0 - Complex.Exp(-d * T)) / (1.0 - g * Complex.Exp(-d * T)));

            // The characteristic function.
            f = Complex.Exp(C + D * V0 + i * Phi * x);

            // The integrand andget return the real part of the integrand.

            integrand = Complex.Exp(-i * Phi * Math.Log(K)) * f / i / Phi;
            return integrand.Real;
        }
        /// <summary>
        /// return P function
        /// </summary>
        /// <param name="num">1 or 2</param>
        /// <param name="S">Initialprice</param>
        /// <param name="K">Strikeprice</param>
        /// <param name="r">Risk-free-rate</param>
        /// <param name="T">Option exercise T</param>
        /// <returns>P1 or P2</returns>
        private double P(int num, double S, double K, double r, double T)
        {
            int Pnumin = num;
            Func<double, double> IntegrateFormula = (x) =>
            {
                return HestonModelPFunc(x, Pnumin, S, K, r, T);
            };
            CompositeIntegrator integrator = new CompositeIntegrator(4);
            double integral = NewtonCotesTrapeziumRule.IntegrateComposite(IntegrateFormula, 0.01, 1000, 1000);
            //double integral = integrator.Integrate(IntegrateFormula, 0.1, 1000, 1000);
            //double integral = DoubleExponentialTransformation.Integrate(IntegrateFormula, 0.1, 1000, 1000);
            return 0.5 + 1 / Math.PI * integral;
        }
        /// <summary>
        ///  Convert Heston Model Calibration Parameters To Array
        /// </summary>
        /// <returns>parameters in array type</returns>
        public double[] ConvertHestonModelCalibrationParamsToArray()
        {

            double[] paramsArray = new double[HestonModelFormula.numModelParams];
            paramsArray[kIndex] = Kappa;
            paramsArray[thetaIndex] = Theta;
            paramsArray[sigmaIndex] = Sigma;
            paramsArray[rhoIndex] = Rho;
            paramsArray[v0Index] = V0;

            return paramsArray;

        }
    }
}
