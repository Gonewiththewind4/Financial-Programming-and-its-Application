namespace HestonModel.Interfaces
{
    public interface ICalibrationResult
    {
        CalibrationOutcome MinimizerStatus { get; }
        double PricingError { get; }
    }
}
