using HestonModel.Interfaces;

public class MonteCarloSettingsGrading : IMonteCarloSettings
{
    public int NumberOfTrials { get; private set; }

    public int NumberOfTimeSteps { get; private set; }

    public MonteCarloSettingsGrading(int numberOfTrials, int numberOfTimeSteps)
    {
        NumberOfTrials = numberOfTrials;
        NumberOfTimeSteps = numberOfTimeSteps;
    }
}

 
            