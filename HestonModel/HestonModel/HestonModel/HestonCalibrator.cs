using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HestonModel
{
    /// <summary>
    /// this class is used for calibration
    /// </summary>
    public class CalibrationFailedException : Exception
    {
        public CalibrationFailedException()
        {
        }
        public CalibrationFailedException(string message)
            : base(message)
        {
        }
    }


    public struct HestonModelCallOptionMarketData
    {

        public double optionExercise;
        public double strike;
        public double marketMidPrice;
    }

    public class HestonCalibrator
    {
        private const double defaultAccuracy = 10e-3;
        private const int defaultMaxIterations = 1000;
        private double accuracy;
        private int maxIterations;

        private LinkedList<HestonModelCallOptionMarketData> marketOptionsList;
        private double r; // initial interest rate, this is observed, no need to calibrate to options
        private double S;
        private CalibrationOutcome outcome;

        private double[] calibratedParams;
        /// <summary>
        /// default parameters
        /// </summary>
        public HestonCalibrator()
        {
            accuracy = defaultAccuracy;
            maxIterations = defaultMaxIterations;
            marketOptionsList = new LinkedList<HestonModelCallOptionMarketData>();
            r = 0.025;
            S = 100;
            calibratedParams = new double[] { 1.5768, 0.0398, 0.5751, -0.5711, 0.0175 };
        }
        /// <summary>
        /// get parameters
        /// </summary>
        /// <param name="S"></param>
        /// <param name="r"></param>
        /// <param name="accuracy"></param>
        /// <param name="maxIterations"></param>
        public HestonCalibrator(double S, double r, double accuracy, int maxIterations)
        {
            this.S = S;
            this.r = r;
            this.accuracy = accuracy;
            this.maxIterations = maxIterations;
            marketOptionsList = new LinkedList<HestonModelCallOptionMarketData>();
            calibratedParams = new double[] { 1.5768, 0.0398, 0.5751, -0.5711, 0.0175 };
        }
        /// <summary>
        /// set up initial parameters
        /// </summary>
        /// <param name="Kappa">Mean reversion speed in Heston model</param>
        /// <param name="Theta">The long-term mean in Heston model</param>
        /// <param name="Sigma">The vol of vol in Heston model</param>
        /// <param name="Rho">Initial variance in Heston model</param>
        /// <param name="V0">The correlation between asset price and vol of vol in Heston model</param>
        public void SetGuessParameters(double Kappa, double Theta, double sigma, double Rho, double V0)
        {
            HestonModelFormula m = new HestonModelFormula(Kappa, Theta, sigma, Rho, V0);
            calibratedParams = m.ConvertHestonModelCalibrationParamsToArray();
        }
        /// <summary>
        /// add data to the calibrator
        /// </summary>
        /// <param name="strike">K</param>
        /// <param name="optionExercise">Manuity</param>
        /// <param name="mktMidPrice">Market Price</param>
        public void AddObservedOption(double strike, double optionExercise, double mktMidPrice)
        {
            HestonModelCallOptionMarketData observedOption;

            observedOption.optionExercise = optionExercise;
            observedOption.strike = strike;
            observedOption.marketMidPrice = mktMidPrice;
            marketOptionsList.AddLast(observedOption);
        }
        /// <summary>
        /// calculate error between market and formula price
        /// </summary>
        /// <param name="m"></param>
        /// <returns>error</returns>
        // Calculate difference between observed and model prices
        public double CalcMeanSquareErrorBetweenModelAndMarket(HestonModelFormula m)
        {
            double meanSqErr = 0;
            foreach (HestonModelCallOptionMarketData option in marketOptionsList)
            {

                double optionExercise = option.optionExercise;
                double strike = option.strike;
                double modelPrice = m.CalculateCallEuropeanOptionPrice(S, strike, r, optionExercise);//换成公式
                double difference = modelPrice - option.marketMidPrice;
                meanSqErr += difference * difference;
            }
            return meanSqErr;// marketOptionsList.Count;
        }

        // Used by Alglib minimisation algorithm
        public void CalibrationObjectiveFunction(double[] paramsArray, ref double func, object obj)
        {
            HestonModelFormula m = new HestonModelFormula(paramsArray);
            func = CalcMeanSquareErrorBetweenModelAndMarket(m);
        }
        /// <summary>
        /// implement the calibration
        /// </summary>
        /// <returns>resultparameters</returns>
        public double[] Calibrate()
        {
            outcome = CalibrationOutcome.NotStarted;

            double[] initialParams = new double[HestonModelFormula.numModelParams];
            //calibratedParams.CopyTo(initialParams, 0);  // a reasonable starting guees

            initialParams = calibratedParams;
            double epsg = accuracy;
            double epsf = accuracy; //1e-4;
            double epsx = accuracy;
            double diffstep = 1.0e-6;
            int maxits = maxIterations;
            double stpmax = 0.05;



            alglib.minlbfgsstate state;
            alglib.minlbfgsreport rep;
            alglib.minlbfgscreatef(1, initialParams, diffstep, out state);
            alglib.minlbfgssetcond(state, epsg, epsf, epsx, maxits);
            alglib.minlbfgssetstpmax(state, stpmax);

            // this will do the work
            alglib.minlbfgsoptimize(state, CalibrationObjectiveFunction, null, null);
            double[] resultParams = new double[HestonModelFormula.numModelParams];
            alglib.minlbfgsresults(state, out resultParams, out rep);
            double[] result = new double[3];
            
            
            System.Console.WriteLine("Termination type: {0}", rep.terminationtype);
            System.Console.WriteLine("Num iterations {0}", rep.iterationscount);
            System.Console.WriteLine("{0}", alglib.ap.format(resultParams, 5));
            

            if (rep.terminationtype == 1			// relative function improvement is no more than EpsF.
                || rep.terminationtype == 2			// relative step is no more than EpsX.
                || rep.terminationtype == 4)
            {    	// gradient norm is no more than EpsG
                outcome = CalibrationOutcome.FinishedOK;
                // we update the ''inital parameters''
                calibratedParams = resultParams;
            }
            else if (rep.terminationtype == 5)
            {	// MaxIts steps was taken
                outcome = CalibrationOutcome.FailedMaxItReached;
                // we update the ''inital parameters'' even in this case
                calibratedParams = resultParams;

            }
            else
            {
                outcome = CalibrationOutcome.FailedOtherReason;
                throw new CalibrationFailedException("Heston model calibration failed badly.");
            }
            //Console.WriteLine("Calibration outcome: {0} and error: ", outcome);
            return resultParams;
        }

        public void GetCalibrationStatus(ref CalibrationOutcome calibOutcome, ref double pricingError)
        {
            calibOutcome = outcome;

            HestonModelFormula m = new HestonModelFormula(calibratedParams);
            pricingError = CalcMeanSquareErrorBetweenModelAndMarket(m);

        }

        public HestonModelFormula GetCalibratedModel()
        {
            HestonModelFormula m = new HestonModelFormula(calibratedParams);
            return m;
        }
    }
}

