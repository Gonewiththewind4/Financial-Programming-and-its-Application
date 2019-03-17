using System;
using HestonModel.Interfaces;

public class OptionMarketDataGrading : IEquatable<OptionMarketDataGrading>, IOptionMarketData<IEuropeanOption>
{
    // options with parameters equal up to delta will be considered equal
    private const double comparisonDelta = 1e-5;

    public IEuropeanOption Option { get; }

    public double Price { get; }

    public OptionMarketDataGrading(IEuropeanOption option, double price)
    {
        Option = option;
        Price = price;
    }

    public bool Equals(OptionMarketDataGrading other)
    {
        return (Math.Abs(Price - other.Price) < comparisonDelta)
        && (Math.Abs(Option.StrikePrice - other.Option.StrikePrice) < comparisonDelta)
        && (Math.Abs(Option.Maturity - other.Option.Maturity) < comparisonDelta)
        && Option.Type == other.Option.Type;
    }
}


