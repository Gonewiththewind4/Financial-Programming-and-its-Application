using System.Collections.Generic;

namespace HestonModel.Interfaces
{
    public interface IShoutOption : IEuropeanOption
    {
        IEnumerable<double> MonitoringTimes { get; }
    }
}
