using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class GasHeater : FacilityThreadElement
  {
    public GasHeater(string id) : base(id)
    {
      Description = "Подогреватель газа";
    }

    public void SetupControl(string gasTemperatureId, string heaterTemperatureId, string heaterStateId)
    {
      GasTemperature = ArtMonbatFacility.DataPoints[gasTemperatureId] as AnalogDataPoint;
      if (GasTemperature == null)
        throw new ArgumentNullException($"Analog input data point {gasTemperatureId} not found for gas temperature control");

      WaterTemperature = ArtMonbatFacility.DataPoints[heaterTemperatureId] as AnalogDataPoint;
      if (WaterTemperature == null)
        throw new ArgumentNullException($"Analog input data point {heaterTemperatureId} not found water temperature control");

      HeaterState = ArtMonbatFacility.DataPoints[heaterStateId] as DiscreteOutputDataPoint;
      if (HeaterState == null)
        throw new ArgumentNullException($"Discrete output data point {heaterStateId} not found heater switching");

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
    public AnalogDataPoint WaterTemperature
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
    private double targetGasTemperature = 25.0;

    /// <summary>
    ///Максимальная температура воды подогревателя газа
    /// </summary>
    public double MaxWaterTemperature
    {
      get => maxWaterTemperature;
      set
      {
        if (value == maxWaterTemperature)
          return;

        maxWaterTemperature = value;
        OnPropertyChanged();
      }
    }

    private double maxWaterTemperature = 45.0;


    protected override void OnControlStarted()
    {
      //сообщаем об запуске потока переключения
      string message = $"Запущена процедура подогрева пропан-бутана";
      OnTick(message, MessageType.Info);
    }

    protected override void OnControlStopped()
    {
      //Выключаем подогреватель газа
      HeaterState.SetState(false);

      //сообщаем об остановке потока переключения 
      string message = $"Процедура процедура подогрева пропан-бутана остановлена";
      OnTick(message, MessageType.Info);
    }
    
    /// <summary>
    /// Процедура контроля температуры газа
    /// </summary>
    protected override void ControlFunction()
    {
      //Если температура газа больше заданной или температура
      //нагревательного элемента выше заданной выключаем  нагрев 
      if (GasTemperature.Value > TargetGasTemperature ||
          WaterTemperature.Value > MaxWaterTemperature)
      {
        if (HeaterState.State != false)
          HeaterState.SetState(false);

        return;
      }

      //если температура газа меньше заданной
      //включаем нагревательный элемент
      if (GasTemperature.Value < TargetGasTemperature)
      {
        if (HeaterState.State != true)
          HeaterState.SetState(true);
      }
        
    }
  }
}
