using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NTCC.NET.Core.Facility;

namespace NTCC.NET.Core.Monitors
{
  internal abstract class MonitorBase : FacilityThreadElement
  {

    public MonitorBase(string id) : base(id)
    {
    }

    public event MonitorEventEventHandler MonitorEvent;

    /// <summary>
    /// Status of monitor
    /// </summary>
    public MonitorStatus Status
    {
      get;
      protected set;
    }

    /// <summary>
    /// Last check time
    /// </summary>
    public DateTime LastCheck
    {
      get;
      protected set;
    }

    /// <summary>
    /// Maximal duration of down status before sending event
    /// </summary>
    public TimeSpan MaxDownDuration
    {
      get;
      set;
    }

    /// <summary>
    /// Duration of down status
    /// </summary>
    public DateTime DownDuration
    {
      get;
      protected set;
    }

    /// <summary>
    /// Facility element check provider
    /// </summary>
    public IFacilityElementCheck facilityElementCheck
    {
      get;
      protected set;
    }

    /// <summary>
    /// Abstract method for checking monitor
    /// </summary>
    /// <returns></returns>
    protected abstract MonitorStatus CheckMonitor();


  }
}
