using System.Collections.Generic;

namespace HestonModel.Interfaces
{
    public interface IAsianOption : IEuropeanOption
    {
        IEnumerable<double> MonitoringTimes { get; }
    }
}
