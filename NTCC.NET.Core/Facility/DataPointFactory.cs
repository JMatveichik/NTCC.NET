using NTCC.NET.Core.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NTCC.NET.Core.Facility
{

  public class DataPointFactory
  {
    public static BaseConverter CreateConverter(XElement dataPointElement, out double minValue, out double maxValue, out double minSignal, out double maxSignal)
    {
      minValue = 0.0;
      maxValue = 100.0;
      minSignal = 0.0;
      maxSignal = 100.0;

      double minV;
      if (!double.TryParse(dataPointElement.Attribute("MinValue")?.Value, out minV))
        return null;

      double maxV;
      if (!double.TryParse(dataPointElement.Attribute("MaxValue")?.Value, out maxV))
        return null;

      double minS;
      if (!double.TryParse(dataPointElement.Attribute("MinSignal")?.Value, out minS))
        return null;

      double maxS;
      if (!double.TryParse(dataPointElement.Attribute("MaxSignal")?.Value, out maxS))
        return null;

      minValue = minV;
      maxValue = maxV;

      minSignal = minS;
      maxSignal = maxS;

      LinearConverter converter = new LinearConverter(minS, minV, maxS, maxV, "", "");
      return converter;
    }

    public static DataPoint CreateDataPoint(XElement dataPointElement)
    {
      string type = dataPointElement.Attribute("Type")?.Value;
      string id = dataPointElement.Attribute("ID")?.Value;
      string name = dataPointElement.Attribute("Name")?.Value;


      //получаем устройство 
      string deviceID = dataPointElement.Attribute("DeviceID")?.Value;
      AcquisitionDeviceBase device = ArtMonbatFacility.Devices[deviceID];
      if (device == null)
        throw new ArgumentException($"Device [{deviceID}] not found.");

      //получаем номер канала
      int channel = -1;
      if (!int.TryParse(dataPointElement.Attribute("Channel")?.Value, out channel))
        throw new ArgumentException($"Invalid channel number [{channel}] for sensor [{id}]");

      string description = dataPointElement.Value;

      string group = dataPointElement.Attribute("Group")?.Value;

      string units = dataPointElement.Attribute("Units")?.Value;

      string toStr = dataPointElement.Attribute("ToStr")?.Value;

      DataPoint dataPoint = null;
      switch (type.ToUpper())
      {
        case "DI":
          {
            dataPoint = new DiscreteDataPoint(id)
            {
              StateStringsMap = DataPointFactory.GetDiscreteStatesAsString(toStr)
            };
          }
          break;
        case "AI":
          {
            double minValue, maxValue, minSignal, maxSignal;
            BaseConverter converter = CreateConverter(dataPointElement, out minValue, out maxValue, out minSignal, out maxSignal);
            dataPoint = new AnalogDataPoint(id)
            {
              Converter = converter,
              MinValue = minValue,
              MaxValue = maxValue,
              MinSignal = minSignal,
              MaxSignal = maxSignal,
              Units = units,
              ValueFormat = toStr
            };
          }
          break;

        case "DO":
          {
            dataPoint = new DiscreteOutputDataPoint(id)
            {
              StateStringsMap = DataPointFactory.GetDiscreteStatesAsString(toStr)
            };
          }
          break;

        case "AO":
          {
            double minValue, maxValue, minSignal, maxSignal;
            BaseConverter converter = CreateConverter(dataPointElement, out minValue, out maxValue, out minSignal, out maxSignal);
            dataPoint = new AnalogOutputDataPoint(id)
            {
              Converter = converter,
              MinValue = minValue,
              MaxValue = maxValue,
              MinSignal = minSignal,
              MaxSignal = maxSignal,
              Units = units,
              ValueFormat = toStr
            };
          }
          break;

        default:
          throw new ArgumentException($"Unsupported sensor type: {type}");
      }

      if (dataPoint != null)
      {
        dataPoint.Title = name;
        dataPoint.Description = description;
        dataPoint.Device = device;
        dataPoint.ListenedChannel = channel;

        if (!string.IsNullOrEmpty(group))
          dataPoint.Group = group;
      }

      return dataPoint;
    }

    public static Dictionary<bool, string> GetDiscreteStatesAsString(string states)
    {
      Dictionary<bool, string> stateStrings = null;

      if (string.IsNullOrEmpty(states))
        return null;

      try
      {
        stateStrings = new Dictionary<bool, string>()
                {
                    { false, "OFF"},
                    { true , "ON" }
                };

        string[] starray = states.Split(';');

        stateStrings[false] = starray[0];
        stateStrings[true] = starray[1];
      }
      catch (Exception ex)
      {
        return null;
      }


      return stateStrings;
    }

  }
}
