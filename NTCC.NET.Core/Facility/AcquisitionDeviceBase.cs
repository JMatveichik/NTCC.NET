using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace NTCC.NET.Core.Facility
{

  public abstract class AcquisitionDeviceBase : FacilityElement
  {
    protected AcquisitionDeviceBase(string id) : base(id)
    {

    }

    #region СОЕДИНЕНИЕ С УСТРОЙСТВОМ

    /// <summary>
    ///Строка для установления связи с устройством
    /// </summary>
    public string ConnectionString
    {
      get => connectionString;
      set
      {
        if (connectionString == value)
          return;

        connectionString = value;
        OnPropertyChanged();
      }
    }
    private string connectionString;

    /// <summary>
    /// Максимальное время ожидание ответа от устройства
    /// </summary>
    public TimeSpan ResponseTimeout
    {
      get => responseTimeout;
      set
      {
        if (responseTimeout == value)
          return;

        responseTimeout = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan responseTimeout = TimeSpan.FromMilliseconds(2000);

    /// <summary>
    /// Установить соединение с устройством
    /// <returns> 
    /// <value>true - если соединение установлено</value>
    /// <value>false - если соединение не удалось установить</value> 
    /// </returns>
    /// </summary>
    public bool Connect()
    {
      
      OnTick($"Попытка подключения к устройству «{Title}»", MessageType.Info);
      var connected = Connect(ConnectionString, (int)ResponseTimeout.TotalMilliseconds);

      if (connected)
      {
        OnTick($"Связь с утройством «{Title}» установлена. Начат сбор данных.", MessageType.Success);
        Start();
      }

      return connected;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public abstract bool Connect(string connection, int timeout);

    /// <summary>
    /// Переподключение к устройству
    /// </summary>
    /// <returns>true - если получилось подключиться к устройству; false - если переподключение не удалось</returns>
    public bool Reconnect(int totalAttempts = 5)
    {
      //останавливаем поток обновления данных
      Stop();

      //сбрасываем текущую попытку подключения
      int currentAttempt = 0;
      
      //пытаемся переподключиться к устройству
      while (currentAttempt < totalAttempts)
      {
        //ожидаем 2 секунды перед следующей попыткой
        Thread.Sleep(2000);

        if (!Connect())
        {
          CurrentAttempt = ++currentAttempt;

          string message = $"Попытка {currentAttempt} восстановления связи с устройством «{Title}» .";
          OnTick(message, MessageType.Warning);
          continue;
        }
        else
        {
          CurrentAttempt = 0;

          string message = $"Связь с устройством «{Title}» восстановлена.";
          OnTick(message, MessageType.Success);
          return true;
        }
      }

      return false;
    }
    #endregion


    #region ВХОДЫ И ВЫХОДЫ

    /// <summary>
    /// Дискретные входы устройства
    /// </summary>
    public BitArray DiscreteInputs
    {
      get => discreteInputs;
      protected set
      {
        discreteInputs = value;
      }
    }
    protected BitArray discreteInputs = null;

    /// <summary>
    /// Дискретные выходы устройства
    /// </summary>
    public BitArray DiscreteOutputs
    {
      get => discreteOutputs;
      protected set
      {
        discreteOutputs = value;
      }
    }
    protected BitArray discreteOutputs = null;

    /// <summary>
    /// Аналоговые входы устройства
    /// </summary>
    public List<double> AnalogInputs
    {
      get => analogInputs;
      protected set
      {
        analogInputs = value;
      }
    }
    protected List<double> analogInputs = null;

    /// <summary>
    ///Аналоговые выходы устройства
    /// </summary>
    public List<double> AnalogOutputs
    {
      get => analogOutputs;
      protected set
      {
        analogOutputs = value;
      }
    }
    protected List<double> analogOutputs = null;

    #endregion


    #region СОБЫТИЯ И ОБРАБОТЧИКИ

    /// <summary>
    /// Событие вызывающееся при обновлении дискретных входов 
    /// </summary>
    public event EventHandler<DiscreteDataEventArgs> DiscreteInputsUpdate;


    /// <summary>
    /// Событие возникающее при обновлении дискретных входов
    /// </summary>
    public event EventHandler<DiscreteDataEventArgs> DiscreteOutputUpdate;


    /// <summary>
    /// Событие возникающее при обновлении аналоговых входов
    /// </summary>
    public event EventHandler<AnalogDataEventArgs> AnalogInputsUpdate;


    /// <summary>
    /// Событие возникающее при обновлении аналоговых выходов
    /// </summary>
    public event EventHandler<AnalogDataEventArgs> AnalogOutputUpdate;


    /// <summary>
    /// Вызов обработчиков при обновлении цифровых входов
    /// </summary>
    /// <param name="dinp"></param>
    protected void OnUpdateDI(BitArray dinp)
    {
      DiscreteInputsUpdate?.Invoke(this, new DiscreteDataEventArgs(dinp));
    }


    /// <summary>
    /// Вызов обработчиков при обновлении цифровых выходов
    /// </summary>
    /// <param name="discreteOutputs"></param>
    protected void OnUpdateDO(BitArray discreteOutputs)
    {
      DiscreteOutputUpdate?.Invoke(this, new DiscreteDataEventArgs(discreteOutputs));
    }

    /// <summary>
    /// Вызов обработчиков при обновлении состояния аналоговых входов
    /// </summary>
    /// <param name="ainp"></param>
    protected void OnUpdateAI(IEnumerable<double> ainp)
    {
      AnalogInputsUpdate?.Invoke(this, new AnalogDataEventArgs(ainp));
    }

    /// <summary>
    /// Вызов обработчиков при обновлении состояния аналоговых выходов
    /// </summary>
    /// <param name="aout"></param>
    protected void OnUpdateAO(IEnumerable<double> aout)
    {
      AnalogOutputUpdate?.Invoke(this, new AnalogDataEventArgs(aout));
    }

    #endregion

    /// <summary>
    /// Текущая попытка получения данных от устройства или установления регистра для устройства 
    /// </summary>
    public int CurrentAttempt
    {       
      get => currentAttempt;
      set
      {
        if (currentAttempt == value)
          return;

        currentAttempt = value;
        OnPropertyChanged();
      }
    }
    private int currentAttempt = 0;

    /// <summary>
    /// Запуск потока обновления данных
    /// </summary>
    /// <returns></returns>
    public bool Start()
    {
      if (controlThread != null && controlThread.IsAlive)
        return false;

      controlThread = new Thread(ControlThreadFunction);
      controlThread.Start();

      //отправляем сообщение о запуске устройства
      string message = $"Устройство «{Title}» запущено!";
      OnTick(message, MessageType.Success);

      return true;
    }

    /// <summary>
    /// Остановка потока обновления данных
    /// </summary>
    public void Stop()
    {
      stopEvent.Set();
    }
   

    protected virtual void OnExit()
    {
      //отправляем сообщение о запуске устройства
      string message = $"Устройство «{Title}» остановлено! Сбор информации от устройства прекращен!";
      OnTick(message, MessageType.Warning);
    }


    #region АБСТРАКТНЫЕ И ВИРТУАЛЬНЫЕ ФУНКЦИИ

    /// <summary>
    /// Установить дискретный выход
    /// </summary>
    /// <param name="ch">Номер дискретного выхода</param>
    /// <param name="state">Состояние в которое переключается выход</param>
    public abstract void SetDiscreteOutput(int ch, bool state);


    /// <summary>
    /// Установить аналоговый выход
    /// </summary>
    /// <param name="ch">Номер аналогового выхода</param>
    /// <param name="value">Значение записываемое в аналоговый выхоод</param>
    public abstract void SetAnalogOutput(int ch, double value);


    /// <summary>
    /// Абстрактная функция обновления дискретные входов
    /// </summary>
    protected abstract void UpdateDiscreteInputs();

    /// <summary>
    /// Абстрактная функция обновления дискретные выходов
    /// </summary>
    protected abstract void UpdateDiscreteOutputs();

    /// <summary>
    /// Абстрактная функция обновления аналоговых  входов
    /// </summary>
    protected abstract void UpdateAnalogInputs();

    /// <summary>
    /// Абстрактная функция обновления аналоговых  выходов
    /// </summary>
    protected abstract void UpdateAnalogOutputs();

    #endregion



    /// <summary>
    /// Функция обновления данных
    /// </summary>
    protected virtual void UpdateData()
    {
      try
      {
        //если у устройства есть цифровые входы обновляем их и вызываем обработчики
        if (DiscreteInputs != null)
        {
          UpdateDiscreteInputs();
          OnUpdateDI(DiscreteInputs);
        }

        //если у устройства есть цифровые выходы обновляем их и вызываем обработчики
        if (DiscreteOutputs != null)
        {
          UpdateDiscreteOutputs();
          OnUpdateDO(DiscreteOutputs);
        }

        //если у устройства есть аналоговые входы обновляем их и вызываем обработчики
        if (AnalogInputs != null)
        {
          UpdateAnalogInputs();
          OnUpdateAI(AnalogInputs);
        }

        //если у устройства есть аналоговые выходы обновляем их и вызываем обработчики
        if (AnalogOutputs != null)
        {
          UpdateAnalogOutputs();
          OnUpdateAO(AnalogOutputs);
        }
      }
      catch (Exception e)
      {
        //отправляем сообщение о запуске устройства
        string message = $"Ошибка обновления текущего состояния устройства «{Title}» : {e.Message}";
        OnTick(message, MessageType.Warning);
      }

    }


    /// <summary>
    /// Main thread object
    /// </summary>
    private Thread controlThread;

    /// <summary>
    /// Событие прерывающее выполнение потока обновленияданных
    /// </summary>
    private readonly ManualResetEvent stopEvent = new ManualResetEvent(false);

    /// <summary>
    /// Функция потока обновления данных
    /// </summary>
    private void ControlThreadFunction()
    {
      while (true)
      {
        if (stopEvent.WaitOne(TimeSpan.FromMilliseconds(0)))
        {
          OnExit();
          break;
        }

        Thread.Sleep(Interval);
        UpdateData();

      }
    }

    /// <summary>
    /// Период обновления данных устройства
    /// </summary>
    public int Interval
    {
      get => interval;
      set
      {
        if (interval == value)
          return;

        interval = (value < MinDelay) ? value : BaseDelay;
      }
    }


    private int interval = BaseDelay;

    /// <summary>
    /// Минимальная задержка при выполнении потока сбора и 
    /// </summary>
    private const int MinDelay = 100;

    /// <summary>
    /// Базовая задержка при выполнении потока сбора и обработки информации
    /// </summary>
    private const int BaseDelay = 250;

  }
}
