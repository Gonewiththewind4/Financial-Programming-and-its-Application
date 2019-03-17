using System;
using System.Collections.Generic;
using HestonModel.Interfaces;

namespace HestonModel
{


    /// <summary> 
    /// This class will be used for grading. 
    /// Don't remove any of the methods and don't modify their signatures. Don't change the namespace. 
    /// Your code should be implemented in other classes (or even projects if you wish), and the relevant functionality should only be called here and outputs returned.
    /// You don't need to implement the interfaces that have been provided if you don't want to.
    /// </summary>
    public static class Heston
    {
        /// <summary>
        /// Method for calibrating the heston model.
        /// </summary>
        /// <param name="guessModelParameters">Object implementing IHestonModelParameters interface containing the risk-free rate, initial stock price
        /// and initial guess parameters to be used in the calibration.</param>
        /// <param name="referenceData">A collection of objects implementing IOptionMarketData<IEuropeanOption> interface. These should contain the reference data used for calibration.</param>
        /// <param name="calibrationSettings">An object implementing ICalibrationSettings interface.</param>
        /// <returns>Object implementing IHestonCalibrationResult interface which contains calibrated model parameters and additional diagnostic information</returns>
        
        public static IHestonCalibrationResult CalibrateHestonParameters(
                                                IHestonModelParameters guessModelParameters,
                                                IEnumerable<IOptionMarketData<IEuropeanOption>> referenceData,
                                                ICalibrationSettings calibrationSettings)
        {

            HestonCalibrator calibrator = new HestonCalibrator(guessModelParameters.InitialStockPrice,
                                                               guessModelParameters.RiskFreeRate,calibrationSettings.Accuracy,
                                                               calibrationSettings.MaximumNumberOfIterations);
            calibrator.SetGuessParameters(guessModelParameters.VarianceParameters.Kappa,
                                          guessModelParameters.VarianceParameters.Theta,
                                          guessModelParameters.VarianceParameters.Sigma,
                                          guessModelParameters.VarianceParameters.Rho,
                                          guessModelParameters.VarianceParameters.V0);

            foreach (IOptionMarketData<IEuropeanOption> M in referenceData)
            {
                calibrator.AddObservedOption(M.Option.StrikePrice, M.Option.Maturity, M.Price);
            }
            calibrator.Calibrate();

            double error = 0;
            CalibrationOutcome outcome = CalibrationOutcome.NotStarted;
            calibrator.GetCalibrationStatus(ref outcome, ref error);
            var calibratedModel = calibrator.GetCalibratedModel();

            var parameters = new HestonParametersTotal(guessModelParameters.InitialStockPrice,
                                                         guessModelParameters.RiskFreeRate, calibratedModel.Kappa, calibratedModel.Theta, calibratedModel.Sigma, calibratedModel.Rho, calibratedModel.V0);

            return new HestonCalibrationResult(parameters, outcome, error);
        }

    
        /// <summary>
        /// Price a European option in the Heston model using the Heston formula. This should be accurate to 5 decimal places
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="europeanOption">Object implementing IEuropeanOption interface, containing the option parameters.</param>
        /// <returns>Option price</returns>
        public static double HestonEuropeanOptionPrice(IHestonModelParameters parameters, IEuropeanOption europeanOption)
        {
            HestonModelFormula hestonModelFormula = new HestonModelFormula(parameters.VarianceParameters.Kappa,
                                         parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                                         parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);

            if (europeanOption.Type == PayoffType.Call)
            {
                return Math.Round(hestonModelFormula.CalculateCallEuropeanOptionPrice(parameters.InitialStockPrice,
                         europeanOption.StrikePrice, parameters.RiskFreeRate, europeanOption.Maturity), 5);
            }
            else
            {
                return Math.Round(hestonModelFormula.CalculatePutEuropeanOptionPrice(parameters.InitialStockPrice,
                         europeanOption.StrikePrice, parameters.RiskFreeRate, europeanOption.Maturity), 5);
            }
        }

        /// <summary>
        /// Price a European option in the Heston model using the Monte-Carlo method. Accuracy will depend on number of time steps and samples
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="europeanOption">Object implementing IEuropeanOption interface, containing the option parameters.</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonEuropeanOptionPriceMC(IHestonModelParameters parameters, IEuropeanOption europeanOption,
                                                            IMonteCarloSettings monteCarloSimulationSettings)
        {
            HestonModelMonteCarlo hestonModelMonteCarloEuropean = new HestonModelMonteCarlo(parameters.VarianceParameters.Kappa,
                                         parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                                         parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);
            if (europeanOption.Type == PayoffType.Call)
            {
                return hestonModelMonteCarloEuropean.CalculateEuropeanCallOption(parameters.InitialStockPrice,
                        europeanOption.StrikePrice, parameters.RiskFreeRate, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps, europeanOption.Maturity);
            }
            else
            {
                return hestonModelMonteCarloEuropean.CalculateEuropeanPutOptionPrice(parameters.InitialStockPrice,
                        europeanOption.StrikePrice, parameters.RiskFreeRate, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps, europeanOption.Maturity);
            }
        }

