using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class GasHeater : FacilityElement
  {
    public GasHeater(string id) : base(id)
    {
    }

    //объект синхронизации потока
    private readonly object threadLock = new object();

    // Флаг остановки потока переключения
    private CancellationTokenSource cancelToken = null;

    // Поток контроля нагрева зоны реактора
    private Thread controlThread = null;

    public void SetupControl(string gasTemperatureId, string heaterTemperatureId, string heaterStateId)
    {
      GasTemperature = ArtMonbatFacility.DataPoints[gasTemperatureId] as AnalogDataPoint;

      if (GasTemperature == null)
        throw new ArgumentNullException($"Discrete output data point {gasTemperatureId} not found for temperature control");

      HeaterState = ArtMonbatFacility.DataPoints[heaterStateId] as DiscreteOutputDataPoint;
      if (HeaterState == null)
        throw new ArgumentNullException($"Discrete output data point {gasTemperatureId} not found heater switching");
    }

    /// <summary>
    /// Точка данных для контроля температуры газа
    /// </summary>
    public AnalogDataPoint GasTemperature 
    { 
      get; 
      private set;
    } = null;

    /// <summary>
    /// Точка данных для контроля температуры нагревательного элемента
    /// </summary>
    public AnalogDataPoint HeaterTemperature
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Точка данных для управления питанием нагревателя
    /// </summary>
    public DiscreteOutputDataPoint HeaterState
    {
      get;
      private set;
    } = null;

    /// <summary>
    ///Целевая температура газа на выходе из подогревателя
    /// </summary>
    public double TargetGasTemperature
    {
      get => targetGasTemperature;
      set
      {
        if (value == targetGasTemperature)
          return;

        targetGasTemperature = value;
        OnPropertyChanged();
      }
    }
    private double targetGasTemperature = 0.0;

    /// <summary>
    ///Максимальная температура нагревательного элемента подогревателя
    /// </summary>
    public double MaxHeaterTemperature
    {
      get => maxHeaterTemperature;
      set
      {
        if (value == maxHeaterTemperature)
          return;

        maxHeaterTemperature = value;
        OnPropertyChanged();
      }
    }

    private double maxHeaterTemperature = 0.0;

    /// <summary>
    /// Запуск контроля температуры газа на выходе из подогревателя
    /// </summary>
    public void StartControl()
    {
      //контроль уже запущен или поток все еще активен
      lock (threadLock)
      {
        //если поток уже запущен запрещаем запуск нового потока 
        if (controlThread != null && controlThread.IsAlive)
          return;

        // Создаем новый экземпляр CancellationTokenSource
        cancelToken = new CancellationTokenSource();

        // Создаем делегат для нестатического метода
        ThreadStart threadDelegate = new ThreadStart(this.ControlFunction);

        controlThread = new Thread(threadDelegate);
        controlThread.Start();

        string message = $"Запущена процедура подогрева пропан-бутана";
        OnTick(message, MessageType.Warning);

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
      controlThread.Join(0);

      //сообщаем об остановке потока контроля нагрева зоны
      string message = $"Процедура процедура подогрева пропан-бутана остановлена";
      OnTick(message, MessageType.Warning);

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
    private void ControlFunction()
    {
      while (true)
      {
        Thread.Sleep(500);

        if (cancelToken.IsCancellationRequested)
        {
          //Выключаем подогреватель газа
          HeaterState.SetState(false);
          break;
        }

        //Если температура газа больше заданной или температура
        //нагревательного элемента выше заданной выключаем  нагрев 
        if (GasTemperature.Value > TargetGasTemperature || 
            HeaterTemperature.Value > MaxHeaterTemperature)
        {
          HeaterState.SetState(false);
        }

        //если температура газа меньше заданной
        //включаем нагревательный элемент
        if (GasTemperature.Value < TargetGasTemperature)
        {
          HeaterState.SetState(true);
        }

      }
    }
  }
}

