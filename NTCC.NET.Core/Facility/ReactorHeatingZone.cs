using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class ReactorHeatingZone : FacilityElement
  {

    public static List<string> PropertiesList = new List<string>()
        {
            "PowerState",
            "Run",
            "Duty",
            "Period",
            "ErrorCode",
            "EStop",
            "PWM",
            "DutyWrite",
            "PeriodWrite",
            "WallTemperature"
        };

    public ReactorHeatingZone(string id) : base(id)
    {

    }

    /// <summary>
    /// Связываем свойства объекта с точками данных
    /// </summary>
    /// <param name="dataPoints">Точки данных для зоны нагрева</param>
    public void Setup(Dictionary<string, DataPoint> dataPoints)
    {
      PowerState = (DiscreteDataPoint)dataPoints["PowerState"];
      Run = (DiscreteOutputDataPoint)dataPoints["Run"];
      Duty = (AnalogDataPoint)dataPoints["Duty"];
      Period = (AnalogDataPoint)dataPoints["Period"];
      ErrorCode = (AnalogDataPoint)dataPoints["ErrorCode"];
      EStop = (DiscreteDataPoint)dataPoints["EStop"];
      PWM = (DiscreteDataPoint)dataPoints["PWM"];
      DutyWrite = (AnalogOutputDataPoint)dataPoints["DutyWrite"];
      PeriodWrite = (AnalogOutputDataPoint)dataPoints["PeriodWrite"];
      WallTemperature = (AnalogDataPoint)dataPoints["WallTemperature"];
    }

    /// <summary>
    /// Мощность нагревателей, %
    /// </summary>                
    public AnalogDataPoint Duty
    {
      get;
      private set;
    }

    /// <summary>
    /// Период импульса регулирования, ms 
    /// </summary>
    public AnalogDataPoint Period
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Код ошибки нагревателя
    /// </summary>
    public AnalogDataPoint ErrorCode
    {
      get;
      private set;
    } = null;


    /// <summary>
    /// ШИМ сигнал
    /// </summary>
    public DiscreteDataPoint PWM
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Состояние питания нгруппы нагрева 
    /// true - Подано питание
    /// false - Питание отсутствует
    /// </summary>
    public DiscreteDataPoint PowerState
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Подать/убрать питание
    /// true - Подано питание
    /// false - Питание отсутствует
    /// </summary>
    public DiscreteOutputDataPoint Run
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Аварийная остановка
    /// false - норма
    /// true - авария 
    /// </summary>
    public DiscreteDataPoint EStop
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Задать скважность (мощность), % 
    /// </summary>
    public AnalogOutputDataPoint DutyWrite
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Задать период ШИМ сигнала, ms 
    /// </summary>
    public AnalogOutputDataPoint PeriodWrite
    {
      get;
      private set;
    } = null;

    /// <summary>
    /// Температура стенки реактора в зоне нагрева
    /// </summary>
    public AnalogDataPoint WallTemperature
    {
      get;
      private set;
    }

    /// <summary>
    /// Минимальная целевая температура стенки реактора зоны нагрева
    /// </summary>
    public double MinTargetWallTemperature
    {
      get => minTargetWallTemperature;
      set
      {
        if (value == minTargetWallTemperature)
          return;

        minTargetWallTemperature = value;
        OnPropertyChanged();
      }
    }
    private double minTargetWallTemperature = 690.0;

    /// <summary>
    /// Максимальная целевая температура стенки реактора зоны нагрева
    /// </summary>
    public double MaxTargetWallTemperature
    {
      get => maxTargetWallTemperature;
      set
      {
        if (value == maxTargetWallTemperature)
          return;

        maxTargetWallTemperature = value;
        OnPropertyChanged();
      }
    }
    private double maxTargetWallTemperature = 700.0;

    /// <summary>
    /// Мощность нагревателей реактора зоны нагрева
    /// </summary>
    public double MaxPowerLevel
    {
      get => maxPowerLevel;
      set
      {
        if (value == maxPowerLevel)
          return;

        maxPowerLevel = value;
        OnPropertyChanged();
      }
    }
    private double maxPowerLevel = 75.0;

    /// <summary>
    /// Максимальная температура нагревателей реактора зоны нагрева
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
    private double maxHeaterTemperature = 810.0;

    /// <summary>
    /// Максимальная температура нагревателей реактора зоны нагрева
    /// </summary>
    public double MaxHeatingElementTemperature
    {
      get => heatingElementsMaxTemperature;
      private set
      {
        if (value == heatingElementsMaxTemperature)
          return;

        heatingElementsMaxTemperature = value;
        OnPropertyChanged();
      }
    }
    private double heatingElementsMaxTemperature = 0.0;

    /// <summary>
    /// Секции зоны нагрева
    /// </summary>
    public ObservableCollection<ReactorHeatingElement> HeatingElements
    {
      get;
      private set;
    } = new ObservableCollection<ReactorHeatingElement>();

    #region Heating Control

    /// <summary>
    /// Изменить параметры контроля нагрева зоны реактора
    /// </summary>
    /// <param name="minTargetWallTemperature"> Минимальная зона прогрева стенки реактора, °C</param>
    /// <param name="maxTargetWallTemperature"> Максимальная температура зоны реактора, °C </param>
    /// <param name="maxPower"> Максимальная мощность нагревательного элемента, % </param>
    /// <param name="maxHeaterTemperature"> Максимальная температура нагревательного элемента, °С </param>
    public void SetupControl(double minTargetWallTemperature,
                            double maxTargetWallTemperature,
                            double maxPower,
                            double maxHeaterTemperature = 810.0)
    {
      MinTargetWallTemperature = minTargetWallTemperature;
      MaxTargetWallTemperature = maxTargetWallTemperature;
      MaxHeaterTemperature = maxHeaterTemperature;
      MaxPowerLevel = maxPower;
    }

    private readonly object threadLock = new object();

    // Флаг остановки потока контроля нагрева зоны реактора
    private CancellationTokenSource cts = null;

    // Поток контроля нагрева зоны реактора
    private Thread heatingControlThread = null;


    /// <summary>
    /// Начать контроль зоны нагрева
    /// </summary>
    public void StartControl()
    {
      //контроль нагрева уже запущен или поток нагрева зоны все еще активен      
      lock (threadLock)
      {
        //если поток контроля нагрева зоны уже запущен запрещаем запуск нового потока 
        if (heatingControlThread != null && heatingControlThread.IsAlive)
          return;

        PeriodWrite.WriteValue(500.0);

        // Создаем новый экземпляр CancellationTokenSource
        cts = new CancellationTokenSource();

        // Создаем делегат для нестатического метода
        ThreadStart threadDelegate = new ThreadStart(this.HeatingZoneControlFunction);

        heatingControlThread = new Thread(threadDelegate);
        heatingControlThread.Start();

        string message = $"Запущена процедура автоматического контроля нагрева зоны реактора";
        OnTick(message, MessageType.Warning);
      }
    }

    /// <summary>
    /// Остановить контроль зоны нагрева
    /// </summary>
    public void StopControl()
    {
      //Выставляем запрос на остановку потока контроля нагрева зоны 
      cts.Cancel();

      //ждем завершения потока контроля нагрева зоны
      heatingControlThread.Join(0);

      //обновляем состояние потока контроля нагрева зоны
      OnPropertyChanged("IsControlStarted");

      //сбросить мощность нагревателя в 0.0%
      DutyWrite.WriteValue(0.0);

      //сообщаем об остановке потока контроля нагрева зоны
      string message = $"Процедура автоматического контроля нагрева зоны реактора остановлена";
      OnTick(message, MessageType.Warning);
    }

    //проверка состояния потока контроля нагрева зоны
    public bool IsControlStarted
    {
      get
      {
        if (heatingControlThread == null)
          return false;

        if (heatingControlThread.IsAlive)
          return true;

        return false;
      }
    }

    /// <summary>
    /// Процедура контроля нагрева зоны  реактора
    /// </summary>
    private void HeatingZoneControlFunction()
    {
      while (true)
      {
        Thread.Sleep(1000);

        if (cts.IsCancellationRequested)
          break;

        //TODO: контроль тока нагревательных элементов
        foreach (var element in HeatingElements)
        {
          if (element.Current.Value == 0)
          {
            //TODO: Перегорел нагревательный элемент 
          }
          else
          {
            //TODO: Рассчет потенциального ресурса 
          }
        }

        double wallTemperature = WallTemperature.Value;
        MaxHeatingElementTemperature = HeatingElements.Max(heatingElement => heatingElement.Temperature.Value);

        //если температура кагого либо наргревателя больше максимальной
        //или температура стенки реактора больше целевой
        //сбросить мощность нагревателя в 0.0%
        if (MaxHeatingElementTemperature > MaxHeaterTemperature)
        {
          if (DutyWrite.Value != 0.0)
          {
            DutyWrite.WriteValue(0.0);

            string message = $"Нагреватель отключен. Текущая температура нагревательных элементов {heatingElementsMaxTemperature}°C превысила заданную {MaxHeaterTemperature}°C.";
            OnTick(message, MessageType.Info);
          }
          continue;
        }

        if (wallTemperature > MaxTargetWallTemperature)
        {
          if (DutyWrite.Value != 0.0)
          {
            //TODO:Обработать сообщение об отключении нагревателя по превышении температуры стенки ревктора
            DutyWrite.WriteValue(0.0);
            string message = $"Нагреватель отключен. Текущая температура стенки зоны реактора {wallTemperature}°C превысила максимально заданную {MaxTargetWallTemperature}°C.";
            OnTick(message, MessageType.Info);
          }
          continue;
        }

        //устанавливаем заданную мощность прогрева если температура стенки реактора
        //упала ниже минимальной
        if (wallTemperature < MinTargetWallTemperature)
        {
          if (DutyWrite.Value != MaxPowerLevel)
          {
            //TODO:Обработать сообщение о включении нагревателя по понижению температуры стенки реактора
            DutyWrite.WriteValue(MaxPowerLevel);

            string message = $"Нагреватель включен. Текущая температура стенки зоны реактора {wallTemperature}°C упала ниже минимально заданной {MinTargetWallTemperature}°C.";
            OnTick(message, MessageType.Info);
          }
          continue;
        }
      }
    }

    #endregion
  }
}
