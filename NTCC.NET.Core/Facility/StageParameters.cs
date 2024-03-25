using NTCC.NET.Core.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{
  public class StageParameters : INotifyPropertyChanged
  {
    private StageParameters()
    {
      
    }

    public static StageParameters FromXml(XElement xmlStage)
    {
      StageParameters parameters = new StageParameters();
      parameters.StageHeatingParameters = new Dictionary<string, HeatingParameters>();

      parameters.Duration = XmlHelper.ParseDoubleAttribute(xmlStage, "Duration", 0.0);
      parameters.FlowRate = XmlHelper.ParseDoubleAttribute(xmlStage, "FlowRate", 0.0);
      parameters.PurgePropaneLine = XmlHelper.ParseBoolAttribute(xmlStage, "PurgePropaneLine", false);
      parameters.AverageTemperature = XmlHelper.ParseDoubleAttribute(xmlStage, "AverageTemperature", 0.0);

      parameters.PassCount = (int)XmlHelper.ParseDoubleAttribute(xmlStage, "PassCount", 15.0);
      parameters.CoolingTime = XmlHelper.ParseDoubleAttribute(xmlStage, "CoolingTime", 10.0);
      parameters.OneWayTimeout = XmlHelper.ParseDoubleAttribute(xmlStage, "OneWayTimeout", 5.0);

      parameters.CheckWaterLevel = XmlHelper.ParseBoolAttribute(xmlStage, "CheckWaterLevel", false);
      parameters.UseGasHeating   = XmlHelper.ParseBoolAttribute(xmlStage, "UseGasHeating", false);

      foreach (var xmlZone in xmlStage.Descendants("Zone"))
      {
        string zoneID = xmlZone.Attribute("ID")?.Value;
        HeatingParameters zoneHeatingParameters = HeatingParameters.FromXml(xmlZone);

        //add zone heating parameters
        parameters.StageHeatingParameters.Add(zoneID, zoneHeatingParameters);
      }

      return parameters;
    }

    /// <summary>
    /// Число проходов
    /// </summary>
    public int PassCount 
    { 
      get => passCount;
      set
      {
        if (passCount == value)
          return;

        passCount = value;
        OnPropertyChanged();
      }
    }
    private int passCount = 0;
    
    /// <summary>
    /// Время охлажнедния штоков
    /// </summary>
    public double CoolingTime 
    { 
      get => coldingTime;
      set
      {
        if (coldingTime == value)
          return;

        coldingTime = value;
        OnPropertyChanged();
      }
    }
    private double  coldingTime = 0.0;

    /// <summary>
    /// Время ожидания одностороннего прохода
    /// </summary>
		public double OneWayTimeout 
    {  
      get => oneWayTimeout;
      set
      {
        if (oneWayTimeout == value)
          return;

        oneWayTimeout = value;
        OnPropertyChanged();
      }
    }
    private double oneWayTimeout = 0.0; 

    public Dictionary<string, HeatingParameters> StageHeatingParameters
    {
      get;
      private set;
    }

    /// <summary>
    /// Расход газа
    /// </summary>
    public double FlowRate
    {
      get => flowRate;
      set
      {
        if (flowRate == value)
          return;

        flowRate = value;
        OnPropertyChanged();
      }
    }
    private double flowRate = 0.0;

    /// <summary>
    /// Длительность стадии
    /// </summary>
    public double Duration
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
    private double duration = 0.0;

    /// <summary>
    /// Продувать линию пропан-бутана
    /// </summary>
    public bool PurgePropaneLine
    {
      get => purgePropaneLine;
      set
      {
        if (purgePropaneLine == value)
          return;

        purgePropaneLine = value;
        OnPropertyChanged();
      }
    }
    private bool purgePropaneLine = false;

    /// <summary>
    /// Средняя целевая температура реактора при прогреве 
    /// </summary>
    public double AverageTemperature
    {
      get => averageTemperature;
      set
      {
        if (averageTemperature == value)
          return;

        averageTemperature = value;
        OnPropertyChanged();
      }
    }
    private double averageTemperature = 0.0;

    /// <summary>
    /// Использовать подогрев газа
    /// </summary>
    public bool UseGasHeating
    {
      get => useGasHeating;
      set
      {
        if (useGasHeating == value)
          return;

        useGasHeating = value;
        OnPropertyChanged();
      }
    }
    private bool useGasHeating = false;

    /// <summary>
    /// Проверять уровень воды
    /// </summary>
    public bool CheckWaterLevel
    {
      get => checkWaterLevel;
      set
      {
        if (checkWaterLevel == value)
          return;

        checkWaterLevel = value;
        OnPropertyChanged();
      }
    }
    private bool checkWaterLevel = false;


    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Реализация интерфейса INotifiPropertyChanged
    /// </summary>
    /// <param name="prop"></param>
    public void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

  }
}
