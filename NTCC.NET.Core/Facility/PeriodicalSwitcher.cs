using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class PeriodicalSwitcher : FacilityElement
  {
    public PeriodicalSwitcher(string id) : base(id)
    {
    }

    //объект синхронизации потока
    private readonly object threadLock = new object();

    // Флаг остановки потока переключения
    private CancellationTokenSource cancelToken = null;

    // Поток контроля нагрева зоны реактора
    private Thread switchingThread = null;

    public void SetupControl(string switcherId, int stateOnMsec = 1000, int stateOffMsec = 1000)
    {
      Switcher = ArtMonbatFacility.DataPoints[switcherId] as DiscreteOutputDataPoint;

      if (Switcher == null)
        throw new ArgumentNullException($"Discrete output data point {switcherId} not found");

      StateDurationOn = TimeSpan.FromMilliseconds(stateOnMsec);
      StateDurationOff = TimeSpan.FromMilliseconds(stateOffMsec);
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
    /// Начать переодическое переключение
    /// </summary>
    public void StartControl()
    {
      //контроль уже запущен или поток все еще активен
      lock (threadLock)
      {
        //если поток уже запущен запрещаем запуск нового потока 
        if (switchingThread != null && switchingThread.IsAlive)
          return;

        // Создаем новый экземпляр CancellationTokenSource
        cancelToken = new CancellationTokenSource();

        // Создаем делегат для нестатического метода
        ThreadStart threadDelegate = new ThreadStart(this.SwitchingFunction);

        switchingThread = new Thread(threadDelegate);
        switchingThread.Start();

        string message = $"Запущена процедура переключения";
        OnTick(message, MessageType.Debug);

        //обновляем состояние потока контроля нагрева зоны
        IsControlStarted = true;
      }
    }

    /// <summary>
    /// Остановить контроль зоны нагрева
    /// </summary>
    public void StopControl()
    {
      //Выставляем запрос на остановку потока контроля нагрева зоны 
      cancelToken.Cancel();

      //ждем завершения потока контроля нагрева зоны
      switchingThread.Join(0);

      //сообщаем об остановке потока контроля нагрева зоны
      string message = $"Процедура переодического переключения остановлена";
      OnTick(message, MessageType.Debug);

      //обновляем состояние потока контроля нагрева зоны
      IsControlStarted = false;
    }

    //проверка состояния потока контроля нагрева зоны
    public bool IsControlStarted
    {
      get => isControlStarted;
      private set
      {
        if (value == isControlStarted)
          return;
        isControlStarted = value;
        OnPropertyChanged();
      }
    }

    private bool isControlStarted = false;

    /// <summary>
    /// Процедура переключения
    /// </summary>
    private void SwitchingFunction()
    {
      while (true)
      {
        if (cancelToken.IsCancellationRequested)
        {
          //Переводим переключатель в состояние off
          Switcher.SetState(false);
          break;
        }

        //Переводим переключатель в состояние on и ожидаем заданное время
        Switcher.SetState(true);
        Thread.Sleep(StateDurationOn);

        //Переводим переключатель в состояние off и ожидаем заданное время
        Switcher.SetState(false);
        Thread.Sleep(StateDurationOff);
      }
    }
  }
}
