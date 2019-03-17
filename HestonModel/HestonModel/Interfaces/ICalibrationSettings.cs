namespace HestonModel.Interfaces
{
    public interface ICalibrationSettings
    {
        /// <summary>
        /// A parameter influencing the accuracy the minimization algorithm is trying to achieve. Note that we are
        /// allowing more options than parameters so we don't necessarily expect to be able to re-price all the options.
        /// </summary>
        double Accuracy { get; }
        /// <summary>
        /// The maximum number of iterations that the minimization algorithm can carry out. Note that even 10 iterations
        /// can take more than a few seconds!
        /// </summary>
        int MaximumNumberOfIterations { get; }
    }
}
