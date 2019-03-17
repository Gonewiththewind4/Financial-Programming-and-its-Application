using System;
using System.Collections.Generic;
using ExcelDna.Integration;
using HestonModel;
using System.Threading.Tasks;

namespace HestonXL
{
    public class HestonXLInterface
    {      
        static HestonXLInterface()
        {
        }

        [ExcelFunction(IsThreadSafe = true)]
        public static object CheckIfAddInLoaded()
        {
            return "true";
        }

        [ExcelFunction(IsThreadSafe = true)]
        public static object HestonOneOptionPrice(double underlying,
                                                double riskFreeRate,
                                                double kappa,
                                                double theta,
                                                double sigma,
                                                double rho,
                                                double v0,
                                                double maturity,
                                                double strike,
                                                string type,
                                                [ExcelArgument(Description = "Timeout set to 1 min if not specified")]
                                                int timeOutInMinutes)
        {

            try
            {
                if (ExcelDnaUtil.IsInFunctionWizard()) return null;

                if (timeOutInMinutes.Equals(0))
                {
                    timeOutInMinutes = 1;
                }      

                var task = Task.Run(() => Heston.HestonEuropeanOptionPrice(new HestonParametersTotal(underlying, riskFreeRate, kappa, theta, sigma, rho, v0),
                    new EuropeanOptionGrading(maturity, strike, GetTypeOfVanillaEuropean(type))));
                if (task.Wait(TimeSpan.FromMinutes(timeOutInMinutes)))
                    return task.Result;
                else
                    throw new Exception($"Timed out ({timeOutInMinutes} mins)");
            }
            catch (Exception e)
            {
                return "HestonOneOptionPrice: unknown error: " + e.Message;
            }
        }

        [ExcelFunction(IsThreadSafe = true)]
        public static object HestonOneOptionPriceMC(double underlying,
                                                double riskFreeRate,
                                                double kappa,
                                                double theta,
                                                double sigma,
                                                double rho,
                                                double v0,
                                                double maturity,
                                                double strike,
                                                string type,
                                                int numSamplePaths,
                                                int numSteps,
                                                [ExcelArgument(Description = "Timeout set to 5 mins if not specified")]
                                                int timeOutInMinutes )
        {
            try
            {
                if (ExcelDnaUtil.IsInFunctionWizard()) return null;

                if (timeOutInMinutes.Equals(0))
                {
                    timeOutInMinutes = 5;
                }

                var task = Task.Run(() => Heston.HestonEuropeanOptionPriceMC(
                    new HestonParametersTotal(underlying, riskFreeRate, kappa, theta, sigma, rho, v0),
                    new EuropeanOptionGrading(maturity, strike, GetTypeOfVanillaEuropean(type)),
                    new MonteCarloSettingsGrading(numSamplePaths, numSteps)));
                if (task.Wait(TimeSpan.FromMinutes(timeOutInMinutes)))
                    return task.Result;
                else
                    throw new Exception($"Timed out ({timeOutInMinutes} mins)");
            }
            catch (Exception e)
            {
                return "HestonOneOptionPriceMC: unknown error: " + e.Message;
            }
        }

        [ExcelFunction(IsThreadSafe = true)]
        public static object CalibrateHestonParameters(object guessModelParameters,
                                                    double riskFreeRate,
                                                    double underlyingPrice,
                                                    object strikes,
                                                    object maturities,
                                                    object type,
                                                    object observedPrices,
                                                    double accuracy,
                                                    int maxIterations,
                                                    [ExcelArgument(Description = "Timeout set to 10 mins if not specified")]
                                                    int timeOutInMinutes)
        {
            try
            {
                if (ExcelDnaUtil.IsInFunctionWizard()) return null;

                if (timeOutInMinutes.Equals(0))
                {
                    timeOutInMinutes = 10;
                }

                double[] strikesArray = ConvertToVector<double>(strikes);
                double[] maturitiesArray = ConvertToVector<double>(maturities);
                string[] optTypeArray = ConvertToVector<string>(type);
                double[] observedPricesArray = ConvertToVector<double>(observedPrices);

                if (strikesArray.Length != maturitiesArray.Length
                    || maturitiesArray.Length != optTypeArray.Length
                    || optTypeArray.Length != observedPricesArray.Length)
                {
                    // must improve error message display
                    return null;
                }

                HestonParametersTotal guessHestonModelParams = ParseParameters(guessModelParameters, underlyingPrice, riskFreeRate);

                if (guessHestonModelParams == null)
                {
                    return null;
                }


                int numObservedOptions = strikesArray.Length;
                var marketData = new OptionMarketDataGrading[numObservedOptions];
                for (int optionIdx = 0; optionIdx < numObservedOptions; ++optionIdx)
                {
                    marketData[optionIdx] = new OptionMarketDataGrading(
                        new EuropeanOptionGrading(
                            maturitiesArray[optionIdx],
                            strikesArray[optionIdx],
                            GetTypeOfVanillaEuropean(optTypeArray[optionIdx])),
                        observedPricesArray[optionIdx]);

                }
                var calibrationSettings = new CalibrationSettingsGrading(accuracy, maxIterations);

                var task = Task.Run(() =>
                {
                    var result = Heston.CalibrateHestonParameters(guessHestonModelParams, marketData, calibrationSettings);

                    const int numCols = 2;
                    const int numRows = 7;
                    object[,] output = new object[numRows, numCols];
                    output[0, 0] = "Kappa"; output[0, 1] = result.Parameters.VarianceParameters.Kappa;
                    output[1, 0] = "Theta"; output[1, 1] = result.Parameters.VarianceParameters.Theta;
                    output[2, 0] = "Sigma"; output[2, 1] = result.Parameters.VarianceParameters.Sigma;
                    output[3, 0] = "Rho"; output[3, 1] = result.Parameters.VarianceParameters.Rho;
                    output[4, 0] = "v0"; output[4, 1] = result.Parameters.VarianceParameters.V0;
                    output[5, 0] = "Minimizer Status";
                    if (result.MinimizerStatus == CalibrationOutcome.FinishedOK)
                        output[5, 1] = "OK";
                    else if (result.MinimizerStatus == CalibrationOutcome.FailedMaxItReached)
                        output[5, 1] = "Reached max. num. iterations.";
                    else if (result.MinimizerStatus == CalibrationOutcome.FailedOtherReason)
                        output[5, 1] = "Failed.";
                    else
                        output[5, 1] = "Unknown outcome.";
                    output[6, 0] = "Pricing error"; output[6, 1] = result.PricingError;
                    return output;
                });

                if (task.Wait(TimeSpan.FromMinutes(timeOutInMinutes)))
                    return task.Result;
                else
                    throw new Exception($"Timed out ({timeOutInMinutes} mins)");
            }
            catch (Exception e)
            {
                return "CalibrateHestonParameters: unknown error: " + e.Message;
            }
        }

