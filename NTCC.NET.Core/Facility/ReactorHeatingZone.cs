﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class ReactorHeatingZone : FacilityThreadElement
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
      Duty.PropertyChanged += OnDutyChanged;
      
      Period = (AnalogDataPoint)dataPoints["Period"];
      ErrorCode = (AnalogDataPoint)dataPoints["ErrorCode"];
      EStop = (DiscreteDataPoint)dataPoints["EStop"];
      PWM = (DiscreteDataPoint)dataPoints["PWM"];
      DutyWrite = (AnalogOutputDataPoint)dataPoints["DutyWrite"];
      PeriodWrite = (AnalogOutputDataPoint)dataPoints["PeriodWrite"];
      WallTemperature = (AnalogDataPoint)dataPoints["WallTemperature"];

    }

    private void OnDutyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == "Value")
      {
        if (Duty.Value > 0.0)
          IsPowerPresent = true;
        else
          IsPowerPresent = false;
      }
    }

    public bool IsPowerPresent
    {
      get => isPowerPresent;
      private set
      {
        if (value == isPowerPresent)
          return;

        isPowerPresent = value;
        OnPropertyChanged();
      }
    }

    private bool isPowerPresent = false;


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

    protected override void ControlFunction()
    {
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

          string message = $"Нагреватель отключен. Текущая температура нагревательных элементов {heatingElementsMaxTemperature:F0}°C превысила заданную {MaxHeaterTemperature:F0}°C.";
          OnTick(message, MessageType.Debug);
        }
        return;
      }

      if (wallTemperature > MaxTargetWallTemperature)
      {
        if (DutyWrite.Value != 0.0)
        {
          //TODO:Обработать сообщение об отключении нагревателя по превышении температуры стенки ревктора
          DutyWrite.WriteValue(0.0);
          string message = $"Нагреватель отключен. Текущая температура стенки зоны реактора {wallTemperature:F0}°C превысила максимально заданную {MaxTargetWallTemperature:F0}°C.";
          OnTick(message, MessageType.Debug);
        }
        return;
      }

      //устанавливаем заданную мощность прогрева если температура стенки реактора
      //упала ниже минимальной
      if (wallTemperature < MinTargetWallTemperature)
      {
        if (DutyWrite.Value != MaxPowerLevel)
        {
          //TODO:Обработать сообщение о включении нагревателя по понижению температуры стенки реактора
          DutyWrite.WriteValue(MaxPowerLevel);

          string message = $"Нагреватель включен. Текущая температура стенки зоны реактора {wallTemperature:F0}°C упала ниже минимально заданной {MinTargetWallTemperature:F0}°C.";
          OnTick(message, MessageType.Debug);
        }
        return;
      }
    }

    protected override void OnControlStarted()
    {
      //сообщаем об запуске потока переключения
      string message = $"Запущена процедура контроля нагрева {Description}";
      OnTick(message, MessageType.Info);
    }

    protected override void OnControlStopped()
    {
      //задаем нулевую мощность 
      DutyWrite.WriteValue(0.0);

      //отключаем питание зоны нагрева
      Run.SetState(false);

      //сообщаем об остановке контроля нагрева зоны
      string message = $"Процедура контроля нагрева {Description} остановлена. Питание нагревателя отключено.";
      OnTick(message, MessageType.Info);
    }

    #endregion
  }
}
