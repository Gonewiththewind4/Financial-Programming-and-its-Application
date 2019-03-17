using HestonModel.Interfaces;

public class CalibrationSettingsGrading : ICalibrationSettings
{
    public double Accuracy { get; private set; }

    public int MaximumNumberOfIterations { get; private set; }

    public CalibrationSettingsGrading(double accuracy, int maximumNumberOfIterations)
    {
        Accuracy = accuracy;
        MaximumNumberOfIterations = maximumNumberOfIterations;
    }
}


