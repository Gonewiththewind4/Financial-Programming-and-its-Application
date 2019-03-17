namespace HestonModel.Interfaces
{
    public interface IHestonCalibrationResult : ICalibrationResult
    {
        IHestonModelParameters Parameters { get; }
    }
}
