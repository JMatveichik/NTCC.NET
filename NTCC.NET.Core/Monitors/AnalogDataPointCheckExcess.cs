using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{
  /// <summary>
  /// Check if the value of the analog data point is greater than the value to check
  /// </summary>
  internal class AnalogDataPointCheckExcess : AnalogDataPointCheck
  {
    public AnalogDataPointCheckExcess(AnalogDataPoint analogDataPoint, double valueToCheck) : base(analogDataPoint, valueToCheck)
    {
    }
   
    /// <summary>
    /// Check if the value of the analog data point is greater than the value to check
    /// </summary>
    /// <returns></returns>
    public override bool CheckElement()
    {
      if (AnalogDataPoint.Value > ValueToCheck)
      {
        return false;
      }
      return true;
    }  
  }
}
