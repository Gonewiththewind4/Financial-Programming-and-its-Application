namespace HestonModel.Interfaces
{
    public interface IHestonModelParameters
    {
        double InitialStockPrice { get; }
        double RiskFreeRate { get; }
        IVarianceProcessParameters VarianceParameters { get; }
        
    }
}
