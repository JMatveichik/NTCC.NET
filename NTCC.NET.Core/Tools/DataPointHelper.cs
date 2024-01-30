using NTCC.NET.Core.Facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NTCC.NET.Core.Tools
{
  public static class DataPointHelper
  {
    public static void SetDiscreteParameter(FacilityElement element, string id, bool state, int delayAfter = 0)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      //получение дискретного источника данных 
      DiscreteOutputDataPoint outputDataPoint = dataPoints[id] as DiscreteOutputDataPoint;

      if (outputDataPoint == null)
        throw new Exception($"Не найдена дикретная точка данных <{id}>");

      //переключение выходной дискретной точки данных в заданное состояние
      outputDataPoint.SetState(state);

      //вызов обработчиков при изменении состояния
      element.OnTick($"Переключение  {outputDataPoint.ID} в состояние {outputDataPoint.StateString}", MessageType.Info);

      //Задержка перед следующей операцией 
      Thread.Sleep(delayAfter);
    }

    public static void SetAnalogParameter(FacilityElement element, string id, double val, int delayAfter = 0)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      //получение аналогового источника данных 
      AnalogOutputDataPoint outputDataPoint = dataPoints[id] as AnalogOutputDataPoint;

      if (outputDataPoint == null)
        throw new Exception($"Не найдена аналоговая точка данных <{id}>");

      //переключение выходной дискретной точки данных в заданное состояние
      outputDataPoint.WriteValue(val);
      element.OnTick($"Задание  параметра {outputDataPoint.ID}  ({val})", MessageType.Info);

      //Задержка перед следующей операцией 
      Thread.Sleep(delayAfter);
    }

    public static void WaitAnalogParameterSet(FacilityElement element, string id, double val, TimeSpan timeout, double precition = 0.1, int delayBeetwenCheck = 100)
    {
      //TODO : need to understand how to check setup parameters 
      return;

      var dataPoints = ArtMonbatFacility.DataPoints;

      var analogDataPoint = dataPoints[id] as AnalogDataPoint;
      if (analogDataPoint == null)
        throw new Exception($"Не найдена аналоговая точка данных <{id}>");

      DateTime startTime = DateTime.Now;
      element.OnTick($"Ожидание установки параметра {analogDataPoint.ID}  ({val})", MessageType.Info);

      while (Math.Abs(analogDataPoint.Value - val) < precition)
      {
        Thread.Sleep(delayBeetwenCheck);

        TimeSpan waitTime = DateTime.Now - startTime;
        if (waitTime > timeout)
          throw new Exception($"Превышено время установления  {analogDataPoint.ID} ({timeout.TotalSeconds} s)");
      }
    }

    public static void WaitDiscreteParameterSet(FacilityElement element, string id, bool state, TimeSpan timeout, int delayBeetwenCheck = 100)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      var discreteDataPoint = dataPoints[id] as DiscreteDataPoint;
      if (discreteDataPoint == null)
        throw new Exception($"Не найдена дискретная точка данных <{id}>");

      DateTime startTime = DateTime.Now;
      element.OnTick($"Ожидание установки параметра {discreteDataPoint.ID}  ({state})", MessageType.Info);

      while (discreteDataPoint.State != state)
      {
        Thread.Sleep(delayBeetwenCheck);

        TimeSpan waitTime = DateTime.Now - startTime;
        if (waitTime > timeout)
          throw new Exception($"Превышено время установления  {discreteDataPoint.ID} ({timeout.TotalSeconds} s)");
      }
    }

    public static bool CheckDiscreteParameter(string id, bool state)
    {
      var dataPoints = ArtMonbatFacility.DataPoints;

      var discreteDataPoint = dataPoints[id] as DiscreteDataPoint;
      if (discreteDataPoint == null)
        throw new Exception($"Не найдена дискретная точка данных <{id}>");

      return discreteDataPoint.State == state;
    }
  }
}
