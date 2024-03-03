using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class PeriodicalSwitcher : FacilityThreadElement
  {
    public PeriodicalSwitcher(string id) : base(id)
    {
    }

    public void SetupControl(string switcherId, TimeSpan stateOnDuration, TimeSpan stateOffDuration)
    {
      Switcher = ArtMonbatFacility.DataPoints[switcherId] as DiscreteOutputDataPoint;

      if (Switcher == null)
        throw new ArgumentNullException($"Discrete output data point {switcherId} not found");

      StateDurationOn  = stateOnDuration;
      StateDurationOff = stateOffDuration;
    }

    /// <summary>
    /// Дискретная точка данных для переключения
    /// </summary>
    public DiscreteOutputDataPoint Switcher { get; private set; } = null;

    /// <summary>
    /// Время нахождения переключателя в состоянии off
    /// </summary>
    public TimeSpan StateDurationOff { get; private set; }

    /// <summary>
    /// Время нахождения переключателя в состоянии on 
    /// </summary>
    public TimeSpan StateDurationOn { get; private set; }

    /// <summary>
    /// Процедура переключения
    /// </summary>
    protected override void ControlFunction()
    {
      //Переводим переключатель в состояние on и ожидаем заданное время (если задано)
      if (StateDurationOn.TotalMilliseconds > 0.0)
      {
        Switcher.SetState(true);
        Thread.Sleep(StateDurationOn);
      }

      //Переводим переключатель в состояние off и ожидаем заданное время (если задано)
      if (StateDurationOff.TotalMilliseconds > 0.0)
      {
        Switcher.SetState(false);
        Thread.Sleep(StateDurationOff);
      }
    }

    protected override void OnControlStarted()
    {
      //сообщаем об запуске потока переключения
      string message = $"Запущена процедура переключения {Title}";
      OnTick(message, MessageType.Info);
    }

    protected override void OnControlStopped()
    {
      Switcher.SetState(false);

      //сообщаем об остановке потока переключения 
      string message = $"Процедура переодического переключения {Title} остановлена";
      OnTick(message, MessageType.Info);
    }

  }
}
