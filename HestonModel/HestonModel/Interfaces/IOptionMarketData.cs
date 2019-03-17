namespace HestonModel.Interfaces
{
    public interface IOptionMarketData<T> where T : IOption
    {
        T Option { get; }
        double Price { get; }
    }
}