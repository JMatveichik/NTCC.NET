using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{
  internal abstract class AnalogDataPointCheck : IFacilityElementCheck
  {
    public AnalogDataPointCheck(AnalogDataPoint analogDataPoint, double valueToCheck) 
    {
      AnalogDataPoint = analogDataPoint;
      ValueToCheck = valueToCheck;
    }

    /// <summary>
    /// The analog data point to check
    /// </summary>
    public AnalogDataPoint AnalogDataPoint
    {
      get;
      protected set;
    }

    /// <summary>
    /// The value to check
    /// </summary>
    public double ValueToCheck
    {
      get;
      protected set;
    }

    abstract public bool CheckElement();
    
  }
}
