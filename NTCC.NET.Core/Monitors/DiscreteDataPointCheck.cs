using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{
  internal class DiscreteDataPointCheck : IFacilityElementCheck
  {
    public DiscreteDataPointCheck(DiscreteDataPoint discreteDataPoint, bool ecpectedState)
    {
    }

    /// <summary>
    /// The discrete data point to check
    /// </summary>
    public DiscreteDataPoint DiscreteDataPoint
    {
      get;
      protected set;
    }

    /// <summary>
    /// Expected state of the discrete data point
    /// </summary>
    public bool ExpectedState
    {
      get;
      protected set;
    }
    
    /// <summary>
    /// Check if the state of the discrete data point is the expected state
    /// </summary>
    /// <returns></returns>
    public bool CheckElement()
    {
      return DiscreteDataPoint.State == ExpectedState;
    }
  }
}
