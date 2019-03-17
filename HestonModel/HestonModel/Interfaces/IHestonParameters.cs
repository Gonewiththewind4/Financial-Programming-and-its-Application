namespace HestonModel.Interfaces
{
    public interface IVarianceProcessParameters
    {
        double Kappa { get; } //  Mean reversion speed in Heston model
        double Theta { get; } // The long-term mean in Heston model
        double Sigma { get; } // The vol of vol in Heston model
        double V0 { get; } // Initial variance in Heston model
        double Rho { get; } // The correlation between asset price and vol of vol in Heston model
    }
}
