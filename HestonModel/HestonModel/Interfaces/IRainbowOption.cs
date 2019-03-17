using System.Collections.Generic;

namespace HestonModel.Interfaces
{
    public interface IRainbowOption : IEuropeanOption
    {
        IEnumerable<double> MonitoringTimes { get; }
    }
}
