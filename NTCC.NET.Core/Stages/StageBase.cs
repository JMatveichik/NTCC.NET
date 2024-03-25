using NTCC.NET.Core.Facility;
using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace NTCC.NET.Core.Stages
{


  public abstract class StageBase : FacilityElement
  {

    protected const double ZERRO = 0.0;

    /// <summary>
    /// Конструктор
    /// </summary>  
    public StageBase(string id) : base(id)
    {
    
    }


    /// <summary>
    ///Стадия владелец (которая вызвала выполнение)
    /// </summary>
    protected StageBase OwnerStage
    {
      get;
      private set;
    }


    
    /// <summary>
    /// Состояние стадии
    /// </summary>
    public StageState State
    {
      get => state;
      protected set
      {
        if (value == state)
          return;

        OnPropertyChanged();
      }
    }

    private StageState state = StageState.Wait;

    public bool IsActive
    {
      get => isActive;
      protected set
      {
        if (value == isActive)
          return;

        isActive = value;
        OnPropertyChanged();
      }
    }
    private bool isActive = false;
    
    
    public StageParameters StageParameters
    {
      get;
      set;
    }

    public static IUserConfirmation UserConfirmation 
    { 
      get; 
      set; 
    } = null;

    /// <summary>
    /// Получаем среднюю температуру стенок реактора по заданным для стадии зонам  
    /// </summary>
    /// <returns> Средняя температура по заданным зонам</returns>
    public double GetAverageTemperature()
    {
      //выбираем зоны по которым вычисляется средняя температура
      var activeZonesIDs = StageParameters?.StageHeatingParameters.Where(z => z.Value.UseWhenAverageTemperatureCalc == true).Select(z => z.Key).ToList();

      //вычисляем среднюю температуру по заданным зонам
      return ArtMonbatFacility.Instance.GetAverageTemperature(activeZonesIDs);
    }

    protected void SetupHeating()
    {
      List<ReactorHeatingZone> reactorHeatingZones = ArtMonbatFacility.ReactorZones.Items.Values.ToList();

      //задать параметры нагрева для всех зон
      foreach (var zone in reactorHeatingZones)
      {
        HeatingParameters stageHeatingParams = StageParameters.StageHeatingParameters[zone.ID];

        zone.SetupControl(stageHeatingParams.MaxWallTemperature,
                            stageHeatingParams.MinWallTemperature,
                            stageHeatingParams.HeaterPower,
                            stageHeatingParams.MaxHeaterTemperature);
      }
    }

    protected bool CheckWaterLevel (TimeSpan waitLevel)
    {
      if (DataPointHelper.CheckDiscreteParameter("M06.1", true))
        return true;

      bool result;
      try
      {
        OnTick($"Долив воды в увлажнитель", MessageType.Warning );
        DataPointHelper.SetDiscreteParameter(this, "YA11.OPN", true, (int) OperationDelay.TotalMilliseconds);
        DataPointHelper.WaitDiscreteParameterSet(this, "M06.1", true, waitLevel);
        result = true;
      }
      catch (Exception e)
      {
        OnTick($"Низкий уровень воды в увлажнителе. Долейте воду в бак! ({e.Message})", MessageType.Error);
        result = false;
      }

      DataPointHelper.SetDiscreteParameter(this, "YA11.OPN", false, (int)OperationDelay.TotalMilliseconds);
      return result;
    }

    #region СОБЫТИЯ и ОбРАБОТЧИКИ
    protected void OnStageStep(StageState stageState, bool delayAfter = true)
    {
      //Выставляем состояние стадии
      State = stageState;
      MessageType messageType;
      string message = StageStepMessage(stageState, this, out messageType);

      StageStep?.Invoke(this, new FacilityMessageArgs(message, messageType));

      OnTick(message, messageType);

      //сделать задержку после выполнения операции
      if (delayAfter)
        Thread.Sleep((int)OperationDelay.TotalMilliseconds);

    }

    public event FacilityMessageEventHandler StageStep;

    public static string StageStepMessage(StageState state, StageBase stage, out MessageType messageType)
    {
      switch (state)
      {
        case StageState.Wait:
          {
            messageType = MessageType.Info;
            return $"Ожидание выполнения стадии {stage.Title}.";
          }
        case StageState.Prepeared:
          {
            messageType = MessageType.Info; 
            return $"Стадия {stage.Title} подготовлена к выполнению.";
          }
          
        case StageState.Started:
          {
            messageType = MessageType.Success;
            return $"Начато выполнения основного алгоритма стадии {stage.Title}.";
          }
          
        case StageState.Completed:
          {
            messageType = MessageType.Success;
            return $"Выполнения основного алготиритма стадии {stage.Title} завершено.";
          }
          
        case StageState.Failed:
          {
            messageType = MessageType.Error;
            return $"Техническая ошибка во время выполнения стадии {stage.Title}.";
          }
          
        case StageState.Stopped:
          {  
            messageType = MessageType.Warning;
            return $"Стадия {stage.Title} остановлена пользователем.";
          }
          
        case StageState.Skipped:
          { 
            messageType = MessageType.Warning;
            return $"Стадия {stage.Title} пропущена пользователем.";
            
          }
          
        case StageState.Finalized:
          {
            messageType = MessageType.Success;
            return $"Cтадия {stage.Title} завершена";
          }
          
        case StageState.Excepted:
          {
            messageType = MessageType.Exception;
            return $"Программная ошибка при выполнении стадии {stage.Title}";
          }

        default:
          {              
            messageType = MessageType.Error;
            return $"Неизвестное состояние стадии {stage.Title}";
          }
      }
    }
    #endregion


    #region ВРЕМЕННЫЕ ХАРАКТЕРИСТИКИ СТАДИИ


    /// <summary>
    ///Продолжительность стадии (от времени начала)
    /// </summary>
    public TimeSpan Duration
    {
      get => duration;
      set
      {
        if (duration == value)
          return;

        duration = value;
        OnPropertyChanged();
      }
    }
    private TimeSpan duration = TimeSpan.FromSeconds(0);


    /// <summary>
    /// Время начала стадии
    /// </summary>
    public DateTime StartTime
    {
      get => startTime;
      set
      {
        if (startTime == value)
          return;

        startTime = value;
        OnPropertyChanged();
      }
    }
    private DateTime startTime;

    /// <summary>
    /// Задержка между выполнение операций стадии (переключение состояния клапанов, насосов, и т.д.)
    /// </summary>
    protected static TimeSpan OperationDelay
    {
      get;
      private set;
    } = TimeSpan.FromMilliseconds(2000);

    /// <summary>
    /// Задержка при работе в основном потоке
    /// </summary>
    protected static TimeSpan ThreadDelay
    {
      get;
      private set;
    } = TimeSpan.FromMilliseconds(500);

    #endregion


    #region ТЕСТЫ


    /// <summary>
    ///Общие аврарийные тесты для любой стадии
    /// </summary>
    /* public static TestGroup CommonAlarmTests
       {
           get;
           private set;
       }


       /// <summary>
       /// Аварийные тесты специфичные для стадии
       /// </summary>
       public TestGroup StageAlarmTests
       {
           get;
           protected set;
       }


       /// <summary>
       /// Предупредительные тесты специфичные для стадии
       /// </summary>
       public TestGroup StageWarningTests
       {
           get;
           protected set;
       }       
    */

    #endregion


    #region ПОЛЯ

    //Запрос на отсановку  стадии
    protected CancellationTokenSource stop = null;

    //запрос на пропуск стадии
    protected CancellationTokenSource skip = null;

    #endregion


    /// <summary>
    /// Запуск стадиии на выполнение
    /// </summary>
    /// <param name="owner">Cтадия инициализировавшая выполнение данной</param>
    /// <returns></returns>
    public StageResult Do(StageBase owner = null)
    {
      //запоминаем владельца стадии
      OwnerStage = owner;

      IsActive = true;

      //инициализация токена остановки стадии по инициативе пользователя
      stop = new CancellationTokenSource();

      //инициализация токена пропуска стадии по инициативе пользователя
      skip = new CancellationTokenSource();

      try
      {

        StageResult result = StageResult.Failed;

        //подготовка к выполнению стадии
        result = Prepare();

        if (result != StageResult.Successful)
        {
          Finalization();
          IsActive = false;
          return result;
        }

        //вызываем обработчики завершения подготовки стадии к выполнению
        OnStageStep(StageState.Prepeared);

        //запускаем основной алгоритм стадии
        Task<StageResult> main = Task.Factory.StartNew<StageResult>(() => Main(stop.Token, skip.Token));

        //вызываем обрабатчики события начала выполнения основного алгоритма стадии
        OnStageStep(StageState.Started);

        //ожидаем завершения основного алгоритма стадии
        main.Wait();

        //вызываем обработчики события окончания выполнения основного алгоритма стадии
        OnStageStep(StageState.Completed);

        result = main.Result;

        switch (main.Result)
        {
          case StageResult.Failed:
            {
              //вызов обработчиков некорректного завершения стадии
              //завершение технологического цикла
              OnStageStep(StageState.Failed);
              break;
            }
          case StageResult.Stopped:
            {
              //вызов обработчиков остановки стадии по инициативе опрератора
              //завершение технологического цикла
              OnStageStep(StageState.Stopped);
              break;
            }
          case StageResult.Skipped:
            {
              //вызов обработчиков пропуска стадии по инициативе опрератора
              //продолжение технологического цикла
              OnStageStep(StageState.Skipped);
              break;
            }
        }
        
        //выполнение завершение стадии
        Finalization();

        //обработчик после выполнением завершения стадии
        OnStageStep(StageState.Finalized);

        IsActive = false;
        return result;

      }
      catch (Exception ex)
      {
        OnTick($"Непредвиденное завершение стадии : {ex.Message}", MessageType.Exception);

        State = StageState.Excepted;
        IsActive = false;

        //выполнение завершение стадии
        Finalization();
        return StageResult.Excepted;
      }

    }


    #region АБСТРАКТНЫЕ ОПЕРАЦИИ ШАБЛОННОГО МЕТОДА


    /// <summary>
    /// Действия при подготовке к стадии
    /// </summary>
    /// <returns></returns>
    public abstract StageResult Prepare();


    /// <summary>
    /// Операции после окончания стадии
    /// </summary>
    /// <returns></returns>
    protected abstract StageResult Finalization();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected abstract StageResult Main(CancellationToken stop, CancellationToken skip);


    /// <summary>
    ///Остановить выполнение стадии (и технологического цикла)
    /// </summary>
    public void Stop()
    {
      if (stop != null)
        stop.Cancel();
    }

    /// <summary>
    ///Пропустить выполнение стадии (продолжить технологическоий цикла)
    /// </summary>
    public void Skip()
    {
      if (skip != null)
        skip.Cancel();
    }
        
    #endregion

  }
}
