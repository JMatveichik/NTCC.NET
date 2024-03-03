using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public abstract class FacilityThreadElement : FacilityElement
  {
    protected FacilityThreadElement(string id) : base(id)
    {

    }

    /// <summary>
    /// Интервал ожидания в цикле выполнения основной процедуры
    /// </summary>
    public TimeSpan Interval
    {
      get => updateInterval;
      set
      {
        if (value == updateInterval)
          return;

        updateInterval = value;
        OnPropertyChanged();
      }
    }

    private TimeSpan updateInterval = TimeSpan.FromMilliseconds(100);

    // Флаг остановки потока 
    private CancellationTokenSource cancelToken = null;

    // Поток контроля 
    private Thread controlThread = null;

    /// <summary>
    /// Запустить поток 
    /// </summary>
    public void StartControl()
    {
      //если поток уже запущен запрещаем запуск нового потока 
      if (controlThread != null && controlThread.IsAlive)
        return;

      // Создаем новый экземпляр CancellationTokenSource для остановки потока
      cancelToken = new CancellationTokenSource();

      // Создаем делегат для нестатического метода
      ThreadStart threadDelegate = new ThreadStart(this.ThreadFunction);

      // Создаем поток
      controlThread = new Thread(threadDelegate);
      controlThread.Start();

      //обновляем состояние потока контроля нагрева зоны
      IsControlStarted = true;

      OnControlStarted();
    }

    /// <summary>
    /// Остановить поток 
    /// </summary>
    public void StopControl()
    {
      //Выставляем запрос на остановку потока
      cancelToken.Cancel();

      //ждем завершения потока
      controlThread.Join(0);

      //обновляем состояние
      IsControlStarted = false;

      OnControlStopped();
    }

    /// <summary>
    /// Состояния потока 
    /// </summary>
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
    /// Функция потока
    /// </summary>
    private void ThreadFunction()
    {
      while (true)
      {
        //выполняем процедуру с заданным интервалом
        Thread.Sleep(Interval);

        //если запрошена остановка потока завершаем выполнение
        if (cancelToken.IsCancellationRequested)
          break;

        //вызываем основную процедуру потока
        ControlFunction();
      }
    }

    /// <summary>
    /// Процедура выполняемая в потоке
    /// </summary>
    protected abstract void ControlFunction();

    //string message = $"Запущена процедура переключения";
    //OnTick(message, MessageType.Debug);
    /// <summary>
    /// Выполняется при запуске потока
    /// </summary>
    protected abstract void OnControlStarted();

    //сообщаем об остановке потока контроля нагрева зоны
    //string message = $"Процедура переодического переключения остановлена";
    //OnTick(message, MessageType.Debug);
    /// <summary>
    /// Выполняется при остановке потока
    /// </summary>
    protected abstract void OnControlStopped();
  }
}
