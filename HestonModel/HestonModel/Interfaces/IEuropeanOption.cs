namespace HestonModel.Interfaces
{
    public interface IEuropeanOption : IOption
    {
        PayoffType Type { get; }
        double StrikePrice { get; }
    }
}