        /// <summary>
        /// Price a Asian option in the Heston model using the 
        /// Monte-Carlo method. Accuracy will depend on number of time steps and samples
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="asianOption">Object implementing IAsian interface, containing the option parameters.</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonAsianOptionPriceMC(IHestonModelParameters parameters, IAsianOption asianOption, IMonteCarloSettings monteCarloSimulationSettings)
        {
            HestonModelMonteCarlo hestonModelMonteCarloAsian = new HestonModelMonteCarlo(parameters.VarianceParameters.Kappa,
                             parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                             parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);

            if (asianOption.Type == PayoffType.Call)
            {
                return hestonModelMonteCarloAsian.CalculateAsianCallOptionPrice(parameters.InitialStockPrice,
                        asianOption.StrikePrice, asianOption.MonitoringTimes, asianOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps, parameters.RiskFreeRate);
                        
            }
            else
            {
                return hestonModelMonteCarloAsian.CalculateAsianPutOptionPrice(parameters.InitialStockPrice,
                        asianOption.StrikePrice, asianOption.MonitoringTimes, asianOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps, parameters.RiskFreeRate);
                        
            }
        }

        /// <summary>
        /// Price a lookback option in the Heston model using the  
        /// a Monte-Carlo method. Accuracy will depend on number of time steps and samples </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="maturity">An object implementing IOption interface and containing option's maturity</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonLookbackOptionPriceMC(IHestonModelParameters parameters,
                                            IOption maturity, IMonteCarloSettings monteCarloSimulationSettings)
        {
            HestonModelMonteCarlo hestonModelMonteCarloLookback = new HestonModelMonteCarlo(parameters.VarianceParameters.Kappa,
                        parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                        parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);

            return hestonModelMonteCarloLookback.CalculateLookbackOption(parameters.InitialStockPrice,
                    0, maturity.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                    monteCarloSimulationSettings.NumberOfTimeSteps, parameters.RiskFreeRate);

        }
        /// <summary>
        /// Price a Asian option in the Heston model using the 
        /// Monte-Carlo method. Accuracy will depend on number of time steps and samples
        /// </summary>
        /// <param name="parameters">Object implementing IHestonModelParameters interface, containing model parameters.</param>
        /// <param name="asianOption">Object implementing IAsian interface, containing the option parameters.</param>
        /// <param name="monteCarloSimulationSettings">An object implementing IMonteCarloSettings object and containing simulation settings.</param>
        /// <returns>Option price</returns>
        public static double HestonShoutOptionPriceMC(IHestonModelParameters parameters, IShoutOption shoutOption, IMonteCarloSettings monteCarloSimulationSettings)
        {
            ExoticOptionExploration hestonModelMonteCarloShout = new ExoticOptionExploration(parameters.VarianceParameters.Kappa,
                             parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                             parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);

            if (shoutOption.Type == PayoffType.Call)
            {
                return hestonModelMonteCarloShout.CalculateCallShoutOption(parameters.InitialStockPrice,
                        shoutOption.StrikePrice, parameters.RiskFreeRate,shoutOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps);

            }
            else
            {
                return hestonModelMonteCarloShout.CalculatePutShoutOption(parameters.InitialStockPrice,
                        shoutOption.StrikePrice, parameters.RiskFreeRate, shoutOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps);

            }
        }
        public static double HestonRainbowOptionPriceMC(IHestonModelParameters parameters, IRainbowOption rainbowOption, IMonteCarloSettings monteCarloSimulationSettings)
        {
            ExoticOptionExploration hestonModelMonteCarloRainbow = new ExoticOptionExploration(parameters.VarianceParameters.Kappa,
                             parameters.VarianceParameters.Theta, parameters.VarianceParameters.Sigma,
                             parameters.VarianceParameters.Rho, parameters.VarianceParameters.V0);

            if (rainbowOption.Type == PayoffType.Call)
            {
                return hestonModelMonteCarloRainbow.CalculateCallShoutOption(parameters.InitialStockPrice,
                        rainbowOption.StrikePrice, parameters.RiskFreeRate, rainbowOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps);

            }
            else
            {
                return hestonModelMonteCarloRainbow.CalculatePutShoutOption(parameters.InitialStockPrice,
                        rainbowOption.StrikePrice, parameters.RiskFreeRate, rainbowOption.Maturity, monteCarloSimulationSettings.NumberOfTrials,
                        monteCarloSimulationSettings.NumberOfTimeSteps);

            }
        }

    }
}
