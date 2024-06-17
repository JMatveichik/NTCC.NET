using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Stages
{
  abstract public class StageTimeBased : StageBase
  {

    protected StageTimeBased(string id) : base(id)
    {
    }

    /// <summary>
    /// Заданная (общая) продолжительность стадии
    /// </summary>
    public TimeSpan TotalDuration
    {
      get => totalDuration;
      set
      {
        if (totalDuration == value)
          return;

        totalDuration = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan totalDuration = TimeSpan.FromSeconds(0);

    /// <summary>
    ///Оставшееся время стадии
    /// </summary>
    public TimeSpan TimeLeft
    {
      get => timeLeft;
      set
      {
        if (timeLeft == value)
          return;

        timeLeft = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan timeLeft = TimeSpan.FromSeconds(0);

    protected override StageResult Main(CancellationToken stop, CancellationToken skip)
    {
      OnTick($"Начато выполнение стадии {Title}.", MessageType.Warning);

      StartTime = DateTime.Now;
      Duration = DateTime.Now - StartTime;

      TotalDuration = TimeSpan.FromMinutes(StageParameters.Duration);

      //ожидаем истечения заданного времени 
      while (Duration < TotalDuration)
      {
        Thread.Sleep((int)ThreadDelay.TotalMilliseconds);

        //проверяем на остановку  стадии пользователем
        if (stop.IsCancellationRequested)
          return StageResult.Stopped;

        //проверяем на пропуск стадии пользователем
        if (skip.IsCancellationRequested)
          return StageResult.Skipped;

        //обновляем данные о времени если параметры изменены пользователем
        TotalDuration = TimeSpan.FromMinutes(StageParameters.Duration);

        Duration = DateTime.Now - StartTime;
        TimeLeft = TotalDuration - Duration;

        //вызываем метод специфичный для данной стадии
        OnMainTick();
      }

      return StageResult.Successful;
    }

    /// <summary>
    /// Абстрактный метод, который вызывается в основном цикле стадии
    /// </summary>
    protected abstract void OnMainTick();
  }
}
