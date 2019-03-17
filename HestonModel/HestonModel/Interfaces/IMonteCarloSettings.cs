namespace HestonModel.Interfaces
{
    public interface IMonteCarloSettings
    {
        /// <summary>
        /// The number of sample paths generated for Monte-Carlo valuation
        /// </summary>
        int NumberOfTrials   { get; }
        /// <summary>
        /// The number of time steps for each path
        /// </summary>
        int NumberOfTimeSteps { get; }
    }
}
