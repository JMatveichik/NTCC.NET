using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Facility
{
  public class ReactorAverageTemperature : FacilityThreadElement
  {

    public ReactorAverageTemperature(string id) : base(id)
    {
    }

    public double AverageTemperatureAllZones 
    { 
      get=>averageTemperatureAllZones; 
      private set
      {
        if (value == averageTemperatureAllZones)
          return;

        averageTemperatureAllZones = value;
        OnPropertyChanged();
      }
    }

    private double averageTemperatureAllZones = 0.0;

    public double AverageTemperatureSelectedZones
    {
      get => averageTemperatureSelectedZones;
      private set
      {
        if (value == averageTemperatureSelectedZones)
          return;

        averageTemperatureSelectedZones = value;
        OnPropertyChanged();
      }
    }

    private double averageTemperatureSelectedZones = 0.0;

    protected override void ControlFunction()
    {
      AverageTemperatureAllZones      = ArtMonbatFacility.Instance.GetAverageTemperature();
      AverageTemperatureSelectedZones = ArtMonbatFacility.FullCycle.CurrentStage.GetAverageTemperature();
    }

    protected override void OnControlStarted()
    {
      //сообщаем о запуске потока вычисления средней температуры
      string message = $"Запущена процедура вычисления средней температуры стенок реактора";
      OnTick(message, MessageType.Info);
    }

    protected override void OnControlStopped()
    {
      //сообщаем об остановке потока вычисления средней температуры
      string message = $"Процедура вычисления средней температуры остановлена";
      OnTick(message, MessageType.Info);
    }
  }
}
