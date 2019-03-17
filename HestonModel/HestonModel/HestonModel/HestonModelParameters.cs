using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HestonModel.Interfaces;

namespace HestonModel
{
    /// <summary>
    /// These class is used to implement the Interface. 
    /// Each class can implement one Interface at a time.
    /// </summary>
    public class HestonOption : IOption
    {
        public double Maturity { get; private set; }
        public HestonOption()
        {
            this.Maturity = 15;
        }
        public HestonOption(double T)
        {
            this.Maturity = T;
        }
    }
    public class HestonParameters : IVarianceProcessParameters
    {

        public double Kappa { get; private set; }//  Mean reversion speed in Heston model
        public double Theta { get; private set; }// The long-term mean in Heston model
        public double Sigma { get; private set; }// The vol of vol in Heston model
        public double Rho { get; private set; }// Initial variance in Heston model
        public double V0 { get; private set; }// The correlation between asset price and vol of vol in Heston model
        public HestonParameters()
        {
            this.Kappa = 1.5768;
            this.Theta = 0.0398;
            this.Sigma = 0.5751;
            this.V0 = 0.0175;
            this.Rho = -0.5711;
        }
        public HestonParameters(double kappa, double theta, double sigma, double rho, double v0)
        {
            this.Theta = theta;
            this.Kappa = kappa;
            this.Sigma = sigma;
            this.Rho = rho;
            this.V0 = v0;
        }
    }
    public class HestonMonteCarloSetting : IMonteCarloSettings
    {
        public int NumberOfTrials { get; private set; }
        public int NumberOfTimeSteps { get; private set; }
        public HestonMonteCarloSetting()
        {
            this.NumberOfTrials = 100000;
            this.NumberOfTimeSteps = 365;
        }
        public HestonMonteCarloSetting(int N, int n)
        {
            this.NumberOfTrials = n;
            this.NumberOfTimeSteps = N;
        }
    }

    public class HestonCalibrationSettings : ICalibrationSettings
    {
        public double Accuracy { get; private set; }
        public int MaximumNumberOfIterations { get; private set; }
        public HestonCalibrationSettings()
        {
            this.Accuracy = 100000;
            this.MaximumNumberOfIterations = 365;
        }
        public HestonCalibrationSettings(double accuracy, int maxiIter)
        {
            this.Accuracy = accuracy;
            this.MaximumNumberOfIterations = maxiIter;
        }
    }
    public class HestonOptionMarketData : IOptionMarketData<IOption>
    {

        public IOption Option { get; private set; }
        public double Price { get; private set; }
        public HestonOptionMarketData()
        {
            this.Option = new HestonOption(1);
            this.Price = 25.72;
        }
        public HestonOptionMarketData(IOption T, double price)
        {
            this.Option = T;
            this.Price = price;
        }

     }
    public class HestonMonteCarloParameters : IHestonModelParameters
    {

        public double InitialStockPrice { get; private set; }
        public double RiskFreeRate { get; private set; }
        public IVarianceProcessParameters VarianceParameters { get; private set; }
        public HestonMonteCarloParameters()
        {
            this.InitialStockPrice = 100;
            this.RiskFreeRate = 0.1;
            this.VarianceParameters = new HestonParameters(2, 0.06, 0.4, 0.5, 0.04);
        }
        public HestonMonteCarloParameters(double S, double r, double kappa, double theta, double sigma, double rho, double v0)
        {
            this.InitialStockPrice = 100;
            this.RiskFreeRate = 0.1;
            this.VarianceParameters = new HestonParameters(kappa, theta, sigma, rho, v0);
        }

    }
    public class HestonEuropeanOption : IEuropeanOption
    {
        public PayoffType Type { get; private set; }
        public double StrikePrice { get; private set; }
        public double Maturity { get; private set; }
        public HestonEuropeanOption()
        {
            this.Type = PayoffType.Call;
            this.StrikePrice = 100;
            this.Maturity = 1;
        }
        public HestonEuropeanOption(PayoffType Type, double K, double maturity)
        {
            this.Type = Type;
            this.StrikePrice = K;
            this.Maturity = maturity;
        }

    }
    public class HestonAsianOption : IAsianOption
    {
        public IEnumerable<double> MonitoringTimes { get; private set; }
        public PayoffType Type { get; private set; }
        public double StrikePrice { get; private set; }
        public double Maturity { get; private set; }
        public HestonAsianOption()
        {
            this.MonitoringTimes = new double[] { 0.75, 1.00 };
            this.Type = PayoffType.Call;
            this.StrikePrice = 100;
            this.Maturity = 1;
        }
        public HestonAsianOption(double[] TM, PayoffType type, double K, double maturity)
        {
            this.MonitoringTimes = TM;
            this.Type = type;
            this.StrikePrice = K;
            this.Maturity = maturity;
        }
    }
    public class HestonCalibrationResult : IHestonCalibrationResult
    {
        public IHestonModelParameters Parameters { get; private set; }

        public CalibrationOutcome MinimizerStatus { get; private set; }

        public double PricingError { get; private set; }

        public HestonCalibrationResult(IHestonModelParameters parameters, CalibrationOutcome minimizerStatus, double pricingError)
        {
            Parameters = parameters;
            MinimizerStatus = minimizerStatus;
            PricingError = pricingError;
        }

    }
    public class HestonParametersTotal : IHestonModelParameters
    {
        public double InitialStockPrice { get; private set; }

        public double RiskFreeRate { get; private set; }

        public IVarianceProcessParameters VarianceParameters { get; private set; }

        public HestonParametersTotal(double initialStockPrice, double riskFreeRate, double kappa, double theta, double sigma, double rho, double v0)
        {
            if (2 * kappa * theta <= sigma * sigma)
            {
                Console.WriteLine("Feller condition 2 kappa theta > sigma^2 condition violated.");
            }
            InitialStockPrice = initialStockPrice;
            RiskFreeRate = riskFreeRate;
            VarianceParameters = new HestonParameters(kappa, theta, sigma, rho, v0);
            //VarianceParameters = new VarianceProcessParametersGrading(kappa, theta, sigma, rho, v0);
        }
    }

}
