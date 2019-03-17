namespace HestonModel.Interfaces
{
    public interface IOption
    {
        double Maturity { get; }  //Option maturity as a year fraction (i.e. 1 means one year)
    }
}
