using HestonModel.Interfaces;
using System;
/*
public class HestonParametersGrading : IHestonModelParameters
{
    public double InitialStockPrice { get; private set; }

    public double RiskFreeRate { get; private set; }

    public IVarianceProcessParameters VarianceParameters { get; private set; }
       
    public HestonParametersGrading(double initialStockPrice, double riskFreeRate, double kappa, double theta, double sigma, double rho, double v0)
    {
        if (2 * kappa * theta <= sigma * sigma)
        {
            Console.WriteLine("Feller condition 2 kappa theta > sigma^2 condition violated.");
        }
        InitialStockPrice = initialStockPrice;
        RiskFreeRate = riskFreeRate;
        VarianceParameters = new VarianceProcessParametersGrading(kappa, theta, sigma, rho, v0);

    }
}
*/