        // Price a call or put Asian option in the Heston model using the 
        // a Monte-Carlo method. Accuracy will depend on numner of time steps and samples
        // Parameters as in HestonOneOptionPriceMC except the additional:
        // monitoringTimes: a M by 1 Excel range of times (expressed as year fraction) 
        // denoting the times over which the average is calculated.
        [ExcelFunction(IsThreadSafe = true)]
        public static object HestonAsianOptionPriceMC(double underlying,
                                                        double riskFreeRate,
                                                        double kappa,
                                                        double theta,
                                                        double sigma,
                                                        double rho,
                                                        double v0,
                                                        double maturity,
                                                        double strike,
                                                        object monitoringTimes,
                                                        string type,
                                                        int numSamplePaths,
                                                        int numSteps,
                                                        [ExcelArgument(Description = "Timeout set to 5 mins if not specified")]
                                                        int timeOutInMinutes)
        {
            try
            {
                if (ExcelDnaUtil.IsInFunctionWizard()) return null;

                if (timeOutInMinutes.Equals(0))
                {
                    timeOutInMinutes = 5;
                }

                var task = Task.Run(() => Heston.HestonAsianOptionPriceMC(
                    new HestonParametersTotal(underlying, riskFreeRate, kappa, theta, sigma, rho, v0),
                    new AsianOptionGrading(ConvertToVector<double>(monitoringTimes), maturity, strike, GetTypeOfVanillaEuropean(type)),
                    new MonteCarloSettingsGrading(numSamplePaths, numSteps)));
                if (task.Wait(TimeSpan.FromMinutes(timeOutInMinutes)))
                    return task.Result;
                else
                    throw new Exception($"Timed out ({timeOutInMinutes} mins)");
            }
            catch (Exception e)
            {
                return "HestonAsianOptionPriceMC: unknown error: " + e.Message;
            }
        }

        // Price a lookback option in the Heston model using the 
        // a Monte-Carlo method. Accuracy will depend on number of time steps and samples
        // Parameters as in HestonOneOptionPriceMC.
        [ExcelFunction(IsThreadSafe = true)]
        public static object HestonLookbackOptionPriceMC(double underlying,
                                                        double riskFreeRate,
                                                        double kappa,
                                                        double theta,
                                                        double sigma,
                                                        double rho,
                                                        double v0,
                                                        double maturity,
                                                        int numSamplePaths,
                                                        int numSteps,
                                                        [ExcelArgument(Description = "Timeout set to 5 mins if not specified")]
                                                        int timeOutInMinutes)
        {
            try
            {
                if (ExcelDnaUtil.IsInFunctionWizard()) return null;

                if (timeOutInMinutes.Equals(0))
                {
                    timeOutInMinutes = 5;
                }

                var task = Task.Run(() => Heston.HestonLookbackOptionPriceMC(
                    new HestonParametersTotal(underlying, riskFreeRate, kappa, theta, sigma, rho, v0),
                    new OptionGrading(maturity),
                    new MonteCarloSettingsGrading(numSamplePaths, numSteps)));

                if (task.Wait(TimeSpan.FromMinutes(timeOutInMinutes)))
                    return task.Result;
                else
                    throw new Exception($"Timed out ({timeOutInMinutes} mins)");
            }
            catch (Exception e)
            {
                return "HestonAsianOptionPriceMC: unknown error: " + e.Message;
            }
        }
        
