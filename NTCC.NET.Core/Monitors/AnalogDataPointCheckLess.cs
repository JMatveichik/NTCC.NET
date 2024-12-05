using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{
  internal class AnalogDataPointCheckLess : AnalogDataPointCheck
  {
    public AnalogDataPointCheckLess(AnalogDataPoint analogDataPoint, double valueToCheck) : base(analogDataPoint, valueToCheck)
    { 
    }

    /// <summary>
    /// Check if the value of the analog data point is less than the value to check
    /// </summary>
    /// <returns></returns>
    public override bool CheckElement()
    {
      if (AnalogDataPoint.Value < ValueToCheck)
      {
        return false;
      }
      return true;
    }
  }
}
