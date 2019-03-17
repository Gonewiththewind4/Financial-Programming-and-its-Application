using System.Collections.Generic;
using HestonModel;
using HestonModel.Interfaces;

public class AsianOptionGrading : EuropeanOptionGrading, IAsianOption
{
    public IEnumerable<double> MonitoringTimes { get; private set; }

    public AsianOptionGrading(IEnumerable<double> monitoringTimes, double timeToExpiry, double strike, PayoffType type) : base(timeToExpiry, strike, type)
    {
        MonitoringTimes = monitoringTimes;
    }


}
            