        private static HestonParametersTotal ParseParameters(object guessModelParameters, double underlying, double riskFreeRate)
        {
            KeyValuePair<string, double>[] paramPairs = ConvertToKeyValuePairs(guessModelParameters);
            if (paramPairs.Length != 5)
            {
                throw new ArgumentException("Expecting 5 parameters", nameof(guessModelParameters));
            }

            double kappa = double.NaN, theta = double.NaN, sigma = double.NaN, rho = double.NaN, v0 = double.NaN;
            for (int paramIdx = 0; paramIdx < paramPairs.Length; ++paramIdx)
            {
                KeyValuePair<string, double> pair = paramPairs[paramIdx];
                string key = pair.Key;
                double value = pair.Value;
                if (key.Equals("kappa", StringComparison.CurrentCultureIgnoreCase))
                    kappa = value;
                else if (key.Equals("theta", StringComparison.CurrentCultureIgnoreCase))
                    theta = value;
                else if (key.Equals("sigma", StringComparison.CurrentCultureIgnoreCase))
                    sigma = value;
                else if (key.Equals("rho", StringComparison.CurrentCultureIgnoreCase))
                    rho = value;
                else if (key.Equals("v0", StringComparison.CurrentCultureIgnoreCase))
                    v0 = value;
                else
                {
                    throw new InvalidOperationException("HestonOptionPrice: keys are: underlyingPrice, riskFreeRate, kappa, theta, sigma, rho, v0.");
                }
            }
            return new HestonParametersTotal(underlying, riskFreeRate, kappa, theta, sigma, rho, v0);
        }

        private static T ConvertTo<T>(object In)
        {
            try
            {
                return (T)In;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not convert object to " + typeof(T).ToString(),e);
            }
        }

        private static T[] ConvertToVector<T>(object In)
        {
            T[] V;
            try
            {
                object[] InVec;
                if (In.GetType() == typeof(object) || In.GetType() == typeof(T))
                {
                    V = new T[1];
                    V[0] = ConvertTo<T>(In);
                    return V;
                }
                else if (In.GetType() == typeof(object[]))
                {
                    InVec = (object[])In;
                    int length = InVec.GetLength(0);
                    V = new T[length];
                    for (int i = 0; i < length; i++)
                    {
                        V[i] = ConvertTo<T>(InVec[i]);
                    }
                    return V;
                }
                else if (In.GetType() == typeof(object[,]))
                {
                    object[,] InM = (object[,])In;
                    int rows = InM.GetLength(0);
                    V = new T[rows];
                    for (int i = 0; i < rows; i++)
                    {
                        V[i] = ConvertTo<T>(InM[i, 0]);
                    }
                    return V;
                }
                else
                {
                    throw new InvalidOperationException("Could not convert input to vector of type " + typeof(T).ToString());
                }


            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not convert input to vector of type " + typeof(T).ToString(), e);
            }
        }

        private static T[,] ConvertToMatrix<T>(object In)
        {
            T[,] M;
            try
            {
                object[,] InM = (object[,])In;
                int rows = InM.GetLength(0);
                int cols = InM.GetLength(1);

                M = new T[rows, cols];
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                        M[i, j] = ConvertTo<T>(InM[i, j]);
                }
                return M;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could not convert input to matrix of type " + typeof(T).ToString(),e);
            }
        }

        private static KeyValuePair<string, double>[] ConvertToKeyValuePairs(object In)
        {
            KeyValuePair<string, double>[] keyValPairs;
            try
            {
                object[,] In2D = (object[,])In;
                int rows = In2D.GetLength(0);
                int cols = In2D.GetLength(1);
                if (cols != 2)
                {
                    Console.WriteLine("Need two colums!");
                    return null;
                }
                keyValPairs = new KeyValuePair<string, double>[rows];
                for (int i = 0; i < rows; i++)
                {
                    string key = ConvertTo<string>(In2D[i, 0]);
                    double value = ConvertTo<double>(In2D[i, 1]);
                    KeyValuePair<string, double> pair = new KeyValuePair<string, double>(key, value);
                    keyValPairs[i] = pair;
                }
                return keyValPairs;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Could create key - value pair.", e);
            }
        }

        private static PayoffType GetTypeOfVanillaEuropean(string s)
        {
            if (ItIsCall(s))
                return PayoffType.Call;
            else
                return PayoffType.Put;
        }

        private static bool ItIsCall(string s)
        {
            if (s.Equals("call", StringComparison.CurrentCultureIgnoreCase) || s == "C" || s == "c")
                return true;
            else if (s.Equals("put", StringComparison.CurrentCultureIgnoreCase) || s == "P" || s == "p")
                return false;
            else
            {
                throw new ArgumentException("Type must be one of call / put or c / p or C / P; this: " + s + " was not recognised.");
            }
        }
    }
}
