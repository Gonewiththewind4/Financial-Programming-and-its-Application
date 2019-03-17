using HestonModel;
using HestonModel.Interfaces;

public class EuropeanOptionGrading : OptionGrading, IEuropeanOption
{
    public double StrikePrice { get; private set; }

    PayoffType IEuropeanOption.Type { get => _type; }

    private PayoffType _type;

    public EuropeanOptionGrading(double timeToExpiry, double strike, PayoffType type) : base(timeToExpiry)
    {
        StrikePrice = strike;
        _type = type;
    }
}
            