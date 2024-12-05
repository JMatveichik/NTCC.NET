using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Monitors
{
  internal abstract class MonitorDataPoint : MonitorBase
  {
    
    public MonitorDataPoint(string id) : base(id)
    {
    }
  }
}
