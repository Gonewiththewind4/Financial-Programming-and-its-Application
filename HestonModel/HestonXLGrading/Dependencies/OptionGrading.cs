using HestonModel.Interfaces;

public class OptionGrading : IOption
{
    public double Maturity { get; private set; }

    public OptionGrading(double maturity)
    {
        Maturity = maturity;
    }
}
            